using UnityEngine;
using TMPro;

public class HungerStatusText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hungerStatusText;
    UIDialogueElement dialogueUIElement;
    //HungerStatus hungerStatus;

    private void OnEnable()
    {
        //HungerHandler.OnHungerStatusChanged += UpdateHungerStatusText;
    }

    private void OnDisable()
    {
        //HungerHandler.OnHungerStatusChanged -= UpdateHungerStatusText;

    }

    private void Start()
    {
        dialogueUIElement = GetComponent<UIDialogueElement>();
    }

    //void UpdateHungerStatusText(HungerStatus _hungerStatus)
    //{
    //    if (_hungerStatus != hungerStatus)
    //    {
    //        hungerStatus = _hungerStatus;


    //        if (_hungerStatus == HungerStatus.Hungry || _hungerStatus == HungerStatus.VeryHungry || _hungerStatus == HungerStatus.Starving)
    //        {
    //            hungerStatusText.text = "Hunger:" + _hungerStatus.ToString();
    //            dialogueUIElement.Show();
    //            Invoke("Hide", 1.5f);
    //        }
    //    }
    //}

    void Hide()
    {
        dialogueUIElement.Hide();
    }
}
