using UnityEngine;
using Sirenix.OdinInspector;

public class Day10CutSceneEventHandler : CutSceneEventHandler
{
    [SerializeField] private CutSceneSceneRefrences cutSceneSceneRefrences;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }


    [Button("Start Day")]
    public override void StartDay()
    {
        base.StartDay();
        AnalyticsEvents.instance.OnLevelStart(10);

        transform.GetChild(0).gameObject.SetActive(false);
        cutSceneSceneRefrences.cutSceneManager.HideButtonElipsis();

        SaveGameManager.instance.LoadVariableData();

        cutSceneSceneRefrences.DisableGameObjects();
        ScriptRegistry.Instance.businessHandler.ShowBusinessEarnings();

        ScriptRegistry.Instance.textGameController.ShowVariableUIElements();

        cutSceneSceneRefrences.cutSceneManager.ContinueConversation("DAY010", 1);
    }

    [Button("Meet Blaze")]
    public void PlayMeetBlaze()
    {
        cutSceneSceneRefrences.DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = cutSceneSceneRefrences.cutSceneManager.GetClip("1-Meet_Blaze");
        cutSceneSceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);
    }
}