using Sirenix.OdinInspector;
using UnityEngine;

public class Day09CutSceneEventHandler : CutSceneEventHandler
{
    [SerializeField] private CutSceneSceneRefrences sceneRefrences;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }


    [Button("1.StartDay")]
    public override void StartDay()
    {
        base.StartDay();
        AnalyticsEvents.instance.OnLevelStart(9);

        SaveGameManager.instance.LoadVariableData();

        sceneRefrences.DisableGameObjects();
        ScriptRegistry.Instance.textGameController.ShowVariableUIElements();

        transform.GetChild(0).gameObject.SetActive(true);
        ScriptRegistry.Instance.businessHandler.ShowBusinessEarnings();

        CutSceneClip clip = sceneRefrences.cutSceneManager.GetClip("1-Day-9_Start");
        sceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);
        ScriptRegistry.Instance.screenFade.FadeInFast(1);
        sceneRefrences.cutSceneManager.ContinueConversation("DAY09", 61);
        AssignReplayButtonEvents();
    }

    private void AssignReplayButtonEvents()
    {
        sceneRefrences.cutSceneManager.gameOverRestartButton.onClick.RemoveAllListeners();
        sceneRefrences.cutSceneManager.gameOverRestartButton.onClick.AddListener(() =>
        {
            ScriptRegistry.Instance.textGameController.ClearPrevousDayTextUIElements();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("dayCompleted").Hide();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("chatContent").Show();

            sceneRefrences.cutSceneManager.SetCameraHalfScreen();
            ScriptRegistry.Instance.textGameController.ShowTextArea();
            StartDay();
        });
    }

    [Button("2.MeetRico")]

    public void MeetRico()
    {
        sceneRefrences.DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = sceneRefrences.cutSceneManager.GetClip("2-Head to warehouse");
        sceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);
        ScriptRegistry.Instance.screenFade.FadeInFast(1);
        sceneRefrences.cutSceneManager.ContinueConversation("DAY09", 65);
    }


    [Button("3.Arrive at Garage")]
    public void ArriveGarage()
    {
        ScriptRegistry.Instance.textGameController.SetHalfScreenCameraRect();

        sceneRefrences.DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);
        CutSceneClip clip = sceneRefrences.cutSceneManager.GetClip("4-Arrive at Garage");
        sceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);
        ScriptRegistry.Instance.screenFade.FadeInFast(1);

        sceneRefrences.cutSceneManager.ContinueConversation("DAY09", 89);
    }

    [Button("4.Play DrivingMinigame")]
    public void PlayMiniGame()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        ScriptRegistry.Instance.deliveryMinigameLoader.LoadScene("Day09DrivingGame");
    }

    public void EndDay()
    {
        sceneRefrences.cutSceneManager.FadeIn(.5f);
        transform.GetChild(0).gameObject.SetActive(false);

        ScriptRegistry.Instance.dayCompleteHandler.GoToSleep();
    }


}

