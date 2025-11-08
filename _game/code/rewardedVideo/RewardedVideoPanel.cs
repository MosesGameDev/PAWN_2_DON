using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;
using System;
using UnityEngine.Events;
using Sirenix.Utilities;
using System.Collections.Generic;
using NSubstitute.Core;

public class RewardedVideoPanel : MonoBehaviour
{
    /// <summary>
    /// Needs variable, requiredValueId, buttonIndex to be affected(0-1)
    /// </summary>

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI reqText;
    public UIDialogueElement rvPanel;
    public Image iconImage;
    [ReadOnly] private string requiredValueId;

    [Space]
    public CanvasGroup responseCanvasGroup;
    [Space]
    public Button NotEnoughRVBtn;
    private int requiredValue;
    private int affectedButton;

    int conversationId;

    [Space]
    public RectTransform scrollView;
    [SerializeField] private Transform responsePanel;

    private bool logPanelOpen = false;
    [Button]
    // public void ShowRVPanel(int _requiredValue)
    // {
    //     requiredValue = _requiredValue;
    //     CheckPowerValue();
    //     titleText.text = "NOT ENOUGH " + requiredValueId;

    //     int v = ScriptRegistry.Instance.textGameController.GetVariableUIElement(requiredValueId).currentValue;
    //     int _v = requiredValue - v;

    //     reqText.text = $"+{_v} Free";

    // }
    public void CreateCurrencyResponseBtn(string requiredValueId, int requiredValue, int responseIndex)
    {
        int currentValue = ScriptRegistry.Instance.textGameController
                            .GetVariableUIElement(requiredValueId).currentValue;
        int missingValue = requiredValue - currentValue;
        Button resBtn = null;
        Debug.Log("Kishan Log responseIndex:" + responseIndex);
        var dialogueUI = DialogueManager.dialogueUI as StandardDialogueUI;

        if (responseIndex == 2)
        {
            GameObject obj = Instantiate(responsePanel.GetChild(responseIndex).gameObject, responsePanel);
            dialogueUI.conversationUIElements.defaultMenuPanel.instantiatedPaidButtons.Add(obj);
            resBtn = obj.GetComponent<Button>();

            resBtn.onClick.RemoveAllListeners();
            resBtn.onClick.AddListener(() => PaidBtnClick(requiredValueId, requiredValue, responseIndex, resBtn, responsePanel.GetChild(responseIndex).GetComponent<Button>()));
            responsePanel.GetChild(responseIndex).gameObject.SetActive(false);

        }
        else
        {
            GameObject obj = Instantiate(responsePanel.parent.GetChild(1).gameObject, responsePanel.parent);
            dialogueUI.conversationUIElements.defaultMenuPanel.instantiatedPaidButtons.Add(obj);
            resBtn = obj.GetComponent<Button>();

            resBtn.onClick.AddListener(() => PaidBtnClick(requiredValueId, requiredValue, responseIndex, resBtn, responsePanel.parent.GetChild(1).GetComponent<Button>()));
            responsePanel.parent.GetChild(1).gameObject.SetActive(false);

        }
        Debug.Log($"<color=cyan>Value of {responsePanel.GetChild(affectedButton).gameObject}</color>", responsePanel.GetChild(affectedButton).gameObject);
        resBtn.GetComponent<ResponseButton>().rvDefaultTex.gameObject.SetActive(false);
        resBtn.GetComponent<ResponseButton>().rvPanel.SetActive(true);
        resBtn.GetComponent<ResponseButton>().iconImage.sprite = ScriptRegistry.Instance.textGameController.GetVariableUIElement(requiredValueId).UIParticleAttractor.GetComponent<Image>().sprite;
        resBtn.GetComponent<ResponseButton>().rvButtonTex.text = resBtn.GetComponent<ResponseButton>().rvDefaultTex.text + " : ";
        resBtn.GetComponent<ResponseButton>().rvValueText.text = requiredValue.ToString();
    }

