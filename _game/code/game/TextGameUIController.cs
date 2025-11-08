using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using DG.Tweening;
using Sirenix.OdinInspector;

public class TextGameUIController : MonoBehaviour
{
    [SerializeField] public int _currentDay { get; private set; }

    public AdvanceButton advanceButton;

    [Space]
    public RectTransform textAreaRect;
    [SerializeField] private RectTransform contentRectTransform;
    [SerializeField] private StandardUIMenuPanel responseDialogueRect;
    [SerializeField] private Transform responseButtonContentHolder;

    [Space]
    [SerializeField] private bool hideHeaderOnStart;
    [SerializeField] private UIDialogueElement header;
    public ScrollRect scrollRect;

    [Space]
    [SerializeField] private TextDialogueUIElement textDialogueUIElementPrefab;
    [SerializeField] private TextDialogueUIElement businessDialogueUIElementPrefab;
    [SerializeField] private TextDialogueUIElement imageDialogueUIElementPrefab;
    [SerializeField] private Image textDialogueImage;

    [Space]
    public VariableUIElement[] variableUIElements;

    float textAreaStartRectYPOS = 0;

    public List<TextDialogueUIElement> textDialogueUIElements = new List<TextDialogueUIElement>();
    public static event Action<int> OnDayProgress;
    public static event Action<int> OnDayChange;

    Subtitle _subtitle;
    private int lastProcessedEntryId = -1;

    public class SubtitleData
    {
        public DialogueEntry entry;
        public string speakerName;

        public SubtitleData(DialogueEntry entry, string speakerName)
        {
            this.entry = entry;
            this.speakerName = speakerName;
        }
    }

    private void Start()
    {
        textAreaStartRectYPOS = textAreaRect.anchoredPosition.y;

        if (hideHeaderOnStart)
        {
            HideTextArea();
            Camera cam = Camera.main;
            float targetPosY = textAreaRect.anchoredPosition.y * -1;
            textAreaRect.DOAnchorPosY(targetPosY, .1f);
            cam.rect = new Rect(0, 1, 1, 1);
            isTextAreaHidden = true;
        }
    }

    private void OnEnable()
    {
        //CombatController.OnBattleStart += CombatController_OnTriggerPlaced;
        //CombatController.OnBattleEnd += CombatController_OnBattleEnd;
    }

    private void OnDisable()
    {
        //CombatController.OnBattleStart -= CombatController_OnTriggerPlaced;
        //CombatController.OnBattleEnd -= CombatController_OnBattleEnd;
    }

    private void CombatController_OnBattleEnd()
    {
        advanceButton.ToggleShowElipsis(false);
    }

    private void CombatController_OnTriggerPlaced()
    {
        advanceButton.ToggleShowElipsis(true);
    }

    public void OnConversationLine(Subtitle subtitle)
    {
        _subtitle = subtitle;

        // Check if we've already processed this entry  
        if (subtitle.dialogueEntry.id == lastProcessedEntryId)
        {
            return;
        }

        // Store this entry ID as processed  
        lastProcessedEntryId = subtitle.dialogueEntry.id;

        responseDialogueRect.onOpen.AddListener(ScrollToBottomSmooth);
        responseDialogueRect.onClose.AddListener(ResetResponseButtons);

        if (Field.FieldExists(_subtitle.dialogueEntry.fields, "IMG"))
        {
            string imageId = Field.LookupValue(_subtitle.dialogueEntry.fields, "IMG");
            CreateImagetDialogueElement(imageId);
            SaveGameManager.instance.OnConversationLinePlayed(subtitle);
            return;
        }

        CreateTextDialogueElement();

        SaveGameManager.instance.OnConversationLinePlayed(subtitle);
    }

    [Button]
    public void ClearPrevousDayTextUIElements()
    {
        for (int i = textDialogueUIElements.Count - 1; i >= 0; i--)
        {
            Destroy(textDialogueUIElements[i].gameObject);
            textDialogueUIElements.RemoveAt(i);
        }
    }

