using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExecuteCombatMoveButton : MonoBehaviour
{
    public Image moveImage;
    public Image cardFront;
    public Image cardBack;
    [Space] 
    public Button button;
    public TextMeshProUGUI moveNameText;
    public CombatMovesManager.CombatMove combatMove;

    public static event Action<string> onPerformFinisher;

    Vector3 backInitRot;
    Vector3 frontInitRot;

    public void SetUIElements()
    {

        if(cardBack)
        {
            backInitRot = cardBack.transform.rotation.eulerAngles;
        }
        if (cardFront)
        {
            frontInitRot = cardFront.transform.rotation.eulerAngles;
        }

        if (combatMove == null || !combatMove.unlocked)
        {
            gameObject.SetActive(false);
            return;
        }
        moveImage.sprite = combatMove.Sprite;
        moveNameText.SetText(combatMove.id);
        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(delegate { OnButtonPress(); } );

    }

    [Button]
    public void OnUnlockMove()
    {
        FeelVibrationManager.Instance.PlayVibration();

        cardFront.rectTransform.DORotate(new Vector3(0, 90, 0), .5f).OnComplete(() => { cardBack.rectTransform.DORotate(Vector3.zero, .5f); });

        Sequence sequence = DOTween.Sequence();

        sequence
            .Append(cardBack.rectTransform.DORotate(new Vector3(0, 90, 0), .3f).OnComplete(() => { cardFront.gameObject.SetActive(true); }))
            .Append(cardFront.rectTransform.DORotate(Vector3.zero, .3f))
            .Insert(0, transform.DOPunchScale(Vector3.one * .25f, sequence.Duration(), 10, .5f));

        //sequence.OnComplete(() => { cardBack.rectTransform.DOPunchScale(Vector3.one * 0.2f, .5f); });
    }

    private void OnButtonPress()
    {
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("finishCombat").Hide();
        Invoke("PlayFinishingMove", .5f);

    }

    [Button]
    void PlayFinishingMove()
    {
        FeelVibrationManager.Instance.PlayVibration();

        //CombatController.instance.PlayFinishMove(combatMove.animation_id);
        onPerformFinisher?.Invoke(combatMove.reactionAnimationId);
    }

    public void Reset()
    {
        cardBack.transform.eulerAngles = Vector3.zero;
        cardFront.transform.eulerAngles = new Vector3(0,90,0);
    }

}
