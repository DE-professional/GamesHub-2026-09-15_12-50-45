using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class RunnerPlayer : MonoBehaviour
{
    public float jumpVelocity = 12f;
    // Extra gravity while falling / after releasing jump, for snappier arcs
    public float fallGravityMultiplier = 2.5f;
    public float lowJumpGravityMultiplier = 2f;

    // Fraction of standing height while crouched
    public float crouchHeightFactor = 0.5f;
    // Holding crouch mid-air pulls the player down at least this fast
    public float fastFallSpeed = 15f;

    public AudioClip jumpSound;
    public AudioClip landSound;
    // Shorter airtime than this doesn't count as a landing (filters contact flicker)
    public float minAirTimeForLand = 0.1f;

    private Rigidbody2D rb;
    private float baseGravity;
    private bool grounded;
    private bool crouching;
    private Vector3 standingScale;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private float airTime;

    public bool IsGrounded => grounded;

    // Peak height of a full (held) jump. Valid after Awake.
    public float JumpApexHeight => jumpVelocity * jumpVelocity / (2f * Mathf.Abs(Physics2D.gravity.y) * baseGravity);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        baseGravity = rb.gravityScale;
        standingScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        var manager = RunnerManager.Instance;
        if (manager != null && (!manager.IsRunning || manager.StartedThisFrame)) return;

        if (grounded)
        {
            if (airTime >= minAirTimeForLand) PlaySound(landSound);
            airTime = 0f;
        }
        else
        {
            airTime += Time.deltaTime;
        }

        SetCrouching(CrouchHeld());

        if (crouching && !grounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, -fastFallSpeed));

        if (grounded && !crouching && JumpPressed())
        {
            rb.linearVelocity = new Vector2(0f, jumpVelocity);
            grounded = false;
            PlaySound(jumpSound);
        }

        if (rb.linearVelocity.y < 0f)
            rb.gravityScale = baseGravity * fallGravityMultiplier;
        else if (rb.linearVelocity.y > 0f && !JumpHeld())
            rb.gravityScale = baseGravity * lowJumpGravityMultiplier;
        else
            rb.gravityScale = baseGravity;
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null) audioSource.PlayOneShot(clip);
    }

    // Shrinks/grows from the feet so the player stays planted on the ground,
    // whatever the sprite's pivot is
    void SetCrouching(bool value)
    {
        if (value == crouching) return;
        crouching = value;

        float oldScaleY = transform.localScale.y;
        float newScaleY = standingScale.y * (crouching ? crouchHeightFactor : 1f);
        transform.localScale = new Vector3(standingScale.x, newScaleY, standingScale.z);

        // Sprite's bottom edge relative to its pivot, in unscaled units (0 for a bottom pivot)
        float spriteBottom = spriteRenderer.sprite != null ? spriteRenderer.sprite.bounds.min.y : 0f;
        transform.position += Vector3.up * (spriteBottom * (oldScaleY - newScaleY));
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                grounded = true;
                return;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<RunnerObstacle>() != null && RunnerManager.Instance != null)
            RunnerManager.Instance.GameOver();
    }

    bool JumpPressed()
    {
        return (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame))
            || (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
    }

    bool CrouchHeld()
    {
        return Keyboard.current != null && (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed || Keyboard.current.leftCtrlKey.isPressed);
    }

    bool JumpHeld()
    {
        return (Keyboard.current != null && (Keyboard.current.spaceKey.isPressed || Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed))
            || (Mouse.current != null && Mouse.current.leftButton.isPressed)
            || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed);
    }
}
