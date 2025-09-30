using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class OneWaySideCollision : MonoBehaviour
{
    public LayerMask playerLayer;
    private Collider2D boxCollider;
    private Rigidbody2D rb;

    [Header("Spirit Possession")]
    public bool isPossessed = false;

    [Header("Settings")]
    public float topTolerance = 0.1f;

    void Start()
    {
        boxCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (!isPossessed)
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCollider.bounds.center, boxCollider.bounds.size, 0f, playerLayer);

        foreach (var playerCol in hits)
        {
            Rigidbody2D playerRb = playerCol.attachedRigidbody;
            if (playerRb == null) continue;

            float playerBottom = playerCol.bounds.min.y;
            float boxTop = boxCollider.bounds.max.y;

            float tolerance = 0.2f + Mathf.Abs(playerRb.linearVelocity.y) * Time.fixedDeltaTime;

            bool onTop = playerBottom <= boxTop && playerBottom >= boxTop - tolerance;
            bool falling = playerRb.linearVelocity.y <= 0f;

            Physics2D.IgnoreCollision(playerCol, boxCollider, !(onTop && falling));
        }
    }
}