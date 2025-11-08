using System;
using UnityEngine;

public class DeliveryCompleteTrigger : MonoBehaviour
{
    public static event Action<int, int> onLevelComplete;

    /// <summary>
    /// I use this to resume dialogue conversation after delivery complete
    /// </summary>
    public int resumeConversationId;
    public int resumeDialogueEntryId;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onLevelComplete?.Invoke(resumeConversationId, resumeDialogueEntryId);
        }
    }   
}
