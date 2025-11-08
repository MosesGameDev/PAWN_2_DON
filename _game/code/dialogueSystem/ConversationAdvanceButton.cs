using PixelCrushers.DialogueSystem;
using UnityEngine;

public class ConversationAdvanceButton : MonoBehaviour
{
    public string converstionID;


    private void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(StartNewConversation);
    }

    public void SetConverstionID(string id)
    {
        converstionID = id;
    }

    public void StartNewConversation()
    {
        DialogueManager.StopAllConversations();
        DialogueManager.StartConversation(converstionID);
    }
}
