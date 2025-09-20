using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxLife = 5;
    public int currentLife;
    public Vector3 startPosition;

    private void Start()
    {
        currentLife = maxLife;
        startPosition = transform.position;
    }

    public void TakeDamage(int amount)
    {
        currentLife -= amount;

        if (currentLife < 0)
            currentLife = 0;
    }

    public void ResetLife()
    {
        currentLife = maxLife;
    }
}