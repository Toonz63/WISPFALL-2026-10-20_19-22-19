using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 7f;
    public LayerMask groundLayer;
    public int Facing = 1;
    public bool canDoubleJump = false;

    Rigidbody2D rb;
    BoxCollider2D col;
    bool usedDoubleJump;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        float x = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
        if (x != 0) Facing = x > 0 ? 1 : -1;

        bool grounded = IsGrounded();

        // stop pushing into a wall while in the air, so Wisp slides down instead of sticking
        if (x != 0 && !grounded && TouchingWall(x)) x = 0f;

        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        if (grounded && rb.linearVelocity.y <= 0.1f) usedDoubleJump = false;

        if (kb.spaceKey.wasPressedThisFrame)
        {
            if (grounded) Jump();
            else if (canDoubleJump && !usedDoubleJump)
            {
                usedDoubleJump = true;
                Jump();
            }
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    bool IsGrounded()
    {
        Bounds b = col.bounds;
        Vector2 center = new Vector2(b.center.x, b.min.y - 0.05f);
        Vector2 size = new Vector2(b.size.x * 0.9f, 0.1f);
        return Physics2D.OverlapBox(center, size, 0f, groundLayer);
    }

    bool TouchingWall(float dir)
    {
        Bounds b = col.bounds;
        Vector2 center = new Vector2(b.center.x + Mathf.Sign(dir) * (b.extents.x + 0.03f), b.center.y);
        Vector2 size = new Vector2(0.06f, b.size.y * 0.8f);
        return Physics2D.OverlapBox(center, size, 0f, groundLayer);
    }
}