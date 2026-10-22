using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public int damage = 1;
    public float attackCooldown = 0.4f;
    public Vector2 attackOffset = new Vector2(0.9f, 0f);
    public Vector2 attackSize = new Vector2(1.2f, 1f);
    public GameObject slashVisual;

    PlayerMovement move;
    float nextAttack;
    PlayerAnimator pa;

    void Awake()
    {
        move = GetComponent<PlayerMovement>();
        if (slashVisual) slashVisual.SetActive(false);
        pa = GetComponent<PlayerAnimator>();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        var kb = Keyboard.current;
        var mouse = Mouse.current;
        bool pressed = (kb != null && kb.jKey.wasPressedThisFrame) ||
                       (mouse != null && mouse.leftButton.wasPressedThisFrame);

        if (pressed && Time.time >= nextAttack)
        {
            nextAttack = Time.time + attackCooldown;
            Attack();
        }
    }

    void Attack()
    {
        if (pa != null) pa.PlayAttack();
        Vector2 center = (Vector2)transform.position +
                         new Vector2(attackOffset.x * move.Facing, attackOffset.y);

        foreach (var hit in Physics2D.OverlapBoxAll(center, attackSize, 0f))
        {
            Husk h = hit.GetComponent<Husk>();
            if (h != null) h.TakeHit(damage, transform.position.x);

            Keeper k = hit.GetComponent<Keeper>();
            if (k != null) k.TakeHit(damage, transform.position.x);
        }

        if (slashVisual) StartCoroutine(ShowSlash(center));
    }

    IEnumerator ShowSlash(Vector2 center)
    {
        slashVisual.transform.position = center;
        slashVisual.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        slashVisual.SetActive(false);
    }
}