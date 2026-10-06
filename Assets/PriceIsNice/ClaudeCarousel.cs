using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple image carousel that can be populated with any number of sprites at runtime.
/// Attach this to a UI GameObject that has an Image component (the display) somewhere
/// in its hierarchy, or assign one directly via the Inspector/SetSprites call.
/// </summary>
public class ImageCarousel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image displayImage;      // The Image component showing the current slide
    [SerializeField] private Button nextButton;        // Optional - wire in Inspector or leave null
    [SerializeField] private Button prevButton;         // Optional - wire in Inspector or leave null

    [Header("Optional: Dot Indicators")]
    [SerializeField] private Transform dotContainer;    // Parent with a Horizontal/Vertical Layout Group
    [SerializeField] private GameObject dotPrefab;       // A simple Image prefab used as a dot
    [SerializeField] private Color activeDotColor = Color.white;
    [SerializeField] private Color inactiveDotColor = new Color(1f, 1f, 1f, 0.4f);

    private List<Sprite> _slides = new List<Sprite>();
    private List<Image> _dots = new List<Image>();
    private int _currentIndex = 0;

    private void Awake()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(Next);

        if (prevButton != null)
            prevButton.onClick.AddListener(Previous);
    }

    private void OnDestroy()
    {
        if (nextButton != null)
            nextButton.onClick.RemoveListener(Next);

        if (prevButton != null)
            prevButton.onClick.RemoveListener(Previous);
    }

    /// <summary>
    /// Populates the carousel with a runtime-defined set of images.
    /// Call this once you know how many images you have (e.g. after a data/network load).
    /// </summary>
    public void SetSprites(IEnumerable<Sprite> sprites, int startIndex = 0)
    {
        _slides.Clear();
        _slides.AddRange(sprites);

        BuildDots();

        _currentIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, _slides.Count - 1));
        UpdateDisplay();
    }

    public void Next()
    {
        if (_slides.Count == 0) return;

        _currentIndex = (_currentIndex + 1) % _slides.Count; // wraps around
        UpdateDisplay();
    }

    public void Previous()
    {
        if (_slides.Count == 0) return;

        _currentIndex = (_currentIndex - 1 + _slides.Count) % _slides.Count; // wraps around
        UpdateDisplay();
    }

    public void GoToIndex(int index)
    {
        if (_slides.Count == 0) return;

        _currentIndex = Mathf.Clamp(index, 0, _slides.Count - 1);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_slides.Count == 0)
        {
             displayImage.enabled = false;
            return;
        }

        displayImage.enabled = true;
        displayImage.sprite = _slides[_currentIndex];

        UpdateDots();
    }

    /// <summary>
    /// Rebuilds dot indicators to match the current slide count.
    /// Skips entirely if dotContainer/dotPrefab weren't assigned.
    /// </summary>
    private void BuildDots()
    {
        if (dotContainer == null || dotPrefab == null)
            return;

        // Clear existing dots
        foreach (Transform child in dotContainer)
            Destroy(child.gameObject);

        _dots.Clear();

        for (int i = 0; i < _slides.Count; i++)
        {
            GameObject dotObj = Instantiate(dotPrefab, dotContainer);
            Image dotImage = dotObj.GetComponent<Image>();

            int index = i; // local copy to avoid closure capture bug
            Button dotButton = dotObj.GetComponent<Button>();
            if (dotButton != null)
                dotButton.onClick.AddListener(() => GoToIndex(index));

            _dots.Add(dotImage);
        }
    }

    private void UpdateDots()
    {
        for (int i = 0; i < _dots.Count; i++)
        {
            if (_dots[i] != null)
                _dots[i].color = (i == _currentIndex) ? activeDotColor : inactiveDotColor;
        }
    }
}