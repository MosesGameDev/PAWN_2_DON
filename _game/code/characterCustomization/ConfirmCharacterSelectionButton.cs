using UnityEngine;
using UnityEngine.UI;


[RequireComponent (typeof(Button))]
public class ConfirmCharacterSelectionButton : MonoBehaviour
{
    CharacterSelectionHandler.BodyType bodyType;  

    private void OnEnable()
    {
        CharacterSelectionHandler.OnBodyTypeSelected += CharacterSelectionHandler_OnBodyTypeSelected;
    }

    private void CharacterSelectionHandler_OnBodyTypeSelected(CharacterSelectionHandler.BodyType obj)
    {
        GetComponent<Button>().interactable = true;
        bodyType = obj;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(delegate {
            SoundManager.Instance.PlaySFX("click");
            FeelVibrationManager.Instance.PlayVibration();
            ScriptRegistry.Instance.characterSelectionHandler.ConfirmSelection(bodyType);});
    }


    private void OnDisable()
    {
        CharacterSelectionHandler.OnBodyTypeSelected -= CharacterSelectionHandler_OnBodyTypeSelected;
    }
}
