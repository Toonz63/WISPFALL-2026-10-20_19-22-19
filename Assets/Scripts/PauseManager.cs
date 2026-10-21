using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject helpPanel;
    bool paused;

    void Start() { SetPaused(false); }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameOver) return;
        var kb = Keyboard.current;
        if (kb != null && kb.escapeKey.wasPressedThisFrame) SetPaused(!paused);
    }

    public void SetPaused(bool value)
    {
        paused = value;
        pausePanel.SetActive(value);
        if (helpPanel != null) helpPanel.SetActive(false);
        Time.timeScale = value ? 0f : 1f;
    }

    public void Resume() { SetPaused(false); }
    public void ToggleHelp()
    {
        bool showHelp = !helpPanel.activeSelf;
        helpPanel.SetActive(showHelp);
        pausePanel.SetActive(!showHelp);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}