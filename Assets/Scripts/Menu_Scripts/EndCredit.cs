using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCredit : MonoBehaviour
{
    [Header("Main Menu Scene Name")]
    public string mainMenuScene = "MainMenu_Scene";

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}