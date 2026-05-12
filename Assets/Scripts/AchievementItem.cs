using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementItem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button claimButton;
    [SerializeField] private Image checkmarkImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;   

    private AchievementData data;
    private bool isUnlocked;
    private bool isClaimed;
    private int currentProgress;

    public void Setup(AchievementData ach, bool unlocked, bool claimed)
    {
        data = ach;
        isUnlocked = unlocked;
        isClaimed = claimed;

        titleText.text = ach.title;
        descriptionText.text = ach.description;
        rewardText.text = $"+{ach.rewardCoins} монет";
        if (iconImage != null && ach.icon != null)
            iconImage.sprite = ach.icon;

        UpdateProgress();

        if (isClaimed)
        {
            claimButton.gameObject.SetActive(false);
            if (checkmarkImage != null) checkmarkImage.gameObject.SetActive(true);
        }
        else if (isUnlocked)
        {
            claimButton.gameObject.SetActive(true);
            claimButton.interactable = true;
            if (checkmarkImage != null) checkmarkImage.gameObject.SetActive(false);
        }
        else
        {
            claimButton.gameObject.SetActive(true);
            claimButton.interactable = false;
            if (checkmarkImage != null) checkmarkImage.gameObject.SetActive(false);
        }

        UpdateBackgroundColor();
    }

    public void UpdateProgress()
    {
        if (data == null || StatisticsManager.Instance == null) return;

        currentProgress = GetCurrentValueForAchievement(data);
        string progressString = $"{currentProgress} / {data.targetValue}";
        if (progressText != null)
            progressText.text = progressString;
    }

    private int GetCurrentValueForAchievement(AchievementData ach)
    {
        switch (ach.type)
        {
            case AchievementType.TotalCoins:
                return StatisticsManager.Instance.GetTotalCoins();
            case AchievementType.TotalBatteries:
                return StatisticsManager.Instance.GetTotalBatteries();
            case AchievementType.TotalDistance:
                return Mathf.FloorToInt(StatisticsManager.Instance.GetTotalDistance());
            case AchievementType.TotalGamesPlayed:
                return StatisticsManager.Instance.GetTotalGames();
            case AchievementType.BestDistance:
                return StatisticsManager.Instance.GetBestDistance();
            default:
                return 0;
        }
    }

    public void OnClaimButton()
    {

        bool success = AchievementManager.Instance.ClaimReward(data.achievementId);
        if (success)
        {
            claimButton.gameObject.SetActive(false);
            if (checkmarkImage) checkmarkImage.gameObject.SetActive(true);
            isClaimed = true;
            UpdateBackgroundColor();
        }
    }

    private void UpdateBackgroundColor()
    {
        
        if (isClaimed)
            backgroundImage.color = Color.green;
        else if (isUnlocked)
            backgroundImage.color = Color.yellow;
        else
            backgroundImage.color = Color.gray;
    }
}