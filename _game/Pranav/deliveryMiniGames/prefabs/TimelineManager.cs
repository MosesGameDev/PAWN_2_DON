using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using PixelCrushers.DialogueSystem;

public class TimelineManager : MonoBehaviour
{
    public Image fadeInImage;

    public PlayerDeliveryController deliveryController;
    public void FadeInImage()
    {
        Color c = fadeInImage.color;
        c.a = 0;
        fadeInImage.color = c;

        fadeInImage.DOFade(1f, 1f);
        StartCoroutine(DisableAfterSeconds(fadeInImage.transform.parent.gameObject, 1.5f));
    }
    public IEnumerator DisableAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }
    public Camera mainCamera;
    public void SetCameraY(float v)
    {
        Rect rect = mainCamera.rect;
        rect.y = v;
        mainCamera.rect = rect;
    }

    public GameObject conversationCanvas;
    public Vector3 inFocusPos = new Vector3(-42f, 285f, 0f);


    public void TogglePositionInFocus(bool isInFocus)
    {
        if (conversationCanvas==null)
        {
            return;
        }
        if (isInFocus)
        {
            conversationCanvas.transform.localPosition = inFocusPos;
        }
        else
        {
            conversationCanvas.transform.localPosition = new Vector3(inFocusPos.x, 0f, 0f);
        }

        isInFocus = !isInFocus;
    }

    [System.Serializable]
    public class TimelineData
    {
        public TimelineClipName name;
        public TimelineAsset clip;
    }
    public GameObject TimeLineMainObject;
    public GameObject[] objectsToDisable;
    void ToggleSceneObjects(bool s)
    {
        foreach (var item in objectsToDisable)
        {
            item.SetActive(s);
        }
    }
    private IEnumerator WaitForTimelineToEnd(PlayableDirector director)
    {
        yield return new WaitForSeconds((float)director.duration);

        TimeLineMainObject.SetActive(false);

        TogglePositionInFocus(false);
        ToggleSceneObjects(true);
    }
    public GameObject PPVolume;
    public void TogglePPVolume(bool s)
    {
        PPVolume.SetActive(s);
    }
    public PlayableDirector director;
    public List<TimelineData> timelines;

    public void PlayTimelineByName(TimelineClipName timelineName)
    {
        Debug.Log($"Timeline called");

        foreach (var timeline in timelines)
        {
            if (timeline.name == timelineName)
            {
                director.playableAsset = timeline.clip;
                ToggleSceneObjects(false);
                TimeLineMainObject.SetActive(true);

                TogglePositionInFocus(true);
                SetCameraY(0.5f);

                director.Play();
                StartCoroutine(WaitForTimelineToEnd(director));
                return;
            }
        }

        Debug.Log($"Timeline '{timelineName}' not found.");
    }


    [Button]
    public void PlayCutScene(TimelineClipName timelineName)
    {
        ToggleSceneObjects(false);
        TimeLineMainObject.SetActive(true);

        PlayTimelineByName(timelineName);
        SetCameraY(0.5f);

        deliveryController.advanceButton.ToggleShowElipsis(true);

        if (ScriptRegistry.Instance)
        {
            ScriptRegistry.Instance.textGameController.ToggleTextArea(false);
            string convo = DialogueManager.GetConversationTitle(2);
            ScriptRegistry.Instance.cutSceneManager.ContinueConversation(convo, 81);

            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("advanceButton").Show();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("deliveryGameMoveButton").Hide();
        }
    }




    public void PlayEndCutScene()
    {
        StartCoroutine(PlayEndCutSceneEnum());

    }


    IEnumerator PlayEndCutSceneEnum()
    {
        yield return new WaitForSeconds(2f);

        PlayCutScene(TimelineClipName.FinalDelivery_Rain);
    }


    public void EndMinigame()
    {
        if (ScriptRegistry.Instance)
        {
            SimpleSceneManager.Instance.UnloadLastScene();
        }
    }


    public void ShowNextDialogueEntry()
    {

        if (ScriptRegistry.Instance)
        {
            if (DialogueManager.isConversationActive)
            {
                DialogueManager.instance.SendMessage(
                    DialogueSystemMessages.OnConversationContinue,
                    DialogueManager.dialogueUI,
                    SendMessageOptions.DontRequireReceiver);
            }
        }

    }


    public void HideEllipsis()
    {
        if(ScriptRegistry.Instance)
        {
            //ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("phoneContacts").Hide();

            //ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("advanceButton").Show();
            //ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("deliveryGameMoveButton").Hide();
            //ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(false);
        }
    }

    public void ShowEllipsis()
    {
        if (ScriptRegistry.Instance)
        {
            //ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("phoneContacts").Hide();

            //ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("advanceButton").Show();
            //ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("deliveryGameMoveButton").Hide();
            //ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(false);
        }
    }


    public void Unpause()
    {
        director.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

    public void Pause()
    {
        director.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }
}
public enum TimelineClipName
{
    NA = 0,
    WakeUp = 1,
    Call1 = 2,
    Call2 = 3,
    CheckPhone = 4,
    FinalDelivery_Rain = 5,
    ReturnHome = 6
}