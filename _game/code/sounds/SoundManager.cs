using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System;

public class SoundManager : MonoBehaviour
{
    // Singleton instance
    public static SoundManager Instance { get; private set; }

    // Audio mixer references
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume";

    [Space]
    public AudioClip sadMusic;
    public AudioClip defaultMusic;

    // Audio sources
    [SerializeField] private AudioSource vehicleDriving;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private List<AudioSource> sfxSources = new List<AudioSource>();

    [Space]
    [SerializeField] private SoundEffect[] soundEffects;
    [SerializeField] private UIDialogueElement settingsDialogue;

    // State variables
    private bool isMusicEnabled = true;
    private bool isSFXEnabled = true;
    private bool isVibrationEnabled = true;

    // PlayerPrefs keys
    private const string MUSIC_ENABLED_KEY = "MusicEnabled";
    private const string SFX_ENABLED_KEY = "SFXEnabled";
    private const string VIBRATION_ENABLED_KEY = "VibrationEnabled";

    // Default volume values
    private const float VOLUME_ON = 0f;      // 0dB = normal volume
    private const float VOLUME_OFF = -80f;   // -80dB = effectively muted

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved preferences
        LoadSoundSettings();

        // Apply loaded settings
        ApplySoundSettings();
    }

    // Toggle music on/off
    public void ToggleMusic()
    {
        isMusicEnabled = !isMusicEnabled;
        ApplyMusicSettings();
        SaveSoundSettings();
    }

    // Toggle SFX on/off
    public void ToggleSFX()
    {
        isSFXEnabled = !isSFXEnabled;
        ApplySFXSettings();
        SaveSoundSettings();
    }

    public void PlayButtonSfx()
    {
        PlaySFX("click");
        FeelVibrationManager.Instance?.PlayLightHaptic();
    }

    public void OnSettingsButtonPressed()
    {
        if(settingsDialogue.isOpen)
        {
            settingsDialogue.Hide();
            if (ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
            {
                ScriptRegistry.Instance.homeScreenManager.homeScreenButtonParent.SetActive(true);
            }
            return;
        }

        settingsDialogue.Show();

        if (ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
        {
            ScriptRegistry.Instance.homeScreenManager.homeScreenButtonParent.SetActive(false);
        }
    }


    // Toggle vibration on/off
    public void ToggleVibration()
    {
        isVibrationEnabled = !isVibrationEnabled;
        SaveSoundSettings();
    }

    // Get music state
    public bool IsMusicEnabled() => isMusicEnabled;

    // Get SFX state
    public bool IsSFXEnabled() => isSFXEnabled;

    // Get vibration state
    public bool IsVibrationEnabled() => isVibrationEnabled;

    // Apply sound settings to the audio mixer
    private void ApplySoundSettings()
    {
        ApplyMusicSettings();
        ApplySFXSettings();
    }

    // Apply music settings
    private void ApplyMusicSettings()
    {
        float musicVolume = isMusicEnabled ? VOLUME_ON : VOLUME_OFF;
        audioMixer.SetFloat(musicVolumeParameter, musicVolume);

        // Also control the audio source directly as a backup
        if (musicSource != null)
        {
            musicSource.mute = !isMusicEnabled;
            vehicleDriving.mute = !isMusicEnabled;
        }
    }

    // Apply SFX settings
    private void ApplySFXSettings()
    {
        float sfxVolume = isSFXEnabled ? VOLUME_ON : VOLUME_OFF;
        audioMixer.SetFloat("SFXVolume", sfxVolume);

        // Also control the audio sources directly as a backup
        foreach (var source in sfxSources)
        {
            if (source != null)
            {
                source.mute = !isSFXEnabled;
            }
        }
    }

    // Play a sound effect
    public void PlaySFX(string clip)
    {
        if (!isSFXEnabled || clip == null) return;

        // Find an available SFX source
        AudioSource availableSource = null;

        foreach (var source in sfxSources)
        {
            if (source != null && !source.isPlaying)
            {
                availableSource = source;
                break;
            }
        }

        // If no available source found, create a new one
        if (availableSource == null)
        {
            availableSource = gameObject.AddComponent<AudioSource>();
            availableSource.outputAudioMixerGroup = sfxSources.Count > 0 ?
                sfxSources[0].outputAudioMixerGroup : null;
            sfxSources.Add(availableSource);
        }

        AudioClip sfxClip =  GetSoundEffect(clip).audioClip;

        availableSource.PlayOneShot(sfxClip);
    }

    // Play a music track
    public void PlayMusic(AudioClip music)
    {
        if (musicSource == null || music == null) return;

        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayVehicleFx(AudioClip vehicle)
    {
        vehicleDriving.clip = vehicle;
        vehicleDriving.loop = true;
        vehicleDriving.Play();
    }

    public void StopVehicleSound()
    {
        vehicleDriving.Stop();
    }

    // For Feel integration - check if vibration should be played
    public bool ShouldPlayVibration()
    {
        return isVibrationEnabled;
    }

    // Play vibration if enabled (for direct integration with Feel)
    public bool TryPlayVibration(Action vibrationAction)
    {
        if (isVibrationEnabled && vibrationAction != null)
        {
            vibrationAction.Invoke();
            return true;
        }
        return false;
    }


    SoundEffect GetSoundEffect(string id)
    {
        for (int i = 0; i < soundEffects.Length; i++)
        {
            if (soundEffects[i].id == id)
            {
                return soundEffects[i];
            }
        }

        return null;
    }


    // Save sound settings to PlayerPrefs
    private void SaveSoundSettings()
    {
        PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, isMusicEnabled ? 1 : 0);
        PlayerPrefs.SetInt(SFX_ENABLED_KEY, isSFXEnabled ? 1 : 0);
        PlayerPrefs.SetInt(VIBRATION_ENABLED_KEY, isVibrationEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Load sound settings from PlayerPrefs
    private void LoadSoundSettings()
    {
        // Default to true if setting doesn't exist
        isMusicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;
        isSFXEnabled = PlayerPrefs.GetInt(SFX_ENABLED_KEY, 1) == 1;
        isVibrationEnabled = PlayerPrefs.GetInt(VIBRATION_ENABLED_KEY, 1) == 1;
    }


    [System.Serializable]
    public class SoundEffect
    {
        public string id;
        public AudioClip audioClip;
    }
}