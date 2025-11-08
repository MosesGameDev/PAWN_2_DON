using UnityEngine;
using DG.Tweening;

/// <summary>
/// Handles projectile movement and hit effects for ranged attacks
/// </summary>
public class ProjectileBehavior : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private AudioSource audioSource;

    [Header("Runtime Data")]
    private RangedCombatAction action;
    private CrewMember sender;
    private CrewMember target;
    private bool hasHit = false;

    private void Awake()
    {
        // Get references if not assigned
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void Initialize(RangedCombatAction action, CrewMember sender, CrewMember target)
    {
        this.action = action;
        this.sender = sender;
        this.target = target;

        // Set up movement
        Vector3 direction = (target.transform.position - transform.position).normalized;

        if (rb != null)
        {
            // Use physics-based movement
            rb.linearVelocity = direction * action.projectileSpeed;
        }
        else
        {
            // Use DOTween for movement
            float distance = Vector3.Distance(transform.position, target.transform.position);
            float travelTime = distance / action.projectileSpeed;

            transform.DOMove(target.transform.position, travelTime)
                .SetEase(Ease.Linear)
                .OnComplete(() => OnProjectileReachedTarget());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // Check if we hit the target
        CrewMember hitMember = other.GetComponent<CrewMember>();
        if (hitMember != null && hitMember == target)
        {
            OnProjectileHit(hitMember);
        }
        else if (other.CompareTag("Environment"))
        {
            // Hit environment, destroy projectile
            SpawnImpactEffect(other.transform.position);
            Destroy(gameObject);
        }
    }

    private void OnProjectileReachedTarget()
    {
        if (hasHit) return;

        if (target != null && target.IsAlive)
        {
            OnProjectileHit(target);
        }

        // If we somehow didn't hit anything, just destroy the projectile
        Destroy(gameObject);
    }

    private void OnProjectileHit(CrewMember hitTarget)
    {
        hasHit = true;

        // Calculate damage
        int damage = action.CalculateDamage(sender.baseDamage);

        // Check if this hit will be fatal
        bool willBeFatal = hitTarget.currentHealth <= damage;

        // Apply damage
        hitTarget.TakeDamage(damage);

        // Trigger hit reaction on target
        string hitReactionAnim = action.GetHitReactionAnimation(willBeFatal);
        hitTarget.animator.CrossFade(hitReactionAnim, 0.1f);

        // Play hit sound
        if (action.hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(action.hitSound);
        }

        // Spawn impact effect
        SpawnImpactEffect(hitTarget.transform.position + Vector3.up * 1f);

        // Apply knockback if enabled
        if (action.knocksBack)
        {
            Vector3 knockbackDirection = (hitTarget.transform.position - transform.position).normalized;
            hitTarget.transform.DOMove(
                hitTarget.transform.position + knockbackDirection * action.knockbackForce,
                0.2f
            ).SetEase(Ease.OutQuad);
        }

        // Apply stun effect if applicable
        if (action.canStun && Random.Range(1, 101) <= action.stunChance)
        {
            hitTarget.Stun();
        }

        // Destroy projectile
        Destroy(gameObject);
    }

    private void SpawnImpactEffect(Vector3 position)
    {
        if (action.createImpactEffect && action.impactEffectPrefab != null)
        {
            GameObject impact = Instantiate(action.impactEffectPrefab, position, Quaternion.identity);
            Destroy(impact, 2f);
        }
    }
}