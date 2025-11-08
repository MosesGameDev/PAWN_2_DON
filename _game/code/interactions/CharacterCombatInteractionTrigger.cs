using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterCombatInteractionTrigger : MonoBehaviour
{
    public CombatController combatController;
    public UnityEvent onTriggerEnter;

    public static event Action OnEnterTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerCharacterController>().StopMoving();
            CrewManager.instance.CrewMembersStop();
            //combatController.npcCharacter.ShowPower();

            OnEnterTrigger?.Invoke();
        }
    }

}
