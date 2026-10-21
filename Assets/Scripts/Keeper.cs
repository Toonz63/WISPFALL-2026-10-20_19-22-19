using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Keeper : MonoBehaviour
{
    enum State { Dormant, Chase, Telegraph, Lunge, Recover, Dead }

    public int maxHealth = 15;
    public float chaseSpeed = 2.5f;
    public float attackRange = 4f;
    public float telegraphTime = 0.7f;
    public float lungeSpeed = 12f;
    public float lungeTime = 0.35f;
    public float recoverTime = 1.2f;
    public int damage = 25;
    public Slider bossBar;
    public GameObject bossBarRoot;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Transform player;
    Color baseColor;
    Color originalColor;
    State state = State.Dormant;
    int health;
    float stateEnd;
    int dir = 1;
    bool dealtDamage;
    bool phase2;
    Vector3 startPos;

    public bool IsAwake
    {
        get { return state != State.Dormant && state != State.Dead; }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        originalColor = sr.color;
        startPos = transform.position;
        health = maxHealth;
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        if (bossBarRoot != null) bossBarRoot.SetActive(false);
    }

    public void Wake()
    {
        if (state != State.Dormant) return;
        state = State.Chase;
        if (bossBarRoot != null) bossBarRoot.SetActive(true);
        UpdateBar();
    }

    void Update()
    {
        if (player == null || state == State.Dead) return;
        float dist = Vector2.Distance(player.position, transform.position);
        float mult = phase2 ? 1.4f : 1f;

        switch (state)
        {
            case State.Dormant:
                break;   // sleeps until BossArena wakes it

            case State.Chase:
                dir = player.position.x >= transform.position.x ? 1 : -1;
                rb.linearVelocity = new Vector2(dir * chaseSpeed * mult, rb.linearVelocity.y);
                if (dist <= attackRange)
                {
                    state = State.Telegraph;
                    stateEnd = Time.time + telegraphTime / mult;
                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                    sr.color = new Color(1f, 0.6f, 0.1f);
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
                    foreach (var c in Physics2D.OverlapCircleAll(transform.position, 1.8f))
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
                    stateEnd = Time.time + recoverTime / mult;
                    sr.color = baseColor;
                }
                break;

            case State.Recover:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                if (Time.time >= stateEnd) state = State.Chase;
                break;
        }
    }

    public void TakeHit(int dmg, float attackerX)
    {
        if (state == State.Dead) return;
        Wake();

        health -= dmg;
        UpdateBar();

        if (!phase2 && health <= maxHealth / 2)
        {
            phase2 = true;
            baseColor = new Color(0.6f, 0.1f, 0.3f);
        }

        if (health <= 0) { StartCoroutine(Die()); return; }
        StartCoroutine(Flash());
    }

    void UpdateBar()
    {
        if (bossBar != null)
        {
            bossBar.maxValue = maxHealth;
            bossBar.value = health;
        }
    }

    IEnumerator Flash()
    {
        Color keep = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.08f);
        if (state != State.Dead) sr.color = (state == State.Lunge || state == State.Telegraph) ? keep : baseColor;
    }

    IEnumerator Die()
    {
        state = State.Dead;
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        rb.simulated = false;

        float t = 0f;
        Vector3 startScale = transform.localScale;
        while (t < 1f)
        {
            t += Time.deltaTime;
            sr.color = new Color(1f, 0.95f, 0.6f, 1f - t);
            transform.localScale = startScale * (1f + 0.4f * t);
            yield return null;
        }

        if (GameManager.Instance != null) GameManager.Instance.Win();
        Destroy(gameObject);
    }

    public void ResetBoss()
    {
        if (state == State.Dead) return;
        StopAllCoroutines();
        health = maxHealth;
        phase2 = false;
        baseColor = originalColor;
        sr.color = originalColor;
        transform.position = startPos;
        rb.linearVelocity = Vector2.zero;
        state = State.Dormant;
        if (bossBarRoot != null) bossBarRoot.SetActive(false);
        UpdateBar();
    }
}