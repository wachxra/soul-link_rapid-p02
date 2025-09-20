using UnityEngine;
using TMPro;

public class Key : MonoBehaviour
{
    [Header("Key Settings")]
    public string keyID;
    public float textDuration = 5f;

    [Header("UI")]
    public GameObject textPrefab;
    public Vector3 textOffset = new Vector3(0, 1.5f, 0);
    public Vector3 textScale = Vector3.one * 0.1f;
    [TextArea] public string pickupMessage = "Collected Key!";

    private static GameObject currentText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.CollectKey(keyID);

                if (textPrefab != null)
                {
                    if (currentText != null) Destroy(currentText);

                    currentText = Instantiate(textPrefab, other.transform.position + textOffset, Quaternion.identity);

                    currentText.transform.rotation = Quaternion.identity;
                    currentText.transform.localScale = textScale;

                    var textMesh = currentText.GetComponent<TextMeshProUGUI>();
                    if (textMesh != null)
                        textMesh.text = pickupMessage;

                    currentText.AddComponent<FollowPlayer>().target = other.transform;
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
            offset = transform.position - target.position;
        }

        private void LateUpdate()
        {
            if (target != null)
                transform.position = target.position + offset;
        }
    }
}