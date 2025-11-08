using UnityEngine;
using TMPro;

public class TestingInputFieldFocus : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI debugText;
    TouchScreenKeyboard keyboard;

    private void Start()
    {
        inputField.onSelect.AddListener(OnFocus);
        inputField.onEndEdit.AddListener(EndEdit);
    }

    public void Focus()
    {
        inputField.Select();
        inputField.ActivateInputField();
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false, "Enter text...");
    }

    void EndEdit(string t)
    {
        debugText.text = "DONE";
    }

    void OnFocus(string t)
    {
        debugText.text = "FOCUSED";
    }
}
