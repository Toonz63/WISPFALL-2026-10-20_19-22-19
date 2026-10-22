using UnityEngine;
using System.Collections;

public class Husk : MonoBehaviour
{
    enum State { Patrol, Telegraph, Lunge, Recover, Dead }

    public float health = 6;
    public float patrolSpeed = 2f;
    public float patrolRange = 3f;
    public float aggroRange = 4f;
    public float telegraphTime = 0.5f;
    public float lungeSpeed = 9f;
    public float lungeTime = 0.3f;
    public float recoverTime = 0.8f;
    public int damage = 15;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Transform player;
    Collider2D col;
    Color baseColor;
    State state = State.Patrol;
    float startX, stateEnd;
    int dir = 1;
    bool dealtDamage;
    LayerMask groundMask;

    float startHealth;
    Vector3 startPos;
    Vector3 startScale;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        startX = transform.position.x;
        startHealth = health;
        startPos = transform.position;
        startScale = transform.localScale;
        groundMask = LayerMask.GetMask("Ground");
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    // true if the next step would walk off a ledge, into a wall, or onto a spike
    bool Blocked()
    {
        Bounds b = col.bounds;

        // ledge: is there ground just ahead of the feet?
        Vector2 footAhead = new Vector2(b.center.x + dir * (b.extents.x + 0.1f), b.min.y + 0.1f);
        if (!Physics2D.Raycast(footAhead, Vector2.down, 0.6f, groundMask)) return true;

        // wall right in front
        if (Physics2D.Raycast(b.center, new Vector2(dir, 0f), b.extents.x + 0.15f, groundMask)) return true;

        // hazard (spikes) just ahead
        Vector2 p = new Vector2(b.center.x + dir * (b.extents.x + 0.35f), b.min.y + 0.2f);
        foreach (var c in Physics2D.OverlapCircleAll(p, 0.3f))
            if (c.GetComponent<Hazard>() != null) return true;

        return false;
    }

    void Update()
    {
        if (player == null) return;

        switch (state)
        {
            case State.Patrol:
                if (transform.position.x > startX + patrolRange) dir = -1;
                else if (transform.position.x < startX - patrolRange) dir = 1;
                if (Blocked()) dir = -dir;
                rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);

                if (Vector2.Distance(player.position, transform.position) <= aggroRange)
                {
                    state = State.Telegraph;
                    stateEnd = Time.time + telegraphTime;
                    sr.color = new Color(1f, 0.6f, 0.1f); // orange warning
                }
                break;

            case State.Telegraph:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                if (Time.time >= stateEnd)
                {
                    dir = player.position.x >= transform.position.x ? 1 : -1;
                    state = State.Lunge;
                    stateEnd = Time.time + lungeTime;
                    dealtDamage = false;
                    sr.color = Color.red;
                }
                break;

            case State.Lunge:
                float vx = Blocked() ? 0f : dir * lungeSpeed;   // won't charge off a ledge
                rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
                if (!dealtDamage)
                {
                    foreach (var c in Physics2D.OverlapBoxAll(col.bounds.center, col.bounds.size, 0f))
                    {
                        if (c.CompareTag("Player"))
                        {
                            Health h = c.GetComponent<Health>();
                            if (h != null) h.TakeDamage(damage);
                            dealtDamage = true;
                        }
                    }
                }
                if (Time.time >= stateEnd)
                {
                    state = State.Recover;
                    stateEnd = Time.time + recoverTime;
                    sr.color = baseColor;
                }
                break;

            case State.Recover:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                if (Time.time >= stateEnd) state = State.Patrol;
                break;
        }
    }

    public void TakeHit(float dmg, float attackerX)
    {
        AudioManager.Hit();
        if (state == State.Dead) return;
        health -= dmg;

        if (health <= 0) { StartCoroutine(Die()); return; }

        // stagger (also interrupts a telegraph)
        state = State.Recover;
        stateEnd = Time.time + 0.4f;
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        if (state != State.Dead) sr.color = baseColor;
    }

    IEnumerator Die()
    {
        state = State.Dead;
        rb.linearVelocity = Vector2.zero;
        col.enabled = false;
        rb.simulated = false;

        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float k = t / 0.5f;
            sr.color = new Color(1f, 0.95f, 0.6f, 1f - k); // fade into light
            transform.localScale = startScale * (1f + 0.5f * k);
            yield return null;
        }

        gameObject.SetActive(false);   // hidden instead of destroyed, so it can respawn
    }

    // Called when the player respawns
    public void ResetHusk()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        health = startHealth;
        transform.position = startPos;
        transform.localScale = startScale;
        sr.color = baseColor;
        col.enabled = true;
        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        dir = 1;
        state = State.Patrol;
    }
}