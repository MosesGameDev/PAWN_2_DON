using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all HUD elements for a battle, creating and tracking health displays for all crew members
/// </summary>
public class CombatHUDManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BattleController battleController;
    [SerializeField] private Canvas hudCanvas;

    [Header("Prefabs")]
    [SerializeField] private GameObject crewMemberHUDPrefab;

    [Header("Settings")]
    [SerializeField] private bool hideHUDOutsideBattle = true;

    [Header("Teams")]
    [SerializeField] private TeamManager playerTeam;
    [SerializeField] private TeamManager enemyTeam;


    private Dictionary<CrewMember, CrewMemberHUD> hudElements = new Dictionary<CrewMember, CrewMemberHUD>();
    private TurnManager turnManager;
    private bool battleActive = false;
    private CrewMember currentActiveMember;

    private void Awake()
    {

        // Create hud canvas if not assigned
        if (hudCanvas == null)
        {
            // Try to find canvas
            hudCanvas = GetComponentInChildren<Canvas>();

            if (hudCanvas == null)
            {
                // Create new canvas
                GameObject canvasObj = new GameObject("HUD Canvas");
                canvasObj.transform.SetParent(transform);
                hudCanvas = canvasObj.AddComponent<Canvas>();
                hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                // Add required components
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
        }

        // Get turn manager reference
        turnManager = battleController.GetComponent<TurnManager>();
        if (turnManager == null)
        {
            Debug.LogWarning("CombatHUDManager: No TurnManager found on BattleController!");
        }
    }

    private void Start()
    {
        // Subscribe to battle events
        BattleController.OnBattleStateChanged += OnBattleStateChanged;

        // Initialize HUD elements if battle is already in progress
        if (battleController.gameObject.activeSelf)
        {
            InitializeHUDElements();
        }

        // Find BattleTargetIndicator and set it up to use this HUD Manager
        BattleTargetIndicator targetIndicator = FindObjectOfType<BattleTargetIndicator>();
        if (targetIndicator != null)
        {
            // You could set up a reference here if needed
        }
    }

    private void OnDestroy()
    {
        if (battleController != null)
        {
            BattleController.OnBattleStateChanged -= OnBattleStateChanged;
        }
    }

    private void Update()
    {
        if (!battleActive || turnManager == null) return;

        // Update active indicator for current turn
        CrewMember currentTurn = turnManager.GetCurrentTurn();

        // Only update if the active member has changed
        if (currentTurn != currentActiveMember)
        {
            // Clear previous active member
            if (currentActiveMember != null && hudElements.ContainsKey(currentActiveMember))
            {
                hudElements[currentActiveMember].SetActive(false);
            }

            // Set new active member
            if (currentTurn != null && hudElements.ContainsKey(currentTurn))
            {
                hudElements[currentTurn].SetActive(true);
                currentActiveMember = currentTurn;
            }
        }
    }

    /// <summary>
    /// Creates HUD elements for all crew members in the battle
    /// </summary>
    private void InitializeHUDElements()
    {
        if (battleController == null || crewMemberHUDPrefab == null) return;

        // Clear any existing HUD elements
        ClearHUDElements();


        // Create HUD elements for player team
        foreach (CrewMember member in playerTeam.GetTeamMembers())
        {
            if (member != null)
            {
                CreateHUDForCrewMember(member);
            }
        }

        // Create HUD elements for enemy team
        foreach (CrewMember member in enemyTeam.GetTeamMembers())
        {
            if (member != null)
            {
                CreateHUDForCrewMember(member);
            }
        }

        battleActive = true;
        currentActiveMember = null;
    }

    /// <summary>
    /// Creates a HUD element for a specific crew member
    /// </summary>
    private void CreateHUDForCrewMember(CrewMember member)
    {
        if (member == null || hudElements.ContainsKey(member)) return;

        // Instantiate HUD prefab
        GameObject hudObj = Instantiate(crewMemberHUDPrefab, hudCanvas.transform);
        CrewMemberHUD hudElement = hudObj.GetComponent<CrewMemberHUD>();

        if (hudElement == null)
        {
            Debug.LogError("CombatHUDManager: HUD prefab does not have CrewMemberHUD component!");
            Destroy(hudObj);
            return;
        }

        // Initialize HUD element
        hudElement.Initialize(member);
        hudElements.Add(member, hudElement);
    }

    /// <summary>
    /// Clears all HUD elements
    /// </summary>
    private void ClearHUDElements()
    {
        foreach (var pair in hudElements)
        {
            if (pair.Value != null)
            {
                Destroy(pair.Value.gameObject);
            }
        }

        hudElements.Clear();
        currentActiveMember = null;
    }

    /// <summary>
    /// Responds to battle state changes
    /// </summary>
    private void OnBattleStateChanged(BattleState state)
    {
        switch (state)
        {
            case BattleState.Start:
                InitializeHUDElements();
                break;

            case BattleState.Victory:
            case BattleState.Defeat:
                if (hideHUDOutsideBattle)
                {
                    // Wait a moment before hiding HUD
                    Invoke("HideHUD", 3f);
                }
                battleActive = false;
                break;
        }
    }

    private void HideHUD()
    {
        ClearHUDElements();
    }

    /// <summary>
    /// Gets the HUD element for a specific crew member
    /// </summary>
    public CrewMemberHUD GetHUDForCrewMember(CrewMember member)
    {
        if (member != null && hudElements.ContainsKey(member))
        {
            return hudElements[member];
        }
        return null;
    }
}