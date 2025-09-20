using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HeartUIManager : MonoBehaviour
{
    [Header("Heart Settings")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Scene Settings")]
    public string gameOverSceneName = "GameOver";

    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        UpdateHearts();
    }

    private void Update()
    {
        UpdateHearts();

        if (playerHealth.currentLife <= 0)
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < playerHealth.currentLife)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }
}