using Sirenix.OdinInspector;
using UnityEngine;

public class CombatMoveSelectorUIE : MonoBehaviour
{
    public ExecuteCombatMoveButton[] executeCombatMoveButtons;

    UIDialogueElement dialogueElement;

    private void OnEnable()
    {
        if (dialogueElement == null)
            dialogueElement = GetComponent<UIDialogueElement>();

        dialogueElement.OnShowDialogue += ShowMoves;
        dialogueElement.OnHideDialogue += OnHide;
    }

    private void Start()
    {
        if (dialogueElement == null)
            dialogueElement = GetComponent<UIDialogueElement>();
    }

    [Button]
    public void ShowMoves()
    {

        
        //CombatMovesManager.CombatMove[] combatMoves = CombatController.instance.movesManager.GetUnlockedCombatMoves();

        //for (int i = 0; i < combatMoves.Length; i++)
        //{
        //    executeCombatMoveButtons[i].gameObject.SetActive(true);
        //    executeCombatMoveButtons[i].combatMove = combatMoves[i];
        //    executeCombatMoveButtons[i].SetUIElements();
        //}

    }

    void OnHide()
    {
        for (int i = 0; i < executeCombatMoveButtons.Length; i++)
        {
            executeCombatMoveButtons[i].gameObject.SetActive(false);
            executeCombatMoveButtons[i].Reset();
        }
    }

    private void OnDisable()
    {
        if (dialogueElement != null)
        {
            dialogueElement.OnShowDialogue -= ShowMoves;
            dialogueElement.OnHideDialogue -= OnHide;
        }
    }

}
