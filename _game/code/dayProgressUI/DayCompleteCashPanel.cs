using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;

public class DayCompleteCashPanel : MonoBehaviour
{
    [SerializeField] private int targetAmount;

    [SerializeField] private TextMeshProUGUI earnedAmount;
    [SerializeField] private TextMeshProUGUI TargetAmount;
    [SerializeField] private TextMeshProUGUI remainingAmount;


    public void SetText()
    {
        float remaining = targetAmount - ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").currentValue;

        earnedAmount.text = ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").currentValue.ToString("N0") + "$";
        TargetAmount.text = targetAmount.ToString("N0") + "$";

        if(remaining < 0)
        {
            remaining = 0;
        }
        remainingAmount.text = remaining.ToString("N0") + "$";
    }


}
