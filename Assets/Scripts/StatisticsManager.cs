using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance { get; private set; }
    
    public event Action OnStatisticsChanged;
    
    private const string STATS_KEY = "GameStatistics";
    private GameStatistics stats;
    
    private int sessionCoins;
    private int sessionBatteries;
    private float sessionStartTime;
    
    [Header("UI References (опционально)")]
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

    public void AddTotalCoins(int amount)
    {
        if (stats != null)
        {
            stats.totalCoinsCollected += amount;
            SaveStatistics(); 
            OnStatisticsChanged?.Invoke(); 
        }
    }
    
    void SaveStatistics()
    {
        stats.UpdateTimestamp();
        string json = JsonUtility.ToJson(stats);
        PlayerPrefs.SetString(STATS_KEY, json);
        PlayerPrefs.Save();
        OnStatisticsChanged?.Invoke();
    }
    
    void ResetSessionStats()
    {
        sessionCoins = 0;
        sessionBatteries = 0;
        sessionStartTime = Time.time;
    }
    
    public void StartNewGame()
    {
        stats.totalGamesPlayed++;
        ResetSessionStats();
        sessionStartTime = Time.time;
        OnStatisticsChanged?.Invoke();
    }
    
    public void EndGame(int finalScore, float finalDistance)
    {
        stats.totalCoinsCollected += sessionCoins;
        stats.totalBatteriesCollected += sessionBatteries;
        stats.totalDistanceRun += finalScore;
        stats.totalPlayTime += Time.time - sessionStartTime;
        
        if (finalScore > stats.bestDistance)
        {
            stats.bestDistance = finalScore;
        }
        
        SaveStatistics(); 
    }
    
    public void AddCoin()
    {
        sessionCoins++;
        OnStatisticsChanged?.Invoke();
    }
    
    public void AddBattery()
    {
        sessionBatteries++;
        OnStatisticsChanged?.Invoke();
    }
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
    
    public int GetTotalGames() => stats.totalGamesPlayed;
    public int GetTotalCoins() => stats.totalCoinsCollected;
    public int GetTotalBatteries() => stats.totalBatteriesCollected;
    public float GetTotalDistance() => stats.totalDistanceRun;
    public float GetTotalPlayTime() => Mathf.Round(stats.totalPlayTime * 100f) / 100f;
    public int GetBestDistance() => stats.bestDistance;
    public string GetLastUpdated() => stats.lastUpdated;
}