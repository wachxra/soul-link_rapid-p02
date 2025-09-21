using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxLife = 5;
    public int currentLife;

    [Header("Shard Settings")]
    public int shardCount = 0;
    public int shardToLife = 4;

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

    public void CollectShard()
    {
        shardCount++;
        if (shardCount >= shardToLife)
        {
            AddLife(1);
            shardCount = 0;
        }
    }

    private void AddLife(int amount)
    {
        currentLife += amount;
        if (currentLife > maxLife)
            currentLife = maxLife;
    }
}