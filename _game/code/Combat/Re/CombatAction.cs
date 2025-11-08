using UnityEngine;

[System.Serializable]
public class CombatAction
{
    public enum ActionType
    {
        CloseCombat,
        RangedCombat
    }

    [Header("Basic Properties")]
    public string actionName;
    public ActionType type;
    public int damageMultiplier = 100; // Percentage, 100 = normal damage

    [Header("Animation")]
    public string animationTrigger;
    public float animationDuration = 1f;
    public float hitFramePercent = 0.5f; // When in the animation (percentage) the hit occurs

    [Header("Hit Reaction")]
    public string hitReactionAnimation = "hit"; // Default hit animation
    public float hitReactionDuration = 0.5f;
    public bool useImpactAnimation = false; // Whether to use special impact animations
    public string impactAnimation = ""; // Special impact animation when hit is critical
    public string deathImpactAnimation = ""; // Special impact animation when hit causes death

    [Header("Special Effects")]
    public bool canStun;
    public int stunChance = 10; // Percentage
    public bool knocksBack = false;
    public float knockbackForce = 2f;

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip hitSound;

    public int CalculateDamage(int baseDamage)
    {
        return Mathf.RoundToInt(baseDamage * (damageMultiplier / 100f));
    }

    /// <summary>
    /// Get the appropriate hit reaction animation for the target
    /// </summary>
    /// <param name="isFatal">Whether this hit will be fatal</param>
    /// <returns>Animation name to play on the target</returns>
    public string GetHitReactionAnimation(bool isFatal)
    {
        // If this is a fatal hit and we have a death impact animation, use it
        if (isFatal && useImpactAnimation && !string.IsNullOrEmpty(deathImpactAnimation))
        {
            return deathImpactAnimation;
        }
        // Otherwise if we're using special impact animations, use the regular impact
        else if (useImpactAnimation && !string.IsNullOrEmpty(impactAnimation))
        {
            return impactAnimation;
        }
        // Fall back to standard hit animation
        else
        {
            return hitReactionAnimation;
        }
    }
}

[System.Serializable]
public class CloseCombatAction : CombatAction
{
    [Header("Close Combat Properties")]
    public float dashSpeed = 5f;
    public float attackRange = 2.3f;

    public CloseCombatAction()
    {
        type = ActionType.CloseCombat;
    }
}

[System.Serializable]
public class RangedCombatAction : CombatAction
{
    [Header("Ranged Combat Properties")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public Transform projectileSpawnPoint;
    public bool createImpactEffect = false;
    public GameObject impactEffectPrefab;

    public RangedCombatAction()
    {
        type = ActionType.RangedCombat;
    }
}