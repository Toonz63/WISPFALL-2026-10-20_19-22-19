using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerCombat : MonoBehaviour
{
    public float damage = 1f;            // base damage
    public float killBonus = 0.1f;       // gained per Husk kill
    public float attackCooldown = 0.4f;
    public Vector2 attackOffset = new Vector2(0.9f, 0f);
    public Vector2 attackSize = new Vector2(1.2f, 1f);
    public float slashDuration = 0.12f;
    public GameObject slashVisual;
    public TMP_Text damageText;          // optional HUD text

    PlayerMovement move;
    PlayerAnimator pa;
    float nextAttack;
    float slashEnd;
    float bonus;
    float savedBonus;
    Vector3 slashBaseScale = Vector3.one;

    float CurrentDamage { get { return damage + bonus; } }

    void Awake()
    {
        move = GetComponent<PlayerMovement>();
        pa = GetComponent<PlayerAnimator>();
        if (slashVisual)
        {
            slashBaseScale = slashVisual.transform.localScale;
            slashVisual.SetActive(false);
        }
    }

    void Start() { UpdateText(); }

    public void AddKillBonus()
    {
        bonus += killBonus;
        UpdateText();
    }

    public void SaveBonus() { savedBonus = bonus; }

    public void ResetBonus()
    {
        bonus = savedBonus;
        UpdateText();
    }

    void UpdateText()
    {
        if (damageText != null) damageText.text = "Attack: " + CurrentDamage.ToString("0.0");
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

    void LateUpdate()
    {
        if (slashVisual == null || !slashVisual.activeSelf) return;

        if (Time.time >= slashEnd)
        {
            slashVisual.SetActive(false);
            return;
        }

        slashVisual.transform.position = SlashCenter();
        Vector3 s = slashBaseScale;
        s.x = Mathf.Abs(s.x) * move.Facing;
        slashVisual.transform.localScale = s;
    }

    Vector2 SlashCenter()
    {
        return (Vector2)transform.position +
               new Vector2(attackOffset.x * move.Facing, attackOffset.y);
    }

    void Attack()
    {
        AudioManager.Swing();
        if (pa != null) pa.PlayAttack();

        Vector2 center = SlashCenter();

        foreach (var hit in Physics2D.OverlapBoxAll(center, attackSize, 0f))
        {
            Husk h = hit.GetComponent<Husk>();
            if (h != null) h.TakeHit(CurrentDamage, transform.position.x);

            Keeper k = hit.GetComponent<Keeper>();
            if (k != null) k.TakeHit(CurrentDamage, transform.position.x);
        }

        if (slashVisual)
        {
            slashEnd = Time.time + slashDuration;
            slashVisual.SetActive(true);
            slashVisual.transform.position = center;
        }
    }
}