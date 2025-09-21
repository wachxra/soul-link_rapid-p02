using UnityEngine;
using TMPro;

public class TextPopupManager : MonoBehaviour
{
    public static TextPopupManager Instance;
    private GameObject currentText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowText(GameObject prefab, Transform target, Vector3 offset, Vector3 scale, string message, float duration)
    {
        if (currentText != null) Destroy(currentText);

        currentText = Instantiate(prefab, target.position + offset, Quaternion.identity);
        currentText.transform.localScale = scale;

        var textMesh = currentText.GetComponent<TextMeshPro>();
        if (textMesh != null)
            textMesh.text = message;

        currentText.AddComponent<FollowTarget>().target = target;
        Destroy(currentText, duration);
    }

    private class FollowTarget : MonoBehaviour
    {
        public Transform target;
        private Vector3 offset;

        private void Start()
        {
            if (target != null)
                offset = transform.position - target.position;
        }

        private void LateUpdate()
        {
            if (target != null)
                transform.position = target.position + offset;
        }
    }
}