using DG.Tweening;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayProgressUIE : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] private Image fillImage;

    [Space]
    [SerializeField] private float progress;
    [SerializeField] private float currentProgress;
    [SerializeField] private float progressMax;
    int currentDay = 1;

    // PlayerPrefs keys
    private const string CURRENT_DAY_KEY = "CURRENT_DAY";
    private const string CURRENT_PROGRESS_KEY = "CURRENT_PROGRESS";
    private const string PROGRESS_MAX_KEY = "PROGRESS_MAX";
    private const string PROGRESS_KEY = "PROGRESS";

    private void OnEnable()
    {
        DayCompleteHandler.OnDayComplete += DayCompleteHandler_OnDayComplete;
        TextGameUIController.OnDayProgress += UpdateProgress;
        TextGameUIController.OnDayChange += TextGameUIController_OnDayChange;
    }

    private void DayCompleteHandler_OnDayComplete()
    {
        dayText.text = "";
    }

    private void LoadProgressData()
    {
        // Load current day
        if (PlayerPrefs.HasKey(CURRENT_DAY_KEY))
        {
            currentDay = PlayerPrefs.GetInt(CURRENT_DAY_KEY);
            dayText.SetText("DAY " + currentDay);
        }

        // Load progress values
        if (PlayerPrefs.HasKey(CURRENT_PROGRESS_KEY))
        {
            currentProgress = PlayerPrefs.GetFloat(CURRENT_PROGRESS_KEY);
        }

        if (PlayerPrefs.HasKey(PROGRESS_MAX_KEY))
        {
            progressMax = PlayerPrefs.GetFloat(PROGRESS_MAX_KEY);
        }
        else
        {
            if (DialogueManager.masterDatabase.GetConversation(DialogueManager.lastConversationStarted) != null)
            {
                progressMax = ScriptRegistry.Instance.textGameController.GetDayMaxProgression();
            }
        }

        if (PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            progress = PlayerPrefs.GetFloat(PROGRESS_KEY);
            fillImage.fillAmount = progress; // Set fill amount directly without animation on load
        }

        TextGameUIController_OnDayChange(currentDay);
    }

    private void SaveProgressData()
    {
        PlayerPrefs.SetInt(CURRENT_DAY_KEY, currentDay);
        PlayerPrefs.SetFloat(CURRENT_PROGRESS_KEY, currentProgress);
        PlayerPrefs.SetFloat(PROGRESS_MAX_KEY, progressMax);
        PlayerPrefs.SetFloat(PROGRESS_KEY, progress);
        PlayerPrefs.Save(); // Explicitly save to ensure data is written immediately
    }

    private void TextGameUIController_OnDayChange(int obj)
    {
        if (currentDay != obj)
        {
            ResetProgress();
            currentDay = obj;
            dayText.SetText("DAY " + obj);
            PlayAnim();
        }
    }

    public void Reset()
    {
        ResetProgress();
        SaveProgressData(); // Save after reset
    }

    public void PlayAnim()
    {
        GetComponent<Animator>().CrossFade("dayGraphicAnimation", .1f);
    }

    public void UpdateProgress(int _progress)
    {
        if (DialogueManager.instance.isInitialized)
        {
            currentProgress += _progress;
            dayText.SetText("DAY " + currentDay);

            progressMax = ScriptRegistry.Instance.textGameController.GetDayMaxProgression();
            progress = currentProgress / progressMax;

            fillImage.DOFillAmount(progress, 0.2f);

        }
    }

    public void ResetProgress()
    {
        currentProgress = 0;
        progress = 0;
        fillImage.fillAmount = 0;
        progressMax = ScriptRegistry.Instance.textGameController.GetDayMaxProgression();
        SaveProgressData(); // Save after resetting
    }

    public void FillProgressx()
    {
        currentProgress = 1;
        fillImage.DOFillAmount(1, 1);
    }

    private void OnDisable()
    {
        TextGameUIController.OnDayProgress -= UpdateProgress;
        TextGameUIController.OnDayChange -= TextGameUIController_OnDayChange;
        DayCompleteHandler.OnDayComplete -= DayCompleteHandler_OnDayComplete;
    }
}