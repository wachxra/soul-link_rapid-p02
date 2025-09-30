using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public class SpiritSceneManager : MonoBehaviour
{
    [Header("Spirit Mode Settings")]
    [Range(0f, 1f)] public float minDarknessAmount = 0.2f;
    [Range(0f, 1f)] public float maxDarknessAmount = 1f;

    [Header("Glow Settings")]
    public Color glowColor = Color.cyan;
    [Range(0.5f, 10f)] public float glowIntensity = 2f;

    [Header("Camera Settings")]
    public Camera mainCamera;
    public Color humanBgColor = Color.white;
    public Color spiritBgColor = Color.black;

    [Header("References")]
    public Transform hostBody;
    public Transform spirit;
    public float maxDistance = 30f;

    [Header("Smooth Settings")]
    public float colorTransitionSpeed = 5f;

    private SpriteRenderer[] allSprites;
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();
    private bool isSpirit = false;

    private TilemapRenderer[] allTilemaps;
    private Dictionary<TilemapRenderer, Color> originalTilemapColors = new Dictionary<TilemapRenderer, Color>();

    private Light2D[] allLights;

    void Awake()
    {
        RefreshSceneObjects();
    }

    void Start()
    {
        if (mainCamera != null)
            mainCamera.backgroundColor = humanBgColor;
    }

    void Update()
    {
        if (isSpirit && spirit != null && hostBody != null)
        {
            float distance = Vector3.Distance(spirit.position, hostBody.position);
            float t = Mathf.Clamp01(distance / maxDistance);

            float currentMinDarkness = Mathf.Lerp(minDarknessAmount, maxDarknessAmount, t);

            float darkness = Mathf.Lerp(currentMinDarkness, maxDarknessAmount, t);

            if (mainCamera != null)
            {
                Color targetBg = Color.Lerp(humanBgColor, spiritBgColor, darkness);
                mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, targetBg, colorTransitionSpeed * Time.deltaTime);
            }

            foreach (var sr in allSprites)
            {
                if (sr == null) continue;

                int layer = sr.gameObject.layer;

                if (layer == LayerMask.NameToLayer("Box") || layer == LayerMask.NameToLayer("Lever"))
                {
                    continue;
                }
                else if (layer == LayerMask.NameToLayer("BG"))
                {
                    continue;
                }
                else if (layer == LayerMask.NameToLayer("Shard") || layer == LayerMask.NameToLayer("GhostDoor"))
                {
                    Color original = originalColors.ContainsKey(sr) ? originalColors[sr] : Color.white;
                    Vector3 currentVec = new Vector3(sr.color.r, sr.color.g, sr.color.b);
                    Vector3 targetVec = new Vector3(original.r, original.g, original.b);
                    Vector3 newVec = Vector3.MoveTowards(currentVec, targetVec, colorTransitionSpeed * Time.deltaTime);
                    sr.color = new Color(newVec.x, newVec.y, newVec.z, sr.color.a);
                }
                else
                {
                    Color original = originalColors.ContainsKey(sr) ? originalColors[sr] : sr.color;
                    float gray = (1f - darkness) * 0.5f;
                    Color target = new Color(gray, gray, gray, original.a);
                    sr.color = Color.Lerp(sr.color, target, colorTransitionSpeed * Time.deltaTime);
                }
            }

            foreach (var tm in allTilemaps)
            {
                if (tm == null) continue;

                float gray = (1f - darkness) * 0.5f;
                Color target = new Color(gray, gray, gray, tm.material.color.a);
                tm.material.color = Color.Lerp(tm.material.color, target, colorTransitionSpeed * Time.deltaTime);
            }
        }
    }

    public void SetSpiritMode(bool active)
    {
        isSpirit = active;

        if (active)
        {
            if (allLights != null)
            {
                foreach (var light in allLights)
                {
                    if (light == null) continue;
                    if (light.gameObject.layer == LayerMask.NameToLayer("Light"))
                        light.enabled = false;
                }
            }

            if (mainCamera != null)
                mainCamera.backgroundColor = spiritBgColor;

            EnableGlowForBoxesAndLevers();
        }
        else
        {
            DisableGlowForBoxesAndLevers();

            if (allLights != null)
            {
                foreach (var light in allLights)
                {
                    if (light == null) continue;
                    if (light.gameObject.layer == LayerMask.NameToLayer("Light"))
                        light.enabled = true;
                }
            }

            if (allSprites != null)
            {
                foreach (var sr in allSprites)
                {
                    if (sr == null) continue;
                    if (originalColors.ContainsKey(sr))
                        sr.color = originalColors[sr];
                }
            }

            if (allTilemaps != null)
            {
                foreach (var tm in allTilemaps)
                {
                    if (tm == null) continue;
                    if (originalTilemapColors.ContainsKey(tm))
                        tm.material.color = originalTilemapColors[tm];
                }
            }

            if (mainCamera != null)
                mainCamera.backgroundColor = humanBgColor;
        }
    }

    private void EnableGlowForBoxesAndLevers()
    {
        foreach (var sr in allSprites)
        {
            if (sr == null) continue;

            int layer = sr.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Box") || layer == LayerMask.NameToLayer("Lever"))
            {
                Light2D light = sr.GetComponent<Light2D>();
                if (light == null)
                    light = sr.gameObject.AddComponent<Light2D>();

                light.lightType = Light2D.LightType.Point;
                light.color = glowColor;
                light.intensity = glowIntensity;
                light.pointLightOuterRadius = 5f;
                light.enabled = true;

                sr.color = glowColor;
            }
        }
    }

    private void DisableGlowForBoxesAndLevers()
    {
        foreach (var sr in allSprites)
        {
            if (sr == null) continue;

            int layer = sr.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Box") || layer == LayerMask.NameToLayer("Lever"))
            {
                Light2D light = sr.GetComponent<Light2D>();
                if (light != null)
                    light.enabled = false;

                if (originalColors.ContainsKey(sr))
                    sr.color = originalColors[sr];
            }
        }
    }

    public void RefreshSceneObjects()
    {
        allSprites = GameObject.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        originalColors.Clear();
        foreach (var sr in allSprites)
            if (sr != null && !originalColors.ContainsKey(sr))
                originalColors[sr] = sr.color;

        allTilemaps = GameObject.FindObjectsByType<TilemapRenderer>(FindObjectsSortMode.None);
        originalTilemapColors.Clear();
        foreach (var tm in allTilemaps)
            if (tm != null && !originalTilemapColors.ContainsKey(tm))
                originalTilemapColors[tm] = tm.material.color;

        allLights = GameObject.FindObjectsByType<Light2D>(FindObjectsSortMode.None);
    }
}