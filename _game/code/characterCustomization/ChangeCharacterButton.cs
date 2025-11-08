using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ChangeCharacterButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Sprite delectedSprite;
    [SerializeField] private Sprite selectedSprite;

    [SerializeField] private CharacterSelectionHandler.BodyType bodyType;
    [SerializeField] private CharacterSelectionHandler customizationHandler;

    public Action onButtonPressed;

    bool isSelected;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnSelected);
        CharacterSelectionHandler.OnBodyTypeSelected += CharacterSelectionHandler_OnBodyTypeSelected;
        button.Select();
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveListener(OnSelected);
        CharacterSelectionHandler.OnBodyTypeSelected -= CharacterSelectionHandler_OnBodyTypeSelected;
    }

    private void CharacterSelectionHandler_OnBodyTypeSelected(CharacterSelectionHandler.BodyType type)
    {
        if(type != bodyType)
        {
            Deselect();
        }
    }

    void OnSelected()
    {
        Button button = GetComponent<Button>();

        customizationHandler.SelectBodyType(bodyType);

        if (!isSelected)
        {
            Select();
        }
        else
        {
            Deselect();
        }
    }

    public void Select()
    {
        Button button = GetComponent<Button>();

        isSelected = true;
        button.image.sprite = selectedSprite;
        buttonText.text = "Selected";
    }


    public void Deselect()
    {
        Button button = GetComponent<Button>();

        isSelected = false;
        button.image.sprite = delectedSprite;
        buttonText.text = "Select";
    }
}
