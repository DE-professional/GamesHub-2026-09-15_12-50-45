using UnityEngine;

// Scrolls this object's child SpriteRenderers left at a fraction of the game speed.
// When a child leaves the left edge of the view it is recycled past the right edge
// with a random gap, height and sprite, so the layer never visibly repeats.
public class RunnerParallaxScatter : MonoBehaviour
{
    // 1 = moves with the obstacles; below 1 for distant layers
    public float speedMultiplier = 0.2f;
    public Sprite[] sprites;
    public float minGap = 2f;
    public float maxGap = 6f;
    // World-space height range for recycled children (their pivot position)
    public float minY = 0f;
    public float maxY = 0f;

    private SpriteRenderer[] items;
    private Camera cam;

    void Awake()
    {
        items = GetComponentsInChildren<SpriteRenderer>();
        cam = Camera.main;
    }

    void Update()
    {
        var manager = RunnerManager.Instance;
        if (manager == null || !manager.IsRunning || cam == null) return;

        float dx = manager.Speed * speedMultiplier * Time.deltaTime;
        float halfView = cam.orthographicSize * cam.aspect;
        float leftEdge = cam.transform.position.x - halfView;
        float rightEdge = cam.transform.position.x + halfView;

        foreach (var item in items)
        {
            item.transform.position += Vector3.left * dx;
            if (item.bounds.max.x < leftEdge) Recycle(item, rightEdge);
        }
    }

    void Recycle(SpriteRenderer item, float rightEdge)
    {
        if (sprites != null && sprites.Length > 0)
            item.sprite = sprites[Random.Range(0, sprites.Length)];

        float rightmost = float.MinValue;
        foreach (var other in items)
            if (other != item) rightmost = Mathf.Max(rightmost, other.bounds.max.x);

        // Place the new left edge a random gap after the rightmost item, but never inside the view
        float halfWidth = item.bounds.extents.x;
        float x = Mathf.Max(rightmost + Random.Range(minGap, maxGap), rightEdge) + halfWidth;
        var pos = item.transform.position;
        item.transform.position = new Vector3(x, Random.Range(minY, maxY), pos.z);
    }
}
