using UnityEngine;

// Scrolls a Tiled-draw-mode SpriteRenderer left with the game speed and wraps it by one
// tile width, so it loops seamlessly. Make the renderer's width a whole number of tiles
// and at least one tile wider than the view.
[RequireComponent(typeof(SpriteRenderer))]
public class RunnerScrollingTiles : MonoBehaviour
{
    // 1 = moves with the obstacles; below 1 for slower background layers
    public float speedMultiplier = 1f;

    private float tileWidth;
    private float startX;
    private float offset;

    void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        tileWidth = sr.sprite != null ? sr.sprite.bounds.size.x * transform.lossyScale.x : 0f;
        startX = transform.localPosition.x;
    }

    void Update()
    {
        var manager = RunnerManager.Instance;
        if (manager == null || !manager.IsRunning || tileWidth <= 0f) return;

        offset = Mathf.Repeat(offset + manager.Speed * speedMultiplier * Time.deltaTime, tileWidth);
        var pos = transform.localPosition;
        transform.localPosition = new Vector3(startX - offset, pos.y, pos.z);
    }
}
