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
        RecordsManager.AddNewScore(lastRunScore);
        Vibrate();
        UpdateLosePanelUI(lastRunScore, recordScore);
        losePanel.SetActive(true);
        if (UpgradesManager.Instance != null)
            UpgradesManager.Instance.ClearPerRunUpgrades();
    }

    private void UpdateLosePanelUI(int lastScore, int record)
    {       
        if (recordText == null)
            recordText = losePanel.GetComponentInChildren<Text>();
        if (recordText != null)
            recordText.text = "Рекорд: " + record.ToString();
    }

    private void Vibrate()
    {
        bool vibrationEnabled = PlayerPrefs.GetInt("VibrationEnabled", 1) == 1;
        if (vibrationEnabled)
        {
            #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
            #endif
        }
    }
}