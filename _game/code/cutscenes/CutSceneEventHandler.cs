using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CutSceneEventHandler : MonoBehaviour
{
    public bool showRv;
    [Space]
    public GameObject[] whiteCharacter;
    public GameObject[] blackCharacter;
    public virtual void StartDay()
    {
        EnableSelectedCharacter();
        ScriptRegistry.Instance.homeScreenManager.homeScreenOpen = false;
        ScriptRegistry.Instance.homeScreenManager.homeScreenButtonParent.SetActive(false);
        PlayerPrefs.SetInt("HOME_SCREEN_OPEN", 0);
        
    }

    public virtual void ShowDayCompleteScreen()
    {
        ScriptRegistry.Instance.dayCompleteHandler.ShowDayCompleteUI();
    }


    int GetDay()
    {
        return PlayerPrefs.GetInt("START_CONVERSATION_ID"); // from home screen manager
    }


    public virtual void EnableSelectedCharacter()
    {
        if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "AFRICAN")
        {
            foreach (GameObject character in blackCharacter)
            {
                character.SetActive(true);

                if(character.GetComponent<CustomizableCharacter>() != null)
                {
                    character.GetComponent<CustomizableCharacter>().EquipOutfitFromSave();
                }
            }

            foreach (GameObject _character in whiteCharacter)
            {
                _character.SetActive(false);

                if (_character.GetComponent<CustomizableCharacter>() != null)
                {
                    _character.GetComponent<CustomizableCharacter>().EquipOutfitFromSave();
                }
            }
        }

        if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "CAUCASIAN")
        {
            foreach (GameObject character in whiteCharacter)
            {
                character.SetActive(true);

                if (character.GetComponent<CustomizableCharacter>() != null)
                {
                    character.GetComponent<CustomizableCharacter>().EquipOutfitFromSave();
                }

            }
            foreach (GameObject character in blackCharacter)
            {
                character.SetActive(false);

                if (character.GetComponent<CustomizableCharacter>() != null)
                {
                    character.GetComponent<CustomizableCharacter>().EquipOutfitFromSave();
                }

            }

            //print("Enabled Caucasian Character");
        }
    }

    public void OnClickRv()
    {
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("rvBlocker").Show();
        AdsManager.instance.ShowRewardAd(OnRVComplete);
    }

    void OnRVComplete()
    {
        Debug.Log("<color=green>Rewarded Video Completed</color>");
        SaveGameManager.instance.LoadVariableData();
        ScriptRegistry.Instance.textGameController.ClearPrevousDayTextUIElements();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("dayCompleted").Hide();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("chatContent").Show();
        CutSceneManager.instance.SetCameraHalfScreen();
        ScriptRegistry.Instance.textGameController.ShowTextArea();
        AnalyticsEvents.instance.UniqueEvent($"day_{GetDay()}_restarted");
        StartDay();

    }
}
