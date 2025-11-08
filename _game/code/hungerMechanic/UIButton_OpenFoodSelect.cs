using UnityEngine;
using UnityEngine.UI;

public class UIButton_OpenFoodSelect : MonoBehaviour
{
    [SerializeField] private Image alertIndicator;


    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OpenFoodSelect);
    }



    void OpenFoodSelect()
    {
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("foodSelect").Show();
    }

    //void UpdateHungerStatusText(HungerStatus _hungerStatus)
    //{
    //    if (_hungerStatus == HungerStatus.Hungry || _hungerStatus == HungerStatus.VeryHungry || _hungerStatus == HungerStatus.Starving)
    //    {
    //        if (alertIndicator.enabled == false)
    //        {
    //            alertIndicator.enabled = true;
    //        }
    //    }
    //    else
    //    {
    //        if (alertIndicator.enabled == true)
    //        {
    //            alertIndicator.enabled = false;
    //        }
    //    }
    //}
}
