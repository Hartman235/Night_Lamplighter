using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RecordsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform recordsContainer; 
    [SerializeField] private GameObject recordEntryPrefab; 
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
        
        if (PlayerPrefs.HasKey(RECORDS_KEY))
        {
            string json = PlayerPrefs.GetString(RECORDS_KEY);
            RecordsWrapper wrapper = JsonUtility.FromJson<RecordsWrapper>(json);
            if (wrapper != null && wrapper.records != null)
            {
                records = wrapper.records;
            }
        }
        
        if (records.Count == 0)
        {
            for (int i = 0; i < MAX_RECORDS; i++)
            {
                records.Add(new RecordData(0));
            }
        }
        
        records.Sort((a, b) => b.score.CompareTo(a.score));
    }
    
    public static void AddNewScore(int score)
    {
        if (score <= 0) return;
        
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
        
        currentRecords.Add(new RecordData(score));
        
        currentRecords.Sort((a, b) => b.score.CompareTo(a.score));
        
        if (currentRecords.Count > MAX_RECORDS)
        {
            currentRecords.RemoveRange(MAX_RECORDS, currentRecords.Count - MAX_RECORDS);
        }
        
        RecordsWrapper saveWrapper = new RecordsWrapper();
        saveWrapper.records = currentRecords;
        string saveJson = JsonUtility.ToJson(saveWrapper);
        PlayerPrefs.SetString(RECORDS_KEY, saveJson);
        PlayerPrefs.Save();
    }
    
    void DisplayRecords()
    {
        foreach (Transform child in recordsContainer)
        {
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < records.Count; i++)
        {
            GameObject entry = Instantiate(recordEntryPrefab, recordsContainer);
            
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

[System.Serializable]
public class RecordsWrapper
{
    public List<RecordData> records;
}