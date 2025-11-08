using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class ConversationNextDialogueEntry : MonoBehaviour
{
    [Button]
    public void AdvanceConversation()
    {
        if (DialogueManager.isConversationActive)
        {
            DialogueManager.instance.SendMessage(
                DialogueSystemMessages.OnConversationContinue,
                DialogueManager.dialogueUI,
                SendMessageOptions.DontRequireReceiver);
        }
    }
}
