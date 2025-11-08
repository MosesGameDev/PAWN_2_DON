using Sirenix.OdinInspector;
using UnityEngine;
using static BusinessHandler;
using DG.Tweening;

public class Day05CutSceneEventHandler : CutSceneEventHandler
{
    [SerializeField] private CutSceneSceneRefrences sceneSceneRefrences;
    [SerializeField] private BusinessHandler businessHandler;

    public enum BusinessSelection
    {
        CounterfeitGoods,
        IllegalLotteries
    }

    [SerializeField] private BusinessClass businessSelection;

    private void Start()
    {
        businessHandler.onBusinessUnlocked += SelectBusiness;
        transform.GetChild(0).gameObject.SetActive(false);
    }


    [Button("1. Start")]
    public override void StartDay()
    {
        base.StartDay();
        AnalyticsEvents.instance.OnLevelStart(5);
        SaveGameManager.instance.LoadVariableData();


        ScriptRegistry.Instance.textGameController.ShowVariableUIElements();

        sceneSceneRefrences.DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = sceneSceneRefrences.cutSceneManager.GetClip("1-Day5 start");
        sceneSceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);

        ScriptRegistry.Instance.screenFade.FadeInFast(1);
        sceneSceneRefrences.cutSceneManager.ContinueConversation("DAY05", 1);

        sceneSceneRefrences.cutSceneManager.gameOverRestartButton.onClick.RemoveAllListeners();
        sceneSceneRefrences.cutSceneManager.gameOverRestartButton.onClick.AddListener(Replay);

    }

    [Button("2. Show Business Panel")]
    public void OpenBusinessFTUE()
    {
        if(businessHandler.hasPlayedFTUE)
        {
            PlayICU();
            return;
        }
        businessHandler.OpenBusinessFTUE();
    }


    [Button("3. Unlock Business")]
    public void UnlockBusiness()
    {
        businessSelection = BusinessClass.CounterfeitGoods;
        SelectBusiness(businessSelection);
    }


    public void SelectBusiness(BusinessClass selection)
    {
        if (businessHandler.isPlayingFTUE)
        {
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("foodAnimatedPointer_business_unlock_03").Show();

            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("softMask").Hide();

            ScriptRegistry.Instance.businessHandler.ToggleBusinessButtons(false);

            businessSelection = selection;
            switch (businessSelection)
            {
                case BusinessClass.CounterfeitGoods:
                    sceneSceneRefrences.cutSceneManager.ContinueConversation("DAY05", 151);
                    break;
                case BusinessClass.IllegalLotteries:
                    sceneSceneRefrences.cutSceneManager.ContinueConversation("DAY05", 150);
                    break;
            }

            businessHandler.shadowMaskShape.SetActive(false);
            //businessHandler.DisableBusinessUnlockButtons();
            businessHandler.ClosePanel.interactable = true;
            BusinessType businessType = businessHandler.GetBusinessByClass(businessSelection);
            businessType.uiElementRectTransform.SetAsFirstSibling();
            businessType.businessUnlocked = true;
            sceneSceneRefrences.cutSceneManager.HideButtonElipsis();
        }


    }


    public void Close()
    {
        switch (businessSelection)
        {
            case BusinessClass.CounterfeitGoods:
                sceneSceneRefrences.cutSceneManager.ContinueConversation("DAY05", 151);
                break;
            case BusinessClass.IllegalLotteries:
                sceneSceneRefrences.cutSceneManager.ContinueConversation("DAY05", 150);
                break;
        }

        CutSceneClip clip = sceneSceneRefrences.cutSceneManager.GetClip("2-business running");
        sceneSceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);
    }


    [Button("4. Play ICU")]
    public void PlayICU()
    {
        sceneSceneRefrences.DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = sceneSceneRefrences.cutSceneManager.GetClip("4-in ICU");
        sceneSceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);

        ScriptRegistry.Instance.screenFade.FadeInFast(1);

        sceneSceneRefrences.cutSceneManager.ContinueConversation("DAY05", 160);

    }


    public void Replay()
    {
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("dayCompleted").Hide();
    }


    private void OnDestroy()
    {
        if (businessHandler != null)
        {
            businessHandler.onBusinessUnlocked -= SelectBusiness;
        }
    }
}

[System.Serializable]
public class CutSceneSceneRefrences
{
    public CutSceneManager cutSceneManager;
    public GameObject[] objectsToDisable;



    public void DisableGameObjects()
    {
        foreach (GameObject g in objectsToDisable)
        {
            g.SetActive(false);
        }
    }
}