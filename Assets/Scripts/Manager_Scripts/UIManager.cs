using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Heart Settings")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Scene Settings")]
    public string gameOverSceneName = "GameOver";

    [Header("Skill UI")]
    public Image skillEIcon;
    public Image skillFIcon;

    [Header("Skill F Sprites")]
    public Sprite fHumanSprite;
    public Sprite fSpiritSprite;

    [Header("Skill E Sprites - Human")]
    public Sprite eHumanIdle;
    public Sprite eHumanLever;
    public Sprite eHumanDoor;

    [Header("Skill E Sprites - Spirit")]
    public Sprite eSpiritIdle;
    public Sprite eSpiritLever;
    public Sprite eSpiritPossessBox;

    [Header("UI Parent")]
    public GameObject uiParent;

    private PlayerHealth playerHealth;
    private bool isSpirit = false;
    private string currentContext = "Idle";

    private void Start()
    {
        SetUIActive(true);

        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        UpdateHearts();
        UpdateSkillIcons();
    }

    private void Update()
    {
        UpdateHearts();

        if (playerHealth.currentLife <= 0)
        {
            playerHealth.ResetLife();
            SceneManager.LoadScene(gameOverSceneName);
        }
    }

    public void SetUIActive(bool active)
    {
        if (uiParent != null)
            uiParent.SetActive(active);
        else
        {
            foreach (var heart in hearts)
                heart.gameObject.SetActive(active);

            if (skillEIcon != null) skillEIcon.gameObject.SetActive(active);
            if (skillFIcon != null) skillFIcon.gameObject.SetActive(active);
        }
    }

    #region Heart UI
    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < playerHealth.currentLife)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
    #endregion

    #region Skill UI
    public void SetForm(bool spirit)
    {
        isSpirit = spirit;
        UpdateSkillIcons();
    }

    public void SetContext(string context)
    {
        currentContext = context;
        UpdateSkillIcons();
    }

    private void UpdateSkillIcons()
    {
        if (skillFIcon != null)
            skillFIcon.sprite = isSpirit ? fSpiritSprite : fHumanSprite;

        if (skillEIcon != null)
        {
            if (!isSpirit)
            {
                switch (currentContext)
                {
                    case "Lever": skillEIcon.sprite = eHumanLever; break;
                    case "Door": skillEIcon.sprite = eHumanDoor; break;
                    default: skillEIcon.sprite = eHumanIdle; break;
                }
            }
            else
            {
                switch (currentContext)
                {
                    case "Lever": skillEIcon.sprite = eSpiritLever; break;
                    case "Box": skillEIcon.sprite = eSpiritPossessBox; break;
                    default: skillEIcon.sprite = eSpiritIdle; break;
                }
            }
        }
    }
    #endregion
}