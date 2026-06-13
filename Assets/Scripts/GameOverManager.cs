using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject losePanel;      
    [SerializeField] private Text recordText;   
    [SerializeField] private GameObject pauseButton;        

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
        
        if (pauseButton != null)
            pauseButton.SetActive(false);
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
<<<<<<< HEAD
        if (!vibrationEnabled) return;

        Debug.Log("Vibration triggered! Native check started.");

        #if UNITY_ANDROID && !UNITY_EDITOR
        try 
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
            {
                using (AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION"))
                {
                    int sdkInt = buildVersion.GetStatic<int>("SDK_INT");
                    
                    if (sdkInt >= 26) 
                    {
                        using (AndroidJavaObject attrBuilder = new AndroidJavaObject("android.media.AudioAttributes$Builder"))
                        {
                            attrBuilder.Call<AndroidJavaObject>("setUsage", 13); 
                            attrBuilder.Call<AndroidJavaObject>("setContentType", 4); 
                            AndroidJavaObject audioAttrs = attrBuilder.Call<AndroidJavaObject>("build");

                            using (AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect"))
                            {
                                long[] timings = new long[] { 0, 200 };
                                int[] amplitudes = new int[] { 0, 255 };

                                AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", timings, amplitudes, -1);
                                
                                vibrator.Call("vibrate", vibrationEffect, audioAttrs);
                                Debug.Log("Vibration: Sent via Waveform with USAGE_GAME (FORCE MODE)");
                            }
                        }
                    }
                    else 
                    {
                        vibrator.Call("vibrate", 200L);
                        Debug.Log("Vibration: Used Legacy Vibrate (SDK < 26)");
                    }
                }
            }
=======

        if (vibrationEnabled)
        {
            Debug.Log("Vibration triggered!"); 
            Handheld.Vibrate();
        }
        else
        {
            Debug.Log("Vibration is disabled in settings.");
>>>>>>> b2122dfef2e29ef77019e6ed4697489353bcc865
        }
        catch (System.Exception e)
        {
            Debug.LogError("Vibration error: " + e.Message);
            Handheld.Vibrate(); 
        }
        #elif UNITY_IOS
            Handheld.Vibrate();
            Debug.Log("Vibration: Used iOS Handheld.Vibrate()");
        #endif
    }
}