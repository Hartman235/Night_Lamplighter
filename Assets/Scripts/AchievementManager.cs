using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [SerializeField] private List<AchievementData> achievements;

    private Dictionary<string, bool> achievementUnlocked = new Dictionary<string, bool>();
    private Dictionary<string, bool> achievementClaimed = new Dictionary<string, bool>();

    public event Action OnAchievementsUpdated;

    private const string UNLOCKED_KEY = "AchievementUnlocked_";
    private const string CLAIMED_KEY = "AchievementClaimed_";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (StatisticsManager.Instance != null)
        {
            StatisticsManager.Instance.OnStatisticsChanged += OnStatisticsChanged;
            CheckAllAchievements();
        }
    }

    private void OnDestroy()
    {
        if (StatisticsManager.Instance != null)
            StatisticsManager.Instance.OnStatisticsChanged -= OnStatisticsChanged;
    }

    public void OnStatisticsChanged()
    {
        CheckAllAchievements();
    }

    public void CheckAllAchievements()
    {
        bool anyChanged = false;
        foreach (var ach in achievements)
        {
            if (ach == null) continue;
            if (achievementClaimed.ContainsKey(ach.achievementId) && achievementClaimed[ach.achievementId])
                continue;

            bool isUnlocked = IsConditionMet(ach);
            if (isUnlocked && (!achievementUnlocked.ContainsKey(ach.achievementId) || !achievementUnlocked[ach.achievementId]))
            {
                achievementUnlocked[ach.achievementId] = true;
                SaveUnlocked(ach.achievementId, true);
                anyChanged = true;
            }
        }
        if (anyChanged)
            OnAchievementsUpdated?.Invoke();
    }

    private bool IsConditionMet(AchievementData ach)
    {
        if (StatisticsManager.Instance == null) return false;

        switch (ach.type)
        {
            case AchievementType.TotalCoins:
                return StatisticsManager.Instance.GetTotalCoins() >= ach.targetValue;
            case AchievementType.TotalBatteries:
                return StatisticsManager.Instance.GetTotalBatteries() >= ach.targetValue;
            case AchievementType.TotalDistance:
                return StatisticsManager.Instance.GetTotalDistance() >= ach.targetValue;
            case AchievementType.TotalGamesPlayed:
                return StatisticsManager.Instance.GetTotalGames() >= ach.targetValue;
            case AchievementType.BestDistance:
                return StatisticsManager.Instance.GetBestDistance() >= ach.targetValue;
            default:
                return false;
        }
    }

    public bool ClaimReward(string achievementId)
    {
        if (!achievementUnlocked.ContainsKey(achievementId) || !achievementUnlocked[achievementId])
            return false;
        if (achievementClaimed.ContainsKey(achievementId) && achievementClaimed[achievementId])
            return false;

        AchievementData ach = achievements.Find(a => a.achievementId == achievementId);
        if (ach == null) return false;

        int currentCoins = PlayerPrefs.GetInt("coins", 0);
        currentCoins += ach.rewardCoins;
        PlayerPrefs.SetInt("coins", currentCoins);
        PlayerPrefs.Save();

        achievementClaimed[achievementId] = true;
        SaveClaimed(achievementId, true);
        
        // Уведомляем UI об изменении (чтобы обновить список)
        OnAchievementsUpdated?.Invoke();
        return true;
    }

    public bool IsUnlocked(string id) => achievementUnlocked.ContainsKey(id) && achievementUnlocked[id];
    public bool IsClaimed(string id) => achievementClaimed.ContainsKey(id) && achievementClaimed[id];
    public List<AchievementData> GetAllAchievements() => achievements;

    private void LoadProgress()
    {
        foreach (var ach in achievements)
        {
            if (ach == null) continue;
            bool unlocked = PlayerPrefs.GetInt(UNLOCKED_KEY + ach.achievementId, 0) == 1;
            bool claimed = PlayerPrefs.GetInt(CLAIMED_KEY + ach.achievementId, 0) == 1;
            achievementUnlocked[ach.achievementId] = unlocked;
            achievementClaimed[ach.achievementId] = claimed;
        }
    }

    private void SaveUnlocked(string id, bool value)
    {
        PlayerPrefs.SetInt(UNLOCKED_KEY + id, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SaveClaimed(string id, bool value)
    {
        PlayerPrefs.SetInt(CLAIMED_KEY + id, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}