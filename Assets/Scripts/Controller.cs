using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Controller : MonoBehaviour
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
            jumpRequested = true;

        jumpTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        // Ground detection
        isGrounded = Mathf.Abs(rb.velocity.y) < 0.05f
                     && Mathf.Abs(transform.position.y - lastYPos) < 0.01f;
        lastYPos = transform.position.y;

        // Horizontal movement
        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;
        float movement = speedDiff * acceleration * Time.fixedDeltaTime;
        rb.AddForce(Vector2.right * movement);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);

        // Jump
        if (jumpRequested && isGrounded && jumpTimer <= 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpRequested = false;
            jumpTimer = jumpCooldown;
        }
        else
        {
            jumpRequested = false;
        }

        if (moveInput == 0 && isGrounded)
        {
            float newX = Mathf.MoveTowards(rb.velocity.x, 0, friction * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newX, rb.velocity.y);
        }
    }
}
