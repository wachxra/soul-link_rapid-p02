using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class OneWaySideCollision : MonoBehaviour
{
    public LayerMask playerLayer;
    private Collider2D boxCollider;
    private Rigidbody2D rb;

    [Header("Spirit Possession")]
    public bool isPossessed = false;

    void Start()
    {
        boxCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (!isPossessed)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCollider.bounds.center, boxCollider.bounds.size, 0f, playerLayer);
        foreach (var playerCol in hits)
        {
            bool onTop = playerCol.bounds.min.y >= boxCollider.bounds.max.y - 0.05f;
            Physics2D.IgnoreCollision(playerCol, boxCollider, !onTop);
        }
    }
}