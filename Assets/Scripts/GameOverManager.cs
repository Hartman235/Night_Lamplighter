using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject losePanel;      
    [SerializeField] private Text recordText;           
       
    public void ProcessGameOver()
    {
        int lastRunScore = PlayerPrefs.GetInt("lastRunScore", 0);
        int recordScore = PlayerPrefs.GetInt("recordScore", 0);

        if (lastRunScore > recordScore)
        {
            recordScore = lastRunScore;
            PlayerPrefs.SetInt("recordScore", recordScore);
        }
        
        // Добавляем в таблицу рекордов
        RecordsManager.AddNewScore(lastRunScore);
        
        UpdateLosePanelUI(lastRunScore, recordScore);
        losePanel.SetActive(true);
    }
    
    private void UpdateLosePanelUI(int lastScore, int record)
    {       
        if (recordText == null)
        {
            recordText = losePanel.GetComponentInChildren<Text>();
        }
        
        if (recordText != null)
        {
            recordText.text = "Рекорд: " + record.ToString();
        }
    }
}