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

    public static int totalShardsCollected = 0;

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

        shardCount = 0;
        totalShardsCollected = 0;
    }

    public void CollectShard()
    {
        if (totalShardsCollected >= shardToLife)
            return;

        shardCount++;
        totalShardsCollected++;

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