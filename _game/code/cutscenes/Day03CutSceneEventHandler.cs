using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class Day03CutSceneEventHandler : CutSceneEventHandler
{
    [SerializeField] private CutSceneManager cutSceneManager;
    [SerializeField] private GameObject[] objectsToDisable;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void StartDay()
    {
        base.StartDay();
        StartDay3();
    }


    [Button("1. Start Day 03")]
    public void StartDay3()
    {
        base.StartDay();
        AnalyticsEvents.instance.OnLevelStart(3);

        SaveGameManager.instance.LoadVariableData();

        DisableGameObjects();
        ScriptRegistry.Instance.textGameController.ShowVariableUIElements();

        if (!DialogueManager.isConversationActive)
        {
            DialogueManager.instance.StartConversation("DAY03");
        }

        DialogueLua.SetVariable("DAY", 3);
        int day = DialogueLua.GetVariable("DAY").asInt;

        transform.GetChild(0).gameObject.SetActive(false);
        cutSceneManager.dayCompleteHandler.transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = cutSceneManager.GetClip("wakeUp");
        cutSceneManager.PlayClipFromStart(clip.id);

        cutSceneManager.ContinueConversation("DAY03", 129);

    }

    public void CheckDay()
    {
        int day = DialogueLua.GetVariable("DAY").asInt;
        if (day == 3)
        {
            cutSceneManager.PauseCutscene();
        }
    }

    public void StartWalkingSim()
    {
        cutSceneManager.dayCompleteHandler.transform.GetChild(0).gameObject.SetActive(false);
        ScriptRegistry.Instance.walkingSimulator.Initialize();
        ScriptRegistry.Instance.cutSceneManager.ShowNextDialogueEntry();
    }

    [Button("2. Start WareHouse")]
    public void VisitWareHouse()
    {
        ScriptRegistry.Instance.walkingSimulator.Exit();
        transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = cutSceneManager.GetClip("2-Warehouse");
        cutSceneManager.PlayClipFromStart(clip.id);
    }

    [Button("2. Start WareHouse Editor")]
    public void VisitWareHouseEditor()
    {
        cutSceneManager.ContinueConversation("DAY03", 162);
    }

    [Button("3. Take Package")]
    public void TakePackage()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = cutSceneManager.GetClip("4-Give_Package");
        cutSceneManager.PlayClipFromStart(clip.id);
    }

    public override void EnableSelectedCharacter()
    {
        base.EnableSelectedCharacter();
    }

    [Button("4. Start Cycling")]
    public void StartCycling()
    {
        EnableSelectedCharacter();
        transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = cutSceneManager.GetClip("5-on the road");
        cutSceneManager.PlayClipFromStart(clip.id);

        cutSceneManager.ContinueConversation("DAY03", 147);
    }

    [Button("5. Return To warehouse")]
    public void ReturnToWarehouse()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        cutSceneManager.ContinueConversation("DAY03", 157);
    }


    [Button]
    void TestUnlockCostumes()
    {
        //ScriptRegistry.Instance.characterCustomizationHandler.EnableCostumeButton();
    }

    public void UnlockCostumes()
    {
        //int day = DialogueLua.GetVariable("DAY").asInt;
        //if (day == 3)
        //{

        //    ScriptRegistry.Instance.characterCustomizationHandler.EnableCostumeButton();
        //    AnalyticsEvents.instance.UniqueEvent("costumes_unlocked");
        //    return;
        //}
    }

    public void DisableGameObjects()
    {
        foreach (GameObject g in objectsToDisable)
        {
            g.SetActive(false);
        }
    }

    private void OnDisable()
    {
        ScriptRegistry.Instance.walkingSimulator.OnInitialized -= StartWalkingSim;
    }
}
