using UnityEngine;

public class BossDoor : MonoBehaviour
{
    public float dropDistance = 2f;   // how far above its closed spot it appears (= door height)
    public float speed = 12f;

    Vector3 closedPos, hiddenPos;
    bool closed;
    Collider2D col;
    SpriteRenderer sr;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        closedPos = transform.position;
        hiddenPos = closedPos + Vector3.up * dropDistance;
        Hide();
    }

    void Update()
    {
        if (!closed) return;
        transform.position = Vector3.MoveTowards(transform.position, closedPos, speed * Time.deltaTime);
    }

    public void Close()
    {
        if (closed) return;
        closed = true;
        transform.position = hiddenPos;
        sr.enabled = true;
        col.enabled = true;
    }

    public void Open()
    {
        closed = false;
        Hide();
    }

    void Hide()
    {
        transform.position = hiddenPos;
        sr.enabled = false;
        col.enabled = false;
    }
}