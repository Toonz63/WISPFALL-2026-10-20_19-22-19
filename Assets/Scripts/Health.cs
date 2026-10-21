using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public float invincibleTime = 0.8f;
    public float killY = -15f;
    public int maxFlasks = 3;
    [Range(0f, 1f)] public float flaskHealPercent = 0.5f;
    public Slider healthBar;
    public TMP_Text flaskText;

    int current;
    int flasks;
    float invincibleUntil;
    Vector3 respawnPoint;
    Rigidbody2D rb;
    bool dead;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    void Start()
    {
        respawnPoint = transform.position;
        current = maxHealth;
        flasks = maxFlasks;
        UpdateBar();
        UpdateFlasks();
    }

    void Update()
    {
        if (dead || Time.timeScale == 0f) return;

        if (transform.position.y < killY) { Die(); return; }

        var kb = Keyboard.current;
        if (kb != null && kb.qKey.wasPressedThisFrame) UseFlask();
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

    public void UseFlask()
    {
        if (dead || flasks <= 0 || current >= maxHealth) return;
        flasks--;
        current = Mathf.Min(maxHealth, current + Mathf.RoundToInt(maxHealth * flaskHealPercent));
        UpdateBar();
        UpdateFlasks();
    }

    // Called by the shrine: full health and full flasks, repeatable
    public void Rest()
    {
        current = maxHealth;
        flasks = maxFlasks;
        UpdateBar();
        UpdateFlasks();
    }

    void UpdateBar()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = current;
        }
    }

    void UpdateFlasks()
    {
        if (flaskText != null) flaskText.text = "Vow Flames: " + flasks + "/" + maxFlasks + "  [Q]";
    }

    void Die()
    {
        if (dead) return;
        dead = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (GameManager.Instance != null) GameManager.Instance.Lose();
    }

    // Called by the Respawn button on the Lose screen
    public void Respawn()
    {
        dead = false;
        transform.position = respawnPoint;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        Rest();
        invincibleUntil = Time.time + 1f;

        Keeper boss = FindFirstObjectByType<Keeper>();
        if (boss != null) boss.ResetBoss();
    }
}