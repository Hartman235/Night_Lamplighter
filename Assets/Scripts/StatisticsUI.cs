using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatisticsUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text totalGamesText;
    [SerializeField] private Text totalCoinsText;
    [SerializeField] private Text totalBatteriesText;
    [SerializeField] private Text totalDistanceText;
    [SerializeField] private Text totalPlayTimeText;
    [SerializeField] private Text bestDistanceText;
    [SerializeField] private Text lastUpdatedText;
    [SerializeField] private string mainMenuScene = "MainMenu";
    
    [Header("Buttons")]
    [SerializeField] private Button backButton;
    
    void Start()
    {
        UpdateUI();
        
        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);           
    }
    
    void UpdateUI()
    {
        if (StatisticsManager.Instance == null) return;
        
        if (totalGamesText != null)
            totalGamesText.text = $"Игр сыграно: {StatisticsManager.Instance.GetTotalGames()}";
            
        if (totalCoinsText != null)
            totalCoinsText.text = $"Монет собрано: {StatisticsManager.Instance.GetTotalCoins()}";
            
        if (totalBatteriesText != null)
            totalBatteriesText.text = $"Батареек собрано: {StatisticsManager.Instance.GetTotalBatteries()}";
            
        if (totalDistanceText != null)
            totalDistanceText.text = $"Дистанция: {StatisticsManager.Instance.GetTotalDistance():F0}";
            
        if (totalPlayTimeText != null)
        {
            float seconds = StatisticsManager.Instance.GetTotalPlayTime();
            int hours = Mathf.FloorToInt(seconds / 3600);
            int minutes = Mathf.FloorToInt((seconds % 3600) / 60);
            int secs = Mathf.FloorToInt(seconds % 60);
            totalPlayTimeText.text = $"Время в игре: {hours:00}:{minutes:00}:{secs:00}";
        }
            
        if (bestDistanceText != null)
            bestDistanceText.text = $"Рекорд: {StatisticsManager.Instance.GetBestDistance()}";
            
        if (lastUpdatedText != null)
            lastUpdatedText.text = $"Обновлено: {StatisticsManager.Instance.GetLastUpdated()}";
    }
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
    
    void OnEnable()
    {
        UpdateUI();
    }
}