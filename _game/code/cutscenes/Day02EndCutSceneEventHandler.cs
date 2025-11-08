using UnityEngine;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using DG.Tweening;

public class Day02EndCutSceneEventHandler : MonoBehaviour
{
    [Title("PlayableDirector")]
    public PlayableDirector playableDirector;
    public Camera cutsceneCamera;

    [Space]
    public DrawingModeManager drawingModeManager;

    [Space]
    [SerializeField] private GameObject[] objectsToDisable;
    [SerializeField] private GameObject[] objectsToEnable;


    [Button]
    public void PlayEndCutscene()
    {
        FadeIn();
        ToggleObjects();
        playableDirector.Play();

        if (ScriptRegistry.Instance)
        {
            SetHalfScreenCameraRect();
            ScriptRegistry.Instance.textGameController.ShowTextArea();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("deliveryGameMoveButton").Hide();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("advanceButton").Show();
            ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(true);
        }
    }

    public void ShowDrawingArea()
    {
        drawingModeManager.ShowDrawingArea();
        //drawingModeManager.MinimizeTextArea();
        playableDirector.Pause();
    }

    public void SetDrawingAreaCameraRect()
    {
        Camera cam = cutsceneCamera;
        float startValue = cam.rect.y;
        float endValue = 0.25f;
        DOTween.To(() => startValue, x => startValue = x, endValue, .5f).OnUpdate(() =>
        {
            cam.rect = new Rect(0, startValue, 1, 1);
        });
    }

    public void SetHalfScreenCameraRect()
    {
        Camera cam = cutsceneCamera;
        float startValue = cam.rect.y;
        float endValue = 0.5f;
        DOTween.To(() => startValue, x => startValue = x, endValue, .5f).OnUpdate(() =>
        {
            cam.rect = new Rect(0, startValue, 1, 1);
        });
    }

    public void ShowTextArea()
    {
        ScriptRegistry.Instance.textGameController.ShowTextArea();
    }

    public void ToggleObjects()
    {
        foreach (GameObject g in objectsToDisable)
        {
            g.SetActive(false);
        }

        foreach (GameObject g in objectsToEnable)
        {
            g.SetActive(true);
        }
    }

    public void ShowButtonElipsis()
    {
        if (ScriptRegistry.Instance)
            ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(true);
    }

    public void HideButtonElipsis()
    {
        if (ScriptRegistry.Instance)
            ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(false);
    }

    public void FadeIn()
    {
        if (ScriptRegistry.Instance)
            ScriptRegistry.Instance.screenFade.FadeInFast(1);
    }

    public void ShowNextDialogueEntry()
    {
        if (DialogueManager.isConversationActive)
        {
            DialogueManager.instance.SendMessage(
                DialogueSystemMessages.OnConversationContinue,
                DialogueManager.dialogueUI,
                SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("No active conversation to continue.");
        }
    }


    public void UnloadLevel()
    {
        if (ScriptRegistry.Instance)
            HideButtonElipsis();
            ScriptRegistry.Instance.deliveryMinigameLoader.UnloadScene();

    }
}
