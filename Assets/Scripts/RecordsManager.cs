using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RecordsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform recordsContainer; // Куда добавлять записи
    [SerializeField] private GameObject recordEntryPrefab; // Префаб записи
    [SerializeField] private Button backButton;
    
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    
    private const string RECORDS_KEY = "GameRecords";
    private List<RecordData> records = new List<RecordData>();
    private const int MAX_RECORDS = 10;
    
    void Start()
    {
        LoadRecords();
        DisplayRecords();
        
        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);
    }
    
    void LoadRecords()
    {
        records.Clear();
        
        // Загружаем сохраненные рекорды
        if (PlayerPrefs.HasKey(RECORDS_KEY))
        {
            string json = PlayerPrefs.GetString(RECORDS_KEY);
            RecordsWrapper wrapper = JsonUtility.FromJson<RecordsWrapper>(json);
            if (wrapper != null && wrapper.records != null)
            {
                records = wrapper.records;
            }
        }
        
        // Если нет рекордов, создаем пустые (10 нулей)
        if (records.Count == 0)
        {
            for (int i = 0; i < MAX_RECORDS; i++)
            {
                records.Add(new RecordData(0));
            }
        }
        
        // Сортируем по убыванию (большие сверху)
        records.Sort((a, b) => b.score.CompareTo(a.score));
    }
    
    // Вызывать из GameOverManager когда игра закончена
    public static void AddNewScore(int score)
    {
        if (score <= 0) return;
        
        // Загружаем текущие рекорды
        List<RecordData> currentRecords = new List<RecordData>();
        if (PlayerPrefs.HasKey(RECORDS_KEY))
        {
            string json = PlayerPrefs.GetString(RECORDS_KEY);
            RecordsWrapper wrapper = JsonUtility.FromJson<RecordsWrapper>(json);
            if (wrapper != null && wrapper.records != null)
            {
                currentRecords = wrapper.records;
            }
        }
        
        // Добавляем новый рекорд
        currentRecords.Add(new RecordData(score));
        
        // Сортируем по убыванию
        currentRecords.Sort((a, b) => b.score.CompareTo(a.score));
        
        // Оставляем только первые 10
        if (currentRecords.Count > MAX_RECORDS)
        {
            currentRecords.RemoveRange(MAX_RECORDS, currentRecords.Count - MAX_RECORDS);
        }
        
        // Сохраняем
        RecordsWrapper saveWrapper = new RecordsWrapper();
        saveWrapper.records = currentRecords;
        string saveJson = JsonUtility.ToJson(saveWrapper);
        PlayerPrefs.SetString(RECORDS_KEY, saveJson);
        PlayerPrefs.Save();
    }
    
    void DisplayRecords()
    {
        // Очищаем контейнер
        foreach (Transform child in recordsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Создаем записи
        for (int i = 0; i < records.Count; i++)
        {
            GameObject entry = Instantiate(recordEntryPrefab, recordsContainer);
            
            // Находим тексты внутри записи
            Text[] texts = entry.GetComponentsInChildren<Text>();
            
            foreach (Text text in texts)
            {
                if (text.name.Contains("Position"))
                {
                    text.text = $"{i + 1}.";
                }
                else if (text.name.Contains("Score"))
                {
                    text.text = records[i].score > 0 
                        ? records[i].score.ToString() 
                        : "-";
                }
            }
            
            // Если это не рекорд (0) - делаем полупрозрачным
            if (records[i].score == 0)
            {
                CanvasGroup group = entry.GetComponent<CanvasGroup>();
                if (group == null)
                    group = entry.AddComponent<CanvasGroup>();
                group.alpha = 0.3f;
            }
        }
    }
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}

// Вспомогательный класс для сериализации списка
[System.Serializable]
public class RecordsWrapper
{
    public List<RecordData> records;
}