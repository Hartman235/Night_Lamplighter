using UnityEngine;

public enum UpgradeType
{
    PermanentSpeed,
    PermanentCoinBonus,
    PermanentBatteryStart,
    PerRunSpeedBoost,
    PerRunCoinMultiplier,
    PerRunInvincibility,
    PerRunMagnet
}

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Game/Shop Upgrade")]
public class ShopUpgradeData : ScriptableObject
{
    [Header("Основное")]
    public string upgradeId;
    public string title;
    public string description;
    public UpgradeType type;
    public bool isPerRun;
    public int basePrice;
    public Sprite icon;

    [Header("Параметры эффекта")]
    public float effectValue;
    public float duration = 0f;

    [Header("Для постоянных улучшений")]
    public int maxLevel = 1;
    public int priceIncreasePerLevel = 0;
}