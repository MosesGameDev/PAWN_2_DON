using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OutfitButtonUI : MonoBehaviour
{
    public Image outfitIconImage;
    [Space]
    public TextMeshProUGUI outfitNameText;
    public TextMeshProUGUI outfitCostText;
    public TextMeshProUGUI outfitEquipedText;

    [Space]
    public Button buyButton;
    public Button equipButton;

    CharacterCustomizationHandler customizationHandler;

    int GetPlayerCash()
    {
        return ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").currentValue;
    }

    public void UpdateButton(OutfitData outfit)
    {
        if (customizationHandler == null)
        {
            customizationHandler = ScriptRegistry.Instance.characterCustomizationHandler;
        }

        if (!outfit.isLocked)
        {
            HandleUnlockedOutfit(outfit);
        }
        else
        {
            HandleLockedOutfit(outfit);
        }

        outfitNameText.text = outfit.outfitName;

        if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "AFRICAN")
        {
            outfitIconImage.sprite = outfit.outfitIcon_african;
        }

        if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "CAUCASIAN")
        {
            outfitIconImage.sprite = outfit.outfitIcon_caucasian;
        }
    }

    void HandleUnlockedOutfit(OutfitData outfit)
    {
        equipButton.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(false);

        if (outfit.isEquipped)
        {
            outfitEquipedText.text = "Equipped";
        }
        else
        {
            outfitEquipedText.text = "Equip";
        }

    }

    void HandleLockedOutfit(OutfitData outfit)
    {
        equipButton.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(true);

        if (GetPlayerCash() >= outfit.unlockCost)
        {
            buyButton.interactable = true;
            outfitCostText.text = "<color=#ffffff> Free</color>";
        }
        else
        {
            buyButton.interactable = false;
            outfitCostText.text = "Buy: <color=#f74c25>" + outfit.unlockCost.ToString("N0") + "</color>";
        }

    }

    OutfitData _outfit;
    public void SetButtonEvents(OutfitData outfit)
    {
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
            if (GetPlayerCash() >= outfit.unlockCost)
            {
                _outfit = outfit;
                SoundManager.Instance.PlaySFX("click");
                FeelVibrationManager.Instance.PlayVibration();
                OnClickRv();
            }
        });


        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(() =>
        {
            customizationHandler.ToggleEquiped(false);

            if (!outfit.isEquipped)
            {
                outfit.Equip();
                customizationHandler.OnEquip(outfit);
                UpdateButton(outfit);

                SoundManager.Instance.PlaySFX("click");
                FeelVibrationManager.Instance.PlayVibration();

            }
            else
            {
                outfit.Unequip();
                UpdateButton(outfit);
            }

            customizationHandler.UpdatButtonUI();
        });

    }


    public void OnClickRv()
    {
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("rvBlocker").Show();
        AdsManager.instance.ShowRewardAd(OnRVComplete);
    }

    void OnRVComplete()
    {
        _outfit.Unlock();
        ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").UpdateUIElement(_outfit.unlockCost);
        UpdateButton(_outfit);
        customizationHandler.UpdatButtonUI();

        customizationHandler.ToggleEquiped(false);

        if (!_outfit.isEquipped)
        {
            _outfit.Equip();
            customizationHandler.OnEquip(_outfit);
            UpdateButton(_outfit);
            AnalyticsEvents.instance.UniqueEvent($"outfit_{_outfit.outfitName}_unlocked");

        }
        else
        {
            _outfit.Unequip();
            UpdateButton(_outfit);
        }


    }

}
