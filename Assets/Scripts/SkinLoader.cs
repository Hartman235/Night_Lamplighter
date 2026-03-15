using UnityEngine;
using System; // Добавляем для Action

public class SkinLoader : MonoBehaviour
{
    [Header("Player Skins")]
    [SerializeField] private Transform skinsParent; // объект, в котором находятся все префабы скинов
    private int chosenIndex;

    // Событие, которое вызывается при загрузке нового скина
    public static event Action OnSkinChanged;

    void Start()
    {
        LoadSelectedSkin();
    }

    void LoadSelectedSkin()
    {
        // Загружаем индекс выбранного скина из PlayerPrefs (по умолчанию 0)
        chosenIndex = PlayerPrefs.GetInt("chosenSkin", 0);

        // Проверяем, что индекс в пределах допустимого
        if (chosenIndex < 0 || chosenIndex >= skinsParent.childCount)
        {
            chosenIndex = 0;
        }

        // Активируем выбранный скин, отключаем все остальные
        for (int i = 0; i < skinsParent.childCount; i++)
        {
            skinsParent.GetChild(i).gameObject.SetActive(i == chosenIndex);
        }
        
        // Уведомляем всех подписчиков о смене скина
        OnSkinChanged?.Invoke();
    }

    // Метод для принудительной перезагрузки скина (если понадобится)
    public void ReloadSkin()
    {
        LoadSelectedSkin();
    }
}