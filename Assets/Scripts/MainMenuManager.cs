using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;

    [Header("Audio Settings Sliders")]
    public UnityEngine.UI.Slider bgSlider;
    public UnityEngine.UI.Slider sfxSlider;
    public UnityEngine.UI.Toggle bgToggle;
    public UnityEngine.UI.Toggle sfxToggle;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            if (bgSlider != null) bgSlider.value = AudioManager.Instance.bgVolume;
            if (sfxSlider != null) sfxSlider.value = AudioManager.Instance.sfxVolume;
            if (bgToggle != null) bgToggle.isOn = !AudioManager.Instance.bgMusicSource.mute;
            if (sfxToggle != null) sfxToggle.isOn = !AudioManager.Instance.sfxSource.mute;
        }

        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void StartGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void OpenCredit(string sceneName)
    {
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

    public void BackFromSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void SetBGVolume(float volume)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetBGVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(volume);
    }

    public void ToggleBGMusic(bool isOn)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ToggleBG(isOn);
    }

    public void ToggleSFX(bool isOn)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ToggleSFX(isOn);
    }
}