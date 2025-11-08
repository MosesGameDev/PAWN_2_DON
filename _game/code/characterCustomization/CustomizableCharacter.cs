using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class CharacterOutfit
{
    public string outfitName;
    public GameObject outfit;
}

public class CustomizableCharacter : MonoBehaviour
{
    public CharacterOutfit currentOutfit;

    [Space]
    public CharacterOutfit[] outfits;

    private void OnEnable()
    {
        CharacterCustomizationHandler.onOutfitEquipped += CharacterCustomizationHandler_onOutfitEquipped;
    }

    private void OnDisable()
    {
        CharacterCustomizationHandler.onOutfitEquipped -= CharacterCustomizationHandler_onOutfitEquipped;
    }

    private void CharacterCustomizationHandler_onOutfitEquipped(OutfitData obj)
    {
        CharacterOutfit outfit = GetOutfit(obj.outfitName);
        if (outfit != null)
        {
            EquipOutfit(outfit);
        }

    }


    [Button]
    public void EquipOutfitFromSave()
    {
        if (PlayerPrefs.HasKey("SELECTED_OUTFIT"))
        {
            EquipOutfit(GetOutfit(ScriptRegistry.Instance.characterCustomizationHandler.GetEquipedOutfit().outfitName));
        }
    }

    private void EquipOutfit(CharacterOutfit outfit)
    {
        if (currentOutfit != null)
        {
            currentOutfit.outfit.SetActive(false);
        }

        currentOutfit = outfit;
        currentOutfit.outfit.SetActive(true);

    }

        CharacterOutfit GetOutfit(string outfitName)
    {
        foreach (var outfit in outfits)
        {
            if (outfit.outfitName == outfitName)
            {
                return outfit;
            }
        }
        return null;
    }
}
