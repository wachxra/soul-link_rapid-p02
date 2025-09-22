using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpiritController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;

    [Header("References")]
    public Transform hostBody;
    public float maxRadius = 30f;

    [HideInInspector] public float boxMoveSpeed = 5f;

    [Header("Camera")]
    public CameraFollowPlayer cameraFollow;

    [Header("Bounce Settings")]
    public float bounceForce = 5f;
    public float bounceDamping = 0.9f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 bounceVelocity;

    private bool isPossessing = false;
    private GameObject possessedBox;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private UIManager ui;

    private void Start()
    {
        ui = FindAnyObjectByType<UIManager>();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        gameObject.layer = LayerMask.NameToLayer("Spirit");
        rb.gravityScale = 0;
    }

    private void Update()
    {
        if (isPossessing)
        {
            HandlePossession();
        }
        else
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            Collider2D nearbyInteractable = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Box", "Lever"));
            if (nearbyInteractable != null)
            {
                if (nearbyInteractable.CompareTag("Box"))
                {
                    ui?.SetContext("Box");
                }
                else if (nearbyInteractable.CompareTag("Lever"))
                {
                    ui?.SetContext("Lever");
                }
            }
            else
            {
                ui?.SetContext("IdleSpirit");
            }

            if (Input.GetKeyDown(KeyCode.E) && nearbyInteractable != null)
            {
                if (nearbyInteractable.CompareTag("Box"))
                {
                    PossessBox(nearbyInteractable.gameObject);
                    ui?.SetContext("Possessing");
                }
                else if (nearbyInteractable.CompareTag("Lever"))
                {
                    Lever lever = nearbyInteractable.GetComponent<Lever>();
                    if (lever != null)
                    {
                        bool anyMoving = false;
                        foreach (var lp in lever.leverPlatforms)
                        {
                            if (lp.isMoving)
                            {
                                anyMoving = true;
                                break;
                            }
                        }

                        if (!anyMoving)
                            lever.ActivateLever();
                    }
                }
            }
        }

        if (hostBody != null)
        {
            float dist = Vector2.Distance(transform.position, hostBody.position);
            if (dist > maxRadius)
            {
                Vector2 dir = (transform.position - hostBody.position).normalized;
                transform.position = hostBody.position + (Vector3)dir * maxRadius;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isPossessing)
        {
            Vector2 moveDir = moveInput.normalized;
            float moveStep = moveSpeed * Time.fixedDeltaTime;
            Vector2 proposedPos = rb.position + moveDir * moveStep;

            int boundaryMask = LayerMask.GetMask("Boundary");
            Collider2D hit = Physics2D.OverlapCircle(proposedPos, 0.2f, boundaryMask);

            if (hit == null)
            {
                rb.MovePosition(proposedPos);
            }
            else
            {
                Vector2 collisionNormal = (rb.position - (Vector2)hit.ClosestPoint(rb.position)).normalized;
                bounceVelocity = collisionNormal * bounceForce;
            }

            if (bounceVelocity.magnitude > 0.01f)
            {
                rb.MovePosition(rb.position + bounceVelocity * Time.fixedDeltaTime);
                bounceVelocity *= bounceDamping;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void PossessBox(GameObject box)
    {
        isPossessing = true;
        possessedBox = box;
        HideSpirit();

        SpiritSceneManager sceneManager = Object.FindFirstObjectByType<SpiritSceneManager>();

        if (sceneManager != null) sceneManager.SetSpiritMode(true);

        if (cameraFollow != null)
            cameraFollow.player = possessedBox.transform;
    }

    private void HandlePossession()
    {
        if (possessedBox == null)
        {
            isPossessing = false;
            ShowSpirit();
            if (cameraFollow != null)
                cameraFollow.player = this.transform;
            return;
        }

        Rigidbody2D boxRb = possessedBox.GetComponent<Rigidbody2D>();
        if (boxRb != null)
        {
            float move = Input.GetAxisRaw("Horizontal");
            Vector2 proposedPos = boxRb.position + new Vector2(move * boxMoveSpeed * Time.deltaTime, 0);

            if (isPossessing && possessedBox != null)
            {
                if (Mathf.Abs(move) > 0.01f)
                {
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlayLoopSFX("boxMoving");
                }
                else
                {
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.StopLoopSFX("boxMoving");
                }
            }
            else
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.StopLoopSFX("boxMoving");
            }

            if (hostBody != null)
            {
                float dist = Vector2.Distance(proposedPos, hostBody.position);
                if (dist > maxRadius)
                {
                    Vector2 dir = (proposedPos - (Vector2)hostBody.position).normalized;
                    proposedPos = (Vector2)hostBody.position + dir * maxRadius;
                }
            }

            boxRb.MovePosition(proposedPos);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isPossessing = false;

            if (possessedBox != null)
            {
                transform.position = possessedBox.transform.position;
            }

            possessedBox = null;
            ShowSpirit();

            if (cameraFollow != null)
                cameraFollow.player = this.transform;

            SpiritSceneManager sceneManager = Object.FindFirstObjectByType<SpiritSceneManager>();
            if (sceneManager != null) sceneManager.SetSpiritMode(false);
        }
    }

    private void ShowSpirit()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (col != null) col.enabled = true;
    }

    private void HideSpirit()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (col != null) col.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (hostBody != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(hostBody.position, maxRadius);
        }
    }
}