using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;

    public Slider bgSlider;
    public Slider sfxSlider;
    public Toggle bgToggle;
    public Toggle sfxToggle;
    public TMP_Text bgLabel;
    public TMP_Text sfxLabel;

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.RegisterUI(
                bgSlider,
                sfxSlider,
                bgToggle,
                sfxToggle,
                bgLabel,
                sfxLabel
            );
        }
    }

    public void StartGame(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void BackFromSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
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