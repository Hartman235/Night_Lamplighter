using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    public static UpgradesManager Instance { get; private set; }

    [SerializeField] private List<ShopUpgradeData> allUpgrades = new List<ShopUpgradeData>();

    private Dictionary<string, int> permanentLevels = new Dictionary<string, int>();
    private List<ShopUpgradeData> activePerRunUpgrades = new List<ShopUpgradeData>();

    public event Action<string, int> OnUpgradePurchased;
    public event Action OnPerRunUpgradesChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPermanentUpgrades();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadPermanentUpgrades()
    {
        if (allUpgrades == null) return;
        foreach (ShopUpgradeData data in allUpgrades)
        {
            if (data != null && !data.isPerRun)
            {
                int level = PlayerPrefs.GetInt($"Upgrade_{data.upgradeId}", 0);
                permanentLevels[data.upgradeId] = level;
            }
        }
    }

    private void SavePermanentUpgrade(string upgradeId, int level)
    {
        PlayerPrefs.SetInt($"Upgrade_{upgradeId}", level);
        PlayerPrefs.Save();
    }

    public int GetUpgradeLevel(string upgradeId)
    {
        return permanentLevels.ContainsKey(upgradeId) ? permanentLevels[upgradeId] : 0;
    }

    public int GetCurrentPrice(ShopUpgradeData upgrade)
    {
        if (upgrade.isPerRun)
            return upgrade.basePrice;
        else
        {
            int level = GetUpgradeLevel(upgrade.upgradeId);
            return upgrade.basePrice + level * upgrade.priceIncreasePerLevel;
        }
    }

    public bool CanPurchase(ShopUpgradeData upgrade)
    {
        if (!upgrade.isPerRun)
        {
            int level = GetUpgradeLevel(upgrade.upgradeId);
            if (level >= upgrade.maxLevel) return false;
        }
        int coins = PlayerPrefs.GetInt("coins", 0);
        return coins >= GetCurrentPrice(upgrade);
    }

    public bool PurchaseUpgrade(ShopUpgradeData upgrade)
    {
        if (!CanPurchase(upgrade)) return false;

        // Для временных улучшений – проверяем, не куплено ли уже
        if (upgrade.isPerRun)
        {
            if (activePerRunUpgrades.Contains(upgrade))
                return false; // уже куплено на этот забег
        }

        int price = GetCurrentPrice(upgrade);
        int coins = PlayerPrefs.GetInt("coins", 0);
        PlayerPrefs.SetInt("coins", coins - price);
        PlayerPrefs.Save();

        if (upgrade.isPerRun)
        {
            activePerRunUpgrades.Add(upgrade);
            OnPerRunUpgradesChanged?.Invoke(); // обновит UI
        }
        else
        {
            int newLevel = GetUpgradeLevel(upgrade.upgradeId) + 1;
            permanentLevels[upgrade.upgradeId] = newLevel;
            SavePermanentUpgrade(upgrade.upgradeId, newLevel);
            OnUpgradePurchased?.Invoke(upgrade.upgradeId, newLevel);
        }
        return true;
    }

    public List<ShopUpgradeData> GetAllUpgrades() => allUpgrades;
    public List<ShopUpgradeData> GetActivePerRunUpgrades() => activePerRunUpgrades;

    public void ClearPerRunUpgrades()
    {
        activePerRunUpgrades.Clear();
        OnPerRunUpgradesChanged?.Invoke();
    }

    public void ApplyPermanentEffects(PlayerController player, Score score)
    {
        if (allUpgrades == null) return;
        foreach (ShopUpgradeData data in allUpgrades)
        {
            if (data.isPerRun) continue;
            int level = GetUpgradeLevel(data.upgradeId);
            if (level == 0) continue;

            switch (data.type)
            {
                case UpgradeType.PermanentSpeed:
                    player.IncreaseBaseSpeed(data.effectValue * level);
                    break;
                case UpgradeType.PermanentCoinBonus:
                    score.AddPermanentCoinBonus((int)(data.effectValue * level));
                    break;
                case UpgradeType.PermanentBatteryStart:
                    player.AddStartBatteries((int)(data.effectValue * level));
                    break;
            }
        }
    }

    public void ApplyPerRunEffects(PlayerController player, Score score)
    {
        foreach (ShopUpgradeData data in activePerRunUpgrades)
        {
            switch (data.type)
            {
                case UpgradeType.PerRunSpeedBoost:
                    player.ApplyTemporarySpeedBoost(data.effectValue, data.duration);
                    break;
                case UpgradeType.PerRunCoinMultiplier:
                    score.AddTemporaryCoinMultiplier(data.effectValue, data.duration);
                    break;
                case UpgradeType.PerRunInvincibility:
                    player.ApplyTemporaryInvincibility(data.duration);
                    break;
                case UpgradeType.PerRunMagnet:
                    player.ApplyTemporaryMagnet(data.effectValue, data.duration);
                    break;
            }
        }
        ClearPerRunUpgrades();
    }

    public bool IsPerRunUpgradePurchased(string upgradeId)
    {
        return activePerRunUpgrades.Exists(u => u.upgradeId == upgradeId);
    }
}