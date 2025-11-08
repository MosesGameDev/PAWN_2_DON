using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;


public class Day02CutSceneEventHandler : CutSceneEventHandler
{

    [Title("PlayableDirector")]
    [SerializeField] private CutSceneSceneRefrences cutSceneSceneRefrences;
    [SerializeField] private CutSceneManager cutSceneManager;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    [Button("1.Start")]
    public override void StartDay()
    {
        base.StartDay();
        PlayWakeupCutscene();
    }

    public void PlayWakeupCutscene()
    {
        if (PlayerPrefs.HasKey("HUNGER_INITIALIZED"))
        {
            PlayerPrefs.DeleteKey("HUNGER_INITIALIZED");
        }
        AnalyticsEvents.instance.OnLevelStart(2);


        SaveGameManager.instance.LoadVariableData();
        //SaveGameManager.instance.SaveVariables();

        EnableSelectedCharacter();
        cutSceneSceneRefrences.DisableGameObjects();

        ScriptRegistry.Instance.textGameController.ShowVariableUIElements();
        transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = cutSceneManager.GetClip("1-day2-Start");
        cutSceneManager.PlayClipFromStart(clip.id);
        cutSceneManager.ContinueConversation("DAY02", 43);

        ScriptRegistry.Instance.hungerHandler.ClearSavedData();
    }


    [Button("2.PlayPawnShop")]
    public void PlayPawnShop()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = cutSceneManager.GetClip("2-Pawn-offer");
        cutSceneManager.PlayClipFromStart(clip.id);

        // cutSceneManager.ContinueConversation("DAY02", 70);
    }


    [Button("3.PlayStartDelivery")]
    public void PlayStartDelivery()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = cutSceneManager.GetClip("4-Delivery start");
        cutSceneManager.PlayClipFromStart(clip.id);

        cutSceneManager.ContinueConversation("DAY02", 58);
    }


    [Button("4.Return Home")]
    public void PlayReturnHome()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = cutSceneManager.GetClip("6-Return home");
        cutSceneManager.PlayClipFromStart(clip.id);
        cutSceneManager.ContinueConversation("DAY02", 65);

    }



    public void StartDeliveryMinigame()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        cutSceneSceneRefrences.DisableGameObjects();
        ScriptRegistry.Instance.deliveryMinigameLoader.LoadScene("Day02DrivingGame");
    }


}
