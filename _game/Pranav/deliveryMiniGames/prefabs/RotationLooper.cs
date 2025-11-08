using UnityEngine;
using DG.Tweening;

public class RotationLooper : MonoBehaviour
{
    public Vector3 startRotation;
    public Vector3 finishRotation;
    public bool startOnEnable = false;

    private Tween rotationTween;

    private void OnEnable()
    {
        if (startOnEnable)
        {
            StartRotationTween();
        }
    }
    public LoopType loopType = LoopType.Yoyo;
    public Ease GetEase = Ease.InOutSine;
    public RotateMode GetRotateMode = RotateMode.Fast;
    public float rotateTime = 1.5f;
    public void Openable(bool shouldOpen)
    {
        if (shouldOpen && rotationTween == null)
        {
            StartRotationTween();
        }
    }

    void StartRotationTween()
    {
        transform.rotation = Quaternion.Euler(startRotation);

        rotationTween = transform
        .DOLocalRotate(finishRotation, rotateTime, GetRotateMode)
            .SetEase(GetEase)
            .SetLoops(-1, loopType);
    }

    public void PauseTween()
    {
        rotationTween?.Pause();
    }

    public void ResumeTween()
    {
        rotationTween?.Play();
    }
}