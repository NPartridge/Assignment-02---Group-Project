using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Image about;

    public void PlayButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }

    public void AboutButton()
    {
        background.gameObject.SetActive(false);
        about.gameObject.SetActive(true);
    }

    public void QuitButton()
    {
        Debug.Log("Quit");
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    public void CloseButton()
    {
        about.gameObject.SetActive(false);
        background.gameObject.SetActive(true);   
    }
}
