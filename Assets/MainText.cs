using System;
using TMPro;
using UnityEngine;

public class MainText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private string def;
    private void Awake()
    {
        GameManager.OnApproxUpdated += UpdatePrice;
        def = text.text;
    }

    private void UpdatePrice(double approxPrice)
    {
        var changedStr = def + $"${approxPrice.ToString()}?";
        text.text = changedStr;
    }
}
