using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Text")]
    public string gameTitle = "GamesHub";

    [Header("Game slot button labels (what the player sees)")]
    public string[] gameDisplayNames = new string[6]
    {
        "Game1", "Game2", "Game3", "Game4", "Game5", "Game6"
    };

    [Header("Game slot scenes (scene names must be added to Build Settings)")]
    public string[] gameSceneNames = new string[6]
    {
        "Game1", "Game2", "Game3", "Game4", "Game5", "Game6"
    };

    [Header("Look")]
    public Color backgroundColor = new Color(0.08f, 0.09f, 0.14f);
    public Color buttonColor = new Color(0.20f, 0.45f, 0.85f);
    public Color textColor = Color.white;

    private GameObject mainPanel;
    private GameObject gamesPanel;
    private Font font;

    private void Awake()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        EnsureEventSystem();
        Canvas canvas = CreateCanvas();

        mainPanel = BuildMainPanel(canvas.transform);
        gamesPanel = BuildGamesPanel(canvas.transform);
        ShowMain();
    }

    // ---------- Button actions ----------

    private void ShowMain()
    {
        mainPanel.SetActive(true);
        gamesPanel.SetActive(false);
    }

    private void ShowGames()
    {
        mainPanel.SetActive(false);
        gamesPanel.SetActive(true);
    }

    private void OnScores()
    {
        // nothing yet lol
    }

    private void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnSelectGame(int index)
    {
        string sceneName = gameSceneNames[index];
        if (string.IsNullOrEmpty(sceneName) || !Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning($"Slot {index + 1}: scene '{sceneName}' is not in Build Settings.");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    // ---------- Panels ----------

    private GameObject BuildMainPanel(Transform parent)
    {
        GameObject panel = CreatePanel("MainPanel", parent);

        CreateText("Title", panel.transform, gameTitle, 90, FontStyle.Bold,
            new Vector2(0.5f, 0.78f), new Vector2(900, 140));

        GameObject column = new GameObject("Buttons", typeof(RectTransform), typeof(VerticalLayoutGroup));
        column.transform.SetParent(panel.transform, false);
        RectTransform rt = column.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.38f);
        rt.sizeDelta = new Vector2(320, 300);

        VerticalLayoutGroup v = column.GetComponent<VerticalLayoutGroup>();
        v.spacing = 20;
        v.childAlignment = TextAnchor.MiddleCenter;
        v.childControlWidth = v.childControlHeight = true;
        v.childForceExpandWidth = v.childForceExpandHeight = true;

        CreateButton("PlayButton", column.transform, "Play", ShowGames);
        CreateButton("ScoresButton", column.transform, "Scores", OnScores);
        CreateButton("QuitButton", column.transform, "Quit", OnQuit);

        return panel;
    }

    private GameObject BuildGamesPanel(Transform parent)
    {
        GameObject panel = CreatePanel("GamesPanel", parent);

        CreateText("Header", panel.transform, "Select a Game", 60, FontStyle.Bold,
            new Vector2(0.5f, 0.88f), new Vector2(900, 100));

        GameObject grid = new GameObject("Slots", typeof(RectTransform), typeof(GridLayoutGroup));
        grid.transform.SetParent(panel.transform, false);
        RectTransform rt = grid.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.52f);
        rt.sizeDelta = new Vector2(900, 440);

        GridLayoutGroup g = grid.GetComponent<GridLayoutGroup>();
        g.cellSize = new Vector2(280, 200);
        g.spacing = new Vector2(30, 40);
        g.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        g.constraintCount = 3;
        g.childAlignment = TextAnchor.MiddleCenter;

        for (int i = 0; i < 6; i++)
        {
            int index = i; // capture for closure
            string label = (gameDisplayNames != null && index < gameDisplayNames.Length
                            && !string.IsNullOrEmpty(gameDisplayNames[index]))
                ? gameDisplayNames[index]
                : $"Game {index + 1}";
            CreateButton($"Slot{i + 1}", grid.transform, label, () => OnSelectGame(index));
        }

        Button back = CreateButton("BackButton", panel.transform, "Back", ShowMain);
        RectTransform brt = back.GetComponent<RectTransform>();
        brt.anchorMin = brt.anchorMax = new Vector2(0.5f, 0.1f);
        brt.sizeDelta = new Vector2(240, 60);

        return panel;
    }

    // ---------- UI helpers ----------

    private Canvas CreateCanvas()
    {
        GameObject go = new GameObject("MenuCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;
        // NOTE: if your project uses ONLY the new Input System package, swap
        // StandaloneInputModule for InputSystemUIInputModule.
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    private GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = backgroundColor;
        return panel;
    }

    private Text CreateText(string name, Transform parent, string content, int size,
        FontStyle style, Vector2 anchor, Vector2 sizeDelta)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.sizeDelta = sizeDelta;

        Text t = go.GetComponent<Text>();
        t.font = font;
        t.text = content;
        t.fontSize = size;
        t.fontStyle = style;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = textColor;
        return t;
    }

    private Button CreateButton(string name, Transform parent, string label, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        Image img = go.GetComponent<Image>();
        img.color = buttonColor;

        Button btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        ColorBlock colors = btn.colors;
        colors.highlightedColor = new Color(0.35f, 0.6f, 1f);
        colors.pressedColor = new Color(0.12f, 0.3f, 0.6f);
        btn.colors = colors;
        btn.onClick.AddListener(onClick);

        Text t = CreateText("Label", go.transform, label, 32, FontStyle.Normal, new Vector2(0.5f, 0.5f), Vector2.zero);
        RectTransform trt = t.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = trt.offsetMax = Vector2.zero;

        return btn;
    }
}
