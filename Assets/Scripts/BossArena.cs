using UnityEngine;

public class BossArena : MonoBehaviour
{
    public Keeper keeper;
    public BossDoor door;
    public float triggerOffset = 1.5f;

    Transform player;
    bool started;
    float side = 1f;   // +1 = boss room is right of the door, -1 = left

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        if (keeper != null && door != null)
            side = keeper.transform.position.x >= door.transform.position.x ? 1f : -1f;
    }

    bool PlayerInside()
    {
        return (player.position.x - door.transform.position.x) * side > triggerOffset;
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
            if (PlayerInside())
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