using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject howToPanel;

    void Start()
    {
        Time.timeScale = 1f;
        ShowMain();
    }

    public void StartGame() { SceneManager.LoadScene("Game"); }

    public void ShowHowTo()
    {
        mainPanel.SetActive(false);
        howToPanel.SetActive(true);
    }

    public void ShowMain()
    {
        mainPanel.SetActive(true);
        howToPanel.SetActive(false);
    }

    public void QuitGame() { Application.Quit(); }
}