    void OnConversationResponseMenu(Response[] responses)
    {
        StartCoroutine(ResponseButtons(responses));
    }

    public void ResetDialogueTracking()
    {
        lastProcessedEntryId = -1;
    }

    // This method clears all dialogue elements from the screen  
    public void ClearDialogueElements()
    {
        foreach (var element in textDialogueUIElements)
        {
            if (element != null)
            {
                Destroy(element.gameObject);
            }
        }
        textDialogueUIElements.Clear();
    }

    // This method recreates dialogue history from saved data  
    public void RecreateDialogueHistory(List<DialogueHistoryEntry> history)
    {

        // Create UI elements directly from history data  
        foreach (var historyEntry in history)
        {
            // Skip entries with no text  
            if (string.IsNullOrEmpty(historyEntry.dialogueText))
            {
                continue;
            }

            TextDialogueUIElement uiElement = Instantiate(textDialogueUIElementPrefab, contentRectTransform);

            uiElement.SetText(historyEntry.dialogueText);

            DialogueEntry dialogueEntry = DialogueManager.masterDatabase.GetDialogueEntry(historyEntry.conversationId, historyEntry.entryId);

            Subtitle _subtitle = new Subtitle(
                historyEntry.speakerInfo,
                historyEntry.coversant,
                new FormattedText(historyEntry.dialogueText),
                string.Empty,
                string.Empty,
                dialogueEntry
            );

            uiElement.UpdateFields(_subtitle);


            // Set day if present  
            if (historyEntry.day > 0)
            {
                uiElement.SetDay("DAY " + historyEntry.day);

                // Update current day in the controller too  
                if (_currentDay != historyEntry.day)
                {
                    _currentDay = historyEntry.day;
                }
            }

            // Update parameters if present  
            if (historyEntry.hp != 0)
            {
                uiElement.EnableParameterUIElement("HP", historyEntry.hp);
            }

            if (historyEntry.power != 0)
            {
                uiElement.EnableParameterUIElement("POWER", historyEntry.power);
            }

            if (historyEntry.cash != 0)
            {
                uiElement.EnableParameterUIElement("CASH", historyEntry.cash);
            }

            if (historyEntry.reputation != 0)
            {
                uiElement.EnableParameterUIElement("REPUTATION", historyEntry.reputation);
            }

            // Handle background type  
            if (!string.IsNullOrEmpty(historyEntry.backgroundType))
            {
                if (historyEntry.backgroundType == "actor" && !string.IsNullOrEmpty(historyEntry.actorName))
                {
                    // Handle actor-type dialogues  
                    Actor actor = DialogueManager.MasterDatabase.GetActor(historyEntry.actorName);
                    if (actor != null)
                    {
                        uiElement.SetupActor(actor, "actor");
                    }
                }
                else
                {
                    // Handle other background types  
                    uiElement.SetBackground(historyEntry.backgroundType);
                }
            }

            // Add to our list of elements  
            textDialogueUIElements.Add(uiElement);
        }

        // Scroll to bottom after recreating all elements  
        Invoke("ScrollToBottomSmooth", 1);
    }



    void CreateImagetDialogueElement(string imageId)
    {
        string text = _subtitle.dialogueEntry.DialogueText;

        if (_subtitle.dialogueEntry.ActorID == 1)
        {
            return;
        }

        if (text.Length < 1)
        {
            return;
        }

        TextDialogueUIElement uIElement = Instantiate(imageDialogueUIElementPrefab, contentRectTransform);
        uIElement.imageGraphic.sprite = Resources.Load<Sprite>("gameChatImages/" + imageId);
        uIElement.SetText(_subtitle.dialogueEntry.DialogueText);
        uIElement.AnimatePopup();

        SoundManager.Instance.PlaySFX("dialoguePopupSfx");

        uIElement.UpdateFields(_subtitle);

        textDialogueUIElements.Add(uIElement);

        PlayButtonFx();
        ScrollToBottomSmooth();
    }


