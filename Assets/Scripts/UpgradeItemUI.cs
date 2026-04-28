using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Image iconImage;

    private ShopUpgradeData upgrade;

    public void Setup(ShopUpgradeData data)
    {
        upgrade = data;
        titleText.text = data.title;
        descText.text = data.description;
        if (iconImage != null && data.icon != null)
            iconImage.sprite = data.icon;
        UpdateState();
    }

    public void UpdateState()
    {
        if (UpgradesManager.Instance == null) return;

        bool canBuy = UpgradesManager.Instance.CanPurchase(upgrade);
        
        if (upgrade.isPerRun)
        {
            // Проверяем, не куплено ли уже это временное улучшение
            bool alreadyPurchased = UpgradesManager.Instance.IsPerRunUpgradePurchased(upgrade.upgradeId);
            if (alreadyPurchased)
            {
                buyButton.interactable = false;
                priceText.text = "Куплено";
            }
            else
            {
                buyButton.interactable = canBuy;
                priceText.text = UpgradesManager.Instance.GetCurrentPrice(upgrade).ToString();
            }
            levelText.gameObject.SetActive(false);
        }
        else // постоянное улучшение
        {
            buyButton.interactable = canBuy;
            priceText.text = UpgradesManager.Instance.GetCurrentPrice(upgrade).ToString();
            int level = UpgradesManager.Instance.GetUpgradeLevel(upgrade.upgradeId);
            levelText.text = $"Уровень: {level}/{upgrade.maxLevel}";
            levelText.gameObject.SetActive(true);
            if (level >= upgrade.maxLevel)
            {
                buyButton.interactable = false;
                priceText.text = "MAX";
            }
        }
    }

    public void OnBuyClick()
    {
        if (UpgradesManager.Instance != null)
        {
            UpgradesManager.Instance.PurchaseUpgrade(upgrade);
        }
    }
}