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
        if (collision.CompareTag("Player") || collision.CompareTag("Spirit"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                if (PlayerHealth.totalShardsCollected >= playerHealth.shardToLife)
                    return;

                playerHealth.CollectShard();

                if (TextPopupManager.Instance != null)
                {
                    TextPopupManager.Instance.ShowText(
                        textPrefab,
                        collision.transform,
                        textOffset,
                        textScale,
                        $"{playerHealth.shardCount}/{playerHealth.shardToLife} Shards",
                        textDuration
                    );
                }

                Destroy(gameObject);
            }
        }
    }

}