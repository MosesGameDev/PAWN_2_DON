using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.Splines.Examples;
using Language.Lua;
using PixelCrushers.DialogueSystem.ChatMapper;

public class InitializationManager : MonoBehaviour
{
    [SerializeField] private CutSceneSceneRefrences cutSceneSceneRefrences;

    [SerializeField] private bool startFromDay;
    [SerializeField] private int startDay;

    [Header("Game Initialization")]
    [SerializeField] private bool resetPlayerPrefs = false;
    [SerializeField] private string startConversation = "DAY01";

    [Header("References")]
    [SerializeField] private UIDialogueElement initializationCanvas;
    [SerializeField] private IntroCutSceneEventHandler introCutSceneEventHandler;

    [Space]
    public bool skipToGameplay = false;

    // Keys for PlayerPrefs
    private const string FIRST_TIME_KEY = "IsFirstTimePlayer";

    private void Awake()
    {
        if (resetPlayerPrefs)
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs reset");
        }
    }

    private void Start()
    {
        initializationCanvas.Show();
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        yield return null; // Wait for one frame to ensure everything is initialized

        bool isFirstTimePlayer = !PlayerPrefs.HasKey(FIRST_TIME_KEY);

        if (!PlayerPrefs.HasKey("INITIALIZED"))
        {
            AnalyticsEvents.instance.UniqueEvent("game_start");
            Debug.Log("<color=green>Game started for the first time</color>");
        }

        if (isFirstTimePlayer || !ScriptRegistry.Instance.characterSelectionHandler.CheckCharacterSelected())
        {
            // First time player flow
            ScriptRegistry.Instance.characterSelectionHandler.ShowCharacterSelectionScreen();
            HideInitializationScreen();
            yield break;
        }
        else
        {
            HideInitializationScreen();
            ScriptRegistry.Instance.characterSelectionHandler.LoadSelection();

            if (PlayerPrefs.HasKey("START_CONVERSATION_ID"))
            {
                ShowHomeScreen();
            }
            else
            {
                if (skipToGameplay)
                {
                    if (PlayerPrefs.GetInt("HOME_SCREEN_OPEN") == 0)
                    {
                        StartIntro();
                    }
                    else if (PlayerPrefs.GetInt("HOME_SCREEN_OPEN") == 1)
                    {
                        ShowHomeScreen();
                    }
                }
            }
        }

    }

    int day;
    public void ShowHomeScreen()
    {
        int startConversationId = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (startConversationId > 10)
        {
            ResetGame();
        }

        day = startConversationId;
        ScriptRegistry.Instance.textGameController.ShowVariableUIElements();
        SaveGameManager.instance.LoadVariableData();
        //print($"<color=cyan>Starting conversation ID: {startConversationId}</color>");

        if (startConversationId < 11)
        {
            if (PlayerPrefs.HasKey("COMPLETED"))
            {
                int v = startConversationId;
                int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
                int d = resetCount * 10;
                v += d;
                print($"<b><color=red>Day: {day}</color></b>");
                AnalyticsEvents.instance.UniqueEvent($"show_homeScreen_day_{v}");
            }

        }

        SoundManager.Instance.PlayMusic(SoundManager.Instance.defaultMusic);

        ScriptRegistry.Instance.dayCompleteHandler.nextDayButton.onClick.RemoveAllListeners();
        ScriptRegistry.Instance.dayCompleteHandler.nextDayButton.onClick.AddListener(() =>
        {
            //startConversationId
            ScriptRegistry.Instance.cutSceneManager.cutSceneEventHandlers[startConversationId].StartDay();
            //CutSceneManager.instance.FadeIn();

            SaveGameManager.instance.LoadVariableData();
            ScriptRegistry.Instance.textGameController.ShowTextArea();
            ScriptRegistry.Instance.dayCompleteHandler.nextDayButton.gameObject.SetActive(false);
            //ScriptRegistry.Instance.businessHandler.ShowBusinessEarnings();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("hungerAlert").Hide();

        });
        ScriptRegistry.Instance.dayCompleteHandler.nextDayButton.gameObject.SetActive(true);
        ScriptRegistry.Instance.dayCompleteHandler.ShowHomeScreenMenu();

        ScriptRegistry.Instance.homeScreenManager.homeScreenOpen = true;
        ScriptRegistry.Instance.homeScreenManager.homeScreenButtonParent.SetActive(true);

        ScriptRegistry.Instance.businessHandler.CheckForUnlockedBusinesses();


        CheckForHungerInitialization(startConversationId);
        CheckForBusinessInitialization(startConversationId);
        CheckForCustomizationInitialization(startConversationId);
        CheckForDeliveryMinigameUnlock(startConversationId);
        StartCoroutine(SetCameraRectEnum());
    }

    void CheckForHungerInitialization(int conversationId)
    {
        if (conversationId >= 2)
        {
            if (PlayerPrefs.HasKey("HUNGER_INITIALIZED"))
            {
                ScriptRegistry.Instance.homeScreenManager.GetButton("Food").EnableButton();
                ScriptRegistry.Instance.hungerHandler.Initialize();
                ScriptRegistry.Instance.hungerHandler.CheckHungerEffects();
                ScriptRegistry.Instance.hungerHandler.hasPlayedFTUE = true;
            }
        }
    }

    void CheckForDeliveryMinigameUnlock(int conversationId)
    {
        if (conversationId > 4)
        {
            ScriptRegistry.Instance.homeScreenManager.GetButton("Delivery").EnableButton();

        }
    }

    void CheckForCustomizationInitialization(int conversationId)
    {
        if (conversationId > 3)
        {
            ScriptRegistry.Instance.characterCustomizationHandler.CheckForSave();
        }
    }

    void CheckForBusinessInitialization(int conversationId)
    {
        if (conversationId > 5)
        {
            ScriptRegistry.Instance.businessHandler.CheckSaveData();
        }
    }

    IEnumerator SetCameraRectEnum()
    {
        yield return new WaitForSeconds(1f);
        Camera.main.rect = new Rect(0, 0f, 1, 1);
        ScriptRegistry.Instance.textGameController.ShowMinimisedTextArea();
        //print("<b><color=yellow>Camera Rect Set</b>");
    }

    public void StartIntro()
    {
        // Mark the player as not a first-time player anymore
        PlayerPrefs.SetInt("START_CONVERSATION_ID", 1);
        PlayerPrefs.SetInt(FIRST_TIME_KEY, 1);
        PlayerPrefs.Save();

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("screenFade").Hide();
        ScriptRegistry.Instance.textGameController.ShowTextArea();

        CutSceneManager.instance.GetCutsceneHandler(1).StartDay();
    }

    public void OnCharacterSelectionComplete()
    {
        // Mark the player as not a first-time player anymore
        PlayerPrefs.SetInt(FIRST_TIME_KEY, 1);
        PlayerPrefs.Save();

        // Start the game
        StartIntro();
    }

    public void HideInitializationScreen()
    {
        StartCoroutine(HideInititializationUI());

    }

    IEnumerator HideInititializationUI()
    {
        yield return new WaitForSeconds(1f);
        initializationCanvas.Hide();
    }

    [Button]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [Button]
    public void ResetGame()
    {
        initializationCanvas.Show();
        StartCoroutine(ResetEnum());
    }

    IEnumerator ResetEnum()
    {
        yield return new WaitForSeconds(1f);
        SimpleSceneManager.Instance.SwapScenes("reset");
    }

    public int GetDay()
    {
        return DialogueLua.GetVariable("DAY").asInt;
    }
}