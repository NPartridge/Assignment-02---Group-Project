using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;
    private CanvasGroup canvasGroup;
    public float fadeDuration = 2f;

    private void Start()
    {
        canvasGroup = gameOverUI.GetComponent<CanvasGroup>();
        
        canvasGroup.alpha = 0f; // Initially the canvas should be invisible (0 alpha)
        gameOverUI.SetActive(false);
    }

    public void ShowGameOverUI()
    {
        gameOverUI.SetActive(true);
        StartCoroutine(FadeInUI());
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
        // Reset time scale (this is only useful if the game has been paused)
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Refresh the scene
    }
}