using UnityEngine;
using DG.Tweening;

public class LightingManager : MonoBehaviour
{
    [Header("Tweening Settings")]
    [Tooltip("Duration of the transition when time of day changes")]
    public float transitionDuration = 1.0f;
    [Tooltip("Easing function to use for transitions")]
    public Ease easeType = Ease.InOutSine;
    [Header("Light References")]
    public Light directionalLight;

    [Header("Building Shaders")]
    [Tooltip("Assign building materials that have the _NightLightOpe property")]
    public Material[] buildingMaterials;
    [Tooltip("Night light value when fully night (timeOfDay = 0)")]
    public float nightLightValue = 0.9f;
    [Tooltip("Night light value when fully day (timeOfDay = 1)")]
    public float dayLightValue = 0.0f;

    [Header("Day Colors")]
    public Color daySkyColor = new Color(0.5f, 0.85f, 1.0f);
    public Color dayEquatorColor = new Color(0.7f, 0.9f, 1.0f);
    public Color dayGroundColor = new Color(0.4f, 0.6f, 0.3f);
    public float dayLightIntensity = 2.0f;

    [Header("Night Colors")]
    public Color nightSkyColor = new Color(0.05f, 0.05f, 0.1f);
    public Color nightEquatorColor = new Color(0.1f, 0.1f, 0.15f);
    public Color nightGroundColor = new Color(0.1f, 0.1f, 0.1f);
    public float nightLightIntensity = 0.0f;

    [Header("Save Settings")]
    [Tooltip("Enable to save lighting state between game sessions")]
    public bool saveSettings = true;
    [Tooltip("Unique key prefix for PlayerPrefs (useful if you have multiple scenes)")]
    public string playerPrefsKeyPrefix = "LightingManager_";

    [SerializeField] private float currentProgress;
    float progressMax;

    [Range(0, 1)]
    public float timeOfDay = 1.0f; // 1 = full day, 0 = full night

    // Keys for PlayerPrefs
    private string TimeOfDayKey => playerPrefsKeyPrefix + "TimeOfDay";
    private string CurrentProgressKey => playerPrefsKeyPrefix + "CurrentProgress";

    private void OnEnable()
    {
        TextGameUIController.OnDayProgress += UpdateProgress;
    }

    public void Reset()
    {
        currentProgress = 1;
        progressMax = ScriptRegistry.Instance.textGameController.GetDayMaxProgression();
        timeOfDay = 1 - (currentProgress / progressMax);
        UpdateLighting(timeOfDay);

        if (saveSettings)
        {
            SaveSettings();
        }
    }

    void UpdateProgress(int _progress)
    {
        currentProgress += _progress;
        progressMax = ScriptRegistry.Instance.textGameController.GetDayMaxProgression();
        timeOfDay = 1 - (currentProgress / progressMax);
        UpdateLighting(timeOfDay);

        if (saveSettings)
        {
            SaveSettings();
        }
    }

    void UpdateProgress()
    {
        progressMax = ScriptRegistry.Instance.textGameController.GetDayMaxProgression();
        timeOfDay = 1 - (currentProgress / progressMax);
        UpdateLighting(timeOfDay);

        if (saveSettings)
        {
            SaveSettings();
        }
    }



    private void OnDisable()
    {
        TextGameUIController.OnDayProgress -= UpdateProgress;

        // Kill all tweens when disabled to prevent errors
        DOTween.Kill(skyColorTween);
        DOTween.Kill(equatorColorTween);
        DOTween.Kill(groundColorTween);
        DOTween.Kill(lightIntensityTween);
        DOTween.Kill(nightLightOpeTween);

        // Save settings when component is disabled
        if (saveSettings)
        {
            SaveSettings();
        }
    }

    // Current values for tweening
    private Color currentSkyColor;
    private Color currentEquatorColor;
    private Color currentGroundColor;
    private float currentLightIntensity;
    private float currentNightLightOpe;

    // Tween objects to manage animations
    private Tween skyColorTween;
    private Tween equatorColorTween;
    private Tween groundColorTween;
    private Tween lightIntensityTween;
    private Tween nightLightOpeTween;

