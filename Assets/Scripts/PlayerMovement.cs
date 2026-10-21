using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public LayerMask groundLayer;
    public int Facing = 1;

    Rigidbody2D rb;
    BoxCollider2D col;

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

        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);
        if (x != 0) Facing = x > 0 ? 1 : -1;

        if (kb.spaceKey.wasPressedThisFrame && IsGrounded())
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    bool IsGrounded()
    {
        Bounds b = col.bounds;
        Vector2 center = new Vector2(b.center.x, b.min.y - 0.05f);
        Vector2 size = new Vector2(b.size.x * 0.9f, 0.1f);
        return Physics2D.OverlapBox(center, size, 0f, groundLayer);
    }
}