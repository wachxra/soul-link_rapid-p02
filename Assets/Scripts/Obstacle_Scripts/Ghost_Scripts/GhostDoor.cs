using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Sprite eyesOpenSprite;
    public Sprite eyesClosedSprite;

    [Header("Door Actual State (Damage applies if open)")]
    public bool isDoorActuallyOpen = true;

    [Header("Visuals")]
    public bool playerSeesOpen = true;
    public bool spiritSeesOpen = true;

    [Header("Scene Settings")]
    public string gameOverSceneName = "GameOver_Scene";

    [Header("Detection Settings")]
    public LayerMask obstacleLayer;
    public LayerMask detectLayer;
    public float detectRange = 3f;

    private SpriteRenderer sr;
    private HashSet<PlayerHealth> damagedThisFrame = new HashSet<PlayerHealth>();

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateDoorVisualForViewer(false);
    }

    private void Update()
    {
        damagedThisFrame.Clear();
        HandlePlayerDetection();
    }

    private void HandlePlayerDetection()
    {
        if (!isDoorActuallyOpen) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange, detectLayer);

        foreach (var hit in hits)
        {
            PlayerHealth health = hit.GetComponent<PlayerHealth>();

            if (health == null)
            {
                SpiritController spirit = hit.GetComponent<SpiritController>();
                if (spirit != null && spirit.hostBody != null)
                    health = spirit.hostBody.GetComponent<PlayerHealth>();
            }

            if (health != null && !damagedThisFrame.Contains(health))
            {
                Vector2 dir = (hit.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, distance, obstacleLayer);
                if (rayHit.collider != null) continue;

                ResetAllBoxes();

                string damageSource = "Player";
                SpiritController spiritCtrl = hit.GetComponent<SpiritController>();
                if (spiritCtrl != null && spiritCtrl.hostBody != null)
                    damageSource = "Spirit";

                health.TakeDamage(1, damageSource);

                damagedThisFrame.Add(health);

                if (health.currentLife > 0)
                {
                    PlayerHealth directPlayer = hit.GetComponent<PlayerHealth>();

                    if (directPlayer != null)
                    {
                        hit.transform.position = health.startPosition;
                    }
                    else
                    {
                        SpiritController spirit = hit.GetComponent<SpiritController>();
                        if (spirit != null && spirit.hostBody != null)
                        {
                            spirit.hostBody.position = health.startPosition;

                            PlayerFormSwitcher switcher = spirit.hostBody.GetComponent<PlayerFormSwitcher>();
                            if (switcher != null)
                            {
                                switcher.SendMessage("ReturnToBody", SendMessageOptions.DontRequireReceiver);
                            }

                            Destroy(spirit.gameObject);
                        }
                    }
                }

                if (health.currentLife <= 0)
                {
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.StopLoopSFX("spiritHaunting");
                    }

                    SceneManager.LoadScene(gameOverSceneName);
                }
            }
        }
    }

    public void UpdateDoorVisualForViewer(bool isSpirit)
    {
        if (sr == null) return;

        if (isSpirit)
            sr.sprite = spiritSeesOpen ? eyesOpenSprite : eyesClosedSprite;
        else
            sr.sprite = playerSeesOpen ? eyesOpenSprite : eyesClosedSprite;
    }

    private void ResetAllBoxes()
    {
        BoxReset[] boxes = GameObject.FindObjectsByType<BoxReset>(FindObjectsSortMode.None);
        foreach (var box in boxes)
            box.ResetPosition();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }

    public bool IsActuallyOpen()
    {
        return isDoorActuallyOpen;
    }
}