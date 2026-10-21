using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CellView : MonoBehaviour
{
    public int row;
    public int col;
    public TMP_Text label;
    public TMP_Text[] noteLabel;
    public Button button;
    public Image background;

    private System.Action<int, int> onClickCallback;

    public void Setup(int row, int col, int value, bool isGiven, System.Action<int, int> onClick)
    {
        this.row = row;
        this.col = col;
        onClickCallback = onClick;

        label.text = value == 0 ? "" : value.ToString();
        label.color = Color.blue;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickCallback?.Invoke(row, col));
    }

    public void SetValue(int value)
    {
        label.text = value == 0 ? "" : value.ToString();
    }

    public void SetHighlight(bool isSelected, bool isSimiliar)
    {
        if(isSelected)
            background.color = new Color(0.6f, 0.75f, 1f);
        else if (isSimiliar)
            background.color = new Color(0.75f, 0.9f, 1f);
        else
            background.color = Color.white;
    }

    public void SetNotes(HashSet<int> notes)
    {
        for (int i = 0; i < 9; i++)
        {
            int number = i + 1;
            noteLabel[i].text = (notes != null && notes.Contains(number)) ? number.ToString() : "";
        }
    }
}