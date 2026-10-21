using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject winPanel;
    public GameObject losePanel;
    public bool gameOver;

    void Awake() { Instance = this; }

    void Start()
    {
        Time.timeScale = 1f;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void Win()
    {
        if (gameOver) return;
        gameOver = true;
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Lose()
    {
        if (gameOver) return;
        gameOver = true;
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Respawn()
    {
        gameOver = false;
        losePanel.SetActive(false);
        Time.timeScale = 1f;
        Health h = FindFirstObjectByType<Health>();
        if (h != null) h.Respawn();
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}