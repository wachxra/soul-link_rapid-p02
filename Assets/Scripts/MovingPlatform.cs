using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Vector3 lastPosition;
    private Transform playerOnPlatform;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = transform.position - lastPosition;

        if (playerOnPlatform != null)
        {
            playerOnPlatform.position += deltaMovement;
        }

        lastPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerOnPlatform = collision.transform;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerOnPlatform = null;
    }
}