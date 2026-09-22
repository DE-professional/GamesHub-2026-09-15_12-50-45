using TMPro;
using UnityEngine;

public class GuessTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI guessText;

    private void OnEnable()
    {
        GameManager.OnGuessesChanged += UpdateGuessUI;
    }

    private void OnDisable()
    {
        GameManager.OnGuessesChanged -= UpdateGuessUI;
    }

    private void UpdateGuessUI(int remainingGuesses)
    {
        if (guessText != null)
        {
            guessText.text = $"Guesses Left: {remainingGuesses}";
        }
    }
}