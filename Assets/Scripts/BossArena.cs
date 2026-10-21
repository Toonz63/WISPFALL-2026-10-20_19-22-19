using UnityEngine;

using UnityEngine;

public class BossArena : MonoBehaviour
{
    public Keeper keeper;
    public BossDoor door;
    public float triggerOffset = 1.5f;

    Transform player;
    bool started;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null || door == null) return;

        if (keeper == null)            // boss defeated and destroyed
        {
            door.Open();
            enabled = false;
            return;
        }

        if (!started)
        {
            if (player.position.x > door.transform.position.x + triggerOffset)
            {
                started = true;
                keeper.Wake();
                door.Close();
            }
        }
        else if (!keeper.IsAwake)      // boss was reset after you died
        {
            started = false;
            door.Open();
        }
    }
}