using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smooth = 6f;
    public Vector2 offset = new Vector2(0f, 1.5f);

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 goal = new Vector3(target.position.x + offset.x, target.position.y + offset.y, -10f);
        transform.position = Vector3.Lerp(transform.position, goal, smooth * Time.deltaTime);
    }
}