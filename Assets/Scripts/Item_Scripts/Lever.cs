using UnityEngine;
using System.Collections;

[System.Serializable]
public class LeverPlatform
{
    public GameObject platform;
    public Lever.MoveDirection direction = Lever.MoveDirection.Horizontal;
    public float moveAmount = 3f;
    public float moveSpeed = 2f;

    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public bool isMoving = false;
}

public class Lever : MonoBehaviour
{
    private UIManager ui;

    public enum MoveDirection { Horizontal, Vertical }

    [Header("Platform Settings")]
    public LeverPlatform[] leverPlatforms;

    private bool playerNearby = false;

    private void Start()
    {
        ui = FindAnyObjectByType<UIManager>();

        foreach (var lp in leverPlatforms)
        {
            if (lp.platform != null)
                lp.originalPosition = lp.platform.transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Spirit")) return;

        playerNearby = true;
        ui?.SetContext("Lever");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Spirit")) return;

        playerNearby = false;
        ui?.SetContext("IdleSpirit");
    }

    private void Update()
    {
        if (!playerNearby) return;

        if (ui != null)
            ui.SetContext("Lever");

        if (Input.GetKeyDown(KeyCode.E))
        {
            bool anyMoving = false;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("leverToggle");

            foreach (var lp in leverPlatforms)
            {
                if (lp.isMoving)
                {
                    anyMoving = true;
                    break;
                }
            }

            if (!anyMoving)
                ActivateLever();
        }
    }

    public void ActivateLever()
    {
        foreach (var lp in leverPlatforms)
        {
            if (lp.platform == null) continue;

            Vector3 targetPos;

            if (Vector3.Distance(lp.platform.transform.position, lp.originalPosition) < 0.01f)
            {
                targetPos = lp.originalPosition;
                if (lp.direction == MoveDirection.Horizontal)
                    targetPos += new Vector3(lp.moveAmount, 0, 0);
                else
                    targetPos += new Vector3(0, lp.moveAmount, 0);
            }
            else
            {
                targetPos = lp.originalPosition;
            }

            lp.isMoving = true;
            StartCoroutine(MovePlatform(lp, targetPos));
        }

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private IEnumerator MovePlatform(LeverPlatform lp, Vector3 targetPos)
    {
        AudioManager.Instance?.PlayLoopSFX("movingPlatform");

        while (Vector3.Distance(lp.platform.transform.position, targetPos) > 0.01f)
        {
            lp.platform.transform.position = Vector3.MoveTowards(lp.platform.transform.position, targetPos, lp.moveSpeed * Time.deltaTime);
            yield return null;
        }

        lp.platform.transform.position = targetPos;
        lp.isMoving = false;

        AudioManager.Instance?.StopLoopSFX("movingPlatform");
    }
}