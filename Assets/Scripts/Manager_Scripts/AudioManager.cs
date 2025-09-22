using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

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

    [Header("Audio State")]
    public bool bgMuted = false;
    public bool sfxMuted = false;

    [Header("Toggle Labels (optional)")]
    public TMP_Text bgToggleLabel;
    public TMP_Text sfxToggleLabel;

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
        UpdateUI();

        StopLoopSFX("walking");
        StopLoopSFX("boxMoving");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
            currentBGM.mute = bgMuted;
            if (!currentBGM.isPlaying && !bgMuted)
                currentBGM.Play();
        }
    }


    public void SetBGVolume(float volume)
    {
        bgVolume = Mathf.Clamp01(volume);
        if (currentBGM != null) currentBGM.volume = bgVolume;
        UpdateUI();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        foreach (var sfx in sfxDict.Values)
            if (sfx != null) sfx.volume = sfxVolume;
        UpdateUI();
    }

    public void ToggleBG(bool isOn)
    {
        bgMuted = !isOn;
        if (currentBGM != null)
        {
            currentBGM.mute = bgMuted;
            if (!bgMuted && !currentBGM.isPlaying)
                currentBGM.Play();
        }

        if (bgToggleLabel != null)
            bgToggleLabel.text = isOn ? "BGM: ON" : "BGM: OFF";

        UpdateUI();
    }

    public void ToggleSFX(bool isOn)
    {
        sfxMuted = !isOn;
        foreach (var sfx in sfxDict.Values)
            if (sfx != null) sfx.mute = sfxMuted;

        if (sfxToggleLabel != null)
            sfxToggleLabel.text = isOn ? "SFX: ON" : "SFX: OFF";

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (bgSlider != null)
            bgSlider.SetValueWithoutNotify(bgVolume);
        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(sfxVolume);

        if (bgToggle != null)
            bgToggle.SetIsOnWithoutNotify(!bgMuted);

        if (sfxToggle != null)
            sfxToggle.SetIsOnWithoutNotify(!sfxMuted);

        if (bgToggleLabel != null)
            bgToggleLabel.text = !bgMuted ? "BGM: ON" : "BGM: OFF";

        if (sfxToggleLabel != null)
            sfxToggleLabel.text = !sfxMuted ? "SFX: ON" : "SFX: OFF";
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

    public void PlayLoopSFX(string key)
    {
        if (sfxDict.TryGetValue(key, out AudioSource sfx))
        {
            if (sfx != null)
            {
                sfx.volume = sfxVolume;
                sfx.loop = true;
                if (!sfx.isPlaying)
                    sfx.Play();
            }
        }
    }

    public void StopLoopSFX(string key)
    {
        if (sfxDict.TryGetValue(key, out AudioSource sfx))
        {
            if (sfx != null && sfx.isPlaying)
            {
                sfx.Stop();
                sfx.loop = false;
            }
        }
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

    public void RegisterUI(Slider bg, Slider sfx, Toggle bgT, Toggle sfxT, TMP_Text bgLabel = null, TMP_Text sfxLabel = null)
    {
        bgSlider = bg;
        sfxSlider = sfx;
        bgToggle = bgT;
        sfxToggle = sfxT;
        bgToggleLabel = bgLabel;
        sfxToggleLabel = sfxLabel;

        if (bgSlider != null)
            bgSlider.onValueChanged.AddListener(SetBGVolume);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        if (bgToggle != null)
            bgToggle.onValueChanged.AddListener(ToggleBG);
        if (sfxToggle != null)
            sfxToggle.onValueChanged.AddListener(ToggleSFX);

        UpdateUI();
    }
}