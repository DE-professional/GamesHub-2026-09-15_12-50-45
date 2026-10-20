using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SteamGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionBox;

    [SerializeField] private Image image;

    public void SetData(string text, Sprite sprite)
    {
        descriptionBox.text = text;
        image.sprite = sprite;
    }
}
