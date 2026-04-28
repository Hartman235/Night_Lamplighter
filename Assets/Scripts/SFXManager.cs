using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    private AudioSource sfxSource;
    private float currentVolume;
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            sfxSource = GetComponent<AudioSource>();
            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadVolume()
    {
        currentVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.5f);
        if (sfxSource != null)
            sfxSource.volume = currentVolume;
    }

    public void PlaySound(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void SetVolume(float volume)
    {
        currentVolume = volume;
        if (sfxSource != null)
            sfxSource.volume = volume;
    }
}