using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Controller : MonoBehaviour
{
    public float maxSpeed = 12f;
    public float acceleration = 80f;
    public float friction = 30f;
    public float braking = 50f;

    public float jumpForce = 15f;
    public float jumpCooldown = 0.1f;
    public float coyoteTime = 0.5f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool jumpRequested;
    private bool isGrounded;
    private float lastYPos;
    private float jumpTimer;
    private float lastGroundedTime;

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
        // Ground detection and coyote time tracking
        bool onFlat = Mathf.Abs(rb.velocity.y) < 0.05f
                      && Mathf.Abs(transform.position.y - lastYPos) < 0.01f;
        if (onFlat)
        {
            isGrounded = true;
            lastGroundedTime = Time.time;
        }
        else
        {
            isGrounded = false;
        }
        lastYPos = transform.position.y;

        // Horizontal movement
        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;

        float accelRate;
        if (moveInput == 0)
        {
            accelRate = friction;            
        }
        else if (Mathf.Sign(moveInput) != Mathf.Sign(rb.velocity.x))
        {
            accelRate = braking;             
        }
        else
        {
            accelRate = acceleration;        
        }

        float movement = speedDiff * accelRate * Time.fixedDeltaTime;
        rb.AddForce(Vector2.right * movement);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);

        // Jump
        bool canUseCoyote = Time.time - lastGroundedTime <= coyoteTime;
        if (jumpRequested && (isGrounded || canUseCoyote) && jumpTimer <= 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpRequested = false;
            jumpTimer = jumpCooldown;
        }
        else
        {
            jumpRequested = false;
        }
    }
}
