using Sirenix.OdinInspector;
using UnityEngine;

public class Day11CutSceneEventHandler : CutSceneEventHandler
{

    [SerializeField] private CutSceneSceneRefrences cutSceneSceneRefrences;

    [Button("Start Day")]
    public override void StartDay()
    {
        base.StartDay();
        SaveGameManager.instance.LoadVariableData();

        cutSceneSceneRefrences.DisableGameObjects();
        ScriptRegistry.Instance.businessHandler.ShowBusinessEarnings();

        ScriptRegistry.Instance.textGameController.ShowVariableUIElements();

        cutSceneSceneRefrences.cutSceneManager.ContinueConversation("DAY011", 38);
    }
}
