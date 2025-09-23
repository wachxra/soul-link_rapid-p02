using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class SpiritSceneManager : MonoBehaviour
{
    [Header("Spirit Mode Settings")]
    [Range(0f, 1f)]
    public float minDarknessAmount = 0.2f;
    [Range(0f, 1f)]
    public float maxDarknessAmount = 0.8f;
    public Color glowColor = Color.cyan;

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

    void Awake()
    {
        allSprites = GameObject.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in allSprites)
        {
            if (sr != null && !originalColors.ContainsKey(sr))
                originalColors[sr] = sr.color;
        }

        allTilemaps = GameObject.FindObjectsByType<TilemapRenderer>(FindObjectsSortMode.None);
        foreach (var tm in allTilemaps)
        {
            if (tm != null && !originalTilemapColors.ContainsKey(tm))
                originalTilemapColors[tm] = tm.material.color;
        }
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
            float darkness = Mathf.Lerp(minDarknessAmount, maxDarknessAmount, t);

            if (mainCamera != null)
            {
                Color targetBg = Color.Lerp(humanBgColor, spiritBgColor, darkness);
                Vector3 currentVec = new Vector3(mainCamera.backgroundColor.r, mainCamera.backgroundColor.g, mainCamera.backgroundColor.b);
                Vector3 targetVec = new Vector3(targetBg.r, targetBg.g, targetBg.b);
                Vector3 newVec = Vector3.MoveTowards(currentVec, targetVec, colorTransitionSpeed * Time.deltaTime);
                mainCamera.backgroundColor = new Color(newVec.x, newVec.y, newVec.z, mainCamera.backgroundColor.a);
            }

            foreach (var sr in allSprites)
            {
                if (sr == null) continue;

                int layer = sr.gameObject.layer;
                if (layer == LayerMask.NameToLayer("Box") || layer == LayerMask.NameToLayer("Lever"))
                {
                    Vector3 currentVec = new Vector3(sr.color.r, sr.color.g, sr.color.b);
                    Vector3 targetVec = new Vector3(glowColor.r, glowColor.g, glowColor.b);
                    Vector3 newVec = Vector3.MoveTowards(currentVec, targetVec, colorTransitionSpeed * Time.deltaTime);
                    sr.color = new Color(newVec.x, newVec.y, newVec.z, sr.color.a);
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
                    float gray = (original.r + original.g + original.b) / 3f;
                    gray *= (1f - darkness);
                    Color target = new Color(gray, gray, gray, original.a);

                    Vector3 currentVec = new Vector3(sr.color.r, sr.color.g, sr.color.b);
                    Vector3 targetVec = new Vector3(target.r, target.g, target.b);
                    Vector3 newVec = Vector3.MoveTowards(currentVec, targetVec, colorTransitionSpeed * Time.deltaTime);
                    sr.color = new Color(newVec.x, newVec.y, newVec.z, sr.color.a);
                }
            }

            foreach (var tm in allTilemaps)
            {
                if (tm == null) continue;

                Color original = originalTilemapColors.ContainsKey(tm) ? originalTilemapColors[tm] : Color.white;
                float gray = (original.r + original.g + original.b) / 3f;
                gray *= (1f - darkness);
                Color target = new Color(gray, gray, gray, original.a);

                Vector3 currentVec = new Vector3(tm.material.color.r, tm.material.color.g, tm.material.color.b);
                Vector3 targetVec = new Vector3(target.r, target.g, target.b);
                Vector3 newVec = Vector3.MoveTowards(currentVec, targetVec, colorTransitionSpeed * Time.deltaTime);
                tm.material.color = new Color(newVec.x, newVec.y, newVec.z, tm.material.color.a);
            }
        }
    }

    public void SetSpiritMode(bool active)
    {
        isSpirit = active;

        if (!isSpirit)
        {
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
        else
        {
            if (mainCamera != null)
                mainCamera.backgroundColor = spiritBgColor;
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
    }
}