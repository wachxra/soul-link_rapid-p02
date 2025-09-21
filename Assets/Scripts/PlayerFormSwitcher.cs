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

    private GameObject spiritInstance;
    private bool isInSpirit = false;

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
        if (spiritPrefab == null) return;

        spiritInstance = Instantiate(spiritPrefab, transform.position, Quaternion.identity);

        SpiritController spiritCtrl = spiritInstance.GetComponent<SpiritController>();

        spiritCtrl.hostBody = this.gameObject.transform;
        spiritCtrl.maxRadius = spiritRadius;
        spiritCtrl.boxMoveSpeed = boxMoveSpeed;

        spiritCtrl.cameraFollow = cameraFollow;

        isInSpirit = true;

        GetComponent<PlayerController>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        if (cameraFollow != null)
            cameraFollow.player = spiritInstance.transform;
    }

    private void ReturnToBody()
    {
        if (spiritInstance != null)
            Destroy(spiritInstance);

        isInSpirit = false;

        GetComponent<PlayerController>().enabled = true;

        if (cameraFollow != null)
            cameraFollow.player = this.transform;
    }
}