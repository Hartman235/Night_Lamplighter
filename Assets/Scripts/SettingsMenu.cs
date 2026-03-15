using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle vibrationToggle; // Новый переключатель
    
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string VIBRATION_KEY = "VibrationEnabled"; // Ключ для вибрации
    
    void Start()
    {
        LoadSettings();
        
        // Добавляем слушатели
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            
        if (vibrationToggle != null)
            vibrationToggle.onValueChanged.AddListener(SetVibration);
    }
    
    void LoadSettings()
    {
        // Загружаем настройки
        if (musicSlider != null)
            musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.5f);
        
        if (sfxSlider != null)
            sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.5f);
            
        if (vibrationToggle != null)
            vibrationToggle.isOn = PlayerPrefs.GetInt(VIBRATION_KEY, 1) == 1; // По умолчанию включено
    }
    
    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
    
    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
    
    public void SetVibration(bool isEnabled)
    {
        PlayerPrefs.SetInt(VIBRATION_KEY, isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}