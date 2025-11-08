using Coffee.UIExtensions;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;

public class TextDialogueParameterUIElement : MonoBehaviour
{
    public string id;
    public RectTransform RectTransform;
    public TextMeshProUGUI valueText;
    [Space]
    public ParticleSystem fxParticle;
    public int value;
    public bool playParticles;

    public void SetParticleAttractor()
    {
        UIParticleAttractor particleAttractor = ScriptRegistry.Instance.textGameController.GetVariableUIElement(id).UIParticleAttractor;
        particleAttractor.AddParticleSystem(fxParticle);

        particleAttractor.onAttracted.RemoveAllListeners() ;
    }


    public void PlayParticle()
    {

        if (value < 0)
        {

            if (value < -999)
            {
                float f = value;
                valueText.SetText($"<color=#ff9191>{f * 0.001f}K</color=red>");
            }
            else
            {
                if (id == "HP")
                {
                    valueText.SetText($"<color=#FFEE7A>{value}</color>");
                    return;
                }
                valueText.SetText($"<color=#FFFFFF>{value}</color>");
            }

        }
        else
        {
            if (value > 999)
            {
                float f = value;
                valueText.SetText($"<color=#FFFFFF>+{f * 0.001f}K</color>");
            }
            else
            {
                valueText.SetText("<color=#FFFFFF>+" + value + "</color>");
            }


            UIParticleAttractor particleAttractor = ScriptRegistry.Instance.textGameController.GetVariableUIElement(id).UIParticleAttractor;
            //particleAttractor.onAttracted.AddListener(delegate { ScriptRegistry.Instance.textGameController.GetVariableUIElement(id).UpdateUIElementDirect(value); });
            ScriptRegistry.Instance.textGameController.GetVariableUIElement(id).UpdateUIElementDirect_2(value);
            fxParticle.Play();
        }
    }


    public void UpdateElement(int value)
    {
        if (value < 0)
        {

            if(value < -999)
            {
                float f = value;
                valueText.SetText($"<color=red>{f * 0.001f}K</color=red>");
            }
            else
            {
                if(id == "HP")
                {
                    valueText.SetText($"<color=#FFEE7A>{value}</color>");
                    return;
                }
                valueText.SetText($"<color=#FFEE7A>{value}</color>");
            }

        }
        else
        {
            if(value > 999)
            {
                float f = value;
                valueText.SetText($"<color=green>${f * 0.001f}K</color=green>");
            }
            else
            {
                if (id == "HP")
                {
                    valueText.SetText("<color=green>" + value + "%</color=green>");
                    return;
                }
                //print("<color=green>" + value + "</color=green>");
                valueText.SetText("<color=green>" + value + "</color=green>");
            }
        }

        //RectTransform.DOPunchScale(Vector3.one * .05f, .3f);
    }



}
