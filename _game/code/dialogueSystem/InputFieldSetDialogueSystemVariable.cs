using UnityEngine;
using TMPro;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;
using JetBrains.Annotations;
using UnityEngine.Events;

public class InputFieldSetDialogueSystemVariable : MonoBehaviour
{
    [SerializeField] private string variableName;

    [Space]
    [SerializeField] private string defaultValue = "Jimmy";

    [Space]
    [SerializeField] private int requiredCharacterCount = 1;
    [Space]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button button;

    public UnityEvent onFocusLost;

    public CutSceneManager GetCutSceneManager;

    private void Start()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
            inputField.onEndEdit.AddListener(CheckFocus);
        }

        if (button != null)
        {
            button.onClick.AddListener(SetVariable);
        }
    }

    public void SetVariable()
    {
        if(inputField.text.Length > 0)
        {
            DialogueLua.SetVariable(variableName, inputField.text);
        }
        else
        {
            DialogueLua.SetVariable(variableName, defaultValue);
        }

        if (GetCutSceneManager!=null)
        {
            GetCutSceneManager.ShowNextDialogueEntry();
        }
    }

    bool isFocused = false;
    void OnInputFieldValueChanged(string value)
    {
        if (button != null)
        {
            button.interactable = value.Length >= requiredCharacterCount;
        }
    }


    void CheckFocus(string value)
    {
        isFocused = inputField.isFocused;

        if (!isFocused)
        {
            onFocusLost.Invoke();
        }
    }
}
