using TMPro;
using UnityEngine;
using UnityEngine.UI;

// I used this guide as a reference for implementing the things to do within the editor (very little code is used from this videO)
// HOW TO MAKE A SIMPLE HEALTH BAR IN UNITY! Unity 2D Tutorial https://www.youtube.com/watch?v=0tDPxNB2JNs&t=34s
// The health bar is entirely based on the one in Path of Exile https://www.poewiki.net/wiki/Life
public class HealthBar : MonoBehaviour
{
    public Image healthFillImage;
    private Player player;
    public TextMeshProUGUI healthText;

    void Start()
    {
        player = FindObjectOfType<Player>();
        
        UpdateHealthBar(player.CurrentHealth, player.MaximumHealth);
        
        player.OnHealthChanged += UpdateHealthBar;
    }

    void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        float fillAmount = (float)currentHealth / maxHealth;
        healthFillImage.fillAmount = fillAmount;
        
        UpdateHealthText(currentHealth, maxHealth);
    }
    
    void UpdateHealthText(int currentHealth, int maxHealth)
    {
        healthText.text = $"{currentHealth} / {maxHealth}";
    }
}