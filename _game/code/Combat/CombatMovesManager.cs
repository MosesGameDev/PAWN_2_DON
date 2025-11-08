using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using NUnit.Framework;

[System.Serializable]
public class CombatMovesManager
{
    [System.Serializable]
    public class CombatMove
    {
        public Sprite Sprite;
        public bool unlocked;
        public string id;
        public string animation_id;
        public string reactionAnimationId;
    }

    [SerializeField] private CombatMove[] combatMoves;

    public CombatMove GetCombatMove(string id)
    {
        foreach (CombatMove move in combatMoves)
        {
            if (move.id == id)
            {
                return move;
            }
        }
        return null;
    }

    public void UnlockCombatMove(string id)
    {
        foreach (CombatMove move in combatMoves)
        {
            if (move.id == id)
            {
                move.unlocked = true;
            }
        }
    }

    public CombatMove[] GetLockedCombatMoves()
    {
        List<CombatMove> list = new List<CombatMove>();

        foreach (CombatMove move in combatMoves)
        {
            if (!move.unlocked)
            {
                list.Add(move);
            }
        }


        return list.ToArray() ;
    }


    public CombatMove[] GetUnlockedCombatMoves()
    {
        List<CombatMove> list = new List<CombatMove>();

        foreach (CombatMove move in combatMoves)
        {
            if (move.unlocked)
            {
                list.Add(move);
            }
        }


        return list.ToArray();
    }

}
