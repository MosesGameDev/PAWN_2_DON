using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class DayCompleteHandler : MonoBehaviour
{
    [Header("Day Complete UI")]
    [SerializeField] private TextMeshProUGUI dayCompleteText;

    public Image replayRVIcon;
    [Space]
    public Button replayLevelButton;
    public Button nextButton;
    public Button nextDayButton;

    [SerializeField] private UIDialogueElement dialogueElement;
    [SerializeField] public CharacterEnabler[] characterEnablers;

    [Space]
    [SerializeField] int startConversationId = 1;

    [Space]
    [SerializeField] private GameObject[] objectsToDisable;

    public static event System.Action OnDayComplete;


    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);

    }
    public void PlayGoToSleep()
    {
        foreach (CharacterEnabler character in characterEnablers)
        {
            character.EnableCharacter();

            if (character.gameObject.GetComponent<CustomizableCharacter>() != null)
            {
                character.gameObject.GetComponent<CustomizableCharacter>().EquipOutfitFromSave();
            }
        }

        CutSceneManager cutSceneManager = CutSceneManager.instance;
        cutSceneManager.SetCameraHalfScreen();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = cutSceneManager.GetClip("sleep");
        cutSceneManager.PlayClipFromStart(clip.id);

        DisableGameObjects();
    }

    public void GoToSleep()
    {
        CutSceneManager cutSceneManager = CutSceneManager.instance;
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = cutSceneManager.GetClip("sleep");
        cutSceneManager.PlayClipFromTime(clip.id, 5);

        DisableGameObjects();
    }

    public void PlayWakeUp()
    {
        foreach (CharacterEnabler character in characterEnablers)
        {
            character.EnableCharacter();

            if (character.gameObject.GetComponent<CustomizableCharacter>() != null)
            {
                character.gameObject.GetComponent<CustomizableCharacter>().EquipOutfitFromSave();
            }
        }

        CutSceneManager cutSceneManager = CutSceneManager.instance;
        cutSceneManager.SetCameraHalfScreen();

        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = cutSceneManager.GetClip("wakeUp");
        cutSceneManager.PlayClipFromStart(clip.id);

        DisableGameObjects();
    }

    public void DisableGameObjects()
    {
        foreach (GameObject g in objectsToDisable)
        {
            g.SetActive(false);
        }
    }

    public void CheckIfHomescreenActive()
    {
        if (homescreenActive)
        {
            CutSceneManager.instance.PauseCutscene();
        }
    }

    bool homescreenActive;
    [Button]
    public void ShowHomeScreenMenu()
    {
        foreach (CharacterEnabler character in characterEnablers)
        {
            character.EnableCharacter();

            if (character.gameObject.GetComponent<CustomizableCharacter>() != null)
            {
                character.gameObject.GetComponent<CustomizableCharacter>().EquipOutfitFromSave();
            }
        }

        transform.GetChild(0).gameObject.SetActive(true);
        homescreenActive = true;
        CutSceneManager.instance.PlayClipFromTime(CutSceneManager.instance.GetClip("sleep").id, 8.5f);
        DisableGameObjects();

    }


    [Button]
    public void ShowDayCompleteUI()
    {

        dayCompleteText.text = $"DAY {startConversationId} COMPLETED!!";
        dialogueElement.Show();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("chatContent").Hide();
        ScriptRegistry.Instance.textGameController.MinimizeTextArea();
        ScriptRegistry.Instance.textGameController.ClearPrevousDayTextUIElements();

        CutSceneManager.instance.PauseCutscene();

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener
        (
            delegate
            {
                CutSceneManager.instance.Unpause();
                ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("dayCompleted").Hide();
                SoundManager.Instance.PlayButtonSfx();
                if (startConversationId == (3+1))
                {
                    print($"<b><color=red>{startConversationId}</color></b>");

                    ScriptRegistry.Instance.characterCustomizationHandler.EnableCostumeButton();
                    AnalyticsEvents.instance.UniqueEvent("costumes_unlocked");
                }

                if(startConversationId == (4+1))
                {
                    ScriptRegistry.Instance.cutSceneManager.GetCutsceneHandler(4).gameObject.GetComponent<Day04CutSceneEventHandler>().ShowRatePopup();
                }

                if (startConversationId == (4 + 1))
                {
                    ScriptRegistry.Instance.cutSceneManager.GetCutsceneHandler(4).gameObject.GetComponent<Day04CutSceneEventHandler>().CheckDay();
                }

            });

        int day = startConversationId;

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }

        AnalyticsEvents.instance.UniqueEvent($"day_{day}_completed");
        AnalyticsEvents.instance.OnLevelEndSuccess(startConversationId);

        bool showRv = CutSceneManager.instance.GetCutsceneHandler(startConversationId).showRv;

        replayLevelButton.onClick.RemoveAllListeners();

        if (showRv)
        {
            replayRVIcon.gameObject.SetActive(true);
            replayLevelButton.onClick.AddListener(() =>
            {
                OnClickRv();
                SoundManager.Instance.PlayButtonSfx();
            });
        }
        else
        {
            replayRVIcon.gameObject.SetActive(false);
            replayLevelButton.onClick.AddListener(() =>
            {
                RestartLevel();
            });
        }


        OnDayComplete?.Invoke();
    }

    [Button]

    public void ShowHomeScreen()
    {
        //print("<b><color=red>SHOWING HOWE SCREEN</b>");

        int day = DialogueLua.GetVariable("DAY").asInt;
        ScriptRegistry.Instance.homeScreenManager.homeScreenOpen = true;
        ScriptRegistry.Instance.homeScreenManager.homeScreenButtonParent.SetActive(true);


        PlayerPrefs.SetInt("HOME_SCREEN_OPEN", 1);

        if (day >= 2)
        {
            ScriptRegistry.Instance.homeScreenManager.GetButton("Food").EnableButton();

            if (day == 3 || day == 4)
            {
                ScriptRegistry.Instance.homeScreenManager.GetButton("Food").button.interactable = false;
            }

        }

        if (day >= 5)
        {
            ScriptRegistry.Instance.homeScreenManager.GetButton("Business").EnableButton();
            //ScriptRegistry.Instance.homeScreenManager.EnableButtonsFTUE();
        }

        startConversationId++;
        PlayerPrefs.SetInt("START_CONVERSATION_ID", startConversationId);

        SaveGameManager.instance.SaveVariables();
        nextDayButton.gameObject.SetActive(true);
        nextDayButton.onClick.RemoveAllListeners();
        nextDayButton.onClick.AddListener(OnNextDayButtonClicked);

        if(startConversationId < 11)
        {

            int _day = startConversationId;

            if (PlayerPrefs.HasKey("COMPLETED"))
            {
                int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
                int d = resetCount * 10;
                day += d;
            }

            //AnalyticsEvents.instance.UniqueEvent($"show_homeScreen_day_{_day}");
        }

        if (startConversationId >= 6)
        {
            ScriptRegistry.Instance.homeScreenManager.EnableButtonsFTUE();
            print("BUTTONS ENABLED");
        }

        System.GC.Collect();

    }

    public void SetConversationId(int conversationId)
    {
        startConversationId = conversationId;
    }

    void OnNextDayButtonClicked()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        CutSceneManager.instance.FadeIn();

        DialogueManager.StopAllConversations();

        if (startConversationId > 10)
        {
            ScriptRegistry.Instance.initializationManager.ResetGame();
            return;
        }

            CutSceneManager.instance.PlayScreenFade();

        string conv = ($"DAY0{startConversationId}");
        print($"<b><color=red>Starting Conversation: {conv}</color></b>");
        DialogueManager.instance.StartConversation(conv);
        ScriptRegistry.Instance.textGameController.ToggleTextArea(false, out Tween t);

        dialogueElement.Hide();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("chatContent").Show();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("hungerAlert").Hide();

        nextDayButton.gameObject.SetActive(false);
        SoundManager.Instance.PlayButtonSfx();

    }


    //This is for Day03, if VAR DAY == 3 , then we need to start WalkingSim
    public void CheckStartWalkingSimulator()
    {
        int day = DialogueLua.GetVariable("DAY").asInt;

        if (day == 3)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            CutSceneManager.instance.GetCutsceneHandler(3).gameObject.GetComponent<Day03CutSceneEventHandler>().StartWalkingSim();
        }
    }


    public void RestartLevel()
    {
        CutSceneManager.instance.FadeIn();
        ScriptRegistry.Instance.textGameController.ClearPrevousDayTextUIElements();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("dayCompleted").Hide();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("chatContent").Show();
        CutSceneManager.instance.SetCameraHalfScreen();
        ScriptRegistry.Instance.textGameController.ShowTextArea();

        CutSceneManager.instance.GetCutsceneHandler(startConversationId).StartDay();
        //StartDay();
    }

    public void OnClickRv()
    {
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("rvBlocker").Show();
        AdsManager.instance.ShowRewardAd(OnRVComplete);
    }

    void OnRVComplete()
    {
        RestartLevel();
    }

    public void ResetGame()
    {
        SaveGameManager.instance.ClearSavedConversationData();
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }
}
