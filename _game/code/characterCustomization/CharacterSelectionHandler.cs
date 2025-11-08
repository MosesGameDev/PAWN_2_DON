using DG.Tweening;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CharacterSelectionHandler;

public class CharacterSelectionHandler : MonoBehaviour
{
    public enum BodyType { AFRICAN, CAUCASIAN };

    private BodyType bodyType;


    [System.Serializable]
    public class CharacterSelection
    {
        public TextMeshProUGUI nameInputFieldText;
        public TextMeshProUGUI characterNameText;
        public SwipeRotator swipeRotator;
        public GameObject selectionScreenEnv;
        [Space]
        public GameObject selectionScreenAfricanCharacter;
        public GameObject selectionScreenCaucasianCharacter;

        public static event Action<BodyType> OnBodyTypeSelected;

        Vector3 selectionScreenAfricanCharacterPos;
        Vector3 selectionScreenCaucasianCharacterPos;

        public void ShowSelectionCharacters()
        {
            selectionScreenEnv.SetActive(true);
            selectionScreenAfricanCharacter.transform.parent.gameObject.SetActive(true);

            selectionScreenAfricanCharacterPos = selectionScreenAfricanCharacter.transform.localPosition;
            selectionScreenCaucasianCharacterPos = selectionScreenCaucasianCharacter.transform.localPosition;
        }

        public void HideSelectionCharacters()
        {
            selectionScreenEnv.SetActive(false);
            selectionScreenAfricanCharacter.transform.parent.gameObject.SetActive(false);
        }


        public void Reset()
        {
            selectionScreenAfricanCharacter.GetComponent<Animator>().CrossFade("root|M_Idle standing", 1);
            selectionScreenCaucasianCharacter.GetComponent<Animator>().CrossFade("root|M_Idle standing", 1);

            selectionScreenAfricanCharacter.transform.localPosition = selectionScreenAfricanCharacterPos;
            selectionScreenCaucasianCharacter.transform.localPosition = selectionScreenCaucasianCharacterPos;
        }

    }

    private bool characterSelected;

    [Header("Character Selection")]
    [SerializeField] private CharacterSelection characterSelection;

    [Space]
    public Sprite africanCharacterImage;
    public Sprite caucasianCharacterImage;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera customizationCamera;

    public static event System.Action<BodyType> OnBodyTypeSelected;
    public static event System.Action OnBodySelectionComplete;



    public CharacterSelection GetCharacterSelection => characterSelection;

    public void LoadSelection()
    {
        if(PlayerPrefs.GetString("SELECTED_CHARACTER") == "AFRICAN")
        {
            bodyType = BodyType.AFRICAN;
        }
        else if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "CAUCASIAN")
        {
            bodyType = BodyType.CAUCASIAN;
        }

        SelectBodyType(bodyType);
    }

    private void Start()
    {
        LoadCharacterSprites();
    }

    public void LoadCharacterSprites()
    {
        if (PlayerPrefs.HasKey("BODY-TYPE"))
        {
            if(PlayerPrefs.GetString("SELECTED_CHARACTER") == "AFRICAN")
            {
                DialogueManager.instance.SetActorPortraitSprite("You", africanCharacterImage);
            }
            else if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "CAUCASIAN")
            {
                DialogueManager.instance.SetActorPortraitSprite("You", caucasianCharacterImage);
            }
        }
    }

    TouchScreenKeyboard keyboard;


    bool isActive;
    public void FocusOnInput()
    {
        keyboard = TouchScreenKeyboard.Open(characterSelection.nameInputFieldText.text);
        //StartCoroutine(TriggerKeyboard());
        isActive = true;
    }


    public void OnKeyboardDone()
    {
        if (name.Length > 0)
        {
            PlayerPrefs.SetString("PLAYER-NAME", name);
            characterSelection.characterNameText.text = characterSelection.nameInputFieldText.text;
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("customizationConfirmation").Show();

        }
    }




    public bool CheckCharacterSelected()
    {
        if (PlayerPrefs.HasKey("SELECTED_CHARACTER"))
        {
            characterSelected = true;
            return true;
        }
        else
        {
            characterSelected = false;
            return false;
        }
    }

    public void ShowCharacterSelectionScreen()
    {
        //ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("swipeFTUE").Show();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("characterSelection").Show();
        characterSelection.ShowSelectionCharacters();
        customizationCamera.Prioritize();
    }

    public void SelectBodyType(BodyType _bodyType)
    {
        customizationCamera.Prioritize();

        switch (_bodyType)
        {
            case BodyType.AFRICAN:
                PlayerPrefs.SetString("SELECTED_CHARACTER", "AFRICAN");
                break;

            case BodyType.CAUCASIAN:
                PlayerPrefs.SetString("SELECTED_CHARACTER", "CAUCASIAN");

                break;
        }

        LoadCharacterSprites();
        OnBodyTypeSelected?.Invoke(_bodyType);
    }

    public void ConfirmSelection(BodyType _bodyType)
    {


        SelectBodyType(_bodyType);
        SetCameraTarget(_bodyType);
        OnCharacterSelectionConfirmed(_bodyType);



        //ScriptRegistry.Instance.textGameController.MinimizeTextArea();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("characterSelection").Hide();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("swipeFTUE").Hide();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("customizationConfirmation").Show();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("nameInput").Hide();
        characterSelection.swipeRotator.inputEnabled = false;
        //ScriptRegistry.Instance.textGameController.ShowMinimisedTextArea();

        bodyType = _bodyType;
    }

    public void ConfirmCharacterName()
    {
        //ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("customizationConfirmation").Show();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("nameInput").Hide();
    }

    public void ReturnToCharacterSelection()
    {
        characterSelection.swipeRotator.inputEnabled = true;

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("characterSelection").Show();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("customizationConfirmation").Hide();
    }

    public void OnCharacterSelectionConfirmed(BodyType bodyType)
    {
        switch (bodyType)
        {
            case BodyType.AFRICAN:
                //characterSelection.AnimateCucasianWalkAway();
                break;
            case BodyType.CAUCASIAN:
                //characterSelection.AnimateAfricanWalkAway();
                break;
        }
    }

    public void SetCameraTarget(BodyType bodyType)
    {
        if (bodyType == BodyType.CAUCASIAN)
        {
            customizationCamera.Follow = characterSelection.selectionScreenCaucasianCharacter.transform;
        }
        else
        {
            customizationCamera.Follow = characterSelection.selectionScreenAfricanCharacter.transform;
        }
    }

    [Button]
    void CheckCharacterName()
    {
        print(PlayerPrefs.GetString("NAME_CHARACTER"));
    }

    public void CompleteCharacterSelection()
    {
        if (characterSelection.nameInputFieldText.text == string.Empty || characterSelection.nameInputFieldText.text == "Enter Name")
        {
            characterSelection.nameInputFieldText.text = "Jack";
            PlayerPrefs.SetString("NAME_CHARACTER", "Jack");
            //print($"<color=cyan> NAME {characterSelection.nameInputFieldText.text}/color>");
        }


        PlayerPrefs.SetString("NAME_CHARACTER", characterSelection.nameInputFieldText.text);


        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("textArea").Show();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("nameInput").Hide();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("screenFade").Show();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("customizationConfirmation").Hide();

        characterSelection.HideSelectionCharacters();

        ScriptRegistry.Instance.initializationManager.OnCharacterSelectionComplete();

        string log = $"SELECTED_CHARACTER_{bodyType}";
        AnalyticsEvents.instance.UniqueEvent(log.ToLower());

    }
}