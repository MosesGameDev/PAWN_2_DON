using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

[System.Serializable]
public class OutfitData
{
    public string outfitName;
    public int unlockCost;


    [Space]
    public Sprite outfitIcon_african;
    public Sprite outfitIcon_caucasian;

    [Space]
    public bool isLocked;
    public bool isEquipped;

    public void SetLockState()
    {
        if(!PlayerPrefs.HasKey(outfitName.ToUpper() + "_LOCKED"))
        {
            PlayerPrefs.SetInt(outfitName.ToUpper() + "_LOCKED", 0);
            isLocked = true;
            return;
        }

        int lockedState = PlayerPrefs.GetInt(outfitName.ToUpper() + "_LOCKED");

        if (lockedState == 1)
        {
            isLocked = false;
        }
        else
        {
            isLocked = true;
        }
    }

    public void SetEquippedState()
    {
        if (!PlayerPrefs.HasKey(outfitName.ToUpper() + "_EQUIPPED"))
        {
            PlayerPrefs.SetInt(outfitName.ToUpper() + "_EQUIPPED", 0);
            isEquipped = false;
            return;
        }
        int equippedState = PlayerPrefs.GetInt(outfitName.ToUpper() + "_EQUIPPED");
        if (equippedState == 1)
        {
            isEquipped = true;
        }
        else
        {
            isEquipped = false;
        }
    }

    public void CheckIfEquiped()
    {

    }

    public void Unlock()
    {
        isLocked = false;
        PlayerPrefs.SetInt(outfitName.ToUpper() + "_LOCKED", 1);

    }

    public void Equip()
    {
        if (!isEquipped)
        {
            isEquipped = true;
            PlayerPrefs.SetInt(outfitName.ToUpper() + "_EQUIPPED", 1);
            PlayerPrefs.SetString("SELECTED_OUTFIT", outfitName);
        }
    }

    public void Unequip()
    {
        isEquipped = false;
        PlayerPrefs.SetInt(outfitName.ToUpper() + "_EQUIPPED", 0);
        PlayerPrefs.Save();
    }
}

public class CharacterCustomizationHandler : MonoBehaviour
{

    private CustomizableCharacter selectedCharacter;
    public CustomizableCharacter blackCharaceter;
    public CustomizableCharacter whiteCharacter;

    [Space]
    public Camera gameCamera;
    public Camera customisationCamera;
    public GameObject[] customizationRoom;

    [Space]
    public OutfitData[] outfits;

    [Space]
    public OutfitButtonUI[] outfitButtons;

    public static event System.Action<OutfitData> onOutfitEquipped;
    

    public void CheckForSave()
    {
        if (PlayerPrefs.HasKey("OUTFITS_UNLOCKED"))
        {
            ScriptRegistry.Instance.homeScreenManager.GetButton("Closet").EnableButton();
        }
    }

    public void EnableCostumeButton()
    {
        ScriptRegistry.Instance.homeScreenManager.DisableButtonsFTUE();
        ScriptRegistry.Instance.homeScreenManager.GetButton("Closet").UnlockButton();
        PlayerPrefs.SetInt("OUTFITS_UNLOCKED", 1);
    }

    public void OpenCustomisationScreen()
    {
        EnableSelectedCharacter();
        CheckForUnlockedOutfits();
        UpdatButtonUI();

        OutfitData outfit = GetOutfitByName(PlayerPrefs.GetString("SELECTED_OUTFIT"));
        if (outfit != null)
        {
            OnEquip(outfit);
        }


    }

    public virtual void EnableSelectedCharacter()
    {
        if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "AFRICAN")
        {
            blackCharaceter.gameObject.SetActive(true);
            whiteCharacter.gameObject.SetActive(false);
            selectedCharacter = blackCharaceter;
        }

        if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "CAUCASIAN")
        {
            blackCharaceter.gameObject.SetActive(false);
            whiteCharacter.gameObject.SetActive(true);
            selectedCharacter = whiteCharacter;
        }

                selectedCharacter.EquipOutfitFromSave();

    }



    public void ShowOutfirSelection()
    {
        OpenCustomisationScreen();
        ScriptRegistry.Instance.screenFade.FadeInFast(1);

        customisationCamera.gameObject.SetActive(true);
        
        if (ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
        {
            ScriptRegistry.Instance.textGameController.ShowVariableUIElements();
            ScriptRegistry.Instance.textGameController.ShowTextArea();
            ScriptRegistry.Instance.dayCompleteHandler.nextDayButton.gameObject.SetActive(false);
        }

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("outfitSelect").Show();
        ToggleRoom(true);

        if(!PlayerPrefs.HasKey("COSTUMES_UNLOCKED"))
        {
            PlayerPrefs.SetInt("COSTUMES_UNLOCKED", 1);
            PlayerPrefs.Save();
        }
    }

    public void CloseOutfitSelection()
    {
        if (ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
        {
            ScriptRegistry.Instance.textGameController.MinimizeTextArea();
            ScriptRegistry.Instance.dayCompleteHandler.nextDayButton.gameObject.SetActive(true);
        }

        customisationCamera.gameObject.SetActive(false);

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("outfitSelect").Hide();
        ToggleRoom(false);
    }

    public void ToggleRoom(bool show)
    {
        foreach (GameObject room in customizationRoom)
        {
            room.SetActive(show);
        }
    }

        void CheckForUnlockedOutfits()
    {
        foreach (OutfitData outfit in outfits)
        {
            if (PlayerPrefs.GetInt(outfit.outfitName.ToUpper() + "_LOCKED") == 1)
            {
                outfit.Unlock();
            }
        }
    }

    public void UpdatButtonUI()
    {
        for (int i = 0; i < outfits.Length; i++)
        {
            outfits[i].SetLockState();
            outfits[i].SetEquippedState();
            outfitButtons[i].UpdateButton(outfits[i]);
            outfitButtons[i].SetButtonEvents(outfits[i]);
        }
    }


    [Button]
    public void ToggleEquiped(bool isEquipped)
    {
        foreach (var outfit in outfits)
        {
            outfit.isEquipped = isEquipped;

            if (isEquipped == false)
            {
                outfit.Unequip();
            }

        }
    }

    public OutfitData GetEquipedOutfit()
    {
        if(PlayerPrefs.HasKey("SELECTED_OUTFIT"))
        {
            return GetOutfitByName(PlayerPrefs.GetString("SELECTED_OUTFIT"));
        }

        return null;
    }


    OutfitData GetOutfitByName(string outfitName)
    {
        foreach (OutfitData outfit in outfits)
        {
            if (outfit.outfitName == outfitName)
            {
                return outfit;
            }
        }
        return null;
    }

    public void OnEquip(OutfitData outfit)
    {
        onOutfitEquipped?.Invoke(outfit);
        print("Equipped: " + outfit.outfitName);
    }

}
