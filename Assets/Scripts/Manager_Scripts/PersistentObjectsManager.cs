using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PersistentObjectsManager : MonoBehaviour
{
    [Header("Objects (Not Reset)")]
    public GameObject[] persistentObjects;

    [Header("Scenes (Not Reset)")]
    public string[] allowedScenes = { "Level01", "Level02", "Level03", "Level04" };

    private static bool isInitialized = false;

    [Header("Persistent Player Settings")]
    public GameObject playerPrefab;
    public Vector3 spawnPosition;
    private GameObject playerInstance;

    public static PlayerData persistentPlayerData;

    public class PlayerData
    {
        public int currentLife;
        public int shardCount;
        public int totalShardsCollected;
    }

    private void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (allowedScenes.Contains(currentScene))
        {
            if (!isInitialized)
            {
                if (playerPrefab != null && GameObject.FindGameObjectWithTag("Player") == null)
                {
                    playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

                    var playerHealth = playerInstance.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        if (persistentPlayerData != null)
                        {
                            playerHealth.currentLife = persistentPlayerData.currentLife;
                            playerHealth.shardCount = persistentPlayerData.shardCount;
                            PlayerHealth.totalShardsCollected = persistentPlayerData.totalShardsCollected;
                        }
                    }

                    DontDestroyOnLoad(playerInstance);
                }

                foreach (GameObject obj in persistentObjects)
                    if (obj != null) DontDestroyOnLoad(obj);

                DontDestroyOnLoad(gameObject);
                isInitialized = true;

                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }
        else
        {
            Destroy(gameObject);
        }

        if (playerPrefab != null && GameObject.FindGameObjectWithTag("Player") == null)
        {
            playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

            var playerHealth = playerInstance.GetComponent<PlayerHealth>();
            if (playerHealth != null && persistentPlayerData != null)
            {
                playerHealth.currentLife = persistentPlayerData.currentLife;
                playerHealth.shardCount = persistentPlayerData.shardCount;
                PlayerHealth.totalShardsCollected = PersistentObjectsManager.persistentPlayerData.totalShardsCollected;
            }

            DontDestroyOnLoad(playerInstance);
        }

    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (GameObject obj in persistentObjects)
        {
            if (obj == null) continue;

            var uiManager = obj.GetComponent<UIManager>();
            if (uiManager != null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    uiManager.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
            }

            var spiritScene = obj.GetComponent<SpiritSceneManager>();
            if (spiritScene != null)
            {
                var camera = Camera.main;
                if (camera != null)
                    spiritScene.mainCamera = camera;

                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    spiritScene.hostBody = player.transform;

                var spiritObj = GameObject.FindWithTag("Spirit");
                if (spiritObj != null)
                    spiritScene.spirit = spiritObj.transform;

                spiritScene.RefreshSceneObjects();
            }

            var pauseMenu = obj.GetComponent<PauseMenuManager>();
            if (pauseMenu != null)
            {
                if (pauseMenu.pauseMenuUI == null)
                    pauseMenu.pauseMenuUI = obj.transform.Find("PauseMenuUI")?.gameObject;

                if (pauseMenu.settingsPanel == null)
                    pauseMenu.settingsPanel = obj.transform.Find("SettingsPanel")?.gameObject;

                if (AudioManager.Instance != null)
                    AudioManager.Instance.RegisterUI(
                        pauseMenu.bgSliderSettings,
                        pauseMenu.sfxSliderSettings,
                        pauseMenu.bgToggleSettings,
                        pauseMenu.sfxToggleSettings,
                        pauseMenu.bgToggleLabelSettings,
                        pauseMenu.sfxToggleLabelSettings
                    );
            }
        }
    }

    public static void ResetPersistentObjects()
    {
        isInitialized = false;
    }
}