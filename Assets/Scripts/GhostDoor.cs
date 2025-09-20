using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject eyesOpenPrefab;
    public GameObject eyesClosedPrefab;
    public float switchInterval = 2f;
    public bool alwaysOpenEyes = false;
    public float detectRange = 3f;

    [Header("Scene Settings")]
    public string gameOverSceneName = "GameOver";
    public string firstSceneName = "Level1";
    public string mainMenuSceneName = "MainMenu";

    [Header("Obstacle Detection")]
    public LayerMask obstacleLayer;

    private GameObject currentDoorInstance;
    private bool isEyesOpen = false;
    private float timer;

    private void Start()
    {
        timer = switchInterval;
        UpdateDoorPrefab();
    }

    private void Update()
    {
        HandleEyeSwitch();
        HandlePlayerDetection();
        UpdateDoorCollider();
    }

    private void HandleEyeSwitch()
    {
        if (!alwaysOpenEyes)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isEyesOpen = !isEyesOpen;
                UpdateDoorPrefab();
                timer = switchInterval;

                if (isEyesOpen)
                {
                    HandlePlayerDetection();
                }
            }
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                bool newEyesOpen = distance <= detectRange;

                if (newEyesOpen != isEyesOpen)
                {
                    isEyesOpen = newEyesOpen;
                    UpdateDoorPrefab();

                    if (isEyesOpen)
                    {
                        HandlePlayerDetection();
                    }
                }
            }
        }
    }

    private void HandlePlayerDetection()
    {
        if (!isEyesOpen) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance <= detectRange && !IsObstacleBlocking())
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(1);

                if (health.currentLife > 0)
                {
                    Vector3 startPos = health.startPosition;
                    player.transform.position = startPos;
                }
                else
                {
                    SceneManager.LoadScene(gameOverSceneName);
                }
            }
        }
    }

    private void UpdateDoorCollider()
    {
        if (currentDoorInstance != null)
        {
            Collider2D doorCollider = currentDoorInstance.GetComponent<Collider2D>();
            if (doorCollider != null)
            {
                doorCollider.enabled = !isEyesOpen || IsObstacleBlocking();
            }
        }
    }

    private void UpdateDoorPrefab()
    {
        if (currentDoorInstance != null)
            Destroy(currentDoorInstance);

        GameObject prefabToInstantiate = isEyesOpen ? eyesOpenPrefab : eyesClosedPrefab;

        if (prefabToInstantiate != null)
        {
            currentDoorInstance = Instantiate(prefabToInstantiate, transform.position, Quaternion.identity);
            currentDoorInstance.transform.SetParent(transform);
        }
    }

    private bool IsObstacleBlocking()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, Vector2.one, 0f, obstacleLayer);
        return hit != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (alwaysOpenEyes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectRange);
        }
        else
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectRange);
        }

        if (currentDoorInstance != null)
        {
            Collider2D col = currentDoorInstance.GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}