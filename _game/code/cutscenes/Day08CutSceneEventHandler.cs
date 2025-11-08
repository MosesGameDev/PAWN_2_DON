using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class Day08CutSceneEventHandler : CutSceneEventHandler
{
    [SerializeField] private CutSceneSceneRefrences sceneSceneRefrences;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    [Button("1.StartDay")]
    public override void StartDay()
    {
        base.StartDay();
        AnalyticsEvents.instance.OnLevelStart(8);

        SaveGameManager.instance.LoadVariableData();

        ScriptRegistry.Instance.textGameController.ShowVariableUIElements();
        AssignReplayButtonEvents();

        StartCoroutine(DelayStartDay());
    }

    [Button]
    public override void ShowDayCompleteScreen()
    {
        base.ShowDayCompleteScreen();
    }

    IEnumerator DelayStartDay()
    {
        yield return new WaitForSeconds(1f);
        sceneSceneRefrences.DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = sceneSceneRefrences.cutSceneManager.GetClip("1-Day-8_Start");
        ScriptRegistry.Instance.businessHandler.ShowBusinessEarnings();
        sceneSceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);
        ScriptRegistry.Instance.screenFade.FadeInFast(1);
        sceneSceneRefrences.cutSceneManager.ContinueConversation("DAY08", 60);

        AssignReplayButtonEvents();

    }

    private void AssignReplayButtonEvents()
    {
        sceneSceneRefrences.cutSceneManager.gameOverRestartButton.onClick.RemoveAllListeners();
        sceneSceneRefrences.cutSceneManager.gameOverRestartButton.onClick.AddListener(() =>
        {
            ScriptRegistry.Instance.textGameController.ClearPrevousDayTextUIElements();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("dayCompleted").Hide();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("chatContent").Show();
            ScriptRegistry.Instance.gameOverScreen.Hide();

            sceneSceneRefrences.cutSceneManager.SetCameraHalfScreen();
            ScriptRegistry.Instance.textGameController.ShowTextArea();
            StartDay();
        });
    }

    [Button("2.MeetRico")]
    public void MeetRico()
    {
        sceneSceneRefrences.DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = sceneSceneRefrences.cutSceneManager.GetClip("3-Meet_Rico");
        sceneSceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);
        ScriptRegistry.Instance.screenFade.FadeInFast(1);
        sceneSceneRefrences.cutSceneManager.ContinueConversation("DAY08", 64);
    }

    public void GameOver()
    {
        ScriptRegistry.Instance.gameOverScreen.Show("WASTED!", "You have been caught by the police!");
        AnalyticsEvents.instance.UniqueEvent("level_8_fail");

    }
}
