using UnityEngine;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using System;

public class SlotEffect : MonoBehaviour
{
    [SerializeField] private RectTransform content;

    [Space]
    [SerializeField] private GameObject[] textEffects;

    public Action<int> onStopSpin;

    public int randomIndex;

    public void Spin()
    {
        content.DOAnchorPosY(-700, .3f).SetLoops(5, LoopType.Restart).OnComplete(OnStopSpin);
    }

    public void OnStopSpin()
    {
        randomIndex = UnityEngine.Random.Range(0, 3);
        content.anchoredPosition = Vector2.zero;
        Conversation conversation = DialogueManager.masterDatabase.GetConversation(DialogueManager.LastConversationStarted);
        

        switch (randomIndex)
        {
            case 0:
                content.DOAnchorPosY(-168, .3f);
                textEffects[0].transform.DOPunchScale(Vector3.one * 0.12f, .3f).SetDelay(.25f).OnComplete(() => { gameObject.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack); });
                break;

            case 1:
                content.DOAnchorPosY(-377, .3f);
                textEffects[1].transform.DOPunchScale(Vector3.one * 0.12f, .3f).SetDelay(.25f).OnComplete(() => { gameObject.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack); });
                break;

            case 2:
                content.DOAnchorPosY(-586, .3f);
                textEffects[2].transform.DOPunchScale(Vector3.one * 0.12f, .3f).SetDelay(.25f).OnComplete(() => { gameObject.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack); });
                break;
        }

        onStopSpin.Invoke(randomIndex);
    }
}