    public void PaidBtnClick(string requiredValueId, int requiredValue, int responseIndex, Button paidBtn, Button main)
    {
        this.requiredValueId = requiredValueId;
        this.requiredValue = requiredValue;
        int currentValue = ScriptRegistry.Instance.textGameController
                             .GetVariableUIElement(requiredValueId).currentValue;

        if (currentValue >= requiredValue)
        {
            // if (responseIndex == 3)
            {
                Debug.Log($"Kishan Paid btn Click with enough currency");
                main.onClick?.Invoke();
            }

        }
        else
        {
            ShowRVPanel(requiredValueId, requiredValue, responseIndex);
        }
    }

    public void ShowRVPanel(string requiredValueId, int requiredValue, int responseIndex)
    {
        int currentValue = ScriptRegistry.Instance.textGameController
                              .GetVariableUIElement(requiredValueId).currentValue;

        int missingValue = requiredValue - currentValue;

        // Update UI
        titleText.text = $"NOT ENOUGH {requiredValueId}";
        reqText.text = $"+{missingValue} Free";

        responseCanvasGroup.interactable = true;

        NotEnoughRVBtn.onClick.RemoveAllListeners();
        NotEnoughRVBtn.onClick.AddListener(() => OnClickRv(responseIndex));

        rvPanel.Show();
        // scrollView.DOAnchorPosY(-270, 0.3f);

        iconImage.sprite = ScriptRegistry.Instance.textGameController
                             .GetVariableUIElement(requiredValueId)
                             .UIParticleAttractor.GetComponent<Image>().sprite;

        RectTransform r = ScriptRegistry.Instance.textGameController
                              .GetVariableUIElement(requiredValueId)
                              .UIParticleAttractor.GetComponent<RectTransform>();
        r.DOScale(1.2f, 0.3f).SetLoops(5, LoopType.Yoyo).OnComplete(() =>
        {
            r.DOScale(1f, 0.1f);
        });

        // Log analytics only once
        if (!logPanelOpen)
        {
            int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");
            if (PlayerPrefs.HasKey("COMPLETED"))
            {
                int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
                day += resetCount * 10;
            }

            AnalyticsEvents.instance.UniqueEvent($"showpanel_rewardedVideo_day_{day}");
            logPanelOpen = true;
        }
    }


    public void SetRequiredVariableID(string variableId)
    {
        requiredValueId = variableId;
    }

    public void SetAffectedButton(int affectedButtonIndex)
    {
        affectedButton = affectedButtonIndex;
    }

    public void SetNextEntryIdButton(int affectedButtonIndex)
    {
        conversationId = affectedButtonIndex;
    }

    [Button]
    public void HideRVPanel()
    {
        rvPanel.Hide();
        // scrollView.DOAnchorPosY(0, 0.3f);

        responseCanvasGroup.interactable = true;
    }


    public bool isCheckingResponse = false;
    public void EnableResponseCheck()
    {
        isCheckingResponse = true;
    }



    public void CheckPowerValue()
    {
        EnableResponseCheck();
        //responseCanvasGroup.interactable = false;
        //StartCoroutine(WaitForResponse());
        //print($"<color=cyan>Checking {requiredValueId} for value {requiredValue}</color>");
    }

    // public void AssignButtonEvent()
    // {
    //     //responsePanel.GetChild(affectedButton).GetComponent<Button>().onClick.RemoveAllListeners();
    //     //responsePanel.GetChild(affectedButton).GetComponent<Button>().onClick.AddListener(ShowPanel);

    //     if (b == null)
    //     {
    //         GameObject obj = Instantiate(responsePanel.GetChild(affectedButton).gameObject, responsePanel);
    //         b = obj.GetComponent<Button>();

    //         //Debug.Log($"<color=cyan>Value of {responsePanel.GetChild(affectedButton).gameObject}</color>", responsePanel.GetChild(affectedButton).gameObject);

    //         responsePanel.GetChild(affectedButton).gameObject.SetActive(false);

