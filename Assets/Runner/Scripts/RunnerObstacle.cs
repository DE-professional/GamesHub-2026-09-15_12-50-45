using UnityEngine;

// Scrolls left at the game speed and despawns off-screen. Needs a trigger Collider2D.
public class RunnerObstacle : MonoBehaviour
{
    public float despawnX = -20f;

    // Ground obstacles: the spawner picks a height in this range (capped to what the player can jump)
    public bool randomHeight;
    public float minHeight = 0.6f;
    public float maxHeight = 2f;

    void Update()
    {
        if (RunnerManager.Instance == null) return;

        transform.position += Vector3.left * (RunnerManager.Instance.Speed * Time.deltaTime);
        if (transform.position.x < despawnX) Destroy(gameObject);
    }

    // Resizes vertically while keeping the bottom edge where it is, whatever the sprite's pivot is
    public void SetHeight(float height)
    {
        var sprite = GetComponent<SpriteRenderer>().sprite;
        if (sprite == null || sprite.bounds.size.y <= 0f) return;

        var scale = transform.localScale;
        float newScaleY = height / sprite.bounds.size.y;
        transform.position += Vector3.up * (sprite.bounds.min.y * (scale.y - newScaleY));
        transform.localScale = new Vector3(scale.x, newScaleY, scale.z);
    }
}
