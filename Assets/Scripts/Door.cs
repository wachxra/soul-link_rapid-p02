using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public string keyID;
    public bool isSceneDoor = false;
    public string sceneName;
    public float moveAmount = 3f;
    public float moveSpeed = 2f;

    [Header("Reference")]
    public GameObject mainDoor;

    [Header("UI")]
    public GameObject promptTextPrefab;
    public float textOffset = 1.5f;

    private GameObject promptTextInstance;
    private bool playerNearby = false;
    private PlayerInventory playerInventory;
    private bool isOpen = false;

    private void Start()
    {
        if (mainDoor == null)
            Debug.LogWarning("Assign Main Door GameObject in Inspector");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen) return;
        if (!other.CompareTag("Player")) return;

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
                OpenDoor();
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

    private void OpenDoor()
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
            if (mainDoor != null)
                StartCoroutine(MoveDoor());
        }
        isOpen = true;
    }

    private IEnumerator MoveDoor()
    {
        Vector3 targetPos = mainDoor.transform.position + new Vector3(0, moveAmount, 0);
        while (Vector3.Distance(mainDoor.transform.position, targetPos) > 0.01f)
        {
            mainDoor.transform.position = Vector3.MoveTowards(mainDoor.transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}