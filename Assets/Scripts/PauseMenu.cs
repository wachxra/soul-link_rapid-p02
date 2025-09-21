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
    public string level1SceneName = "Level01_Scene";

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
            pauseMenuUI.SetActive(true);

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

        ResetGameData();
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        GameIsPaused = false;
    }

    public void BackToMainMenu(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        ResetGameData();
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        GameIsPaused = false;
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