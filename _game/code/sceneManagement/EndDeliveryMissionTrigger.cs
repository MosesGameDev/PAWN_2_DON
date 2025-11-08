using UnityEngine;
using System;
using UnityEngine.UI;

public class EndDeliveryMissionTrigger : MonoBehaviour
{
    public Button continueButton;
    public UIDialogueElement endScreen;
    public static event Action OnMinigameMissionComplete;



    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(EndLevel);
            endScreen.Show();
        }
    }


    public void EndLevel()
    {
        endScreen.Hide();
        SoundManager.Instance.PlayButtonSfx();
        ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").UpdateUIElement(120);
        OnMinigameMissionComplete?.Invoke();
    }
}
