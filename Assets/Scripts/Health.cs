using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public float invincibleTime = 0.8f;
    public float killY = -15f;
    public int lives = 3;
    public Slider healthBar;
    public TMP_Text livesText;

    int current;
    float invincibleUntil;
    Vector3 respawnPoint;
    Rigidbody2D rb;
    bool dead;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    void Start()
    {
        respawnPoint = transform.position;
        current = maxHealth;
        UpdateBar();
        UpdateLives();
    }

    void Update()
    {
        if (!dead && transform.position.y < killY) Die();
    }

    public void SetCheckpoint(Vector3 pos) { respawnPoint = pos; }

    public void TakeDamage(int amount)
    {
        if (dead || Time.time < invincibleUntil) return;
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

    void UpdateLives()
    {
        if (livesText != null) livesText.text = "Vow Flames: " + lives;
    }

    void Die()
    {
        if (dead) return;
        lives--;
        UpdateLives();

        if (lives <= 0)
        {
            dead = true;
            if (GameManager.Instance != null) GameManager.Instance.Lose();
            return;
        }

        Keeper boss = FindFirstObjectByType<Keeper>();
        if (boss != null) boss.ResetBoss();
        transform.position = respawnPoint;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        current = maxHealth;
        invincibleUntil = Time.time + 1f;
        UpdateBar();
    }
}