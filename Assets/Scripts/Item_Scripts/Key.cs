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
                AudioManager.Instance.PlaySFX("keyCollected");

                if (TextPopupManager.Instance != null)
                {
                    TextPopupManager.Instance.ShowText(
                        textPrefab,
                        other.transform,
                        textOffset,
                        textScale,
                        pickupMessage,
                        textDuration
                    );
                }

                Destroy(gameObject);
            }
        }
    }
}