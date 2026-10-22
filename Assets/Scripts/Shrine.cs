using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine;
using UnityEngine.InputSystem;

public class Shrine : MonoBehaviour
{
    public Color activeColor = new Color(1f, 0.85f, 0.3f);
    public Color idleColor = new Color(0.5f, 0.5f, 0.5f);

    Health playerHealth;
    SpriteRenderer[] renderers;
    bool inRange;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        SetColor(idleColor);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (inRange && kb != null && kb.eKey.wasPressedThisFrame)
        {
            playerHealth.SetCheckpoint(transform.position + new Vector3(-1.5f, 1f, 0f));
            playerHealth.Rest();
            SetColor(activeColor);
        }
    }

    void SetColor(Color c)
    {
        foreach (var r in renderers) r.color = c;
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