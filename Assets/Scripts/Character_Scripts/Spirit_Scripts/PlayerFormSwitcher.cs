using System.Linq;
using UnityEngine;

public class PlayerFormSwitcher : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject spiritPrefab;

    [Header("Settings")]
    public KeyCode switchKey = KeyCode.F;
    public float spiritRadius = 5f;
    public float boxMoveSpeed = 5f;

    [Header("Camera")]
    public CameraFollowPlayer cameraFollow;

    [Header("Spirit Scene")]
    public SpiritSceneManager spiritScene;

    private GameObject spiritInstance;
    private bool isInSpirit = false;

    private UIManager ui;

    void Start()
    {
        ui = FindAnyObjectByType<UIManager>();

        Collider2D playerCol = GetComponent<Collider2D>();
        Collider2D[] boxCols = GameObject.FindGameObjectsWithTag("Box")
                                       .Select(b => b.GetComponent<Collider2D>())
                                       .ToArray();

        foreach (var boxCol in boxCols)
        {
            if (boxCol != null)
                Physics2D.IgnoreCollision(playerCol, boxCol, true);
        }

        UpdateAllDoorsVisual(false);

        if (spiritScene != null)
            spiritScene.SetSpiritMode(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            if (!isInSpirit)
                BecomeSpirit();
            else
                ReturnToBody();
        }
    }

    private void BecomeSpirit()
    {
        ui?.SetForm(true);

        if (spiritPrefab == null) return;

        AudioManager.Instance?.PlaySFX("spiritOn");
        AudioManager.Instance?.StopLoopSFX("walking");

        spiritInstance = Instantiate(spiritPrefab, transform.position, Quaternion.identity);

        SpiritController spiritCtrl = spiritInstance.GetComponent<SpiritController>();
        spiritCtrl.hostBody = this.transform;
        spiritCtrl.maxRadius = spiritRadius;
        spiritCtrl.cameraFollow = cameraFollow;

        spiritCtrl.SetBoxMoveSpeed(boxMoveSpeed);

        if (spiritScene != null)
        {
            spiritScene.spirit = spiritInstance.transform;
            spiritScene.hostBody = this.transform;
            spiritScene.SetSpiritMode(true);
        }

        isInSpirit = true;

        GetComponent<PlayerController>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        if (cameraFollow != null)
            cameraFollow.player = spiritInstance.transform;

        UpdateAllDoorsVisual(true);

        AudioManager.Instance?.PlayLoopSFX("spiritHaunting");
    }


    private void ReturnToBody()
    {
        ui?.SetForm(false);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance?.StopLoopSFX("boxMoving");
            AudioManager.Instance.StopLoopSFX("spiritHaunting");
            AudioManager.Instance.PlaySFX("spiritOff");
        }

        if (spiritInstance != null)
        {
            SpiritController spiritCtrl = spiritInstance.GetComponent<SpiritController>();
            if (spiritCtrl != null)
            {
                spiritCtrl.ReleasePossessedBox();
            }

            Destroy(spiritInstance);
        }

        isInSpirit = false;

        GetComponent<PlayerController>().enabled = true;

        if (cameraFollow != null)
            cameraFollow.player = this.transform;

        UpdateAllDoorsVisual(false);

        if (spiritScene != null)
        {
            spiritScene.SetSpiritMode(false);
            spiritScene.spirit = null;
        }
    }

    private void UpdateAllDoorsVisual(bool isSpirit)
    {
        GhostDoor[] doors = GameObject.FindObjectsByType<GhostDoor>(FindObjectsSortMode.None);
        foreach (var door in doors)
            door.UpdateDoorVisualForViewer(isSpirit);
    }
}