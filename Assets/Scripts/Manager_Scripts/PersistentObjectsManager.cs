using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PersistentObjectsManager : MonoBehaviour
{
    [Header("Objects ที่อยากให้ไปต่อในด่านอื่นๆ")]
    public GameObject[] persistentObjects;

    [Header("ชื่อ Scene ที่จะให้คงอยู่ (ไม่ reset)")]
    public string[] allowedScenes = { "Level01", "Level02", "Level03", "Level04" };

    private static bool isInitialized = false;

    void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (allowedScenes.Contains(currentScene))
        {
            if (!isInitialized)
            {
                foreach (GameObject obj in persistentObjects)
                {
                    if (obj != null)
                        DontDestroyOnLoad(obj);
                }

                DontDestroyOnLoad(gameObject);
                isInitialized = true;

                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
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