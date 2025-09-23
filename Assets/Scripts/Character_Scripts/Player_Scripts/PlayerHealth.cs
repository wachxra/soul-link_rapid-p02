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

    [Header("Damage Settings")]
    public float damageCooldown = 1f;
    private float lastDamageTime = -999f;

    private void Start()
    {
        currentLife = maxLife;
        startPosition = transform.position;
    }

    public void TakeDamage(int amount, string sourceTag = "Player")
    {
        if (Time.time - lastDamageTime < damageCooldown) return;

        currentLife -= amount;
        lastDamageTime = Time.time;

        if (AudioManager.Instance != null)
        {
            if (sourceTag == "Player")
                AudioManager.Instance.PlaySFX("playerTakeDamage");
            else if (sourceTag == "Spirit")
                AudioManager.Instance.PlaySFX("spiritTakeDamage");
        }

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