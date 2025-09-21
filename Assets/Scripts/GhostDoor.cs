using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    [Header("Obstacle Detection")]
    public LayerMask obstacleLayer;
    public LayerMask detectLayer;

    private GameObject currentDoorInstance;
    private bool isEyesOpen = false;
    private float timer;

    private HashSet<PlayerHealth> damagedThisFrame = new HashSet<PlayerHealth>();

    private void Start()
    {
        timer = switchInterval;
        UpdateDoorPrefab();
    }

    private void Update()
    {
        damagedThisFrame.Clear();

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
                }
            }
        }
    }

    private void HandlePlayerDetection()
    {
        if (!isEyesOpen) return;

        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, detectRange, detectLayer);

        foreach (Collider2D hit in potentialTargets)
        {
            PlayerHealth health = hit.GetComponent<PlayerHealth>();

            if (health == null)
            {
                SpiritController spirit = hit.GetComponent<SpiritController>();
                if (spirit != null && spirit.hostBody != null)
                {
                    health = spirit.hostBody.GetComponent<PlayerHealth>();
                }
            }

            if (health != null && !damagedThisFrame.Contains(health))
            {
                Vector2 dir = (hit.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, distance, obstacleLayer);
                if (rayHit.collider != null)
                {
                    continue;
                }

                health.TakeDamage(1);
                damagedThisFrame.Add(health);

                if (health.currentLife > 0)
                {
                    if (hit.GetComponent<PlayerHealth>() != null)
                        hit.transform.position = health.startPosition;
                }
                else
                {
                    BoxReset[] boxes = GameObject.FindObjectsByType<BoxReset>(FindObjectsSortMode.None);
                    foreach (var box in boxes)
                    {
                        box.ResetPosition();
                    }

                    SceneManager.LoadScene(gameOverSceneName);
                }

                BoxReset[] allBoxes = GameObject.FindObjectsByType<BoxReset>(FindObjectsSortMode.None);
                foreach (var box in allBoxes)
                {
                    box.ResetPosition();
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
        Gizmos.color = alwaysOpenEyes ? Color.red : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectRange);

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