using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using System.Resources;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;

[System.Serializable]
public class TextDialogueBackground
{
    public string name;
    public GameObject background;
    public Transform textHolder;
}


public class TextDialogueUIElement : MonoBehaviour
{
    public Sprite whiteMan, blackMan, whiteMan_phone, BlackMan_phone;
    public Image imageGraphic;
   
   // public Image playerImage;

    public void SetPlayerIcon(Sprite image)
    {
        if (image!=null && actorDialogue.portraitImg != null)
        {
            actorDialogue.portraitImg.sprite = image;
          //  Debug.Log("set player text icon to " + image.name);
        }
    }
    void UpdatePlayerIcon()
    {
        if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "AFRICAN")
        {
            SetPlayerIcon(blackMan);
        }
        else if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "CAUCASIAN")
        {
            SetPlayerIcon(whiteMan);
        }
    }

    public void SetPlayerPhoneIcon()
    {
        if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "AFRICAN")
        {
            SetPlayerIcon(BlackMan_phone);
        }
        else if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "CAUCASIAN")
        {
            SetPlayerIcon(whiteMan_phone);
        }

    }


    private void OnEnable()
    {
        UpdatePlayerIcon();
    }

    [System.Serializable]
    public class ActorDialogueUIElement
    {
        [Header("Dialogue element GameObject")]
        public TextMeshProUGUI text;
        public Image backgroundImage;

        [Header("Portrait element")]
        public TextMeshProUGUI actorNameText;
        public RectTransform portraitRect;
        public Image portraitImg;

        [Header("Dialogue element GameObject")]
        public GameObject dialogueGameObject;

        public void SetBackgroundColor(Color color)
        {
            backgroundImage.color = color;
        }
    }

    public bool isActor;

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogues")]
    [SerializeField] private GameObject defaultDialogue;
    [SerializeField] private ActorDialogueUIElement actorDialogue;
    [SerializeField] private ActorDialogueUIElement npcActorDialogue;

    [Header("Backgrounds")]
    [SerializeField] private TextDialogueBackground[] backgrounds;

    [Space]
    public SlotEffect slotSelector;

    [Space]
    [SerializeField] private Transform textDialogueTransform;
    public RectTransform dateRectTransform;

    [Space]
    [SerializeField] private GameObject parametersContentHolder;
    [SerializeField] private TextDialogueParameterUIElement[] parameterUIElements;

    int day, hp, crew, cash, territory = 0;
    string bg;

    Subtitle subtitle;

    public void SetText(string textVal)
    {
        if (textVal.Contains("<PLAYER>") || textVal.Contains("<BABY>"))
        {
            if (textVal.Contains("<PLAYER>"))
            {
                string playerName = PlayerPrefs.GetString("NAME_CHARACTER");
                string _textVal = textVal.Replace("<PLAYER>", $"<color=yellow>{playerName}</color>");
                dialogueText.text = _textVal;
            }

            if (textVal.Contains("<BABY>"))
            {
                string babyName = DialogueLua.GetVariable("NAME_BABY").asString;
                string _textVal = textVal.Replace("<BABY>", $"<color=#330D2F>{babyName}</color>");
                dialogueText.SetText(_textVal);
            }
        }
        else
        {
            dialogueText.SetText(textVal);
        }
    }

    public void SetIcon()
    {

    }

    public void SetActorText(string textVal, ActorDialogueUIElement actorDialogueUIElement)
    {
        if (textVal.Contains("<PLAYER>") || textVal.Contains("<BABY>"))
        {
            if (textVal.Contains("<PLAYER>"))
            {
                string playerName = PlayerPrefs.GetString("NAME_CHARACTER");
                string _textVal = textVal.Replace("<PLAYER>", $"<color=blue>{playerName}</color>");
                actorDialogueUIElement.text.SetText(_textVal);
            }

            if (textVal.Contains("<BABY>"))
            {
                string babyName = PlayerPrefs.GetString("BABY_NAME");
                string _textVal = textVal.Replace("<BABY>", $"<color=#710082>{babyName}</color>");
                actorDialogueUIElement.text.SetText(_textVal);
            }
        }
        else
        {
            actorDialogueUIElement.text.SetText(textVal);
        }
    }

    public void SetDay(string textVal)
    {
        dayText.SetText(textVal);
        dateRectTransform.gameObject.SetActive(true);
    }

    public void AnimatePopup()
    {
        textDialogueTransform.localScale = Vector3.zero;
        textDialogueTransform.DOScale(Vector3.one * 0.9f, 0.3f).SetEase(Ease.OutBounce).SetDelay(0.15f);
    }

    // New method to handle updating from field list directly (for saved history)
    public void UpdateFields(List<Field> fields, string backgroundType = null)
    {
        if (fields == null) return;

        // Process the various field types
        foreach (Field field in fields)
        {
            if (field.title == "DAY" && !string.IsNullOrEmpty(field.value))
            {
                int dayValue;
                if (int.TryParse(field.value, out dayValue) && dayValue > 0)
                {
                    SetDay("DAY " + dayValue);
                }
            }
            else if (field.title == "HP" && !string.IsNullOrEmpty(field.value))
            {
                int hpValue;
                if (int.TryParse(field.value, out hpValue) && hpValue != 0)
                {
                    EnableParameterUIElement("HP", hpValue);
                }
            }
            else if (field.title == "POWER" && !string.IsNullOrEmpty(field.value))
            {
                int powerValue;
                if (int.TryParse(field.value, out powerValue) && powerValue != 0)
                {
                    EnableParameterUIElement("POWER", powerValue);
                }
            }
            else if (field.title == "CASH" && !string.IsNullOrEmpty(field.value))
            {
                int cashValue;
                if (int.TryParse(field.value, out cashValue) && cashValue != 0)
                {
                    EnableParameterUIElement("CASH", cashValue);
                }
            }
            else if (field.title == "REPUTATION" && !string.IsNullOrEmpty(field.value))
            {
                int repValue;
                if (int.TryParse(field.value, out repValue) && repValue != 0)
                {
                    EnableParameterUIElement("REPUTATION", repValue);
                }
            }
        }

        // Handle background if provided
        if (!string.IsNullOrEmpty(backgroundType))
        {
            SetBackground(backgroundType);
        }
    }

    // Original method for live subtitles
    public void UpdateFields(Subtitle _subtitle)
    {

        subtitle = _subtitle;

        if (Field.FieldExists(_subtitle.dialogueEntry.fields, "DAY"))
        {
            day = Field.LookupInt(_subtitle.dialogueEntry.fields, "DAY");

            if (day != 0)
            {
                SetDay("DAY " + day);
                dateRectTransform.gameObject.SetActive(true);
            }
        }

        if (Field.FieldExists(_subtitle.dialogueEntry.fields, "HP"))
        {
            hp = Field.LookupInt(_subtitle.dialogueEntry.fields, "HP");
            if (hp != 0)
            {
                EnableParameterUIElement("HP", hp, _subtitle);
            }
        }

        if (Field.FieldExists(_subtitle.dialogueEntry.fields, "POWER"))
        {
            crew = Field.LookupInt(_subtitle.dialogueEntry.fields, "POWER");

            if (crew != 0)
            {
                EnableParameterUIElement("POWER", crew, _subtitle);
            }
        }

        if (Field.FieldExists(_subtitle.dialogueEntry.fields, "CASH"))
        {
            cash = Field.LookupInt(_subtitle.dialogueEntry.fields, "CASH");

            if (cash != 0)
            {
                EnableParameterUIElement("CASH", cash, _subtitle);
            }
        }

        if (Field.FieldExists(_subtitle.dialogueEntry.fields, "REPUTATION"))
        {
            territory = Field.LookupInt(_subtitle.dialogueEntry.fields, "REPUTATION");

            if (territory != 0)
            {
                EnableParameterUIElement("REPUTATION", territory, _subtitle);
            }
        }

        if (Field.FieldExists(_subtitle.dialogueEntry.fields, "ENTRY-BG"))
        {
            bg = Field.LookupValue(_subtitle.dialogueEntry.fields, "ENTRY-BG");

            if (bg != string.Empty)
            {
                if (bg == "actor")
                {

                    isActor = true;
                    Actor a = DialogueManager.MasterDatabase.GetActor(_subtitle.dialogueEntry.ActorID);
                    if (a.Name == "You")
                    {
                        DisplayActorDialogue(_subtitle, a, actorDialogue);
                    }
                    else
                    {
                        DisplayActorDialogue(_subtitle, a, npcActorDialogue);
                    }
                    //string actorName = a.LookupValue("Display Name");
                    //actorDialogue.gameObject.SetActive(true);
                    //SetActorText(_subtitle.dialogueEntry.subtitleText);

                    //actorDialogue.actorNameText.SetText(actorName);
                    //defaultDialogue.gameObject.SetActive(false);

                    //if (a.Name == "You")
                    //{
                    //    if(Field.FieldExists(_subtitle.dialogueEntry.fields, "phone"))
                    //    {
                    //        SetPlayerPhoneIcon();
                    //    }
                    //    else
                    //    {
                    //        UpdatePlayerIcon();
                    //    }
                    //}
                    //else
                    //{
                    //    actorDialogue.portraitImg.sprite = TextureToSprite(DialogueManager.MasterDatabase.GetActor(_subtitle.dialogueEntry.ActorID).portrait);
                    //}


                    //actorDialogue.portraitRect.localScale = Vector3.one;
                    //actorDialogue.backgroundImage.transform.localScale = Vector3.one;


                    //MoveVariablesToActorDialogue();
                    return;
                }

                SetBackground(bg, _subtitle);
            }
        }
    }

    public void DisplayActorDialogue(Subtitle _sub, Actor actor, ActorDialogueUIElement actorDialogueUIElement)
    {
        if (actorDialogueUIElement.dialogueGameObject == null)
        {
            return;
        }
        actorDialogueUIElement.dialogueGameObject.transform.parent.gameObject.SetActive(true);

        actorDialogueUIElement.dialogueGameObject.SetActive(true);

        defaultDialogue.gameObject.SetActive(false);
        Actor a = DialogueManager.MasterDatabase.GetActor(_sub.dialogueEntry.ActorID);

        string actorName = a.LookupValue("Display Name");
        SetActorText(_sub.dialogueEntry.subtitleText, actorDialogueUIElement);

        actorDialogueUIElement.actorNameText.SetText(actorName);

        actorDialogueUIElement.portraitRect.localScale = Vector3.one;
        actorDialogueUIElement.backgroundImage.transform.localScale = Vector3.one;

        if (a.Name == "You")
        {
            if (Field.FieldExists(_sub.dialogueEntry.fields, "phone"))
            {
                SetPlayerPhoneIcon();
            }
            else
            {
                UpdatePlayerIcon();
            }
            return;
        }

        actorDialogueUIElement.portraitImg.sprite = TextureToSprite(DialogueManager.MasterDatabase.GetActor(_sub.dialogueEntry.ActorID).portrait);

        //MoveVariablesToActorDialogue();
    }


    public static Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f), // Pivot in the center
            100f // Pixels per unit (adjust as needed)
        );
    }


    // New method to set up actor dialogue for historical entries
    public void SetupActor(Actor actor, string backgroundType)
    {
        if (actor == null) return;

        isActor = true;

        string actorName = actor.LookupValue("Display Name");


        if (string.IsNullOrEmpty(actorName))
        {
            actorName = actor.Name;
        }

        actorDialogue.dialogueGameObject.SetActive(true);
        actorDialogue.text.SetText(dialogueText.text);
        actorDialogue.actorNameText.SetText(actorName);
        defaultDialogue.gameObject.SetActive(false);
    }

    // Simplified version for historical entries
    public void EnableParameterUIElement(string id, int value)
    {
        for (int i = 0; i < parameterUIElements.Length; i++)
        {
            if (parameterUIElements[i].id == id)
            {
                parametersContentHolder.SetActive(true);
                parameterUIElements[i].gameObject.SetActive(true);
                parameterUIElements[i].value = value;

                if (value > 0)
                {
                    parameterUIElements[i].UpdateElement(value);
                }
                else
                {
                    parameterUIElements[i].UpdateElement(value);

                    // Only update game variables if TextGameUIController exists
                    if (ScriptRegistry.Instance.textGameController != null)
                    {
                        VariableUIElement variableElement = ScriptRegistry.Instance.textGameController.GetVariableUIElement(id);
                        if (variableElement != null)
                        {
                            variableElement.UpdateUIElement(value);
                            variableElement.ShakeFx();
                        }
                    }
                }
            }
        }
    }

    public void EnableParameterUIElement(string id, int value, Subtitle subtitle)
    {

        for (int i = 0; i < parameterUIElements.Length; i++)
        {
            if (parameterUIElements[i].id == id)
            {
                parametersContentHolder.SetActive(true);

                parameterUIElements[i].gameObject.SetActive(true);
                parameterUIElements[i].value = value;

                if (value > 0)
                {
                    parameterUIElements[i].SetParticleAttractor();
                    parameterUIElements[i].playParticles = true;
                    parameterUIElements[i].PlayParticle();
                }
                else
                {
                    parameterUIElements[i].UpdateElement(value);
                    ScriptRegistry.Instance.textGameController.GetVariableUIElement(id).UpdateUIElement(value);
                    ScriptRegistry.Instance.textGameController.GetVariableUIElement(id).ShakeFx();
                }
            }
        }
    }

    TextDialogueBackground GetBackground(string backgroundID)
    {
        TextDialogueBackground textDialogueBackground = null;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i].name == backgroundID)
            {
                textDialogueBackground = backgrounds[i];
            }
        }

        return textDialogueBackground;
    }

    // Simplified version for historical entries
    public void SetBackground(string backgroundID)
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i].name == backgroundID)
            {
                if (backgroundID != "normal")
                {
                    dialogueText.color = Color.black;
                    dialogueText.transform.SetParent(backgrounds[i].textHolder);

                    if (parametersContentHolder != null)
                    {
                        parametersContentHolder.transform.SetParent(backgrounds[i].textHolder);
                        parametersContentHolder.transform.localScale = Vector3.one;
                    }

                    dialogueText.transform.localScale = Vector3.one;

                    if (GetComponent<VerticalLayoutGroup>())
                    {
                        GetComponent<VerticalLayoutGroup>().padding = new RectOffset(30, 30, 0, 215);
                    }
                }

                backgrounds[i].background.SetActive(true);
            }
            else
            {
                backgrounds[i].background.SetActive(false);
            }
        }
    }


    [Button]
    public void MoveVariablesToActorDialogue()
    {

        parametersContentHolder.transform.SetParent(actorDialogue.dialogueGameObject.transform.GetChild(0));
        HorizontalLayoutGroup layoutGroup = parametersContentHolder.GetComponent<HorizontalLayoutGroup>();

        layoutGroup.padding = new RectOffset(0, 0, 0, 0);


        parametersContentHolder.SetActive(true);
        parametersContentHolder.transform.localScale = Vector3.one;

    }

    // Original method for live subtitle updates
    void SetBackground(string backgroundID, Subtitle _subtitle)
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i].name == backgroundID)
            {
                if (backgroundID != "normal")
                {
                    dialogueText.color = Color.black;
                    dialogueText.transform.SetParent(backgrounds[i].textHolder);
                    parametersContentHolder.transform.SetParent(backgrounds[i].textHolder);
                    parametersContentHolder.transform.localScale = Vector3.one;
                    dialogueText.transform.localScale = Vector3.one;

                    if (GetComponent<VerticalLayoutGroup>())
                    {
                        GetComponent<VerticalLayoutGroup>().padding = new RectOffset(30, 30, 0, 215);
                    }
                }

                backgrounds[i].background.SetActive(true);
            }
            else
            {
                backgrounds[i].background.SetActive(false);
            }
        }
    }


}