using UnityEngine;
using Sirenix.OdinInspector;
using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine.UI;


public class Day06CutSceneEventHandler : CutSceneEventHandler
{
    [SerializeField] private CutSceneSceneRefrences sceneSceneRefrences;
    [SerializeField] private Transform responsePanel;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);

    }

    [Button("0. Start Day Editor")]
    public override void StartDay()
    {
        base.StartDay();
        AnalyticsEvents.instance.OnLevelStart(6);
        SaveGameManager.instance.LoadVariableData();


        ScriptRegistry.Instance.textGameController.ShowVariableUIElements();

        //StartCoroutine(StartDayEnum());
        //ScriptRegistry.Instance.screenFade.FadeInFast(1.25f);
        StartDayContent();
        print("<b><color=red>Starting Day 6</color></b>");
        print($"<b><color=red>IsConversationActive:  {DialogueManager.IsConversationActive}</color></b>");

        Invoke("ForceStart", 1);
    }


    public void ForceStart()
    {
        if(!DialogueManager.IsConversationActive)
        {
            print("No    conversation   Active");
        }
    }



    IEnumerator StartDayEnum()
    {
        yield return new WaitForSeconds(1f);
        //StartDayContent();
    }

    void StartDayContent()
    {
        // print in bold and  color orange

        sceneSceneRefrences.DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = sceneSceneRefrences.cutSceneManager.GetClip("1-Day6-Start");
        sceneSceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);

        ScriptRegistry.Instance.businessHandler.ShowBusinessEarnings();

        CutSceneManager.instance.ContinueConversation("DAY06", 42);

        ScriptRegistry.Instance.textGameController.ScrollToBottomSmooth();

    }

    [Button("2. Play WareHouse")]
    public void PlayWareHouse()
    {
        print("Playing Warehouse Cutscene");
        ScriptRegistry.Instance.textGameController.ShowVariableUIElements();
        sceneSceneRefrences.DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);


        CutSceneClip clip = sceneSceneRefrences.cutSceneManager.GetClip("2-Warehouse_day06");
        sceneSceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);
        sceneSceneRefrences.cutSceneManager.ContinueConversation("DAY06", 74);
    }

    public void PlayExitWarehouse()
    {
        CutSceneClip clip = sceneSceneRefrences.cutSceneManager.GetClip("3-2-Hesitate");
        sceneSceneRefrences.cutSceneManager.PlayClipFromTime(clip.id, 6.3f );
    }

    [Button("3. Play Bar")]
    public void PlayBar()
    {

        sceneSceneRefrences.DisableGameObjects();
        transform.GetChild(0).gameObject.SetActive(true);

        CutSceneClip clip = sceneSceneRefrences.cutSceneManager.GetClip("4-Enter_Bar");
        sceneSceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);
        sceneSceneRefrences.cutSceneManager.ContinueConversation("DAY06", 77);
    }
    
    
    [Button("4.Return Home")]
    public void ReturnHome()
    {

        CutSceneClip clip = sceneSceneRefrences.cutSceneManager.GetClip("Return_Home_06");
        sceneSceneRefrences.cutSceneManager.PlayClipFromStart(clip.id);

    }


    public void CheckPowerValue()
    {
        StartCoroutine(WaitForResponse());
    }

    IEnumerator WaitForResponse()
    {
        yield return new WaitForSeconds(.6f);
        int power = ScriptRegistry.Instance.textGameController.GetVariableUIElement("POWER").currentValue;

        if (power < 50 && DialogueLua.GetVariable("HasGun").asBool == true)
        {
            responsePanel.GetChild(1).GetComponent<Button>().interactable = false;
        }

        print("Power Value: " + power);
    }

}
