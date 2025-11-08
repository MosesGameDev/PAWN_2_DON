using System.Collections;
using UnityEngine;

public class ScreenFade : MonoBehaviour
{
    public bool isVisible => UIDialogueElement.isOpen;
    [Space]
    [SerializeField] private UIDialogueElement UIDialogueElement;

    public void FadeInFast(float fadeDuration)
    {
        UIDialogueElement.canvasGroup.alpha = 1;
        StartCoroutine(FadeOutEnum(fadeDuration));

    }

    public void FadeIn()
    {
        UIDialogueElement.canvasGroup.alpha = 1;
    }

    public void FadeOutFast()
    {
        UIDialogueElement.canvasGroup.alpha = 0;
    }


    public void PlayFade(float fadeDuration)
    {
        UIDialogueElement.Show();
        StartCoroutine(FadeOutEnum(fadeDuration));
    }

    IEnumerator FadeOutEnum(float fadeDuration)
    {
        yield return new WaitForSeconds(fadeDuration);
        UIDialogueElement.Hide();
    }
}
