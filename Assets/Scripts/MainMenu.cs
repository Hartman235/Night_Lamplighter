using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text coinsText;
    
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "Game";      // Сцена с игрой
    [SerializeField] private string settingsSceneName = "Settings"; // Сцена настроек
    [SerializeField] private string recordsSceneName = "Records"; // Новая
    [SerializeField] private string shopSceneName = "Records"; // Новая
    [SerializeField] private string statisticsSceneName = "Statistics";
    [SerializeField] private string tutorialSceneName = "Tutorial";
    
    void Start()
    {
        // Загружаем количество монет
        int coins = PlayerPrefs.GetInt("coins", 0);
        coinsText.text = coins.ToString();
        
        // Убеждаемся, что время идет нормально (если вернулись из игры)
        //Time.timeScale = 1;
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
}