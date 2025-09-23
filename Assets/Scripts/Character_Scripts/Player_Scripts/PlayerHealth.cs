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
        startPosition = transform.position;
    }

    private void Awake()
    {
        if (PersistentObjectsManager.persistentPlayerData != null)
        {
            currentLife = PersistentObjectsManager.persistentPlayerData.currentLife;
            shardCount = PersistentObjectsManager.persistentPlayerData.shardCount;
            totalShardsCollected = PersistentObjectsManager.persistentPlayerData.totalShardsCollected;
        }
    }

    private void UpdatePersistentData()
    {
        if (PersistentObjectsManager.persistentPlayerData == null)
            PersistentObjectsManager.persistentPlayerData = new PersistentObjectsManager.PlayerData();

        PersistentObjectsManager.persistentPlayerData.currentLife = currentLife;
        PersistentObjectsManager.persistentPlayerData.shardCount = shardCount;
        PersistentObjectsManager.persistentPlayerData.totalShardsCollected = totalShardsCollected;
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

        UpdatePersistentData();
    }

    public void ResetLife()
    {
        currentLife = maxLife;

        shardCount = 0;
        totalShardsCollected = 0;

        UpdatePersistentData();
    }

    public void CollectShard()
    {
        shardCount++;
        totalShardsCollected++;

        if (shardCount >= shardToLife)
        {
            AddLife(1);
            shardCount = 0;
        }

        UpdatePersistentData();
    }

    private void AddLife(int amount)
    {
        currentLife += amount;
        if (currentLife > maxLife)
            currentLife = maxLife;

        UpdatePersistentData();
    }
}