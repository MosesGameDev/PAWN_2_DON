using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PixelCrushers.DialogueSystem;
using MoreMountains.Tools;
using DG.Tweening;

public class DeliveryMinigameLoader : MonoBehaviour
{
    public bool isPlayingDeliveryMinigameReplay;
    [Space]
    public string sceneName = "Delivery MiniGame 1- final part 1";
    public Button moveButton; 
    public PlayerDeliveryController deliveryController;

    [Space]
    public Camera mainCamera;

    [Space]
    public GameObject[] objectsToToggle;

    string[] minigamelevels = new string[] { "DeliveryDay_Replay", "DeliveryDay_Replay_1", "DeliveryDay_Replay_2" };
    public int currentLevelIndex = 0;

    private void OnEnable()
    {
        ScriptRegistry.onControllerAssigned += ScriptRegistry_onControllerAssigned;
        SimpleSceneManager.OnSceneLoaded += Instance_OnSceneLoaded;
        SimpleSceneManager.OnSceneUnloaded += SimpleSceneManager_OnSceneUnloaded;
        DeliveryCompleteTrigger.onLevelComplete += DeliveryCompleteTrigger_onLevelComplete;
        EndDeliveryMissionTrigger.OnMinigameMissionComplete += EndDeliveryMissionTrigger_OnMinigameMissionComplete;
    }


    private void EndDeliveryMissionTrigger_OnMinigameMissionComplete()
    {
        ScriptRegistry.Instance.screenFade.FadeInFast(1);
        ScriptRegistry.Instance.deliveryMinigameLoader.UnloadScene();
        //ScriptRegistry.Instance.initializationManager.ShowHomeScreen();
    }

    public void UnlockDeliveryMinigames()
    {
        ScriptRegistry.Instance.homeScreenManager.DisableButtonsFTUE();
        ScriptRegistry.Instance.homeScreenManager.UnlockButton("Delivery");
        AnalyticsEvents.instance.UniqueEvent("deliveries_unlocked");
    }


    public void LoadDeliveryMinigameScene()
    {
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("deliveriesCanvas_popup").Hide();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("hungerAlert").Hide();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("textArea").Hide();
        ScriptRegistry.Instance.dayCompleteHandler.nextDayButton.gameObject.SetActive(false);
        ScriptRegistry.Instance.textGameController.HideTextArea();
        isPlayingDeliveryMinigameReplay = true;
        mainCamera.rect = new Rect(0, 0, 1, 1); // Reset camera rect to full screen

        LoadScene(minigamelevels[currentLevelIndex]);
        if(currentLevelIndex > 2)
        {
            currentLevelIndex = 0; // Reset to first level if exceeded
        }
        else
        {
            currentLevelIndex++;

            if (currentLevelIndex > 2)
            {
                currentLevelIndex = 0; // Reset to first level if exceeded
            }
        }

        
    }

    private void SimpleSceneManager_OnSceneUnloaded(string obj)
    {
        ToggleGameObjects(true);

        deliveryController.RemoveButtonEvents();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("initializationCanvas").Hide();
        OnSceneUnloaded();

        if (obj == "Day04DrivingGame")
        {
            CutSceneManager.instance.GetCutsceneHandler(4).gameObject.GetComponent<Day04CutSceneEventHandler>().OnCompleteDrivingMinigame();
        }

        if (obj == "DeliveryDay_Replay" || obj == "DeliveryDay_Replay_1" || obj == "DeliveryDay_Replay_2")
        {
            ScriptRegistry.Instance.deliveryMinigameLoader.isPlayingDeliveryMinigameReplay = false;
            ScriptRegistry.Instance.textGameController.ShowTextArea();
            ScriptRegistry.Instance.homeScreenManager.GetButton("Delivery").GetComponent<DeliveriesButton>().StartCountdown();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("advanceButton").Show();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("deliveryGameMoveButton").Hide();
            ScriptRegistry.Instance.textGameController.ShowTextArea();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("textArea").Show();
            ScriptRegistry.Instance.initializationManager.ShowHomeScreen();

            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("chatContent").Show();

            int _day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

            if (PlayerPrefs.HasKey("COMPLETED"))
            {
                int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
                int d = resetCount * 10;
                _day += d;
            }


            AnalyticsEvents.instance.UniqueEvent($"minigame_end:{sceneName.ToLower()}");
            return;
        }

        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }


