using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Currently just handles the players experience/leveling
public class PlayerUIManager : MonoBehaviour
{
    public Player player;
    
    public Image experienceBarFill;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI experienceText;

    void Start()
    {
        player.OnExperienceChanged += UpdateUI;
        UpdateUI();
    }

    void OnDestroy()
    {
        player.OnExperienceChanged -= UpdateUI;
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
}