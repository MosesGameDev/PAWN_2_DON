using UnityEngine;
using MoreMountains.Feedbacks;
using DamageNumbersPro.Demo; // Make sure to add the correct using directive for Feel

/// <summary>
/// Handles integration between SoundManager and Feel's haptic system.
/// </summary>
public class FeelVibrationManager : MonoBehaviour
{
    // Singleton instance
    public static FeelVibrationManager Instance { get; private set; }

    // Reference to common haptic feedbacks (optional)
    [Header("Common Haptic Feedbacks")]
    [SerializeField] private MMFeedbacks lightHapticFeedback;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Plays a haptic feedback if vibration is enabled in the SoundManager.
    /// </summary>
    /// <param name="hapticFeedback">The MMF_Player containing the feedback to play</param>
    /// <returns>True if the feedback was played, false otherwise</returns>
    public bool PlayHaptic(MMFeedbacks hapticFeedback)
    {
        if (hapticFeedback == null) return false;

        // Check if SoundManager exists and if vibration is enabled
        if (SoundManager.Instance != null && SoundManager.Instance.IsVibrationEnabled())
        {
            hapticFeedback.PlayFeedbacks();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Plays the light haptic feedback if vibration is enabled.
    /// </summary>
    public bool PlayLightHaptic()
    {
        return PlayHaptic(lightHapticFeedback);
    }

    public void PlayVibration()
    {
        PlayHaptic(lightHapticFeedback);
    }



}