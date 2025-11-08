using UnityEngine;
using Sirenix.OdinInspector;
using PixelCrushers.DialogueSystem;
using System.Collections;

public class Day04CutSceneEventHandler : CutSceneEventHandler
{
    [SerializeField] private CutSceneManager cutSceneManager;
    [SerializeField] private GameObject[] objectsToDisable;


    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void DisableGameObjects()
    {
        foreach (GameObject g in objectsToDisable)
        {
            g.SetActive(false);
        }
    }


    [Button("1. Start")]
    public override void StartDay()
    {
        base.StartDay();
        AnalyticsEvents.instance.OnLevelStart(4);

        SaveGameManager.instance.LoadVariableData();

        DisableGameObjects();
        ScriptRegistry.Instance.textGameController.ShowVariableUIElements();

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("softMask").Hide();

        transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = cutSceneManager.GetClip("1-Day4 Start");
        cutSceneManager.PlayClipFromStart(clip.id);

        ScriptRegistry.Instance.screenFade.FadeInFast(1);
        cutSceneManager.ContinueConversation("DAY04", 257);

    }

    [Button("2. WareHouse")]

    public void PlayWareHouse()
    {
        DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = cutSceneManager.GetClip("3-warehouse");
        cutSceneManager.PlayClipFromStart(clip.id);

        ScriptRegistry.Instance.screenFade.FadeInFast(1);
        cutSceneManager.ContinueConversation("DAY04", 266);

    }
    
    
    [Button("3. Bank minigame")]

    public void PlayBank()
    {
        DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        ScriptRegistry.Instance.textGameController.SetHalfScreenCameraRect();

        CutSceneClip clip = cutSceneManager.GetClip("4-Bank");
        cutSceneManager.PlayClipFromStart(clip.id);

        ScriptRegistry.Instance.screenFade.FadeInFast(1);
        cutSceneManager.ContinueConversation("DAY04", 306);
    }

    [Button("4. PlayBankHeist")]

    public void PlayBankHeist()
    {
        DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = cutSceneManager.GetClip("4-Bank");
        cutSceneManager.PlayClipFromTime(clip.id, 9f);

        ScriptRegistry.Instance.screenFade.FadeInFast(1);
        cutSceneManager.ContinueConversation("DAY04", 273);
    }


    bool ratePopupShown = false;
    public void ShowRatePopup()
    {
        if (ratePopupShown)
        {
            Debug.Log("Rate popup already shown for this day.");
            return;
        }
        Gley.RateGame.API.ForceShowRatePopupWithCallback(PopupClosedMethod);
        ratePopupShown = true;
        //Gley.RateGame.API.ShowNativeRatePopup();
    }

    private void PopupClosedMethod(Gley.RateGame.PopupOptions result, string message)
    {
        Debug.Log($"Popup Closed-> Result: {result}, Message: {message} -> Resume Game");
    }



    [Button("4. Cop")]

    public void PlayCop()
    {
        DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = cutSceneManager.GetClip("6-Cop forward");
        cutSceneManager.PlayClipFromStart(clip.id);

        //ScriptRegistry.Instance.screenFade.FadeInFast(1);
    } 
    
    
    [Button("4. Run Away_Editor")]

    public void RunAway()
    {
        DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = cutSceneManager.GetClip("7-Fire and runaway");
        cutSceneManager.PlayClipFromTime(clip.id, 11);

        cutSceneManager.ContinueConversation("DAY04", 295);
    }
    
    [Button("4. Run Away")]

    public void PlayRunAway()
    {
        DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = cutSceneManager.GetClip("7-Fire and runaway");
        cutSceneManager.PlayClipFromStart(clip.id);

        //cutSceneManager.ContinueConversation("DAY04", 291);
    }

    public void PlayDrivingMinigame()
    {
        cutSceneManager.FadeIn();
        transform.GetChild(0).gameObject.SetActive(false);
        cutSceneManager.PauseCutscene();
        ScriptRegistry.Instance.deliveryMinigameLoader.LoadScene("Day04DrivingGame");
    }

    [Button]
    public void TestUnlockDeliveries()
    {
        ScriptRegistry.Instance.deliveryMinigameLoader.UnlockDeliveryMinigames();

    }

    public void CheckDay()
    {
        int day = DialogueLua.GetVariable("DAY").asInt;
        if (day == 4)
        {
            StartCoroutine(DeliveryFtue());
        }
    }

    public void PlayGunFx()
    {
        SoundManager.Instance.PlaySFX("gunshot");
    }

    IEnumerator DeliveryFtue()
    {
        int day = DialogueLua.GetVariable("DAY").asInt;
        yield return new WaitForSeconds(1f);
        ScriptRegistry.Instance.deliveryMinigameLoader.UnlockDeliveryMinigames();
        print($"<color=cyan>Day {day} XXXXX unlocked</color>");
    }



    public void OnCompleteDrivingMinigame()
    {
        cutSceneManager.FadeIn();
        DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = cutSceneManager.GetClip("4-Bank");
        cutSceneManager.PlayClipFromTime(clip.id, 7.466667f);
        ScriptRegistry.Instance.textGameController.SetHalfScreenCameraRect();
        ScriptRegistry.Instance.textGameController.ToggleTextArea(false);
        ScriptRegistry.Instance.homeScreenManager.homeScreenButtonParent.SetActive(false);

    }

    [Button("5.Play Freeze ")]

    public void PlayFreeze()
    {
        DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = cutSceneManager.GetClip("5-2_Freeze");
        cutSceneManager.PlayClipFromStart(clip.id);

        //cutSceneManager.ContinueConversation("DAY04", 291);
    }

}
