using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI stuff")]
    public GameObject mainMenuPanel;
    public Button playButton;
    public Button quitButton;
    
    [Header("Upgrade menu,")]
    public GameObject upgradePanel;  

    private bool isGamePaused = false;
    private bool wasUpgradeMenuOpen = false;

    private void Start()
    {
        if (mainMenuPanel == null || playButton == null || quitButton == null || upgradePanel == null)
        {
            Debug.LogError("Missing UI ref");
        }
        
        // Hide main menu on init
        mainMenuPanel.SetActive(false);
        
        playButton.onClick.AddListener(() => ResumeGame(true));
        quitButton.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        // Player pauses/unpauses the game by pressing escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (upgradePanel != null && upgradePanel.activeSelf)
            {
                // If upgrade menu is open, switch to pause menu without resuming the game
                SwitchToPauseMenu();
            }
            else
            {
                // Toggle pause as usual
                TogglePause();
            }
        }
    }
    
    public bool IsAnyMenuOpen()
    {
        return mainMenuPanel.activeSelf || upgradePanel.activeSelf;
    }
    
    private void SwitchToPauseMenu()
    {
        if (upgradePanel != null && upgradePanel.activeSelf)
        {
            // Hide the upgrade menu and remember that it was open, the idea is to prevent these order of events from happening
            // Upgrade menu is open -> player opens main menu -> player closes main menu -> the upgrade menu disappears and they can't acquire their upgrade
            upgradePanel.SetActive(false);
            wasUpgradeMenuOpen = true;
        }
        else
        {
            wasUpgradeMenuOpen = false;
        }
        
        // SHow main menu and pause the game
        mainMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        
        isGamePaused = true;
    }
    
    private void TogglePause()
    {
        if (isGamePaused)
        {
            ResumeGame(true);
        }
        else
        {
            PauseGame(true);
        }
    }
    
    public void PauseGame(bool showMenu = true)
    {
        Time.timeScale = 0f; // Pause the game
        if (showMenu)
        {
            mainMenuPanel.SetActive(true); // Show main menu
        }
        isGamePaused = true;
    }
    
    public void ResumeGame(bool hideMenu = true)
    {
        if (hideMenu)
        {
            mainMenuPanel.SetActive(false);
        }

        if (wasUpgradeMenuOpen)
        {
            // Re-open the upgrade menu and also keep game paused
            if (upgradePanel != null)
            {
                upgradePanel.SetActive(true);
            }
            
            Time.timeScale = 0f;
            isGamePaused = true;
        }
        else
        {
            // Resume the game
            Time.timeScale = 1f;
            isGamePaused = false;
        }
        
        wasUpgradeMenuOpen = false;
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game..... ");
    }
}
