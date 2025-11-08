using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using static CombatMovesManager;

public class CombatUnlockMoveUIE : MonoBehaviour
{
    [SerializeField] private ExecuteCombatMoveButton[] executeCombatMoveButtons;
    [SerializeField] private UIDialogueElement movedUnlockDialogue;
    [SerializeField] private UIDialogueElement movedUnlockedDialogue;
    [SerializeField] private TextMeshProUGUI movedUnlockedDialogueText;
    UIDialogueElement dialogueElement;
    CombatMovesManager.CombatMove[] combatMoves;

    private void Start()
    {
        dialogueElement = GetComponent<UIDialogueElement>();
        dialogueElement.OnShowDialogue += ShowMoves;
        dialogueElement.OnHideDialogue += OnHide;
    }

    public void ShowMoves()
    {

        //combatMoves = CombatController.instance.movesManager.GetLockedCombatMoves();


        for (int i = 0; i < combatMoves.Length; i++)
        {
            if (!combatMoves[i].unlocked)
            {
                executeCombatMoveButtons[i].gameObject.SetActive(true);

                executeCombatMoveButtons[i].combatMove = combatMoves[i];
                CombatMovesManager.CombatMove move = executeCombatMoveButtons[i].combatMove;

                executeCombatMoveButtons[i].moveNameText.text = combatMoves[i].id;
                executeCombatMoveButtons[i].moveImage.sprite = combatMoves[i].Sprite;

                executeCombatMoveButtons[i].button.interactable = true;

                executeCombatMoveButtons[i].button.onClick.RemoveAllListeners();
                executeCombatMoveButtons[i].button.onClick.AddListener(delegate { UnlockMove(move); });

            }

        }

    }

    void UnlockMove(CombatMovesManager.CombatMove move)
    {

        SoundManager.Instance.PlaySFX("button");
        for (int i = 0; i < executeCombatMoveButtons.Length; i++)
        {
            executeCombatMoveButtons[i].button.interactable = false;
        }

        move.unlocked = true;
        movedUnlockedDialogueText.SetText(move.id);
        //CombatController.instance.movesManager.UnlockCombatMove(move.id);



        Invoke("OnMoveUnlocked", 1);
    }

    void OnMoveUnlocked()
    {

        movedUnlockDialogue.Hide();
        movedUnlockedDialogue.Show();
    }

    public void Reset()
    {
        OnHide();
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
        dialogueElement.OnShowDialogue -= ShowMoves;
        dialogueElement.OnHideDialogue -= OnHide;
    }
}
