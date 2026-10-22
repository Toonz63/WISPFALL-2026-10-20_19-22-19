using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    SpriteRenderer sr;
    PlayerMovement move;
    BoxCollider2D col;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        move = GetComponent<PlayerMovement>();
        col = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("Grounded", IsGrounded());
        sr.flipX = move.Facing < 0;
    }

    bool IsGrounded()
    {
        Bounds b = col.bounds;
        Vector2 center = new Vector2(b.center.x, b.min.y - 0.05f);
        Vector2 size = new Vector2(b.size.x * 0.9f, 0.1f);
        return Physics2D.OverlapBox(center, size, 0f, move.groundLayer);
    }

    public void PlayAttack() { anim.SetTrigger("Attack"); }
}