using UnityEngine;
using UnityEngine.UI;

public class DeliveryDropPoint : MonoBehaviour
{
    public int targetItems;
    public int currentItems;
    public bool processed;

    public Text targetItemText; 
    public GameObject targetItemParent; 
    public GameObject winCondition;
    public GameObject loseCondition; 
    public GameObject completeOrderFX;

    public GameObject dropPointIndicator;

    private void Start()
    {
        if (targetItemText != null)
        {
            targetItemText.text = targetItems.ToString();
        }
    }

    public void AcceptItems(int amount)
    {
        if (processed) return;

        currentItems += amount;
        processed = true;

        if (targetItemParent != null)
        {
            targetItemParent.SetActive(false);
            dropPointIndicator.SetActive(false);
        }

        if (currentItems >= targetItems)
        {
            if (winCondition != null) winCondition.SetActive(true);
            if (completeOrderFX != null) completeOrderFX.SetActive(true);
        }
        else
        {
            if (loseCondition != null) loseCondition.SetActive(true);
        }
    }
}