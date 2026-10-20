using System.Collections;
using UnityEngine;

public class CanvasRumble : MonoBehaviour
{
    [Header("Default Settings")]
    [Tooltip("Offset distance in UI pixels.")]
    [SerializeField] private float defaultIntensity = 15f;
    [Tooltip("Duration in seconds.")]
    [SerializeField] private float defaultDuration = 0.5f;

    private RectTransform rectTransform;
    private Vector2 originalAnchoredPosition;
    private Coroutine currentRumble;
    private int previousGuesses = -1;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        originalAnchoredPosition = rectTransform.anchoredPosition;
        GameManager.OnGuessesChanged += HandleGuessesChanged;
    }
    private void HandleGuessesChanged(int currentGuesses)
    {
        // Prevent MissingReferenceException if object was destroyed mid-event
        if (this == null || gameObject == null) return;

        if (previousGuesses == -1 || currentGuesses > previousGuesses)
        {
            previousGuesses = currentGuesses;
            return;
        }

        if (currentGuesses < previousGuesses)
        {
            previousGuesses = currentGuesses;
            TriggerRumble(defaultDuration, defaultIntensity);
        }
    }

    public void TriggerRumble(float duration, float intensity)
    {
        if (currentRumble != null)
        {
            StopCoroutine(currentRumble);
            rectTransform.anchoredPosition = originalAnchoredPosition;
        }

        currentRumble = StartCoroutine(RumbleRoutine(duration, intensity));
    }

    private IEnumerator RumbleRoutine(float duration, float intensity)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Smoothly damp the intensity over time so it fades out
            float damping = 1.0f - (elapsed / duration);

            // Generate random pixel displacement offset
            Vector2 randomOffset = Random.insideUnitCircle * intensity * damping;

            rectTransform.anchoredPosition = originalAnchoredPosition + randomOffset;

            elapsed += Time.unscaledDeltaTime; // Uses unscaled time so it shakes even if timeScale is paused
            yield return null;
        }

        rectTransform.anchoredPosition = originalAnchoredPosition;
        currentRumble = null;
    }
}