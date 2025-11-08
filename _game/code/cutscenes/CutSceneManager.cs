using PixelCrushers.DialogueSystem;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class CutSceneClip
{
    public string id;
    public PlayableAsset playableAsset;
    public PlayableDirector playableDirector;
}
[System.Serializable]
public class CutSceneList
{
    public string dayName;
    public CutSceneClip[] cutSceneClips;
}

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager instance;

    public bool isPaused;

    public CutSceneEventHandler[] cutSceneEventHandlers;


    [Space]
    public Button gameOverRestartButton;

    [Space]
    public DayCompleteHandler dayCompleteHandler;

    [Space]
    public CutSceneList[] cutSceneList;

    [Space]
    // public CutSceneClip[] cutSceneClips;

    public CutSceneClip[] cutSceneClipsCopy;


    [Space]
    [Sirenix.OdinInspector.ReadOnly]
    public PlayableDirector currentActiveDirector;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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

    public void ShowDrawingArea()
    {
        print("Showing Drawing Area");
        DrawingModeManager.instance.ShowDrawingArea();
        DrawingModeManager.instance.MinimizeTextArea();
        PlaySlowMotion();
    }

    public void ShowTextArea()
    {
        ScriptRegistry.Instance.textGameController.ShowTextArea();
        DrawingModeManager.instance.drawingAreaDialogueCanvas.Hide();
    }

    public CutSceneEventHandler GetCutsceneHandler(int day)
    {
        return cutSceneEventHandlers[day];
    }


    public void PauseCutscene()
    {
        if (currentActiveDirector != null && currentActiveDirector.playableGraph.IsValid())
        {
            currentActiveDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
            isPaused = true;
        }
    }

    public void PlaySlowMotion()
    {
        if (currentActiveDirector != null && currentActiveDirector.playableGraph.IsValid())
        {
            currentActiveDirector.playableGraph.GetRootPlayable(0).SetSpeed(0.125f);
            isPaused = true;
        }

        Invoke("PauseCutscene", 2);
    }


    public void PausePlayableDirector(PlayableDirector playableDirector)
    {
        if (playableDirector != null)
        {
            playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
            isPaused = true;
        }
    }

    public void SetCameraHalfScreen()
    {
        ScriptRegistry.Instance.textGameController.SetHalfScreenCameraRect();
    }

    public void Unpause()
    {

        if (currentActiveDirector != null && currentActiveDirector.playableGraph.IsValid())
        {
            isPaused = false;
            currentActiveDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        }
    }

    public void UnpausePlayableDirector(PlayableDirector playableDirector)
    {
        playableDirector.Resume();
        if (playableDirector != null)
        {
            playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
            isPaused = false;
        }
    }

    public void ContinueConversation(string ConversationId, int dialogueEntry)
    {
        Transform player = ScriptRegistry.Instance.player.transform;
        DialogueManager.instance.StopAllConversations();

        DialogueManager.instance.StartConversation(ConversationId, player, DialogueManager.instance.transform, dialogueEntry);
    }


    public void DelayHideElipsis()
    {
        StartCoroutine(HideElipsisEnum());
    }

    IEnumerator HideElipsisEnum()
    {
        yield return new WaitForSeconds(0.5f);
        HideButtonElipsis();
    }

    public void DelayShowNextDialogueEntry()
    {
        StartCoroutine(HideElipsisEnum());
    }

    IEnumerator DelayShowNextDialogueEntryEnum()
    {
        yield return new WaitForSeconds(1);
        print("Showing next dialogue entry");
        ShowNextDialogueEntry();
    }

    public void FadeIn(float v = 1)
    {
        ScriptRegistry.Instance.screenFade.FadeInFast(v);
    }

    public void FadeOut()
    {
        ScriptRegistry.Instance.screenFade.FadeOutFast();
    }

    public void PlayScreenFade()
    {
        FadeIn();
    }


    public void PlayClipFromStart(string id)
    {
        Unpause();
        // foreach (var clip in cutSceneClips)
        // {
        //     if (clip.id == id)
        //     {
        //         currentActiveDirector = clip.playableDirector;
        //         currentActiveDirector.playableAsset = clip.playableAsset;
        //         currentActiveDirector.time = 0;
        //         currentActiveDirector.Play();
        //         break;
        //     }
        // }

        foreach (var list in cutSceneList) // go through each day list
        {
            foreach (var clip in list.cutSceneClips) // go through each clip
            {
                if (clip.id == id)
                {
                    currentActiveDirector = clip.playableDirector;
                    currentActiveDirector.playableAsset = clip.playableAsset;
                    currentActiveDirector.time = 0;
                    currentActiveDirector.Play();
                    return; // stop once found
                }
            }
        }

        Debug.LogWarning($"CutSceneClip with id '{id}' not found!");
    }


    public void PauseForInput()
    {
        PauseCutscene();
        HideButtonElipsis();
    }


    public void PlayClip(string id)
    {
        Unpause();
        foreach (var list in cutSceneList) // go through each day list
        {
            foreach (var clip in list.cutSceneClips) // go through each clip
            {
                if (clip.id == id)
                {
                    currentActiveDirector = clip.playableDirector;
                    currentActiveDirector.playableAsset = clip.playableAsset;
                    currentActiveDirector.Play();
                    break;
                }
            }
        }

    }

    public void PlayClipFromTime(string id, float time, bool startPaused = false)
    {
        Unpause();
        foreach (var list in cutSceneList) // go through each day list
        {
            foreach (var clip in list.cutSceneClips) // go through each clip
            {
                if (clip.id == id)
                {
                    currentActiveDirector = clip.playableDirector;
                    currentActiveDirector.time = time;
                    currentActiveDirector.playableAsset = clip.playableAsset;
                    if (!startPaused)
                    {
                        currentActiveDirector.Play();
                    }
                    break;
                }
            }
        }
    }




    public CutSceneClip GetClip(string id)
    {
        // foreach (var clip in cutSceneClips)
        // {
        //     if (clip.id == id)
        //     {
        //         return clip;
        //     }
        // }

        // return null;

        foreach (var list in cutSceneList) // loop through all lists
        {
            foreach (var clip in list.cutSceneClips) // loop through all clips in that list
            {
                if (clip.id == id)
                {
                    return clip;
                }
            }
        }

        return null; // if nothing found
    }

    [Button]
    public void ShowButtonElipsis()
    {
        ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(true);
    }

    [Button]
    public void HideButtonElipsis()
    {
        ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(false);
    }


    public void StartMinigame(string levelName)
    {
        ScriptRegistry.Instance.deliveryMinigameLoader.LoadScene(levelName);
    }

    [Button]
    public void CheckCutSceneClipExists(string id)
    {
        foreach (var list in cutSceneList) // go through each day list
        {
            foreach (var clip in list.cutSceneClips) // go through each clip
            {
                if (clip.id == id)
                {
                    Debug.Log($"Cutscene clip with ID '{id}' exists. {clip.playableDirector.name}");
                    return;
                }
            }
        }
        Debug.LogWarning($"Cutscene clip with ID '{id}' does not exist.");
    }

}
