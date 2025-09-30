using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class SpiritController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;

    [Header("References")]
    public Transform hostBody;
    public float maxRadius = 30f;

    [Header("Camera")]
    public CameraFollowPlayer cameraFollow;

    [Header("Bounce Settings")]
    public float bounceForce = 5f;
    public float bounceDamping = 0.9f;

    [Header("Ground Check")]
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 bounceVelocity;

    [Header("Animate")]
    [SerializeField] private Animator animator;

    private bool isPossessing = false;
    private GameObject possessedBox;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private UIManager ui;

    private Vector2 lastBoxPos;

    private float boxMoveSpeed;

    private bool isFacingRight = true;

    public void SetBoxMoveSpeed(float speed)
    {
        boxMoveSpeed = speed;
    }

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
        if (PauseMenuManager.GameIsPaused) return;

        if (isPossessing)
        {
            HandlePossession();
        }
        else
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (moveInput.x > 0 && !isFacingRight) Flip();
            else if (moveInput.x < 0 && isFacingRight) Flip();

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

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = !isFacingRight;
        }
    }

    private void FixedUpdate()
    {
        if (PauseMenuManager.GameIsPaused) return;

        if (!isPossessing)
        {
            Vector2 moveDir = moveInput.normalized;
            float moveStep = moveSpeed * Time.fixedDeltaTime;
            Vector2 proposedPos = rb.position + moveDir * moveStep;
            animator.SetFloat("Speed", moveDir.magnitude);

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

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("spiritHitBoundary");
            }

            if (bounceVelocity.magnitude > 0.01f)
            {
                rb.MovePosition(rb.position + bounceVelocity * Time.fixedDeltaTime);
                bounceVelocity *= bounceDamping;
            }
        }
        else
        {
            HandleBoxMovement();
        }
    }

    private void HandleBoxMovement()
    {
        if (possessedBox == null) return;

        Rigidbody2D boxRb = possessedBox.GetComponent<Rigidbody2D>();
        if (boxRb != null)
        {
            float move = Input.GetAxisRaw("Horizontal");

            Vector2 vel = boxRb.linearVelocity;
            vel.x = move * boxMoveSpeed;
            boxRb.linearVelocity = vel;

            if (hostBody != null)
            {
                float dist = Vector2.Distance(boxRb.position, hostBody.position);
                if (dist > maxRadius)
                {
                    Vector2 dir = (boxRb.position - (Vector2)hostBody.position).normalized;
                    Vector2 limitedPos = (Vector2)hostBody.position + dir * maxRadius;
                    boxRb.position = new Vector2(limitedPos.x, boxRb.position.y);
                }
            }

            bool isActuallyMoving = Vector2.Distance(boxRb.position, lastBoxPos) > 0.001f;
            bool isOnGround = boxRb.IsTouchingLayers(LayerMask.GetMask("Ground"));

            if (Mathf.Abs(move) > 0.01f && isOnGround && isActuallyMoving)
                AudioManager.Instance?.PlayLoopSFX("boxMoving");
            else
                AudioManager.Instance?.StopLoopSFX("boxMoving");

            lastBoxPos = boxRb.position;
        }
    }

    private void PossessBox(GameObject box)
    {
        isPossessing = true;
        possessedBox = box;
        HideSpirit();

        Rigidbody2D boxRb = possessedBox.GetComponent<Rigidbody2D>();
        if (boxRb != null)
        {
            boxRb.bodyType = RigidbodyType2D.Dynamic;
            boxRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            boxRb.gravityScale = 1f;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("spiritPossessBox");

        if (cameraFollow != null)
            cameraFollow.player = possessedBox.transform;

        SpiritSceneManager sceneManager = Object.FindFirstObjectByType<SpiritSceneManager>();
        if (sceneManager != null)
        {
            sceneManager.spirit = this.transform;
            sceneManager.hostBody = hostBody;
            sceneManager.SetSpiritMode(true);
        }
    }

    public void ReleasePossessedBox()
    {
        if (possessedBox != null)
        {
            Rigidbody2D boxRb = possessedBox.GetComponent<Rigidbody2D>();
            if (boxRb != null)
            {
                boxRb.bodyType = RigidbodyType2D.Kinematic;
                boxRb.linearVelocity = Vector2.zero;
                boxRb.angularVelocity = 0f;
                boxRb.gravityScale = 0f;
            }

            possessedBox = null;
        }

        isPossessing = false;
        ShowSpirit();

        if (cameraFollow != null)
            cameraFollow.player = this.transform;
    }

    private void HandlePossession()
    {
        if (possessedBox == null)
        {
            isPossessing = false;
            ShowSpirit();
            if (cameraFollow != null)
                cameraFollow.player = this.transform;

            SpiritSceneManager sceneManager = Object.FindFirstObjectByType<SpiritSceneManager>();
            if (sceneManager != null) sceneManager.SetSpiritMode(false);

            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isPossessing = false;

            if (possessedBox != null)
            {
                Rigidbody2D possessedRb = possessedBox.GetComponent<Rigidbody2D>();
                if (possessedRb != null)
                    possessedRb.linearVelocity = Vector2.zero;

                transform.position = possessedBox.transform.position;
                AudioManager.Instance?.StopLoopSFX("boxMoving");
            }

            possessedBox = null;
            ShowSpirit();

            if (cameraFollow != null)
                cameraFollow.player = this.transform;

            SpiritSceneManager sceneManager = Object.FindFirstObjectByType<SpiritSceneManager>();
            if (sceneManager != null)
            {
                sceneManager.SetSpiritMode(false);
                sceneManager.spirit = null;
            }
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

    private class BoxCollisionHelper : MonoBehaviour
    {
        private SpiritController controller;
        private bool isGrounded = false;

        public void Setup(SpiritController ctrl, LayerMask mask)
        {
            controller = ctrl;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                if (!isGrounded)
                {
                    isGrounded = true;
                    AudioManager.Instance?.PlaySFX("boxDrop");
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                isGrounded = false;
            }
        }
    }
}