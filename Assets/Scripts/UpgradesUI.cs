using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradesUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject upgradeItemPrefab;
    [SerializeField] private Text coinsText;

    private List<UpgradeItemUI> items = new List<UpgradeItemUI>();

    private void OnEnable()
    {
        // Подписываемся только когда объект активен
        if (UpgradesManager.Instance != null)
        {
            UpgradesManager.Instance.OnUpgradePurchased += OnUpgradePurchasedHandler;
            UpgradesManager.Instance.OnPerRunUpgradesChanged += OnPerRunUpgradesChangedHandler;
        }
        UpdateCoins();
        RefreshUI();
    }

    private void OnDisable()
    {
        // Отписываемся, когда объект выключается или уничтожается
        if (UpgradesManager.Instance != null)
        {
            UpgradesManager.Instance.OnUpgradePurchased -= OnUpgradePurchasedHandler;
            UpgradesManager.Instance.OnPerRunUpgradesChanged -= OnPerRunUpgradesChangedHandler;
        }
    }

    private void OnUpgradePurchasedHandler(string id, int level)
    {
        // Дополнительная проверка – объект ещё существует и активен
        if (this == null || !gameObject.activeInHierarchy) return;
        RefreshUI();
        UpdateCoins();
    }

    private void OnPerRunUpgradesChangedHandler()
    {
        if (this == null || !gameObject.activeInHierarchy) return;
        RefreshUI();
        UpdateCoins();
    }

    private void UpdateCoins()
    {
        if (coinsText != null)
            coinsText.text = PlayerPrefs.GetInt("coins", 0).ToString();
    }

    private void RefreshUI()
    {
        // Проверяем, что родительский контейнер ещё существует
        if (contentParent == null) return;

        // Очищаем старые элементы
        foreach (Transform child in contentParent)
        {
            if (child != null)
                Destroy(child.gameObject);
        }
        items.Clear();

        if (UpgradesManager.Instance == null) return;

        // Создаём новые элементы
        foreach (var upgrade in UpgradesManager.Instance.GetAllUpgrades())
        {
            if (upgrade == null) continue;
            GameObject go = Instantiate(upgradeItemPrefab, contentParent);
            UpgradeItemUI ui = go.GetComponent<UpgradeItemUI>();
            if (ui != null)
                ui.Setup(upgrade);
            items.Add(ui);
        }
    }
}