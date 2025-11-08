using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using DamageNumbersPro;

public class CrewMember : MonoBehaviour
{
    [Header("Basic Properties")]
    public Sprite characterImage;
    public string characterName;
    public int maxHealth;
    public int currentHealth;
    public int baseDamage;

    [Header("Combat State")]
    private CombatAction currentAction;
    private CrewMember currentTarget;
    private bool isExecutingAction = false;

    [Header("Components")]
    public Animator animator;
    [SerializeField] private AudioSource audioSource;

    [Space]
    [SerializeField] DamageNumber damageNumberPrefab;
    [SerializeField] private ParticleSystem hitFx;

    [Header("Status")]
    [SerializeField] private bool isStunned;

    [Header("Combat Actions")]
    public List<CombatAction> combatActions = new List<CombatAction>();

    public event Action<int> OnHealthChanged;
    public event Action<CrewMember> OnDefeated;
    public event Action<string> OnAnimationEvent;

    public Vector3 bodyPos { get; set; }

    public bool IsAlive => currentHealth > 0;
    public bool IsExecutingAction => isExecutingAction;

    void Start()
    {
        currentHealth = maxHealth;
        bodyPos = transform.localPosition;

        // Make sure we have an audio source
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void Initialize()
    {
        Move();
    }

    #region Movement
    public void Move()
    {
        if (PlayerCharacterController.instance != null)
        {
            if (characterName == "player")
            {
                PlayerCharacterController.instance.Move();

                return;
            }
        }

        animator.CrossFade("root|M_Walk normal", .1f);
    }

    public void Run()
    {
        if (PlayerCharacterController.instance != null)
        {
            if (characterName == "player")
            {
                PlayerCharacterController.instance.Run();
                return;
            }
        }

        animator.CrossFade("root|M_Run", .1f);
    }

    public void JumpIn()
    {
        animator.CrossFade("jump_in", .1f);
    }

    public void JumpOut()
    {
        animator.CrossFade("Dodging Back", .1f);
    }

    public void Stop()
    {
        if (PlayerCharacterController.instance != null)
        {
            if (characterName == "player")
            {
                PlayerCharacterController.instance.StopMoving();
                return;
            }
        }

        animator.CrossFade("root|M_Idle standing", .1f);
    }

    public void SetCombatIdle()
    {
        animator.CrossFade("combat_Idle", .1f);

    }
    #endregion

    #region Combat
    public void OnStartFight()
    {

        animator.CrossFade("combat_Idle", .1f);
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        damageNumberPrefab.Spawn(pos, $"-{damage}");
        hitFx.Play();
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void PrepareAction(CombatAction action, CrewMember target)
    {
        currentAction = action;
        currentTarget = target;
        isExecutingAction = true;
    }

    public void ClearAction()
    {
        currentAction = null;
        currentTarget = null;
        isExecutingAction = false;
    }

    private void Die()
    {
        // Play death animation
        animator.CrossFade("death", .1f);

        // Invoke defeated event
        OnDefeated?.Invoke(this);
    }

    public void Stun()
    {
        isStunned = true;

        // Play dizzy animation
        animator.CrossFade("Dizzy", .1f);
    }

    public void RemoveStun()
    {
        isStunned = false;

        // Return to combat idle
        animator.CrossFade("combat_Idle", .1f);
    }

    public bool IsStunned()
    {
        return isStunned;
    }
    #endregion

    #region Animation Events
    // These methods are called from animation events

    // Called when an attack animation reaches the hit frame
    public void OnAttackHit()
    {
        if (currentAction != null && currentTarget != null)
        {
            // Calculate damage
            int damage = currentAction.CalculateDamage(baseDamage);

            // Check if this hit will be fatal
            bool willBeFatal = currentTarget.currentHealth <= damage;

            // Apply damage
            currentTarget.TakeDamage(damage);

            // Trigger hit reaction on target
            string hitReactionAnim = currentAction.GetHitReactionAnimation(willBeFatal);
            currentTarget.animator.CrossFade(hitReactionAnim, 0.1f);

            // Play hit sound if available
            if (currentAction.hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(currentAction.hitSound);
            }

            // Apply knockback if enabled
            if (currentAction.knocksBack)
            {
                Vector3 knockbackDirection = (currentTarget.transform.position - transform.position).normalized;
                currentTarget.transform.DOMove(
                    currentTarget.transform.position + knockbackDirection * currentAction.knockbackForce,
                    0.2f
                ).SetEase(Ease.OutQuad);
            }

            // Apply stun effect if applicable
            if (currentAction.canStun && UnityEngine.Random.Range(1, 101) <= currentAction.stunChance)
            {
                currentTarget.Stun();
            }
        }

        // Notify system that hit has occurred
        OnAnimationEvent?.Invoke("hit");
    }

    // Called when a ranged attack animation reaches the projectile launch frame
    public void OnProjectileLaunch()
    {
        if (currentAction != null && currentTarget != null && currentAction is RangedCombatAction rangedAction)
        {
            if (rangedAction.projectilePrefab != null)
            {
                // Get spawn position
                Transform spawnPoint = rangedAction.projectileSpawnPoint;
                if (spawnPoint == null)
                {
                    // If no specific spawn point, use a default position
                    spawnPoint = transform;
                }

                // Instantiate projectile
                GameObject projectile = Instantiate(rangedAction.projectilePrefab,
                    spawnPoint.position,
                    Quaternion.LookRotation(currentTarget.transform.position - spawnPoint.position));

                // Set up projectile movement and behavior
                ProjectileBehavior behavior = projectile.GetComponent<ProjectileBehavior>();
                if (behavior != null)
                {
                    behavior.Initialize(rangedAction, this, currentTarget);
                }
                else
                {
                    // If no behavior component, just move it with DOTween
                    float travelTime = Vector3.Distance(spawnPoint.position, currentTarget.transform.position) / rangedAction.projectileSpeed;
                    projectile.transform.DOMove(currentTarget.transform.position, travelTime)
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            if (projectile != null)
                            {
                                Destroy(projectile);
                            }
                        });

                    // Schedule the hit to occur when projectile arrives
                    StartCoroutine(DelayedProjectileHit(rangedAction, currentTarget, travelTime));
                }
            }
        }

        OnAnimationEvent?.Invoke("projectileLaunch");
    }

