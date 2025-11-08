using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the HUD element for a single crew member, displaying health and status
/// </summary>
public class CrewMemberHUD : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject stunIcon;
    [SerializeField] private GameObject activeIndicator;

    [Header("Animation")]
    [SerializeField] private float healthChangeSpeed = 2f;
    [SerializeField] private CanvasGroup canvasGroup;
    //[SerializeField] private float fadeSpeed = 5f;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1.8f, 0); // Offset above character


    [Header("Transparency Settings")]
    [SerializeField] private float defaultAlpha = 0.25f;
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float transitionSpeed = 4f;

    private Camera mainCamera;
    private RectTransform rectTransform;
    private CrewMember crewMember;
    private float targetHealthFill = 1f;
    private Coroutine healthChangeCoroutine;
    private bool isActive = false;
    private bool isTargeted = false;
    private bool isBehindCamera = false;
    private bool isDead = false;

    public void Initialize(CrewMember member)
    {
        crewMember = member;

        // Subscribe to crew member events
        crewMember.OnHealthChanged += OnHealthChanged;
        crewMember.OnDefeated += OnCrewMemberDefeated;

        // Set initial values
        if (nameText != null)
        {
            nameText.text = crewMember.characterName;
        }



        // Initial health fill
        targetHealthFill = (float)crewMember.currentHealth / crewMember.maxHealth;
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = targetHealthFill;
        }

        // Initialize other elements
        if (stunIcon != null)
        {
            stunIcon.SetActive(crewMember.IsStunned());
        }

        if (activeIndicator != null)
        {
            activeIndicator.SetActive(false); // Initially not active
        }

        // Set initial alpha
        if (canvasGroup != null)
        {
            canvasGroup.alpha = defaultAlpha;
        }

        // Get required components
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (crewMember != null)
        {
            crewMember.OnHealthChanged -= OnHealthChanged;
            crewMember.OnDefeated -= OnCrewMemberDefeated;
        }
    }

    private void Update()
    {
        if (crewMember == null || mainCamera == null) return;

        // Update position to follow crew member
        UpdatePosition();

        // Update stun icon if needed
        if (stunIcon != null && crewMember.IsStunned() != stunIcon.activeSelf)
        {
            stunIcon.SetActive(crewMember.IsStunned());
        }

        // Update transparency based on state
        UpdateTransparency();
    }

    private void UpdatePosition()
    {
        // Convert world position to screen position
        Vector3 targetPosition = crewMember.transform.position + offset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetPosition);

        // Update UI position only if target is in front of camera
        if (screenPos.z > 0)
        {
            rectTransform.position = new Vector3(screenPos.x, screenPos.y, 0);
            isBehindCamera = false;
        }
        else
        {
            // Target is behind camera
            isBehindCamera = true;
        }
    }

    private void UpdateTransparency()
    {
        if (canvasGroup == null || isDead) return;

        // Calculate target alpha based on state
        float targetAlpha = defaultAlpha;

        // If behind camera, always fade out completely
        if (isBehindCamera)
        {
            targetAlpha = 0f;
        }
        // If active or targeted, use active alpha
        else if (isActive || isTargeted)
        {
            targetAlpha = activeAlpha;
        }
        // Otherwise use default alpha
        else
        {
            targetAlpha = defaultAlpha;
        }

        // Smoothly transition to target alpha
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * transitionSpeed);
    }

    /// <summary>
    /// Called when the crew member's health changes
    /// </summary>
    private void OnHealthChanged(int newHealth)
    {
        float newFill = (float)newHealth / crewMember.maxHealth;

        // Only update if there's a meaningful change
        if (Mathf.Abs(newFill - targetHealthFill) > 0.01f)
        {
            // Stop any ongoing health animation
            if (healthChangeCoroutine != null)
            {
                StopCoroutine(healthChangeCoroutine);
            }

            targetHealthFill = newFill;
            healthChangeCoroutine = StartCoroutine(AnimateHealthChange(targetHealthFill));
        }
    }

    /// <summary>
    /// Called when the crew member is defeated
    /// </summary>
    private void OnCrewMemberDefeated(CrewMember member)
    {
        // Animate health to zero
        if (healthChangeCoroutine != null)
        {
            StopCoroutine(healthChangeCoroutine);
        }

        targetHealthFill = 0f;
        healthChangeCoroutine = StartCoroutine(AnimateHealthChange(targetHealthFill));

        // Mark as dead and fade out the HUD
        isDead = true;
        StartCoroutine(FadeOutHUD());
    }

    private IEnumerator AnimateHealthChange(float targetFill)
    {
        float startFill = healthFillImage.fillAmount;
        float elapsed = 0f;
        float duration = Mathf.Abs(startFill - targetFill) * (1f / healthChangeSpeed);

        // Ensure we have at least some animation time
        duration = Mathf.Max(0.1f, duration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentFill = Mathf.Lerp(startFill, targetFill, t);

            // Update fill amount
            healthFillImage.fillAmount = currentFill;


            yield return null;
        }

        // Ensure we reach the target fill exactly
        healthFillImage.fillAmount = targetFill;

        healthChangeCoroutine = null;
    }


    private IEnumerator FadeOutHUD()
    {
        // Wait a moment to show the zero health
        yield return new WaitForSeconds(1.5f);

        // Fade out
        float elapsed = 0f;
        float duration = 1f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Set this crew member as the active character
    /// </summary>
    public void SetActive(bool active)
    {
        isActive = active;

        if (activeIndicator != null)
        {
            activeIndicator.SetActive(active);
        }
    }

    /// <summary>
    /// Set this crew member as being targeted
    /// </summary>
    public void SetTargeted(bool targeted)
    {
        isTargeted = targeted;
    }

    /// <summary>
    /// Force update of health display (for initialization)
    /// </summary>
    public void ForceUpdateHealth()
    {
        if (healthFillImage != null && crewMember != null)
        {
            float fill = (float)crewMember.currentHealth / crewMember.maxHealth;
            healthFillImage.fillAmount = fill;
        }
    }

    /// <summary>
    /// Get the crew member this HUD represents
    /// </summary>
    public CrewMember GetCrewMember()
    {
        return crewMember;
    }
}