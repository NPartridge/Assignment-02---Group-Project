using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void PlayButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }

    public void AboutButton()
    {
        Debug.Log("About");
    }

    public void QuitButton()
    {
        Debug.Log("Quit");
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("Menu");
    }
}
