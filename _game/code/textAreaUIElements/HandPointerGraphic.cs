using DG.Tweening;
using UnityEngine;

public class HandPointerGraphic : MonoBehaviour
{
    [SerializeField] private float targetPosition;
    [Space]
    public UIDialogueElement dialogue;
    private Tween animationTween;

    /*
    private void Start()
    {
        StartAnimation();
    }
    */
    private void OnEnable()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);
        StartAnimation();
    }

    private void StartAnimation()
    {
        // Kill any existing tween to prevent duplicates

        if (animationTween != null)
        {
            animationTween.Kill();
        }

        // Start new animation
        animationTween = GetComponent<RectTransform>()
            .DOAnchorPosY(targetPosition, .5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }

    public void RestartAnimation()
    {
        StartAnimation();
    }

    private void OnDisable()
    {
        // Clean up tween when disabled
        if (animationTween != null)
        {
            animationTween.Kill();
            animationTween = null;
        }
    }
}