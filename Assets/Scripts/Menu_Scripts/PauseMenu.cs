using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("Panels")]
    public GameObject pauseMenuUI;
    public GameObject settingsPanel;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu_Scene";
    public string level1SceneName = "Level01";

    [Header("Settings UI Elements")]
    public Slider bgSliderSettings;
    public Slider sfxSliderSettings;
    public Toggle bgToggleSettings;
    public Toggle sfxToggleSettings;
    public TMP_Text bgToggleLabelSettings;
    public TMP_Text sfxToggleLabelSettings;

    private void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Awake()
    {
        if (pauseMenuUI != null)
        {
            Canvas canvas = pauseMenuUI.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 999;
            }
        }

        var eventSystems = GameObject.FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None);

        if (eventSystems.Length > 1)
        {
            for (int i = 1; i < eventSystems.Length; i++)
                Destroy(eventSystems[i].gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            CanvasGroup cg = pauseMenuUI.GetComponent<CanvasGroup>();
            if (cg != null) cg.interactable = true;
        }

        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.RegisterUI(
                bgSliderSettings,
                sfxSliderSettings,
                bgToggleSettings,
                sfxToggleSettings,
                bgToggleLabelSettings,
                sfxToggleLabelSettings
            );
        }
    }

    public void BackFromSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void ResetGameData()
    {
        PersistentObjectsManager.ResetPersistentObjects();

        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.ResetLife();
            player.shardCount = 0;
        }
    }

    public void Retry(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        ResetGameData();
        Time.timeScale = 1f;
        GameIsPaused = false;

        SceneManager.LoadScene(sceneName);
    }

    public void PlayAgain(string sceneName)
    {
        ResetGameData();
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void BackToMainMenu(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        ResetGameData();
        Time.timeScale = 1f;
        GameIsPaused = false;

        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}