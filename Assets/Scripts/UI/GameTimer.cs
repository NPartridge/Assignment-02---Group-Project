using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private float elapsedTime = 0f;

    void Start()
    {
        UpdateTimerUI();
    }

    void Update()
    {
        // We only update the timer when the game is not paused
        if (Time.timeScale > 0f)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }
    
    public void HideTimerUI()
    {
        timerText.gameObject.SetActive(false);
    }
    
    public void ShowTimerUI()
    {
        timerText.gameObject.SetActive(true);
    }
    
    // This functions code comes from here. I didn't watch the video at all... just skipped to the end to see the code
    // How to make a Countdown Timer in Unity (in minutes + seconds) https://www.youtube.com/watch?v=HmHPJL-OcQE
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime % 1) * 1000);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        //timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds); // Can use this if we want to include milliseconds
    }
    
    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerUI();
        
        ShowTimerUI();
    }
    
    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}