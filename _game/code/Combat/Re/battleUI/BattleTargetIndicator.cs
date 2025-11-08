using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the targeting UI for the current attack target
/// </summary>
public class BattleTargetIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CombatHUDManager hudManager; // Reference to the HUD manager
    [SerializeField] private TargetUIElement targetUIElement;
    [SerializeField] private Image targetImage;
    [SerializeField] private Animator targetAnimator;

    [Header("Appearance")]
    [SerializeField] private Color meleeTargetColor = new Color(1f, 0.5f, 0.5f, 0.8f); // Reddish for melee attacks
    [SerializeField] private Color rangedTargetColor = new Color(0.5f, 0.5f, 1f, 0.8f); // Bluish for ranged attacks

    private static BattleTargetIndicator _instance;
    public static BattleTargetIndicator Instance { get { return _instance; } }

    private CrewMember currentTarget;
    private CrewMemberHUD currentTargetHUD;

    private void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;

        // Create the TargetUIElement component if it doesn't exist
        if (targetUIElement == null)
        {
            targetUIElement = GetComponent<TargetUIElement>();
            if (targetUIElement == null)
            {
                targetUIElement = gameObject.AddComponent<TargetUIElement>();
            }
        }

        // Find or create the target image
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        // Find the animator if not assigned
        if (targetAnimator == null)
        {
            targetAnimator = GetComponent<Animator>();
        }

        // Hide the indicator initially
        ClearTarget();

        // Find HUD manager if not assigned
        if (hudManager == null)
        {
            hudManager = FindObjectOfType<CombatHUDManager>();
        }
    }

    /// <summary>
    /// Set a target and specify if it's a close combat (melee) or ranged attack target
    /// </summary>
    /// <param name="target">The target Transform to follow</param>
    /// <param name="isCloseCombat">True for melee attack, false for ranged attack</param>
    public void SetTarget(Transform target, bool isCloseCombat)
    {
        if (target == null)
        {
            ClearTarget();
            return;
        }

        // Clear previous target's HUD state
        ClearTargetHUD();

        // Get the CrewMember from the target
        currentTarget = target.GetComponent<CrewMember>();

        // Update the HUD transparency if possible
        if (hudManager != null && currentTarget != null)
        {
            currentTargetHUD = hudManager.GetHUDForCrewMember(currentTarget);
            if (currentTargetHUD != null)
            {
                currentTargetHUD.SetTargeted(true);
            }
        }

        // Update the target tracker
        targetUIElement.SetTarget(target);

        // Update color based on attack type
        if (targetImage != null)
        {
            targetImage.color = isCloseCombat ? meleeTargetColor : rangedTargetColor;
        }

        // Play animation if available
        if (targetAnimator != null)
        {
            targetAnimator.SetTrigger("Show");
            targetAnimator.SetBool("IsCloseCombat", isCloseCombat);
        }

        // Enable the game object
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Clear the current target and hide the indicator
    /// </summary>
    public void ClearTarget()
    {
        targetUIElement.ClearTarget();

        // Clear the target HUD state
        ClearTargetHUD();

        // Play hide animation if available
        if (targetAnimator != null)
        {
            targetAnimator.SetTrigger("Hide");
        }
    }

    private void ClearTargetHUD()
    {
        if (currentTarget != null && currentTargetHUD != null)
        {
            currentTargetHUD.SetTargeted(false);
            currentTargetHUD = null;
            currentTarget = null;
        }
    }
}