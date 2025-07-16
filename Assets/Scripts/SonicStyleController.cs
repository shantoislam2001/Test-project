using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class SonicStyleController : MonoBehaviour
{
 
    public float maxSpeed = 12f;
    public float acceleration = 80f;
    public float friction = 30f;

    public float jumpForce = 15f;
    public float jumpCooldown = 0.1f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool jumpRequested;
    private bool isGrounded;
    private float lastYPos;
    private float jumpTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Keyboard.current.leftArrowKey.isPressed ? -1 :
                    Keyboard.current.rightArrowKey.isPressed ? 1 : 0;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }

        // Cooldown timer
        jumpTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        // Ground detection
        isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.05f && Mathf.Abs(transform.position.y - lastYPos) < 0.01f;
        lastYPos = transform.position.y;

        // Movement
        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float movement = speedDiff * acceleration * Time.fixedDeltaTime;
        rb.AddForce(Vector2.right * movement);

        if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y);

        // Jumping
        if (jumpRequested && isGrounded && jumpTimer <= 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpRequested = false;
            jumpTimer = jumpCooldown;
        }
        else
        {
            jumpRequested = false;
        }

        if (moveInput == 0 && isGrounded)
        {
            rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, 0, friction * Time.fixedDeltaTime), rb.linearVelocity.y);
        }
    }
}
