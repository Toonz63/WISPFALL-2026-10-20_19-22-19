using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public float invincibleTime = 0.8f;
    public Slider healthBar;

    int current;
    float invincibleUntil;

    void Start()
    {
        current = maxHealth;
        UpdateBar();
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
        // Temporary: reload the scene. We'll replace this with Lose / respawn later.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}