using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Currently handles speed buff, experience bar, and player level text
public class PlayerUIManager : MonoBehaviour
{
    public Player player;
    
    public Image experienceBarFill;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI experienceText;
    
    public Image speedBuffIcon;

    void Start()
    {
        player.OnExperienceChanged += UpdateUI;
        player.OnSpeedBuffStarted += ShowSpeedBuffIcon;
        player.OnSpeedBuffEnded += HideSpeedBuffIcon;
        
        UpdateUI();
        HideSpeedBuffIcon(); // Just making sure its hidden on start
    }

    void OnDestroy()
    {
        player.OnExperienceChanged -= UpdateUI;
        player.OnSpeedBuffStarted -= ShowSpeedBuffIcon;
        player.OnSpeedBuffEnded -= HideSpeedBuffIcon;
    }

    void UpdateUI()
    {
        UpdateExperienceBar();
        UpdateLevelText();
    }

    void UpdateExperienceBar()
    {
        int xpIntoCurrentLevel = player.ExperienceIntoCurrentLevel;
        int xpRequiredForNextLevel = player.ExperienceRequiredForNextLevel;

        float fillAmount = (float)xpIntoCurrentLevel / xpRequiredForNextLevel;
        experienceBarFill.fillAmount = fillAmount;
        
        experienceText.text = $"{xpIntoCurrentLevel} / {xpRequiredForNextLevel} XP";
    }

    void UpdateLevelText()
    {
        levelText.text = $"Level {player.level}";
    }
    
    void ShowSpeedBuffIcon()
    {
        if (speedBuffIcon != null)
        {
            speedBuffIcon.enabled = true;
        }
    }

    void HideSpeedBuffIcon()
    {
        if (speedBuffIcon != null)
        {
            speedBuffIcon.enabled = false;
        }
    }
}