    private System.Collections.IEnumerator DelayedProjectileHit(RangedCombatAction action, CrewMember target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target != null && target.IsAlive)
        {
            // Calculate damage
            int damage = action.CalculateDamage(baseDamage);

            // Check if this hit will be fatal
            bool willBeFatal = target.currentHealth <= damage;

            // Apply damage
            target.TakeDamage(damage);

            // Trigger hit reaction on target
            string hitReactionAnim = action.GetHitReactionAnimation(willBeFatal);
            target.animator.CrossFade(hitReactionAnim, 0.1f);

            // Play hit sound if available
            if (action.hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(action.hitSound);
            }

            // Create impact effect if enabled
            if (action.createImpactEffect && action.impactEffectPrefab != null)
            {
                GameObject impact = Instantiate(action.impactEffectPrefab,
                    target.transform.position + Vector3.up * 1f,
                    Quaternion.identity);
                Destroy(impact, 2f);
            }

            // Apply knockback if enabled
            if (action.knocksBack)
            {
                Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;
                target.transform.DOMove(
                    target.transform.position + knockbackDirection * action.knockbackForce,
                    0.2f
                ).SetEase(Ease.OutQuad);
            }

            // Apply stun effect if applicable
            if (action.canStun && UnityEngine.Random.Range(1, 101) <= action.stunChance)
            {
                target.Stun();
            }
        }
    }

    // Called when an attack animation is complete
    public void OnAttackComplete()
    {
        OnAnimationEvent?.Invoke("attackComplete");
    }

    // Play attack sound (called from animation event)
    public void PlayAttackSound()
    {
        if (currentAction != null && currentAction.attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(currentAction.attackSound);
        }
    }
    #endregion
}