    void CreateTextDialogueElement()
    {
        string text = _subtitle.dialogueEntry.DialogueText;

        if (_subtitle.dialogueEntry.ActorID == 1)
        {
            return;
        }

        if (text.Length < 1)
        {
            return;
        }

        TextDialogueUIElement uIElement = Instantiate(textDialogueUIElementPrefab, contentRectTransform);
        uIElement.SetText(_subtitle.dialogueEntry.DialogueText);
        uIElement.AnimatePopup();

        SoundManager.Instance.PlaySFX("dialoguePopupSfx");

        //if(CheckDialogueEntryActor(_subtitle))  
        //{  
        //    //Debug.Log("Actor: " + _subtitle.dialogueEntry.ActorID);  
        //}  

        uIElement.UpdateFields(_subtitle);

        textDialogueUIElements.Add(uIElement);

        if (Field.FieldExists(_subtitle.dialogueEntry.fields, "Day"))
        {
            OnDayProgress?.Invoke(Field.LookupInt(_subtitle.dialogueEntry.fields, "Day"));
        }

        if (Field.FieldExists(_subtitle.dialogueEntry.fields, "DAY"))
        {
            int day = Field.LookupInt(_subtitle.dialogueEntry.fields, "DAY");
            if (day != 0)
            {
                _currentDay = day;
                OnDayChange?.Invoke(day);
            }
        }

        PlayButtonFx();
        ScrollToBottomSmooth();
    }

