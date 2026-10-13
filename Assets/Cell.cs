using UnityEngine;
using UnityEngine.EventSystems; // Required for Pointer Handlers
using TMPro;
using UnityEngine.UI;           // Required if using UI Image

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public bool isMine;
    public bool isRevealed;
    public bool isFlagged;
    public int adjacentMines;

    private int gridX;
    private int gridY;

    // Use TextMeshProUGUI for Canvas UI objects
    public TextMeshProUGUI numberText;
    private Image cellImage;

    void Awake()
    {
        cellImage = GetComponent<Image>();

        // Auto-assign text from child object
        if (numberText == null)
        {
            numberText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (numberText != null)
        {
            numberText.text = "";
        }
    }

    public void Setup(int x, int y)
    {
        gridX = x;
        gridY = y;
    }

    // Listens for clicks directly via Canvas Event System
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.isGameOver || isRevealed) return;

        // Right Click -> Flag
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            GameManager.Instance.ToggleFlagState(gridX, gridY);
        }
        // Left Click -> Reveal
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!isFlagged) // Prevent clicking flagged tiles
            {
                GameManager.Instance.HandleCellClick(gridX, gridY);
            }
        }
    }

    public void RevealVisuals()
    {
        if (isMine)
        {
            if (cellImage != null) cellImage.color = Color.red;
        }
        else
        {
            if (cellImage != null) cellImage.color = Color.gray;

            if (numberText != null)
            {
                if (adjacentMines > 0)
                {
                    numberText.text = adjacentMines.ToString();
                    SetNumberColor();
                }
                else
                {
                    numberText.text = "";
                }
            }
        }
    }

    public void UpdateFlagVisuals()
    {
        if (cellImage != null)
        {
            cellImage.color = isFlagged ? Color.yellow : Color.white;
        }
    }

    void SetNumberColor()
    {
        if (numberText == null) return;

        switch (adjacentMines)
        {
            case 1: numberText.color = Color.blue; break;
            case 2: numberText.color = new Color(0, 0.5f, 0); break;
            case 3: numberText.color = Color.red; break;
            case 4: numberText.color = new Color(0, 0, 0.5f); break;
            case 5: numberText.color = new Color(0.5f, 0, 0); break;
            case 6: numberText.color = Color.cyan; break;
            case 7: numberText.color = Color.black; break;
            case 8: numberText.color = Color.gray; break;
        }
    }
}