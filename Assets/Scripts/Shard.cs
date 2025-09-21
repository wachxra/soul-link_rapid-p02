using UnityEngine;
using TMPro;

public class Shard : MonoBehaviour
{
    [Header("UI")]
    public GameObject textPrefab;
    public Vector3 textOffset = new Vector3(0, 1.5f, 0);
    public Vector3 textScale = Vector3.one * 0.1f;
    public float textDuration = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.CollectShard();

                if (textPrefab != null)
                {
                    GameObject textObj = Instantiate(textPrefab, collision.transform.position + textOffset, Quaternion.identity);
                    textObj.transform.rotation = Quaternion.identity;
                    textObj.transform.localScale = textScale;

                    var textMesh = textObj.GetComponent<TextMeshProUGUI>();
                    if (textMesh != null)
                        textMesh.text = $"{playerHealth.shardCount}/{playerHealth.shardToLife} Shards";

                    textObj.AddComponent<FollowPlayer>().target = collision.transform;
                    Destroy(textObj, textDuration);
                }

                Destroy(gameObject);
            }
        }
    }

    private class FollowPlayer : MonoBehaviour
    {
        public Transform target;
        private Vector3 offset;

        private void Start()
        {
            if (target != null)
                offset = transform.position - target.position;
        }

        private void LateUpdate()
        {
            if (target != null)
                transform.position = target.position + offset;
        }
    }
}