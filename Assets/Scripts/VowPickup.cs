using UnityEngine;
using TMPro;
using System.Collections;

public class VowPickup : MonoBehaviour
{
    public TMP_Text message;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null) return;

        pm.canDoubleJump = true;
        if (message != null)
        {
            message.gameObject.SetActive(true);
            pm.StartCoroutine(Hide(message.gameObject));
        }
        Destroy(gameObject);
    }

    static IEnumerator Hide(GameObject g)
    {
        yield return new WaitForSeconds(3f);
        g.SetActive(false);
    }
}