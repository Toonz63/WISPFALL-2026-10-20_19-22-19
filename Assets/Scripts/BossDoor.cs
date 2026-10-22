using UnityEngine;

public class BossDoor : MonoBehaviour
{
    public float dropDistance = 2f;   // how far above its closed spot it appears (= door height)
    public float speed = 12f;

    Vector3 closedPos, hiddenPos;
    bool closed;
    Collider2D col;
    SpriteRenderer[] renderers;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
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
        SetVisible(true);
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
        SetVisible(false);
        col.enabled = false;
    }

    void SetVisible(bool v)
    {
        foreach (var r in renderers) r.enabled = v;
    }
}