    //         b.GetComponent<ResponseButton>().rvDefaultTex.gameObject.SetActive(false);
    //         b.GetComponent<ResponseButton>().rvPanel.SetActive(true);
    //         b.GetComponent<ResponseButton>().iconImage.sprite = ScriptRegistry.Instance.textGameController.GetVariableUIElement(requiredValueId).UIParticleAttractor.GetComponent<Image>().sprite;
    //         b.GetComponent<ResponseButton>().rvButtonTex.text = b.GetComponent<ResponseButton>().rvDefaultTex.text + " : ";
    //         b.GetComponent<ResponseButton>().rvValueText.text = requiredValue.ToString();


    //         b.onClick.AddListener(ShowPanel);

    //         if (affectedButton == 1)
    //         {
    //             b.transform.SetSiblingIndex(1);
    //         }

    //         int secButtIndex = 0;
    //         if (affectedButton == 1)
    //         {
    //             secButtIndex = 3; // If affected button is 1, then the second button is at index 2
    //         }
    //         else if (affectedButton == 2)
    //         {
    //             secButtIndex = 1; // Otherwise, it's the same as affected button
    //         }

    //         x = responsePanel.GetChild(secButtIndex).GetComponent<Button>();
    //         x.onClick.AddListener(Skip);
    //     }

    // }

    // public void Skip()
    // {
    //     x.onClick.RemoveListener(Skip);
    //     OnWatchRewardedVideo();

    //     int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

    //     if (PlayerPrefs.HasKey("COMPLETED"))
    //     {
    //         int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
    //         int d = resetCount * 10;
    //         day += d;
    //     }
    //     AnalyticsEvents.instance.UniqueEvent($"rewardedVideo_{requiredValueId.ToLower()}_skipped_day_{day}");
    //     logPanelopen = false;

    // }

    public void OnClickRv(int index)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AdsManager.instance.ShowRewardAd(()=>OnRVComplete(index));
#endif
#if UNITY_EDITOR
        OnRVComplete(index);
#endif
    }


    int GetDay()
    {
        return PlayerPrefs.GetInt("START_CONVERSATION_ID"); // from home screen manager
    }

    void OnRVComplete(int index)
    {

        OnWatchRewardedVideo(index);
        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");
        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            day += resetCount * 10;
        }

        AnalyticsEvents.instance.UniqueEvent($"rewardedVideo_{requiredValueId.ToLower()}_watched_day_{day}");
    }
    public void OnWatchRewardedVideo(int buttonIndex)
    {
        Debug.Log($"Video Rv Index :" + buttonIndex);
        if (buttonIndex == 2)
        {
            Destroy(responsePanel.GetChild(3).gameObject);
            responsePanel.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            Destroy(responsePanel.parent.GetChild(2).gameObject);
            responsePanel.parent.GetChild(1).gameObject.SetActive(true);
        }


        // Update variable value if needed
        int currentValue = ScriptRegistry.Instance.textGameController
                        .GetVariableUIElement(requiredValueId).currentValue;
        int missingValue = requiredValue - currentValue;
        if (missingValue > 0)
        {
            ScriptRegistry.Instance.textGameController
                .GetVariableUIElement(requiredValueId).UpdateUIElement(missingValue);
        }

        HideRVPanel();

        // Transform originalButton = responsePanel.GetChild(buttonIndex);
        // originalButton.gameObject.SetActive(true);
        // Button btnComp = originalButton.GetComponent<Button>();
        // btnComp.interactable = true;

        // // Optional punch scale effect
        // originalButton.GetComponent<RectTransform>()
        //     .DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1)
        //     .SetDelay(1)
        //     .OnComplete(() => originalButton.localScale = Vector3.one);

        // // Update variable value if needed
        // int currentValue = ScriptRegistry.Instance.textGameController
        //                     .GetVariableUIElement(requiredValueId).currentValue;
        // int missingValue = requiredValue - currentValue;
        // if (missingValue > 0)
        // {
        //     ScriptRegistry.Instance.textGameController
        //         .GetVariableUIElement(requiredValueId).UpdateUIElement(missingValue);
        // }

        // HideRVPanel();
        // logPanelOpen = false;
    }
}
