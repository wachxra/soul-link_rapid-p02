using UnityEngine;
using TMPro;

public class Shard : MonoBehaviour
{
    [Header("UI")]
    public GameObject textPrefab;
    public Vector3 textOffset = new Vector3(0, 1.5f, 0);
    public Vector3 textScale = Vector3.one * 0.1f;
    public float textDuration = 2f;

    private static GameObject currentText; // เพิ่มตรงนี้

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
                    if (currentText != null) Destroy(currentText); // ลบ text เก่าก่อน

                    currentText = Instantiate(textPrefab, collision.transform.position + textOffset, Quaternion.identity);
                    currentText.transform.rotation = Quaternion.identity;
                    currentText.transform.localScale = textScale;

                    var textMesh = currentText.GetComponent<TextMeshPro>();
                    if (textMesh != null)
                        textMesh.text = $"{playerHealth.shardCount}/{playerHealth.shardToLife} Shards";

                    currentText.AddComponent<FollowPlayer>().target = collision.transform;
                    Destroy(currentText, textDuration);
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
