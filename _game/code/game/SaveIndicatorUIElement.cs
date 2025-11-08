using System.Collections;
using UnityEngine;

public class SaveIndicatorUIElement : MonoBehaviour
{
    [SerializeField] private UIDialogueElement dialogueElement;

    private void OnEnable()
    {
        SaveGameManager.OnDataSaved += SaveGameManager_OnDataSaved;
    }

    private void SaveGameManager_OnDataSaved()
    {
        dialogueElement.Show();
        StartCoroutine("HideUI");
    }
    IEnumerator HideUI()
    {
        yield return new WaitForSeconds(1);
        dialogueElement.Hide();
    }

    private void OnDisable()
    {
        SaveGameManager.OnDataSaved -= SaveGameManager_OnDataSaved;
    }

}
