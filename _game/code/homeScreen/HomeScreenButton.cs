using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class HomeScreenButton : MonoBehaviour
{
    public string Id;
    public string dialogueOpenId;
    [Space]
    public Button button;

    [Space]
    [SerializeField] private TextMeshProUGUI buttonText;

    [Space]
    [SerializeField] private Image iconGraphic;
    [SerializeField] private Image lockGraphic;
    [SerializeField] private GameObject pointerGraphic;
    [SerializeField] private Image maskGraphic;

    [Space]
    public UnityEvent onUnlock;

    public bool unlocked;


    public void UnlockButton()
    {

        transform.parent.gameObject.SetActive(true);

        gameObject.SetActive(true);
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("softMask").Show();

        lockGraphic.enabled = false;
        iconGraphic.enabled = true;

        iconGraphic.rectTransform.DOScale(Vector3.one * 1.1f, 2f).SetEase(Ease.OutBack).SetLoops(-1);

        pointerGraphic.SetActive(true);
        maskGraphic.gameObject.SetActive(true);
        StartCoroutine(DelayShowPointer());
        ScriptRegistry.Instance.homeScreenManager.DisableButtonsFTUE();
        button.interactable = true;
        unlocked = true;
    }

    public void UnlockButton2()
    {

        transform.parent.gameObject.SetActive(true);

        gameObject.SetActive(true);

        lockGraphic.enabled = false;
        iconGraphic.enabled = true;

        ScriptRegistry.Instance.homeScreenManager.DisableButtonsFTUE();
        button.interactable = true;

    }

    IEnumerator DelayShowPointer()
    {
        yield return new WaitForSeconds(0.5f);
        pointerGraphic.SetActive(true);
    }

    public void EnableButton()
    {
        gameObject.SetActive(true);
        button.interactable = true;
        iconGraphic.enabled = true;
        lockGraphic.enabled = false;
        pointerGraphic.gameObject.SetActive(false);
        maskGraphic.gameObject.SetActive(false);
        unlocked = true;
    }

    [Button]
    public void OnButtonPress()
    {
        iconGraphic.rectTransform.DOKill();
        iconGraphic.rectTransform.localScale = Vector3.one;
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("softMask").Hide();
        EnableButton();
        SoundManager.Instance.PlaySFX("click");
        FeelVibrationManager.Instance.PlayVibration();
        ScriptRegistry.Instance.homeScreenManager.EnableButtonsFTUE();

    }


}
