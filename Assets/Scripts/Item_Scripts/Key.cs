using UnityEngine;

public class Key : MonoBehaviour
{
    [Header("Key Settings")]
    public string keyID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.CollectKey(keyID);
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("keyCollected");

                Destroy(gameObject);
            }
        }
    }
}