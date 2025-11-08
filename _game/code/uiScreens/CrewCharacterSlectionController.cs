using UnityEngine;
using TMPro;

public class CrewCharacterSlectionController : MonoBehaviour
{
    public TextMeshProUGUI headerText;

    [Space]
    public UIDialogueElement crewCanvas;
    public UIDialogueElement nameCharacterScreen;
    public UIDialogueElement inputFieldUIE;

    [Space]
    public CharacterSelectionUIE selectedCharacterUIE;
    public TMP_InputField inputField;

    private void Start()
    {
    }

    public void SelectCharacter(CharacterSelectionUIE _characterSelectionUIE)
    {
        headerText.text = "GOOD CHOICE...!";
        selectedCharacterUIE.progileImage.sprite = _characterSelectionUIE.progileImage.sprite;

        selectedCharacterUIE.healthText.text = _characterSelectionUIE.healthText.text;
        selectedCharacterUIE.cashText.text = _characterSelectionUIE.cashText.text;
        selectedCharacterUIE.powerText.text = _characterSelectionUIE.powerText.text;
        selectedCharacterUIE.repText.text = _characterSelectionUIE.repText.text;

        OnCharacterSelected();
    }

    void OnCharacterSelected()
    {
        nameCharacterScreen.Show();
        inputFieldUIE.Show();
        inputField.Select();
        Invoke("Close", 1.5f);
    }

    void Close()
    {
        crewCanvas.Hide();
    }

}
