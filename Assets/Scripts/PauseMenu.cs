using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton; 
    
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    
    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        
        if (pauseButton != null) pauseButton.gameObject.SetActive(false); 
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (pauseButton != null) pauseButton.gameObject.SetActive(true);
        isPaused = false;
    }

    public void ToMenu()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.ForceGameOver(false); 
        }
        
        Time.timeScale = 1; 
        SceneManager.LoadScene(mainMenuScene);
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}