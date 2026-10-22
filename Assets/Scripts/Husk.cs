using UnityEngine;
using System.Collections;

public class Husk : MonoBehaviour
{
    enum State { Patrol, Telegraph, Lunge, Recover, Dead }

    public int health = 3;
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
    Color baseColor;
    State state = State.Patrol;
    float startX, stateEnd;
    int dir = 1;
    bool dealtDamage;

    int startHealth;
    Vector3 startPos;
    Vector3 startScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        startX = transform.position.x;
        startHealth = health;
        startPos = transform.position;
        startScale = transform.localScale;
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        switch (state)
        {
            case State.Patrol:
                if (transform.position.x > startX + patrolRange) dir = -1;
                else if (transform.position.x < startX - patrolRange) dir = 1;
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
                rb.linearVelocity = new Vector2(dir * lungeSpeed, rb.linearVelocity.y);
                if (!dealtDamage)
                {
                    foreach (var c in Physics2D.OverlapCircleAll(transform.position, 0.8f))
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

    public void TakeHit(int dmg, float attackerX)
    {
        if (state == State.Dead) return;
        health -= dmg;

        if (health <= 0) { StartCoroutine(Die()); return; }

        // stagger + knockback (also interrupts a telegraph)
        state = State.Recover;
        stateEnd = Time.time + 0.4f;
        float push = transform.position.x >= attackerX ? 1f : -1f;
        rb.linearVelocity = new Vector2(push * 4f, 3f);
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
        GetComponent<Collider2D>().enabled = false;
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

        gameObject.SetActive(false);   // hidden instead of destroyed, so it can come back
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
        GetComponent<Collider2D>().enabled = true;
        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        dir = 1;
        state = State.Patrol;
    }
}