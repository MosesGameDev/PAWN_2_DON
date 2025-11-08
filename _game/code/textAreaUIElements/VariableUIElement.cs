using Coffee.UIExtensions;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DamageNumbersPro;
using System;

public class VariableUIElement : MonoBehaviour
{
    public string id;
    public int currentValue;
    public int maxValue;

    public static event Action<int> onCashChanged;

    [Space]
    [SerializeField] private Image progressImage;
    [SerializeField] private TextMeshProUGUI valueText;

    [Space]
    [SerializeField] private CanvasGroup canvasGroup;

    [Space]
    [SerializeField] private bool startVisible;

    [Space]
    public UIParticleAttractor UIParticleAttractor;

    // PlayerPrefs key for CASH
    private string CASH_PREFS_KEY = "PlayerCash";
    private int VARIABLE_VISIBLE;

    Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;

        // Special handling for CASH
        if (id == "CASH" && PlayerPrefs.HasKey(CASH_PREFS_KEY))
        {
            //// Load CASH from PlayerPrefs
            //currentValue = PlayerPrefs.GetInt(CASH_PREFS_KEY, 0);

            //// Update the DialogueLua variable to match the loaded value
            //DialogueLua.SetVariable(id, currentValue);
        }
        else
        {
            // Normal behavior for other variables
            //currentValue = DialogueLua.GetVariable(id).asInt;
        }
        currentValue = DialogueLua.GetVariable(id).asInt;
        UpdateUIElementDirect(currentValue);

        if (startVisible)
        {       
            Show(transform.GetSiblingIndex());
        }
        else
        {
            canvasGroup.alpha = 0;
        }
    }


    public void UpdateUIElementPercentage(int percentageValue)
    {
        float v = currentValue + percentageValue;
        currentValue = (int)v;
        progressImage.DOFillAmount(v / 100, .3f);
        valueText.SetText($"{currentValue}%K");

        PlayPunch();

        // Save CASH to PlayerPrefs if this is the CASH variable
        if (id == "CASH")
        {
            SaveCashToPlayerPrefs();
        }

    }

    public void UpdateUIElement(int Value)
    {
        currentValue += Value;

        // Update the text display based on value
        UpdateTextDisplay();

        // Update the progress bar
        UpdateProgressBar();

        // Update the DialogueLua variable
        if (!string.IsNullOrEmpty(id))
        {
            DialogueLua.SetVariable(id, currentValue);

            // Save CASH to PlayerPrefs if this is the CASH variable
            if (id == "CASH")
            {
                onCashChanged?.Invoke(currentValue);
                print($"<color=cyan>Updated CASH to {currentValue}.</color>");
                SaveCashToPlayerPrefs();
            }

        }

        PlayPunch();
    }



    public void UpdateUIElementDirect(int Value)
    {
        currentValue = Value;

        // Update the text display based on value
        UpdateTextDisplay();

        // Update the progress bar
        UpdateProgressBar();

        // Save CASH to PlayerPrefs if this is the CASH variable
        if (id == "CASH")
        {
            SaveCashToPlayerPrefs();
        }
    }

    public void UpdateUIElementDirect_2(int Value)
    {
        if (Value > 0)
        {
            currentValue += Value;
        }
        else
        {
            currentValue -= Value;
        }


        // Update the text display based on value
        UpdateTextDisplay();

        // Update the progress bar
        UpdateProgressBar();

        // Save CASH to PlayerPrefs if this is the CASH variable
        if (id == "CASH")
        {
            SaveCashToPlayerPrefs();
        }

    }

    private void UpdateTextDisplay()
    {

        // Handle negative values
        if (currentValue < 0)
        {
            currentValue = 0;
        }

        if (currentValue == 0)
        {
            if (id == "HP")
            {
                currentValue = 5;
            }
        }

        // Format the display text with K for thousands
        if (currentValue >= 1000)
        {
            float displayValueK = currentValue / 1000f;
            valueText.SetText($"{displayValueK:F1}K");
        }
        else
        {
            valueText.SetText($"{currentValue}");
        }
    }

    private void UpdateProgressBar()
    {
        if (progressImage != null && maxValue > 0)
        {
            float progressRatio = (float)currentValue / maxValue;
            progressImage.DOFillAmount(progressRatio, 0.5f);

            // For progress bars, show both current and max values
            if (maxValue >= 1000)
            {
                float currentK = currentValue / 1000f;
                float maxK = maxValue / 1000f;
                valueText.SetText($"{currentK:F1}K/{maxK:F1}K");
            }
            else
            {
                valueText.SetText($"{currentValue}/{maxValue}");
            }

        }

    }

    // Helper method to save CASH to PlayerPrefs
    private void SaveCashToPlayerPrefs()
    {
        if (id == "CASH")
        {
            PlayerPrefs.SetInt(CASH_PREFS_KEY, currentValue);
            PlayerPrefs.Save();

            onCashChanged?.Invoke(currentValue);
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt(id, currentValue);
        //print($"<color=cyan>Saved {id} with value : {PlayerPrefs.GetInt(id)} to PlayerPrefs.</color>");
    }

    //Load data from PlayerPrefs
    public void LoadData()
    {
        if (PlayerPrefs.HasKey(id))
        {
            currentValue = PlayerPrefs.GetInt(id);
            //print($"<color=yellow>Loaded {id} with value : {currentValue} from PlayerPrefs.</color>");
            UpdateUIElementDirect(currentValue);
        }
        else
        {
            Debug.LogWarning($"No data found for {id} in PlayerPrefs.");
        }
    }

    void PlayPunch()
    {
        transform.DOKill();
        transform.transform.localScale = startScale;
        transform.DOPunchScale(startScale * 0.1f, 0.3f);
    }

    public void ShakeFx()
    {
        valueText.DOColor(Color.red, .5f).OnComplete(() => valueText.DOColor(Color.white, .3f));
        valueText.rectTransform.DOShakePosition(1f, 5, 20, 90, false, true);
        PlayPunch();
    }

    bool isVisible;
    public void Show(int index)
    {
        if (!isVisible)
        {
            if (canvasGroup.alpha > 0)
            {
                return;
            }
            canvasGroup.DOFade(1, .5f);

            // Special handling for CASH when showing
            if (id == "CASH" && PlayerPrefs.HasKey(CASH_PREFS_KEY))
            {
                currentValue = PlayerPrefs.GetInt(CASH_PREFS_KEY, 0);
                DialogueLua.SetVariable(id, currentValue);
            }
            else
            {
                currentValue = DialogueLua.GetVariable(id).asInt;
            }

            UpdateUIElement(0);
            isVisible = true;

            transform.SetSiblingIndex(index);
        }
    }

    public int GetCurrentValue()
    {
        return currentValue;
    }

    public void Hide()
    {
        canvasGroup.DOFade(0, .5f);
        isVisible = false;
    }




    private void OnApplicationQuit()
    {
        // Save CASH one final time when the application quits
        if (id == "CASH")
        {
            SaveCashToPlayerPrefs();
        }

    }
}


