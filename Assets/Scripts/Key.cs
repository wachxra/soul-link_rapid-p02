using UnityEngine;
using TMPro;

public class Key : MonoBehaviour
{
    [Header("Key Settings")]
    public string keyID;
    public float textDuration = 5f;

    [Header("UI")]
    public GameObject textPrefab;
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

                    currentText = Instantiate(textPrefab, other.transform.position + Vector3.up * 1.5f, Quaternion.identity);
                    currentText.transform.SetParent(other.transform);

                    var textMesh = currentText.GetComponent<TMPro.TextMeshProUGUI>();
                    if (textMesh != null) textMesh.text = pickupMessage;

                    Destroy(currentText, textDuration);
                }

                Destroy(gameObject);
            }
        }
    }
}