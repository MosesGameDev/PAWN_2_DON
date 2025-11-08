using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SoundToggleUI : MonoBehaviour
{
    public enum SoundType
    {
        Music,
        SFX,
        Vibration
    }

    [SerializeField] private SoundType soundType;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        // Subscribe to toggle events
        toggle.onValueChanged.AddListener(OnToggleValueChanged);

        // Initialize toggle state based on current sound settings
        UpdateToggleState();
    }

    private void OnEnable()
    {
        // Update the toggle state when the UI becomes visible
        UpdateToggleState();
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (SoundManager.Instance == null) return;

        // Call appropriate method in SoundManager
        switch (soundType)
        {
            case SoundType.Music:
                if (SoundManager.Instance.IsMusicEnabled() != isOn)
                {
                    SoundManager.Instance.ToggleMusic();
                }
                break;

            case SoundType.SFX:
                if (SoundManager.Instance.IsSFXEnabled() != isOn)
                {
                    SoundManager.Instance.ToggleSFX();
                }
                break;

            case SoundType.Vibration:
                if (SoundManager.Instance.IsVibrationEnabled() != isOn)
                {
                    SoundManager.Instance.ToggleVibration();
                }
                break;
        }
    }

    // Update toggle state to match SoundManager
    private void UpdateToggleState()
    {
        if (SoundManager.Instance == null) return;

        // Temporarily remove listener to avoid triggering the event
        toggle.onValueChanged.RemoveListener(OnToggleValueChanged);

        // Set toggle state
        switch (soundType)
        {
            case SoundType.Music:
                toggle.isOn = SoundManager.Instance.IsMusicEnabled();
                break;

            case SoundType.SFX:
                toggle.isOn = SoundManager.Instance.IsSFXEnabled();
                break;

            case SoundType.Vibration:
                toggle.isOn = SoundManager.Instance.IsVibrationEnabled();
                break;
        }

        // Re-add the listener
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnDestroy()
    {
        // Clean up event subscription
        toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }
}