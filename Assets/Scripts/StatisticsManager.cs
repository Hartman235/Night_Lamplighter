using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance { get; private set; }
    
    private const string STATS_KEY = "GameStatistics";
    private GameStatistics stats;
    
    // Текущие значения для этой сессии
    private int sessionCoins;
    private int sessionBatteries;
    private float sessionDistance;
    private float sessionStartTime;
    
    [Header("UI References (опционально)")]
    [SerializeField] private bool updateUIInRealTime = false;
    [SerializeField] private string mainMenuScene = "MainMenu";
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
           
            LoadStatistics();
        }
        else
        {
            Destroy(gameObject);
        }
    }
        
    void Start()
    {
        ResetSessionStats();
    }
    
    void LoadStatistics()
    {
        if (PlayerPrefs.HasKey(STATS_KEY))
        {
            string json = PlayerPrefs.GetString(STATS_KEY);
            stats = JsonUtility.FromJson<GameStatistics>(json);
        }
        
        if (stats == null)
        {
            stats = new GameStatistics();
        }
    }
    
    void SaveStatistics()
    {
        stats.UpdateTimestamp();
        string json = JsonUtility.ToJson(stats);
        PlayerPrefs.SetString(STATS_KEY, json);
        PlayerPrefs.Save();
    }
    
    void ResetSessionStats()
    {
        sessionCoins = 0;
        sessionBatteries = 0;
        sessionDistance = 0;
        sessionStartTime = Time.time;
    }
    
    // Вызывается при старте новой игры
    public void StartNewGame()
    {
        stats.totalGamesPlayed++;
        ResetSessionStats();
        sessionStartTime = Time.time;
    }
    
    // Вызывается при окончании игры
    public void EndGame(int finalScore, float finalDistance)
    {
        // Обновляем общую статистику
        stats.totalCoinsCollected += sessionCoins;
        stats.totalBatteriesCollected += sessionBatteries;
        stats.totalDistanceRun += sessionDistance;
        stats.totalPlayTime += Time.time - sessionStartTime;
        
        // Обновляем лучший результат
        if (finalScore > stats.bestDistance)
        {
            stats.bestDistance = finalScore;
        }
        
        SaveStatistics();
    }
    
    // Вызывается при сборе монеты
    public void AddCoin()
    {
        sessionCoins++;
        if (updateUIInRealTime)
        {
            // Можно обновлять UI в реальном времени, если нужно
        }
    }
    
    // Вызывается при сборе батарейки
    public void AddBattery()
    {
        sessionBatteries++;
    }
    
    // Обновление пройденной дистанции (вызывать из PlayerController)
    public void UpdateDistance(float distance)
    {
        sessionDistance = distance;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
    
    // Геттеры для UI
    public int GetTotalGames() => stats.totalGamesPlayed;
    public int GetTotalCoins() => stats.totalCoinsCollected;
    public int GetTotalBatteries() => stats.totalBatteriesCollected;
    public float GetTotalDistance() => Mathf.Round(stats.totalDistanceRun * 100f) / 100f;
    public float GetTotalPlayTime() => Mathf.Round(stats.totalPlayTime * 100f) / 100f;
    public int GetBestDistance() => stats.bestDistance;
    public string GetLastUpdated() => stats.lastUpdated;
}