using UnityEngine;
using Sirenix.OdinInspector;

public class HomeScreenManager : MonoBehaviour
{
    public bool homeScreenOpen;
    public GameObject homeScreenButtonParent;
    [SerializeField] private HomeScreenButton[] homeScreenButtons;



    public HomeScreenButton GetButton(string id)
    {
        // Find the button with the specified ID
        HomeScreenButton button = System.Array.Find(homeScreenButtons, b => b.Id == id);
        return button;
    }

    [Button("Unlock Button")]
    public void UnlockButton(string id = "Food")
    {
        HomeScreenButton button = System.Array.Find(homeScreenButtons, b => b.Id == id);
        if (button == null)
        {
            Debug.LogWarning($"Button with ID {id} not found.");
            return;
        }

        if(button.unlocked)
        {
           // return;
        }
        print($"<color=yellow><b>UnlockButton {id} </b></color>");
        button.button.interactable = true;  
        button.gameObject.SetActive(true);
        button.UnlockButton();

    }

    public void DisableButtonsFTUE()
    {
        foreach (HomeScreenButton button in homeScreenButtons)
        {
            button.button.interactable = false;
        }
    }
    public void EnableButtonsFTUE()
    {
        foreach (HomeScreenButton button in homeScreenButtons)
        {
            button.button.interactable = true;
        }
    }


    [Button("Unlock Button")]
    public void UnlockButton(HomeScreenButton button)
    {
        if (button == null) return;
        button.UnlockButton();
    }
}
