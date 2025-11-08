using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;
using TMPro;
using UnityEngine.UI;

public class ResponseButton : MonoBehaviour
{

    [SerializeField] private TextDialogueParameterUIElement[] parameterUIElements;
    [SerializeField] private GameObject parametersContentHolder;
    [SerializeField] private int hp, crew, cash, territory;

    [Space]
    public GameObject rvPanel;
    public TextMeshProUGUI rvDefaultTex;
    public TextMeshProUGUI rvButtonTex;
    public Image iconImage;
    public TextMeshProUGUI rvValueText;


    public void UpdateFields(List<Field> fields)
    {
        hp = 0; crew = 0; cash = 0; territory = 0;


        for (int i = 0; i < fields.Count; i++)
        {
            if (fields[i].title == "HP")
            {
                if (int.Parse(fields[i].value) != 0)
                {
                    EnableParameterUIElement("HP", int.Parse(fields[i].value));
                }
            }

            if (fields[i].title == "POWER")
            {
                if (int.Parse(fields[i].value) != 0)
                {
                    EnableParameterUIElement("POWER", int.Parse(fields[i].value));
                }
            }

            if (fields[i].title == "CASH")
            {
                if (int.Parse(fields[i].value) != 0)
                {
                    EnableParameterUIElement("CASH", int.Parse(fields[i].value));
                }
            }

            if (fields[i].title == "REPUTATION")
            {
                if (int.Parse(fields[i].value) != 0)
                {
                    EnableParameterUIElement("REPUTATION", int.Parse(fields[i].value));
                }
            }
        }
    }


    public void EnableParameterUIElement(string id, int value)
    {
        for (int i = 0; i < parameterUIElements.Length; i++)
        {
            if (parameterUIElements[i].id == id)
            {
                if (!parametersContentHolder.activeInHierarchy)
                {
                    parametersContentHolder.SetActive(true);
                }

                parameterUIElements[i].gameObject.SetActive(true);
                parameterUIElements[i].UpdateElement(value);
            }
        }
    }

    public void Reset()
    {
        for (int i = 0; i < parameterUIElements.Length; i++)
        {
            parameterUIElements[i].gameObject.SetActive(false);
        }
    }
}

