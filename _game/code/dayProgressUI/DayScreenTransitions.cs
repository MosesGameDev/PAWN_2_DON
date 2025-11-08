using DG.Tweening;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;


[ RequireComponent(typeof(UIDialogueElement))]
public class DayScreenTransitions : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI DayText;
    [SerializeField] private TextMeshProUGUI chapterDescriptionText;
    int currentDay;

    private void OnEnable()
    {
        TextGameUIController.OnDayChange += TextGameUIController_OnDayChange;
    }

    private void TextGameUIController_OnDayChange(int obj)
    {
        if (currentDay != obj)
        {
            currentDay = obj;
            GetComponent<UIDialogueElement>().Show();
            PlayTween();
            DayText.SetText("CHAPTER " + obj);
        }

    }

    void PlayTween()
    {
        chapterDescriptionText.rectTransform.DOAnchorPosY(-66, 2).SetDelay(1).SetEase(Ease.OutBack).OnComplete(() => { Invoke("Hide", 1); });
        string description = DialogueManager.masterDatabase.GetConversation(DialogueManager.lastConversationStarted).Description;
        chapterDescriptionText.SetText(description);
    }

    void Hide()
    {
        chapterDescriptionText.rectTransform.anchoredPosition = new Vector2(0, 66);
        GetComponent<UIDialogueElement>().Hide();
    }

    private void OnDisable()
    {
        TextGameUIController.OnDayChange -= TextGameUIController_OnDayChange;

    }
}
