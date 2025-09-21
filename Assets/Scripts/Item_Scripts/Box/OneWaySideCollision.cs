using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OneWaySideCollision : MonoBehaviour
{
    public LayerMask playerLayer;
    private Collider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCollider.bounds.center, boxCollider.bounds.size, 0f, playerLayer);

        foreach (var playerCol in hits)
        {
            Rigidbody2D playerRb = playerCol.GetComponent<Rigidbody2D>();
            if (playerRb == null) continue;

            if (playerCol.bounds.min.y >= boxCollider.bounds.max.y - 0.05f)
            {
                Physics2D.IgnoreCollision(playerCol, boxCollider, false);
            }
            else
            {
                Physics2D.IgnoreCollision(playerCol, boxCollider, true);
            }

            Rigidbody2D boxRb = boxCollider.GetComponent<Rigidbody2D>();
            if (boxRb != null && !playerCol.CompareTag("Spirit"))
                boxRb.linearVelocity = Vector2.zero;
        }
    }
}