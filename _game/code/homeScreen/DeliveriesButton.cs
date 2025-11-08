using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveriesButton : MonoBehaviour
{
    public Button button;
    [Space]
    public TextMeshProUGUI timerText;

    private int currentTime;

    private void Start()
    {
        timerText.text = "Deliveries";
    }

    public void StartCountdown(int startValue = 30)
    {
        currentTime = startValue;

        DOTween.To(() => currentTime, x => {
            currentTime = x;
            timerText.text = "Deliveries <color=red>" + currentTime.ToString("00") + "s</color>";
            button.interactable = false;

        }, 0, startValue)
        .SetEase(Ease.Linear)
        .SetUpdate(true)
        .OnComplete(() => {
            button.interactable = true;
            timerText.text = "Deliveries";
        });
    }
}
