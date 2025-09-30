using UnityEngine;
using UnityEngine.SceneManagement;

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

    private UIManager ui;

    private bool playerNearby = false;
    private PlayerInventory playerInventory;
    private bool isOpen = false;
    private SpriteRenderer doorRenderer;
    private Collider2D doorCollider;

    private void Start()
    {
        ui = FindAnyObjectByType<UIManager>();

        if (mainDoor == null)
        {
            Debug.LogWarning("Assign Main Door GameObject in Inspector");
            return;
        }

        doorRenderer = mainDoor.GetComponent<SpriteRenderer>();
        doorCollider = mainDoor.GetComponent<Collider2D>();

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

        ui?.SetContext("Door");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        playerInventory = null;

        ui?.SetContext("Idle");
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

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("normalDoorToggle");
            }
            else
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("doorLocked");
            }
        }
    }

    private void ToggleDoor()
    {
        if (isSceneDoor)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("moveToNextLevel");

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopLoopSFX("spiritHaunting");
                AudioManager.Instance.StopLoopSFX("walking");
                AudioManager.Instance?.StopLoopSFX("boxMoving");
            }

            if (!string.IsNullOrEmpty(sceneName))
                SceneManager.LoadScene(sceneName);
        }
        else
        {
            if (doorRenderer == null || doorCollider == null) return;

            isOpen = !isOpen;

            if (isOpen && openSprite != null)
            {
                doorRenderer.sprite = openSprite;
                doorCollider.enabled = false;
            }
            else if (!isOpen && closedSprite != null)
            {
                doorRenderer.sprite = closedSprite;
                doorCollider.enabled = true;
            }
        }
    }
}