using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialoguesManager : MonoBehaviour
{

    public List<UIDialogueElement> uIDialogues = new List<UIDialogueElement>();


    //Debug method to check if a dialogue is already in the list
    [Button("Check if Dialogue Exists")]
    public void CheckIfDialogueExists(string id)
    {
        UIDialogueElement dialogue = GetUIDialogue(id);
        if (dialogue != null)
        {
            Debug.Log("Dialogue with ID: " + id + " already exists in the list.");
        }
        else
        {
            Debug.Log("Dialogue with ID: " + id + " does not exist in the list.");
        }
    }

    public UIDialogueElement GetUIDialogue(string id)
    {
        for (int i = 0; i < uIDialogues.Count; i++)
        {
            if (uIDialogues[i] != null)
            {
                if (uIDialogues[i].dialogueID == id)
                {
                    return uIDialogues[i];
                }
            }
        }

        return null;
    }



}
