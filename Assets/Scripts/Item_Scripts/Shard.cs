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
            if (IsBlockedByBox(collision.transform))
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

    private bool IsBlockedByBox(Transform target)
    {
        Vector2 start = target.position;
        Vector2 end = transform.position;
        Vector2 dir = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        start += dir * 0.1f;

        int mask = LayerMask.GetMask("Box");
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, dir, distance, mask);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                return true;
            }
        }

        return false;
    }
}