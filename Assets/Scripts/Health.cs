using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public float invincibleTime = 0.8f;
    public float killY = -15f;
    public Slider healthBar;

    int current;
    float invincibleUntil;
    Vector3 respawnPoint;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        respawnPoint = transform.position;
        current = maxHealth;
        UpdateBar();
    }

    void Update()
    {
        if (transform.position.y < killY) Die();
    }

    public void SetCheckpoint(Vector3 pos)
    {
        respawnPoint = pos;
    }

    public void TakeDamage(int amount)
    {
        if (Time.time < invincibleUntil) return;
        invincibleUntil = Time.time + invincibleTime;
        current = Mathf.Max(0, current - amount);
        UpdateBar();
        if (current == 0) Die();
    }

    public void Heal(int amount)
    {
        current = Mathf.Min(maxHealth, current + amount);
        UpdateBar();
    }

    public void FullHeal()
    {
        current = maxHealth;
        UpdateBar();
    }

    void UpdateBar()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = current;
        }
    }

    void Die()
    {
        transform.position = respawnPoint;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        current = maxHealth;
        invincibleUntil = Time.time + 1f;
        UpdateBar();
    }
}