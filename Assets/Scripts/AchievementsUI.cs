using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AchievementsUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject achievementItemPrefab;
    [SerializeField] private Button backButton;

    private List<AchievementItem> spawnedItems = new List<AchievementItem>();

    private void Start()
    {
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnAchievementsUpdated += RefreshUI;
            RefreshUI();
        }

        if (StatisticsManager.Instance != null)
        {
            StatisticsManager.Instance.OnStatisticsChanged += UpdateAllProgress;
        }

        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);
    }

    private void RefreshUI()
    {
        // очистка
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
        spawnedItems.Clear();

        if (AchievementManager.Instance == null) return;

        foreach (var ach in AchievementManager.Instance.GetAllAchievements())
        {
            if (ach == null) continue;
            GameObject item = Instantiate(achievementItemPrefab, contentParent);
            AchievementItem aItem = item.GetComponent<AchievementItem>();
            bool unlocked = AchievementManager.Instance.IsUnlocked(ach.achievementId);
            bool claimed = AchievementManager.Instance.IsClaimed(ach.achievementId);
            aItem.Setup(ach, unlocked, claimed);
            spawnedItems.Add(aItem);
        }
    }

    private void UpdateAllProgress()
    {
        foreach (var item in spawnedItems)
            item.UpdateProgress();
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void OnDestroy()
    {
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnAchievementsUpdated -= RefreshUI;
        if (StatisticsManager.Instance != null)
            StatisticsManager.Instance.OnStatisticsChanged -= UpdateAllProgress;
    }
}