using UnityEngine;
using DG.Tweening;
using System;
using PixelCrushers.DialogueSystem;
using DamageNumbersPro;
using Unity.Cinemachine;
using Sirenix.OdinInspector;

public class CombatController : MonoBehaviour
{
    public static CombatController instance;

    public float enemyHealth = 100f;
    public CombatHudElements combatHudElements;
    public Transform enemy;
    public Transform player;
    [Space]
    public DamageNumber damageNumber;

    public void StartFight()
    {
        CutSceneManager.instance.PauseCutscene();
        DrawingModeManager.instance.MinimizeTextArea();
        DrawingModeManager.instance.PickPattern();
        combatHudElements.targetToFollow = enemy;
        combatHudElements.gameObject.SetActive(true);
        combatHudElements.ShowHealth();

        AnalyticsEvents.instance.UniqueEvent("fight_start");
    }



    public void WaitForInput()
    {

        CutSceneManager.instance.PlaySlowMotion();
        DrawingModeManager.instance.ShowDrawingArea();

    }

    public void AttackEnemy()
    {
        if (enemyHealth > 0)
        {
            int r = UnityEngine.Random.Range(33, 35);
            enemyHealth -= r;
            combatHudElements.SetHealthFill(enemyHealth);

            if(enemyHealth < 10)
            {
                combatHudElements.gameObject.SetActive(false);
            }
            damageNumber.Spawn(enemy.position, r);

            ScriptRegistry.Instance.feelVibrationManager.PlayLightHaptic();
        }
    }

    public void AttackPlayer()
    {
        ScriptRegistry.Instance.textGameController.GetVariableUIElement("HP").UpdateUIElement(-15);
        damageNumber.Spawn(player.position, 15);
        ScriptRegistry.Instance.feelVibrationManager.PlayLightHaptic();

    }

    public void SetText(string text)
    {
        DrawingModeManager.instance.instructionText.text = text;
    }

    public void End()
    {
        ScriptRegistry.Instance.textGameController.ShowTextArea();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("drawingArea_canvas").Hide();
        combatHudElements.gameObject.SetActive(false);

    }
}