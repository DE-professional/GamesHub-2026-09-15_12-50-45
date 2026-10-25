using UnityEngine;

// Flipbook animation for the player: cycles run frames on the ground, shows a jump
// frame in the air and a death frame on game over. Script-driven rather than an
// Animator because game over sets timeScale to 0, which would freeze an Animator.
[RequireComponent(typeof(SpriteRenderer), typeof(RunnerPlayer))]
public class RunnerPlayerAnimator : MonoBehaviour
{
    public Sprite[] runFrames;
    public Sprite jumpSprite;
    public Sprite dieSprite;
    public float framesPerSecond = 12f;
    // Speeds the run cycle up with the game so the legs keep pace with the ground
    public bool scaleWithSpeed = true;

    private SpriteRenderer spriteRenderer;
    private RunnerPlayer player;
    private float frameTime;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GetComponent<RunnerPlayer>();
    }

    void Update()
    {
        var manager = RunnerManager.Instance;

        if (manager != null && manager.IsGameOver)
        {
            if (dieSprite != null) spriteRenderer.sprite = dieSprite;
            return;
        }

        if (runFrames == null || runFrames.Length == 0) return;

        if (manager == null || !manager.IsRunning)
        {
            spriteRenderer.sprite = runFrames[0];
            return;
        }

        if (!player.IsGrounded && jumpSprite != null)
        {
            spriteRenderer.sprite = jumpSprite;
            return;
        }

        float fps = framesPerSecond * (scaleWithSpeed ? manager.Speed / manager.startSpeed : 1f);
        frameTime = Mathf.Repeat(frameTime + Time.deltaTime * fps, runFrames.Length);
        spriteRenderer.sprite = runFrames[Mathf.Min((int)frameTime, runFrames.Length - 1)];
    }
}
