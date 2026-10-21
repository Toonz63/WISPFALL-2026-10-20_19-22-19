using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damage = 20;

    void OnTriggerStay2D(Collider2D other)
    {
        Health h = other.GetComponent<Health>();
        if (h != null) h.TakeDamage(damage);
    }
}