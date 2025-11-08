using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using static CharacterSelectionHandler;
using UnityEngine.EventSystems;
using System.Collections;
using PixelCrushers.DialogueSystem;



public class IntroCutSceneEventHandler : CutSceneEventHandler
{
    [SerializeField] private TextMeshProUGUI babyNameInputField;
    [SerializeField] private CutSceneSceneRefrences cutSceneSceneRefrences;
    [SerializeField] private CutSceneManager cutSceneManager;

    [Space]


    bool useCab;

    private void OnEnable()
    {
        DeliveryCompleteTrigger.onLevelComplete += DeliveryCompleteTrigger_onLevelComplete;
    }

    private void OnDisable()
    {
        DeliveryCompleteTrigger.onLevelComplete -= DeliveryCompleteTrigger_onLevelComplete;
    }

    private void DeliveryCompleteTrigger_onLevelComplete(int conversationId, int dialogueEntryId)
    {
        //PlayReturnHome();
    }

    public void SetTravelOption(bool v)
    {
        useCab = v;
    }

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    [Button("Play Start")]
    public override void StartDay()
    {
        base.StartDay();
        PlayHouseIntro();
    }


    public void FocusOnBabyNameInputField()
    {
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("nameInput_baby").Show();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("Keyboard_02").Show();
    }


    public void PlayHouseIntro()
    {
        AnalyticsEvents.instance.OnLevelStart(1);
        cutSceneSceneRefrences.DisableGameObjects();
        SaveGameManager.instance.SaveVariables();
        EnableSelectedCharacter();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);

        CutSceneClip clip = cutSceneManager.GetClip("introStart");
        cutSceneManager.PlayClipFromStart(clip.id);
        cutSceneManager.ContinueConversation("DAY01", 50);

        //SoundManager.Instance.PlayMusic(SoundManager.Instance.sadMusic);

    }


    [Button("Play Name The Baby")]
    public void PlayNameTheBaby()
    {
        CutSceneClip clip = cutSceneManager.GetClip("wifeCome");
        cutSceneManager.PlayClipFromStart(clip.id);

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);

        cutSceneManager.ContinueConversation("DAY01", 5);
    }

    [Button("Play Delivery")]
    public void LoadDeliveryGame()
    {
        ScriptRegistry.Instance.deliveryMinigameLoader.LoadScene();
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
    }


    public void PlayClip(string id)
    {

        CutSceneClip clip = cutSceneManager.GetClip(id);
        cutSceneManager.PlayClipFromStart(clip.id);
    }


    [Button("Play Hospital")]
    public void StartHospitalIntro()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(false);

        CutSceneClip clip = cutSceneManager.GetClip("hospital");
        cutSceneManager.PlayClipFromStart(clip.id);

        //print(clip.playableDirector.name);


        cutSceneManager.ContinueConversation("DAY01", 24);
    }



    [Button("Play Missing Wife")]
    public void PlayReturnHome()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);

        ScriptRegistry.Instance.screenFade.FadeOutFast();
        ScriptRegistry.Instance.textGameController.ToggleTextArea(false);
        Camera.main.rect = new Rect(0, .5f, 1, 1);

        CutSceneClip clip = cutSceneManager.GetClip("missingWife");
        cutSceneManager.PlayClipFromStart(clip.id);


        cutSceneManager.ContinueConversation("DAY01", 16);

        //print("Playing missing wife clip: " + clip.playableDirector.name);
    }

    [Button("Play Return Home From Hospital")]
    public void PlayBackHomeHospital()
    {
        CutSceneClip clip = cutSceneManager.GetClip("returnHomeFromHospital");
        cutSceneManager.PlayClipFromStart(clip.id);

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);

        cutSceneManager.ContinueConversation("DAY01", 32);
    }


    public void ChooseTransportOption()
    {
        if (useCab)
        {
            PlayCabTimeline();
            return;
        }

        PlayCycleTimeline();
    }

    public void PlayCabTimeline()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);

        cutSceneManager.FadeIn(.6f);

        CutSceneClip clip = cutSceneManager.GetClip("car");
        cutSceneManager.PlayClipFromStart(clip.id);
    }
    public void PlayAskNeighbourTimeline()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);

        cutSceneManager.FadeIn(.6f);

        CutSceneClip clip = cutSceneManager.GetClip("askNeighbour");
        cutSceneManager.PlayClipFromStart(clip.id);
    }
    public void PlayCycleTimeline()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);

        cutSceneManager.FadeIn(.6f);

        CutSceneClip clip = cutSceneManager.GetClip("cycle");
        cutSceneManager.PlayClipFromStart(clip.id);
    }


    public void ShowNameInput()
    {
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("nameInput_baby").Show();
    }

    public void SetBabyName()
    {
        PlayerPrefs.SetString("BABY_NAME", babyNameInputField.text);
        if (PlayerPrefs.GetString("BABY_NAME") == "Enter" || PlayerPrefs.GetString("BABY_NAME") == "")
        {
            PlayerPrefs.SetString("BABY_NAME", "Timmy");

        }
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("nameInput_baby").Hide();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("Keyboard_02").Hide();
        cutSceneManager.ShowNextDialogueEntry();
        AnalyticsEvents.instance.UniqueEvent("name_the_baby");
    }


    public void PauseForInput()
    {
        int day = DialogueLua.GetVariable("DAY").asInt;

        if (day != 1)
        {
            return;
        }

        cutSceneManager.PauseCutscene();
        cutSceneManager.HideButtonElipsis();

        SoundManager.Instance.PlayMusic(SoundManager.Instance.defaultMusic);

    }

}