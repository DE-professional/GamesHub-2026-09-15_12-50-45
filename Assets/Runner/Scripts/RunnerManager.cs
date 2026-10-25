using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RunnerManager : MonoBehaviour
{
    public static RunnerManager Instance { get; private set; }

    public float startSpeed = 6f;
    public float maxSpeed = 18f;
    public float speedIncreasePerSecond = 0.15f;

    public GameObject titlePanel;
    public TMP_Text scoreLabel;
    public GameObject gameOverPanel;
    public TMP_Text gameOverScoreLabel;
    public AudioSource sfxSource;
    public AudioClip dieSound;

    public float Speed { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsStarted { get; private set; }
    public bool IsRunning => IsStarted && !IsGameOver;
    // The click/key that starts the game shouldn't also make the player jump
    public bool StartedThisFrame => startFrame == Time.frameCount;

    private float distance;
    private int startFrame = -1;

    // Restarting goes straight back into a run instead of the title screen
    private static bool skipTitle;

    // Statics survive between Play sessions when domain reload is disabled
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() => skipTitle = false;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        Speed = startSpeed;
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (skipTitle || titlePanel == null)
            StartGame();
        else
            titlePanel.SetActive(true);
    }

    // Wired to the title screen's Start button
    public void StartGame()
    {
        if (IsStarted) return;
        IsStarted = true;
        startFrame = Time.frameCount;
        if (titlePanel != null) titlePanel.SetActive(false);
    }

    void Update()
    {
        if (IsGameOver)
        {
            if (RestartPressed()) Restart();
            return;
        }

        if (!IsStarted)
        {
            if (StartKeyPressed()) StartGame();
            return;
        }

        Speed = Mathf.Min(maxSpeed, Speed + speedIncreasePerSecond * Time.deltaTime);
        distance += Speed * Time.deltaTime;

        if (scoreLabel != null) scoreLabel.text = Score.ToString();
    }

    public int Score => Mathf.FloorToInt(distance);

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Time.timeScale = 0f;
        // AudioSources keep playing at timeScale 0
        if (sfxSource != null && dieSound != null) sfxSource.PlayOneShot(dieSound);

        int best = Mathf.Max(Score, PlayerPrefs.GetInt("RunnerBest", 0));
        PlayerPrefs.SetInt("RunnerBest", best);

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverScoreLabel != null)
            gameOverScoreLabel.text = $"Score: {Score}\nBest: {best}\n\nPress Space to restart";
    }

    // Hook this up to a UI button too if you want one
    public void Restart()
    {
        Time.timeScale = 1f;
        skipTitle = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    bool RestartPressed()
    {
        return (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.rKey.wasPressedThisFrame))
            || (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
    }

    bool StartKeyPressed()
    {
        return Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
