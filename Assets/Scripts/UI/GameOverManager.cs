using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;
    private CanvasGroup canvasGroup;
    public float fadeDuration = 2f;
    
    public TextMeshProUGUI survivalTimeText;
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI experienceText;
    
    private GameTimer gameTimer;

    private MusicPlayer musicPlayer;

    private void Start()
    {
        canvasGroup = gameOverUI.GetComponent<CanvasGroup>();
        
        canvasGroup.alpha = 0f; // Initially the canvas should be invisible (0 alpha)
        gameOverUI.SetActive(false);
        
        gameTimer = FindObjectOfType<GameTimer>();

        // We need to load the game from the main menu for the music player to be present in the game scene
        try
        {
            musicPlayer = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<MusicPlayer>();
        }
        catch (System.Exception)
        {
            Debug.Log("Music Player is NULL");
        }

    }

    public void ShowGameOverUI()
    {
        gameOverUI.SetActive(true);

        // Play the game over music
        musicPlayer.PlayGameOverMusic();

        Player player = FindObjectOfType<Player>();
        
        int experienceCollected = player != null ? player.experience : 0;
        float survivalTime = gameTimer != null ? gameTimer.GetElapsedTime() : 0f;
        int enemiesKilled = Enemy.enemiesKilled;

        survivalTimeText.text = "Time survived: " + FormatTime(survivalTime);
        enemiesKilledText.text = "Enemies killed: " + enemiesKilled;
        experienceText.text = "Experience Collected: " + experienceCollected;
        
        gameTimer.HideTimerUI(); // We disable the actual timer of the game
        
        StartCoroutine(FadeInUI());
    }
    
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return $"{minutes:00}:{seconds:00}";
    }

    private IEnumerator FadeInUI()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            // To fade in we are just increasing the alpha over time. This is basically the opposite to what we are doing with the damage numbers
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        
        // After the window has faded in we can pause the game instead here
        // Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        // Reset everything relevant that we need to here
        Enemy.enemiesKilled = 0;
        gameTimer.ResetTimer();
        gameTimer.ShowTimerUI();

        // Reset time scale (this is only useful if the game has been paused)
        Time.timeScale = 1f;
        // Turn off the game over music
        musicPlayer.AudioSource.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Refresh the scene
    }
}