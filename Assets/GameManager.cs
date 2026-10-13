using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Grid & Prefabs")] 
    public GameObject cellHolder;
    public GameObject cellPrefab;
    public int width = 20;
    public int height = 20;
    public float spacing = 1.05f;

    [Header("Mine Settings")]
    public int minMines = 15;
    public int maxMines = 30;

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI flagText;
    public TextMeshProUGUI highScoreText;

    public bool isGameOver { get; private set; } = false;

    private Cell[,] grid;
    private bool isFirstClick = true;
    private int totalMines;
    private int flagsPlaced = 0;
    private int cellsRevealed = 0;
    private float timer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        grid = new Cell[width, height];
        totalMines = Random.Range(minMines, maxMines + 1);
        
        LoadDailyHighScore();
        UpdateFlagUI();
        SpawnGrid();
    }

    void Update()
    {
        if (!isGameOver && !isFirstClick)
        {
            timer += Time.deltaTime;
            if (timerText != null)
                timerText.text = $"Time: {Mathf.FloorToInt(timer)}s";
        }
    }

    void SpawnGrid()
    {
        float startX = -(width * spacing / 2f) + (spacing / 2f);
        float startY = -(height * spacing / 2f) + (spacing / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(startX + (x * spacing), startY + (y * spacing));
                GameObject spawnedCell = Instantiate(cellPrefab, cellHolder.transform);
                spawnedCell.transform.position = position;
                
                Cell cellScript = spawnedCell.GetComponent<Cell>();
                cellScript.Setup(x, y);
                grid[x, y] = cellScript;
            }
        }
        
        if (Camera.main != null)
            Camera.main.orthographicSize = (height * spacing) / 2f + 2f;
    }

    public void HandleCellClick(int x, int y)
    {
        if (isGameOver || grid[x, y].isFlagged) return;

        if (isFirstClick)
        {
            isFirstClick = false;
            PlaceMinesSafely(x, y);
            CalculateAdjacencies();
        }

        if (grid[x, y].isMine)
        {
            TriggerGameOver(false);
            grid[x, y].RevealVisuals();
        }
        else
        {
            FloodFillReveal(x, y);
            CheckWinCondition();
        }
    }

    void PlaceMinesSafely(int safeX, int safeY)
    {
        int minesPlaced = 0;
        while (minesPlaced < totalMines)
        {
            int rx = Random.Range(0, width);
            int ry = Random.Range(0, height);

            bool isSafeZone = Mathf.Abs(rx - safeX) <= 1 && Mathf.Abs(ry - safeY) <= 1;

            if (!grid[rx, ry].isMine && !isSafeZone)
            {
                grid[rx, ry].isMine = true;
                minesPlaced++;
            }
        }
    }

    void CalculateAdjacencies()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].isMine) continue;

                int minesFound = 0;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = x + dx, ny = y + dy;
                        if (nx >= 0 && nx < width && ny >= 0 && ny < height && grid[nx, ny].isMine)
                            minesFound++;
                    }
                }
                grid[x, y].adjacentMines = minesFound;
            }
        }
    }

    void FloodFillReveal(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return;
        
        Cell cell = grid[x, y];
        if (cell.isRevealed || cell.isFlagged || cell.isMine) return;

        cell.isRevealed = true;
        cellsRevealed++;
        cell.RevealVisuals();

        if (cell.adjacentMines == 0)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    FloodFillReveal(x + dx, y + dy);
                }
            }
        }
    }

    public void ToggleFlagState(int x, int y)
    {
        Cell cell = grid[x, y];
        if (cell.isRevealed) return;

        cell.isFlagged = !cell.isFlagged;
        flagsPlaced += cell.isFlagged ? 1 : -1;
        UpdateFlagUI();
        
        cell.GetComponent<SpriteRenderer>().color = cell.isFlagged ? Color.yellow : Color.white;
    }

    void UpdateFlagUI()
    {
        if (flagText != null)
            flagText.text = $"Flags: {Mathf.Max(0, totalMines - flagsPlaced)}";
    }

    void CheckWinCondition()
    {
        if (cellsRevealed == (width * height) - totalMines)
        {
            TriggerGameOver(true);
        }
    }

    public void TriggerGameOver(bool won)
    {
        isGameOver = true;
        
        if (won)
        {
            Debug.Log("You Win!");
            SaveDailyHighScore();
        }
        else
        {
            Debug.Log("Game Over! You hit a mine.");
            foreach (Cell cell in grid)
            {
                if (cell.isMine && !cell.isFlagged) cell.RevealVisuals();
            }
        }
    }

    void LoadDailyHighScore()
    {
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        string savedDate = PlayerPrefs.GetString("HighScoreDate", "");

        if (savedDate != today)
        {
            PlayerPrefs.SetFloat("DailyHighScore", float.MaxValue);
            PlayerPrefs.SetString("HighScoreDate", today);
            PlayerPrefs.Save();
        }

        float highScore = PlayerPrefs.GetFloat("DailyHighScore", float.MaxValue);
        if (highScoreText != null)
        {
            highScoreText.text = highScore < float.MaxValue ? $"Best Today: {Mathf.FloorToInt(highScore)}s" : "Best Today: --";
        }
    }

    void SaveDailyHighScore()
    {
        float currentHigh = PlayerPrefs.GetFloat("DailyHighScore", float.MaxValue);
        if (timer < currentHigh)
        {
            PlayerPrefs.SetFloat("DailyHighScore", timer);
            PlayerPrefs.Save();
            if (highScoreText != null) highScoreText.text = $"Best Today: {Mathf.FloorToInt(timer)}s";
        }
    }
}