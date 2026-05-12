using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text coinsText;
    
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "Game";      
    [SerializeField] private string settingsSceneName = "Settings"; 
    [SerializeField] private string recordsSceneName = "Records";
    [SerializeField] private string shopSceneName = "Records"; 
    [SerializeField] private string statisticsSceneName = "Statistics";
    [SerializeField] private string tutorialSceneName = "Tutorial";
    [SerializeField] private string achievements = "Achievements";
    
    void Start()
    {
        if (MusicManager.Instance != null)
        MusicManager.Instance.PlayMusic();
        int coins = PlayerPrefs.GetInt("coins", 0);
        coinsText.text = coins.ToString();
    }
    
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
    
    public void OpenSettings()
    {
        SceneManager.LoadScene(settingsSceneName);
    }
    
    public void ExitGame()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OpenRecords()
    {
        SceneManager.LoadScene(recordsSceneName);
    }

    public void OpenShop()
    {
        SceneManager.LoadScene(shopSceneName);
    }

    public void OpenStatistics()
    {
        SceneManager.LoadScene(statisticsSceneName);
    }

    public void OpenTutorial()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }

    public void OpenAchievements()
    {
        SceneManager.LoadScene(achievements);
    }
}