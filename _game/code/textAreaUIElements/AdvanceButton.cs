using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using Coffee.UIEffects;

public class AdvanceButton : MonoBehaviour
{
    public Button button;
    [Space]
    public float fillDuration = 1f;
    [SerializeField] private Image fillImage;
    [SerializeField] private float pointerReenableDelay = 5f; // New field for the delay

    [Space]
    [SerializeField] ButtonEffect[] effects;

    [Space]
    public UIEffect buttonEffect;

    [Space]
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject elipsisFX;
    [SerializeField] private GameObject text;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private GameObject handPointer;

    public static event Action<Button> OnButtonClicked;

    private void OnEnable()
    {
        //CombatController.OnTriggerPlaced += () => { ToggleShowElipsis(true); };
        CharacterCombatInteractionTrigger.OnEnterTrigger += () => { ToggleShowElipsis(false); };
    }

    private void OnDisable()
    {
        //CombatController.OnTriggerPlaced -= () => { ToggleShowElipsis(true); };
        CharacterCombatInteractionTrigger.OnEnterTrigger -= () => { ToggleShowElipsis(false); };
    }

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(delegate { FillImage(); });
            Vibration.Init();
        }
    }

    public void ToggleButton(bool interactible)
    {
        button.enabled = interactible;
        background.SetActive(interactible);
    }

    public void SetText(string value)
    {
        buttonText.text = value;
    }

    void FillImage()
    {
        button.interactable = false;
        button.image.sprite = button.spriteState.pressedSprite;
        fillImage.DOFillAmount(1, fillDuration).OnComplete(OnFillCompleted);
    }


    void OnFillCompleted()
    {
        //SoundsVibrationController.instance.PlayVibration();
        FeelVibrationManager.Instance.PlayVibration();

        fillImage.fillAmount = 0;
        button.image.sprite = button.spriteState.highlightedSprite;

        transform.DOPunchScale(Vector3.one * 0.1f, 0.5f)
        .OnComplete(() =>
        {
            button.interactable = true;
        });

        OnButtonClicked?.Invoke(button);
    }

    public void ToggleShowElipsis(bool show)
    {
        if (!button)
        {
            return;
        }

        if (show)
        {
            button.enabled = false;
            elipsisFX.SetActive(show);
            buttonEffect.enabled = false;
            text.SetActive(false);
        }
        else
        {
            button.enabled = true;
            elipsisFX.SetActive(show);
            buttonEffect.enabled = true;
            text.SetActive(true);
        }
    }

    public void PlayButtonEffect(string fxName)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].name == fxName)
            {
                effects[i].fx.Play();
                return;
            }
        }
    }
}

[System.Serializable]
public class ButtonEffect
{
    public string name;
    public ParticleSystem fx;
}