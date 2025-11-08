using TMPro;
using UnityEngine;

public class MoneyUIEBusinessPanle : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    private void OnEnable()
    {
        // Subscribe to the cash changed event
        VariableUIElement.onCashChanged += UpdateMoneyText;
    }

    private void OnDisable()
    {
        // Unsubscribe from the cash changed event
        VariableUIElement.onCashChanged -= UpdateMoneyText;
    }


    void UpdateMoneyText(int newValue)
    {
        if (newValue >= 999)
        {
            float displayValueK = newValue / 1000f;
            moneyText.SetText($"{displayValueK:F1}K");
        }
        else
        {
            moneyText.text = newValue.ToString("N0"); // Format as a number with commas
        }
    }
}
