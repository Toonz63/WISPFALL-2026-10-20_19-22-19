using UnityEngine;
using UnityEngine.InputSystem;

public class Shrine : MonoBehaviour
{
    public Color activeColor = new Color(1f, 0.85f, 0.3f);
    public Color idleColor = new Color(0.5f, 0.5f, 0.5f);

    Health playerHealth;
    SpriteRenderer sr;
    bool inRange;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = idleColor;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (inRange && kb != null && kb.eKey.wasPressedThisFrame)
        {
            playerHealth.SetCheckpoint(transform.position + Vector3.up * 1f);
            playerHealth.Rest();
            sr.color = activeColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Health h = other.GetComponent<Health>();
        if (h != null) { playerHealth = h; inRange = true; }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Health>() != null) inRange = false;
    }
}