    // Initialize current values
    private void Start()
    {
        // Load saved settings if enabled
        if (saveSettings)
        {
            LoadSettings();
        }

        // Initialize with current time of day
        currentSkyColor = Color.Lerp(nightSkyColor, daySkyColor, timeOfDay);
        currentEquatorColor = Color.Lerp(nightEquatorColor, dayEquatorColor, timeOfDay);
        currentGroundColor = Color.Lerp(nightGroundColor, dayGroundColor, timeOfDay);
        currentLightIntensity = Mathf.Lerp(nightLightIntensity, dayLightIntensity, timeOfDay);
        currentNightLightOpe = Mathf.Lerp(dayLightValue, nightLightValue, 1 - timeOfDay);

        // Apply initial values
        RenderSettings.ambientSkyColor = currentSkyColor;
        RenderSettings.ambientEquatorColor = currentEquatorColor;
        RenderSettings.ambientGroundColor = currentGroundColor;
        if (directionalLight != null)
        {
            directionalLight.intensity = currentLightIntensity;
        }
        UpdateBuildingMaterials(currentNightLightOpe);
    }

    public void UpdateLighting(float t)
    {
        // Target colors and values based on time of day
        Color targetSkyColor = Color.Lerp(nightSkyColor, daySkyColor, t);
        Color targetEquatorColor = Color.Lerp(nightEquatorColor, dayEquatorColor, t);
        Color targetGroundColor = Color.Lerp(nightGroundColor, dayGroundColor, t);
        float targetLightIntensity = Mathf.Lerp(nightLightIntensity, dayLightIntensity, t);
        float targetNightLightOpe = Mathf.Lerp(dayLightValue, nightLightValue, 1 - t);

        // Kill previous tweens if they exist
        skyColorTween?.Kill();
        equatorColorTween?.Kill();
        groundColorTween?.Kill();
        lightIntensityTween?.Kill();
        nightLightOpeTween?.Kill();

        // Tween ambient colors
        skyColorTween = DOTween.To(() => currentSkyColor, x => {
            currentSkyColor = x;
            RenderSettings.ambientSkyColor = x;
        }, targetSkyColor, transitionDuration).SetEase(easeType);

        equatorColorTween = DOTween.To(() => currentEquatorColor, x => {
            currentEquatorColor = x;
            RenderSettings.ambientEquatorColor = x;
        }, targetEquatorColor, transitionDuration).SetEase(easeType);

        groundColorTween = DOTween.To(() => currentGroundColor, x => {
            currentGroundColor = x;
            RenderSettings.ambientGroundColor = x;
        }, targetGroundColor, transitionDuration).SetEase(easeType);

        // Tween light intensity
        if (directionalLight != null)
        {
            lightIntensityTween = DOTween.To(() => currentLightIntensity, x => {
                currentLightIntensity = x;
                directionalLight.intensity = x;
            }, targetLightIntensity, transitionDuration).SetEase(easeType);
        }

        // Tween building shader property
        nightLightOpeTween = DOTween.To(() => currentNightLightOpe, x => {
            currentNightLightOpe = x;
            UpdateBuildingMaterials(x);
        }, targetNightLightOpe, transitionDuration).SetEase(easeType);
    }

    private void UpdateBuildingMaterials(float value)
    {
        if (buildingMaterials != null && buildingMaterials.Length > 0)
        {
            foreach (Material mat in buildingMaterials)
            {
                if (mat != null && mat.HasProperty("_NightLightOpe"))
                {
                    mat.SetFloat("_NightLightOpe", value);
                }
            }
        }
    }

    public void SetTimeOfDay(float newTime)
    {
        timeOfDay = Mathf.Clamp01(newTime);
        UpdateLighting(timeOfDay);

        if (saveSettings)
        {
            SaveSettings();
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(TimeOfDayKey, timeOfDay);
        PlayerPrefs.SetFloat(CurrentProgressKey, currentProgress);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey(TimeOfDayKey))
        {
            timeOfDay = PlayerPrefs.GetFloat(TimeOfDayKey);
        }

        if (PlayerPrefs.HasKey(CurrentProgressKey))
        {
            currentProgress = PlayerPrefs.GetFloat(CurrentProgressKey);

            Invoke("UpdateProgress", 1);
        }
    }

    public void DeleteSavedSettings()
    {
        PlayerPrefs.DeleteKey(TimeOfDayKey);
        PlayerPrefs.DeleteKey(CurrentProgressKey);
        PlayerPrefs.Save();
    }
}