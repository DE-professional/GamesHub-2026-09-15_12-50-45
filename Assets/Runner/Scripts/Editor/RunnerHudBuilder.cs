using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// One-shot builder: Tools > Runner > Build HUD
public static class RunnerHudBuilder
{
    const string ScenePath = "Assets/Runner/Runner.unity";
    const string HudName = "RunnerHUD";

    [MenuItem("Tools/Runner/Build HUD")]
    static void BuildHud()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != ScenePath)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            scene = EditorSceneManager.OpenScene(ScenePath);
        }

        if (GameObject.Find(HudName) != null)
        {
            EditorUtility.DisplayDialog("Runner HUD", $"'{HudName}' already exists in the scene. Delete it first to rebuild.", "OK");
            return;
        }

        Undo.SetCurrentGroupName("Build Runner HUD");
        int undoGroup = Undo.GetCurrentGroup();

        // Canvas
        var canvasGo = new GameObject(HudName, typeof(RectTransform), typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGo, "Create HUD");
        canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.GetComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // Score label: top centre
        var scoreLabel = CreateText("ScoreLabel", canvasGo.transform, "0", 96, FontStyles.Bold);
        var scoreRt = scoreLabel.rectTransform;
        scoreRt.anchorMin = scoreRt.anchorMax = new Vector2(0.5f, 1f);
        scoreRt.pivot = new Vector2(0.5f, 1f);
        scoreRt.sizeDelta = new Vector2(600, 120);
        scoreRt.anchoredPosition = new Vector2(0, -40);

        // Game over panel: full-screen dim overlay
        var panelGo = new GameObject("GameOverPanel", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        panelGo.transform.SetParent(canvasGo.transform, false);
        var panelRt = (RectTransform)panelGo.transform;
        panelRt.anchorMin = Vector2.zero;
        panelRt.anchorMax = Vector2.one;
        panelRt.offsetMin = panelRt.offsetMax = Vector2.zero;
        var panelImage = panelGo.GetComponent<UnityEngine.UI.Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.75f);
        panelImage.raycastTarget = false;

        var title = CreateText("Title", panelGo.transform, "GAME OVER", 140, FontStyles.Bold);
        var titleRt = title.rectTransform;
        titleRt.anchorMin = titleRt.anchorMax = new Vector2(0.5f, 0.5f);
        titleRt.pivot = new Vector2(0.5f, 0f);
        titleRt.sizeDelta = new Vector2(1400, 180);
        titleRt.anchoredPosition = new Vector2(0, 60);

        var gameOverScore = CreateText("GameOverScoreLabel", panelGo.transform, "Score: 0\nBest: 0\n\nPress Space to restart", 60, FontStyles.Normal);
        var goRt = gameOverScore.rectTransform;
        goRt.anchorMin = goRt.anchorMax = new Vector2(0.5f, 0.5f);
        goRt.pivot = new Vector2(0.5f, 1f);
        goRt.sizeDelta = new Vector2(1400, 360);
        goRt.anchoredPosition = new Vector2(0, 20);

        panelGo.SetActive(false);

        // Wire to RunnerManager, creating one if the scene doesn't have it yet
        var manager = Object.FindFirstObjectByType<RunnerManager>();
        if (manager == null)
        {
            var managerGo = new GameObject("RunnerManager");
            Undo.RegisterCreatedObjectUndo(managerGo, "Create RunnerManager");
            manager = managerGo.AddComponent<RunnerManager>();
        }
        Undo.RecordObject(manager, "Wire HUD");
        manager.scoreLabel = scoreLabel;
        manager.gameOverPanel = panelGo;
        manager.gameOverScoreLabel = gameOverScore;
        EditorUtility.SetDirty(manager);

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Selection.activeGameObject = canvasGo;
        Debug.Log("Runner HUD built and wired to RunnerManager.");
    }

    static TextMeshProUGUI CreateText(string name, Transform parent, string text, float size, FontStyles style)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.fontStyle = style;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;
        return tmp;
    }
}