        AnalyticsEvents.instance.UniqueEvent($"day:{day}:minigame_end:{sceneName.ToLower()}");


    }



    private int dialogueEntryId;
    private void DeliveryCompleteTrigger_onLevelComplete(int conversationId, int dialogueEntryId)
    {
        //print("Level complete"); 
        ScriptRegistry.Instance.screenFade.FadeIn();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("advanceButton").Show();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("deliveryGameMoveButton").Hide();
        ScriptRegistry.Instance.textGameController.ToggleTextArea(false);

        string conversationIdString = DialogueManager.instance.GetConversationTitle(conversationId);
        CutSceneManager.instance.ContinueConversation(conversationIdString, dialogueEntryId);

        ScriptRegistry.Instance.deliveryMinigameLoader.UnloadScene();
    }

    private void ScriptRegistry_onControllerAssigned(PlayerDeliveryController obj)
    {
        deliveryController.GetDeliveryAgent.feelVibrationManager = ScriptRegistry.Instance.feelVibrationManager;
        deliveryController.Initialize(moveButton);
    }

    private void Instance_OnSceneLoaded(string obj)
    {
        ToggleGameObjects(false);
        ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(false);
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("initializationCanvas").Hide();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("advanceButton").Hide();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("deliveryGameMoveButton").Show();
        ScriptRegistry.Instance.textGameController.HideTextArea();
        ScriptRegistry.Instance.textGameController.SetFullScreenCameraRect();
        ScriptRegistry.Instance.cutSceneManager.FadeIn();
        ScriptRegistry.Instance.cutSceneManager.Unpause();


        if (obj == "DeliveryDay_Replay" || obj == "DeliveryDay_Replay_1" || obj == "DeliveryDay_Replay_2")
        {
            int _day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

            if (PlayerPrefs.HasKey("COMPLETED"))
            {
                int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
                int d = resetCount * 10;
                _day += d;
            }

            AnalyticsEvents.instance.UniqueEvent($"minigame_start:{sceneName.ToLower()}");
            return;
        }


        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }

        if(obj != "reset")
        {
            AnalyticsEvents.instance.UniqueEvent($"day:{day}:minigame_start:{sceneName.ToLower()}");
        }



        StartCoroutine(SetUpAdvanceButton());
    }


    public void ToggleGameObjects(bool setActive)
    {
        for (int i = 0; i < objectsToToggle.Length; i++)
        {
            if (objectsToToggle[i] != null)
            {
                objectsToToggle[i].SetActive(setActive);
            }
        }
    }

    IEnumerator SetUpAdvanceButton()
    {
        yield return new WaitForSeconds(1f);
        deliveryController.advanceButton = ScriptRegistry.Instance.textGameController.advanceButton;

    }


    public void OnSceneUnloaded()
    {
        //ScriptRegistry.Instance.textGameController.advanceButton.ToggleButton(false);
    }

    private void OnDisable()
    {
        SimpleSceneManager.OnSceneLoaded -= Instance_OnSceneLoaded;
        ScriptRegistry.onControllerAssigned -= ScriptRegistry_onControllerAssigned;
        DeliveryCompleteTrigger.onLevelComplete -= DeliveryCompleteTrigger_onLevelComplete;
        SimpleSceneManager.OnSceneUnloaded -= SimpleSceneManager_OnSceneUnloaded;
        EndDeliveryMissionTrigger.OnMinigameMissionComplete -= EndDeliveryMissionTrigger_OnMinigameMissionComplete;

    }

    [Button]
    public void LoadScene()
    {
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("initializationCanvas").Show();
        StartCoroutine(LoadSceneEnum());
    }

    public void LoadScene(string scene)
    {
        sceneName = scene;
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("loadingBar").Show();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("initializationCanvas").Show();
        StartCoroutine(LoadSceneEnum());
    }

    public void UnLoadScene()
    {
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("loadingBar").Show();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("initializationCanvas").Show();
        StartCoroutine(UnloadSceneEnum());
    }

    IEnumerator UnloadSceneEnum()
    {
        yield return new WaitForSeconds(.6f);
        print("UNLOAD SCENE: " + sceneName);
        SimpleSceneManager.Instance.UnloadScene(sceneName);

    }


    IEnumerator LoadSceneEnum()
    {
        yield return new WaitForSeconds(.3f);
        SimpleSceneManager.Instance.LoadSceneAdditive(sceneName, true);


    }

    public void UnloadScene()
    {
        SimpleSceneManager.Instance.UnloadScene(sceneName);
    }
}
