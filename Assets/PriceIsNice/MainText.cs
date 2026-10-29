using System;
using TMPro;
using UnityEngine;

public class MainText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private void Awake()
    {
        GameManager.OnApproxUpdated += UpdatePrice;
    }

    private void OnDestroy()
    {
        GameManager.OnApproxUpdated -= UpdatePrice;
    }

    private void UpdatePrice(double approxPrice)
    {
        text.text = $"${approxPrice:F2}";
    }
}
