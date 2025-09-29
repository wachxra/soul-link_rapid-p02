using UnityEngine;
using TMPro;

public class Shard : MonoBehaviour
{
    [Header("UI")]
    public GameObject textPrefab;
    public Vector3 textOffset = new Vector3(0, 1.5f, 0);
    public Vector3 textScale = Vector3.one * 0.1f;
    public float textDuration = 2f;

    private static GameObject currentText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerHealth = null;

        if (collision.CompareTag("Player"))
        {
            playerHealth = collision.GetComponent<PlayerHealth>();
        }
        else if (collision.CompareTag("Spirit"))
        {
            SpiritController spirit = collision.GetComponent<SpiritController>();
            if (spirit != null && spirit.hostBody != null)
                playerHealth = spirit.hostBody.GetComponent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            if (!IsVisibleTo(collision.transform))
                return;

            bool willGainLife = (playerHealth.shardCount + 1 >= playerHealth.shardToLife);

            playerHealth.CollectShard();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("shardCollected");

            if (TextPopupManager.Instance != null)
            {
                string message = willGainLife
                    ? "Life Point +1"
                    : $"{playerHealth.shardCount}/{playerHealth.shardToLife} Shards";

                TextPopupManager.Instance.ShowText(
                    textPrefab,
                    collision.transform,
                    textOffset,
                    textScale,
                    message,
                    textDuration
                );
            }

            Destroy(gameObject);
        }
    }

    private bool IsVisibleTo(Transform target)
    {
        Vector2 start = target.position;
        Vector2 end = transform.position;

        Vector2 dir = (end - start).normalized;
        start += dir * 0.1f;

        int mask = LayerMask.GetMask("Box", "Ground", "Shard");
        RaycastHit2D hit = Physics2D.Linecast(start, end, mask);

        if (hit.collider != null)
        {
            return hit.collider.gameObject == this.gameObject;
        }

        return false;
    }
}