using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private bool isFacingRight = true;
    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckDistance = 0.1f;
    public float groundCheckWidth = 0.28f;
    public LayerMask groundLayer;
    public LayerMask boxLayer;

    [Header("Jump Tuning")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Animate")]
    [SerializeField] private Animator animator;
    public Animator HostAnimator => animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1.2f;
    }

    private bool wasGrounded;

    private void Update()
    {
        if (PauseMenuManager.GameIsPaused) return;

        moveInput = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        animator.SetBool("isGrounded", isGrounded);

        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        if (AudioManager.Instance != null && CompareTag("Player"))
        {
            if (isGrounded && Mathf.Abs(moveInput) > 0.01f)
                AudioManager.Instance.PlayLoopSFX("walking");
            else
                AudioManager.Instance.StopLoopSFX("walking");
        }

        if (Input.GetButtonDown("Jump") && isGrounded && CompareTag("Player"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            AudioManager.Instance?.PlaySFX("jumpOn");
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();
    }

    private void FixedUpdate()
    {
        if (PauseMenuManager.GameIsPaused) return;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        bool currentlyGrounded = IsGrounded();

        if (!wasGrounded && currentlyGrounded)
        {
            AudioManager.Instance?.PlaySFX("jumpOff");
        }

        wasGrounded = currentlyGrounded;
        isGrounded = currentlyGrounded;
    }

    private bool IsGrounded()
    {
        int mask = groundLayer | boxLayer;

        Vector3 center = groundCheck.position;
        Vector3 left = center + Vector3.left * groundCheckWidth;
        Vector3 right = center + Vector3.right * groundCheckWidth;

        bool hitCenter = Physics2D.Raycast(center, Vector2.down, groundCheckDistance, mask);
        bool hitLeft = Physics2D.Raycast(left, Vector2.down, groundCheckDistance, mask);
        bool hitRight = Physics2D.Raycast(right, Vector2.down, groundCheckDistance, mask);

        return hitCenter || hitLeft || hitRight;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Vector3 center = groundCheck.position;
            Vector3 left = center + Vector3.left * groundCheckWidth;
            Vector3 right = center + Vector3.right * groundCheckWidth;

            Gizmos.DrawLine(center, center + Vector3.down * groundCheckDistance);
            Gizmos.DrawLine(left, left + Vector3.down * groundCheckDistance);
            Gizmos.DrawLine(right, right + Vector3.down * groundCheckDistance);
        }
    }
}