using Sirenix.OdinInspector;
using UnityEngine;

public class Day07CutSceneEventHandler : CutSceneEventHandler
{
    [SerializeField] private CutSceneSceneRefrences refrences;
    [SerializeField] private CutSceneManager cutSceneManager;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    [Button("1.Start")]
    public override void StartDay()
    {
        base.StartDay();
        AnalyticsEvents.instance.OnLevelStart(7);
        SaveGameManager.instance.LoadVariableData();

        refrences.DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        ScriptRegistry.Instance.textGameController.ShowVariableUIElements();

        ScriptRegistry.Instance.businessHandler.ShowBusinessEarnings();


        CutSceneClip clip = cutSceneManager.GetClip("1-Day-7_Start");
        cutSceneManager.PlayClipFromStart(clip.id);

        cutSceneManager.ContinueConversation("DAY07", 43);
        AssignRestartButtonEvents();
    }

    public void AssignRestartButtonEvents()
    {
        cutSceneManager.gameOverRestartButton.onClick.RemoveAllListeners();
        cutSceneManager.gameOverRestartButton.onClick.AddListener(() =>
        {
            ScriptRegistry.Instance.textGameController.ClearPrevousDayTextUIElements();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("dayCompleted").Hide();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("chatContent").Show();
            ScriptRegistry.Instance.gameOverScreen.Hide();

            cutSceneManager.SetCameraHalfScreen();
            ScriptRegistry.Instance.textGameController.ShowTextArea();
            StartDay();
        });

    }

    [Button("3.Hospital")]
    public void EnterHospital()
    {
        refrences.DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = cutSceneManager.GetClip("3_Reach-hospital");
        cutSceneManager.PlayClipFromStart(clip.id);

        cutSceneManager.ContinueConversation("DAY07", 61);

    }


    public void ShowGameOverScreen()
    {
        ScriptRegistry.Instance.gameOverScreen.Show("WASTED!", "No one steals from Razor");
        AnalyticsEvents.instance.OnLevelEndFail(7);
        AnalyticsEvents.instance.UniqueEvent("level_7_fail");
    }


    public void Restart()
    {
        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        AnalyticsEvents.instance.OnLevelEndFail(day);
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("gameOver").Hide();
        ScriptRegistry.Instance.textGameController.ClearPrevousDayTextUIElements();
        StartDay();
    }

    public void EndDay()
    {
        cutSceneManager.FadeIn(.5f);
        ScriptRegistry.Instance.dayCompleteHandler.GoToSleep();
    }
}

