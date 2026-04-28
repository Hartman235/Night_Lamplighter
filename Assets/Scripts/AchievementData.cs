using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievement", menuName = "Game/Achievement")]
public class AchievementData : ScriptableObject
{
    public string achievementId;
    public string title;
    public string description;
    public AchievementType type;
    public int targetValue;
    public int rewardCoins;
    public Sprite icon;
}

public enum AchievementType
{
    TotalCoins,
    TotalBatteries,
    TotalDistance,
    TotalGamesPlayed,
    BestDistance
}