    bool CheckDialogueEntryActor(Subtitle _subtitle)
    {
        if (Field.FieldExists(_subtitle.dialogueEntry.fields, "ENTRY-BG"))
        {
            string bg = Field.LookupValue(_subtitle.dialogueEntry.fields, "ENTRY-BG");
            if (bg != string.Empty)
            {
                if (bg == "actor")
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void ChangeDay(int day)
    {
        OnDayChange?.Invoke(day);
    }

    IEnumerator ResponseButtons(Response[] responses)
    {
        yield return new WaitForSeconds(.3f);
        Debug.Log("Kishan Log ResponseButtons:" + responses.Length);
        var dialogueUI = DialogueManager.dialogueUI as StandardDialogueUI;


        for (int i = 0; i < responses.Length; i++)
        {
            if (Field.LookupBool(responses[i].destinationEntry.fields, "PAID_BY_REPUTATION"))
            {
                int required = Math.Abs(Field.LookupInt(responses[i].destinationEntry.fields, "REPUTATION"));
                int current = ScriptRegistry.Instance.textGameController.GetVariableUIElement("REPUTATION").currentValue;

                // if (current < required)
                // {
                // Not enough → Show RV panel instead of continuing
                // ScriptRegistry.Instance.rewardedVideoPanel.ShowRVPanel("REPUTATION", required, i);
                Debug.Log("Kishan Log PAID_BY_REPUTATION:" + i);

                ScriptRegistry.Instance.rewardedVideoPanel.CreateCurrencyResponseBtn("REPUTATION", required, i + 1);
                // }
            }
            if (Field.LookupBool(responses[i].destinationEntry.fields, "PAID_BY_POWER"))
            {
                int required = Math.Abs(Field.LookupInt(responses[i].destinationEntry.fields, "POWER"));
                int current = ScriptRegistry.Instance.textGameController.GetVariableUIElement("POWER").currentValue;
                Debug.Log($"Power Log {required} : {current}");
                // if (current < required)
                {
                    // Not enough → Show RV panel instead of continuing
                    // ScriptRegistry.Instance.rewardedVideoPanel.ShowRVPanel("POWER", required, i);
                    ScriptRegistry.Instance.rewardedVideoPanel.CreateCurrencyResponseBtn("POWER", required, i + 1);

                }
            }
            dialogueUI.conversationUIElements.defaultMenuPanel.instantiatedButtons[i].GetComponent<Button>().onClick.AddListener
            (
                delegate
                {
                    SoundManager.Instance.PlayButtonSfx();
                }
            );
        }
        // if (ScriptRegistry.Instance.rewardedVideoPanel.isCheckingResponse)
        // {
        //     ScriptRegistry.Instance.rewardedVideoPanel.AssignButtonEvent();
        //     ScriptRegistry.Instance.rewardedVideoPanel.isCheckingResponse = false;
        // }

        // for (int i = 0; i < responses.Length; i++)
        // {
        //     dialogueUI.conversationUIElements.defaultMenuPanel.instantiatedButtons[i].GetComponent<Button>().onClick.AddListener
        //     (
        //         delegate
        //         {
        //             SoundManager.Instance.PlayButtonSfx();
        //         }
        //     );
        // }
    }

    void ResetResponseButtons()
    {
        var dialogueUI = DialogueManager.dialogueUI as StandardDialogueUI;

        for (int i = 0; i < dialogueUI.conversationUIElements.defaultMenuPanel.instantiatedButtons.Count; i++)
        {
            dialogueUI.conversationUIElements.defaultMenuPanel.instantiatedButtons[i].GetComponent<ResponseButton>().Reset();
        }
    }

    public void ScrollToBottomSmooth()
    {
        responseDialogueRect.transform.SetAsLastSibling();

        StartCoroutine(SmoothScrollToBottom());
        PlayButtonFx();
    }

    void PlayButtonFx()
    {
        // Ignore if _subtitle is null (can happen when recreating history)  
        if (_subtitle == null)
        {
            return;
        }

        //Ignore first dialogeue Entry  
        if (_subtitle.dialogueEntry.id == 1)
        {
            return;
        }

        if (Field.FieldExists(_subtitle.dialogueEntry.fields, "ENTRY-BG"))
        {
            string entryType = Field.LookupValue(_subtitle.dialogueEntry.fields, "ENTRY-BG");
            advanceButton.PlayButtonEffect(entryType);
        }
    }

    private IEnumerator SmoothScrollToBottom()
    {
        yield return new WaitForEndOfFrame(); // Wait for UI to update  

        float elapsed = 0f;
        float duration = 0.3f; // Adjust duration as needed  
        float startValue = scrollRect.verticalNormalizedPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(startValue, 0f, elapsed / duration);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;
    }

    public VariableUIElement GetVariableUIElement(string id)
    {
        for (int i = 0; i < variableUIElements.Length; i++)
        {
            if (variableUIElements[i].id == id)
            {
                return variableUIElements[i];
            }
        }

        return null;
    }

    public void ShowVariableUIElements()
    {
        for (int i = 0; i < variableUIElements.Length; i++)
        {
            variableUIElements[i].Show(i);
        }
    }

    [Button]
    public bool isTextAreaHidden = false;

    [Button]
    public void MinimizeTextArea()
    {
        Camera cam = Camera.main;

        float startValue = cam.rect.y;
        float endValue = 0.25f;

        float targetPosY = -70f;
        textAreaRect.DOAnchorPosY(targetPosY, .5f).OnComplete(() => isTextAreaHidden = true);

        DOTween.To(() => startValue, x => startValue = x, endValue, .5f).OnUpdate(() =>
        {
            cam.rect = new Rect(0, startValue, 1, 1);
        });
    }

    [Button]
    public void ShowMinimisedTextArea()
    {
        Camera cam = Camera.main;

        float startValue = cam.rect.y;
        float endValue = 0.25f;

        float targetPosY = -70f;
        textAreaRect.DOAnchorPosY(targetPosY, .5f).OnComplete(() => isTextAreaHidden = false);

        DOTween.To(() => startValue, x => startValue = x, endValue, .5f).OnUpdate(() =>
        {
            cam.rect = new Rect(0, startValue, 1, 1);
        });

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("header").Show();

    }

    public void ToggleTextArea(bool hide, out Tween t)
    {
        Camera cam = Camera.main;

        float startValue = cam.rect.y;
        float endValue = hide ? 0f : 0.5f;

        float targetPosY = hide ? textAreaRect.anchoredPosition.y * -1 : textAreaStartRectYPOS;
        t = textAreaRect.DOAnchorPosY(targetPosY, .5f);

        DOTween.To(() => startValue, x => startValue = x, endValue, .5f).OnUpdate(() =>
        {
            cam.rect = new Rect(0, startValue, 1, 1);
        }).SetDelay(.6f);
    }

    public void SetHalfScreenCameraRect()
    {
        Camera cam = Camera.main;
        float startValue = cam.rect.y;
        float endValue = 0.5f;
        DOTween.To(() => startValue, x => startValue = x, endValue, .5f).OnUpdate(() =>
        {
            cam.rect = new Rect(0, startValue, 1, 1);
        });
    }

    public void SetFullScreenCameraRect()
    {
        Camera cam = Camera.main;
        float startValue = cam.rect.y;
        float endValue = 0f;
        DOTween.To(() => startValue, x => startValue = x, endValue, .5f).OnUpdate(() =>
        {
            cam.rect = new Rect(0, startValue, 1, 1);
        });
    }

    public void ToggleTextArea(bool hide)
    {
        float targetPosY = hide ? textAreaRect.anchoredPosition.y * -1 : textAreaStartRectYPOS;
        Tween t = textAreaRect.DOAnchorPosY(targetPosY, .5f);
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("header").Show();
    }

    public void CreateTextDialogueElement(string dialogueText, out TextDialogueUIElement textDialogueUI)
    {
        //print("Creating text dialogue element with text: " + dialogueText);
        TextDialogueUIElement uIElement = Instantiate(textDialogueUIElementPrefab, contentRectTransform);
        uIElement.SetText(dialogueText);
        uIElement.AnimatePopup();

        textDialogueUIElements.Add(uIElement);

        PlayButtonFx();
        ScrollToBottomSmooth();
        textDialogueUI = uIElement;
    }

    public void CreateBusinessDialogueElement(string dialogueText, BusinessHandler.BusinessType business, out TextDialogueUIElement textDialogueUI)
    {
        //print("Creating text dialogue element with text: " + dialogueText);
        TextDialogueUIElement uIElement = Instantiate(businessDialogueUIElementPrefab, contentRectTransform);
        uIElement.SetText(dialogueText);
        uIElement.imageGraphic.sprite = business.businessIcon;
        uIElement.AnimatePopup();

        textDialogueUIElements.Add(uIElement);

        PlayButtonFx();
        ScrollToBottomSmooth();
        textDialogueUI = uIElement;
    }

    public void HideTextArea()
    {
        header.Hide();
        ToggleTextArea(true, out Tween t);
        isTextAreaHidden = true;
    }

    public void ShowTextArea()
    {
        ToggleTextArea(false, out Tween t);
        isTextAreaHidden = false;

        t.OnComplete(() => { header.Show(); });
    }

    [Button]
    public void ToggleTextArea()
    {
        if (isTextAreaHidden)
            ShowTextArea();
        else
            HideTextArea();
    }

    public void AdvanceConversation()
    {
        if (DialogueManager.isConversationActive)
        {
            DialogueManager.instance.SendMessage(
                DialogueSystemMessages.OnConversationContinue,
                DialogueManager.dialogueUI,
                SendMessageOptions.DontRequireReceiver);
        }
    }

    public int GetDayMaxProgression()
    {
        int maxprogress = 0;

        List<DialogueEntry> dialogueEntries = DialogueManager.masterDatabase.GetConversation(DialogueManager.lastConversationStarted).dialogueEntries;
        for (int i = 0; i < dialogueEntries.Count; i++)
        {
            if (Field.FieldExists(dialogueEntries[i].fields, "Day"))
            {
                int value = Field.LookupInt(dialogueEntries[i].fields, "Day");
                maxprogress += value;
            }
        }

        return maxprogress;
    }
}
