using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public string keyID;
    public bool isSceneDoor = false;
    public string sceneName;

    [Header("Reference")]
    public GameObject mainDoor;
    public Sprite closedSprite;
    public Sprite openSprite;

    [Header("UI")]
    public GameObject promptTextPrefab;
    public float textOffset = 1.5f;

    private GameObject promptTextInstance;
    private bool playerNearby = false;
    private PlayerInventory playerInventory;
    private bool isOpen = false;
    private SpriteRenderer doorRenderer;
    private Collider2D doorCollider;

    private void Start()
    {
        if (mainDoor == null)
            Debug.LogWarning("Assign Main Door GameObject in Inspector");

        if (mainDoor != null)
        {
            doorRenderer = mainDoor.GetComponent<SpriteRenderer>();
            doorCollider = mainDoor.GetComponent<Collider2D>();
        }

        if (doorRenderer != null && closedSprite != null)
            doorRenderer.sprite = closedSprite;

        if (doorCollider != null)
            doorCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (isOpen) return;

        playerNearby = true;
        playerInventory = other.GetComponent<PlayerInventory>();
        ShowPrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        playerInventory = null;
        HidePrompt();
    }

    private void Update()
    {
        if (isOpen || !playerNearby || playerInventory == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerInventory.gameObject.CompareTag("Spirit")) return;

            if (playerInventory.HasKey(keyID))
            {
                playerInventory.UseKey(keyID);
                ToggleDoor();
                AudioManager.Instance.PlaySFX("normalDoorToggle");
                HidePrompt();
            }
            else
            {
                if (promptTextInstance != null)
                {
                    TextMesh textMesh = promptTextInstance.GetComponent<TextMesh>();
                    if (textMesh != null)
                        textMesh.text = "Need the correct key!";
                }

                AudioManager.Instance.PlaySFX("doorLocked");
            }
        }
    }

    private void ShowPrompt()
    {
        if (promptTextPrefab == null) return;

        if (promptTextInstance != null) Destroy(promptTextInstance);

        promptTextInstance = Instantiate(promptTextPrefab, transform.position + Vector3.up * textOffset, Quaternion.identity);
        promptTextInstance.transform.SetParent(null);

        TextMesh textMesh = promptTextInstance.GetComponent<TextMesh>();
        if (textMesh != null)
        {
            textMesh.text = "E to unlock";
        }
    }

    private void HidePrompt()
    {
        if (promptTextInstance != null)
        {
            Destroy(promptTextInstance);
            promptTextInstance = null;
        }
    }

    private void ToggleDoor()
    {
        if (isSceneDoor)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        else
        {
            if (doorRenderer == null || doorCollider == null) return;

            isOpen = !isOpen;

            if (isOpen && openSprite != null)
            {
                doorRenderer.sprite = openSprite;
                doorCollider.enabled = false;

                HidePrompt();

                if (promptTextPrefab != null)
                {
                    GameObject unlockedText = Instantiate(
                        promptTextPrefab,
                        transform.position + Vector3.up * textOffset,
                        Quaternion.identity
                    );

                    var textMesh = unlockedText.GetComponent<TextMesh>();
                    if (textMesh != null)
                        textMesh.text = "Unlocked!";

                    Destroy(unlockedText, 1.5f);
                }
            }
            else if (!isOpen && closedSprite != null)
            {
                doorRenderer.sprite = closedSprite;
                doorCollider.enabled = true;
            }
        }
    }
}