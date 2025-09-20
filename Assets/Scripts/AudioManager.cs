using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class SceneBGM
{
    public string sceneName;
    public AudioSource bgmSource;
}

[System.Serializable]
public class SFXEntry
{
    public string key;
    public AudioSource sfxSource;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Volume Settings")]
    [Range(0, 1)] public float bgVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;

    [Header("UI Elements")]
    public Slider bgSlider;
    public Slider sfxSlider;
    public Toggle bgToggle;
    public Toggle sfxToggle;

    [Header("BGM per Scene")]
    public List<SceneBGM> sceneBGMs;

    [Header("SFX Entries")]
    public List<SFXEntry> sfxList;

    private Dictionary<string, AudioSource> sfxDict = new Dictionary<string, AudioSource>();
    private AudioSource currentBGM = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (transform.parent != null) transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (var sfx in sfxList)
        {
            if (sfx.sfxSource != null && !sfxDict.ContainsKey(sfx.key))
                sfxDict.Add(sfx.key, sfx.sfxSource);
        }

        PlayBGMForScene(SceneManager.GetActiveScene().name);

        UpdateVolumes();
        UpdateUI();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (bgSlider != null)
            bgSlider.onValueChanged.AddListener(SetBGVolume);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        if (bgToggle != null)
            bgToggle.onValueChanged.AddListener(ToggleBG);
        if (sfxToggle != null)
            sfxToggle.onValueChanged.AddListener(ToggleSFX);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    public void PlayBGMForScene(string sceneName)
    {
        AudioSource newBGM = null;
        foreach (var bg in sceneBGMs)
        {
            if (bg.sceneName == sceneName && bg.bgmSource != null)
            {
                newBGM = bg.bgmSource;
                break;
            }
        }

        if (currentBGM != null && currentBGM != newBGM)
            currentBGM.Stop();

        currentBGM = newBGM;

        if (currentBGM != null)
        {
            currentBGM.volume = bgVolume;
            currentBGM.mute = (bgToggle != null && !bgToggle.isOn) ? true : false;
            if (!currentBGM.isPlaying)
                currentBGM.Play();
        }
    }

    public void SetBGVolume(float volume)
    {
        bgVolume = Mathf.Clamp01(volume);
        if (currentBGM != null) currentBGM.volume = bgVolume;
        UpdateUI();
    }

    public void ToggleBG(bool isOn)
    {
        if (currentBGM != null)
        {
            currentBGM.mute = !isOn;
            if (isOn && !currentBGM.isPlaying)
                currentBGM.Play();
        }
        UpdateUI();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        foreach (var sfx in sfxDict.Values)
            if (sfx != null) sfx.volume = sfxVolume;
        UpdateUI();
    }

    public void ToggleSFX(bool isOn)
    {
        foreach (var sfx in sfxDict.Values)
            if (sfx != null) sfx.mute = !isOn;
        UpdateUI();
    }

    public void PlaySFX(string key)
    {
        if (sfxDict.TryGetValue(key, out AudioSource sfx))
        {
            if (sfx != null)
            {
                sfx.volume = sfxVolume;
                sfx.Play();
            }
        }
    }

    private void UpdateUI()
    {
        if (bgSlider != null)
            bgSlider.SetValueWithoutNotify(bgVolume);
        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(sfxVolume);
        if (bgToggle != null)
            bgToggle.SetIsOnWithoutNotify(currentBGM != null && !currentBGM.mute);
        if (sfxToggle != null)
            sfxToggle.SetIsOnWithoutNotify(CheckAnySFXEnabled());
    }

    private bool CheckAnySFXEnabled()
    {
        foreach (var sfx in sfxDict.Values)
            if (sfx != null && !sfx.mute)
                return true;
        return false;
    }

    private void UpdateVolumes()
    {
        if (currentBGM != null) currentBGM.volume = bgVolume;
        foreach (var sfx in sfxDict.Values)
            if (sfx != null) sfx.volume = sfxVolume;
    }
}