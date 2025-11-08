using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;
using DamageNumbersPro;

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat,
    Paused
}

public class BattleController : MonoBehaviour
{
    public static BattleController Instance { get; private set; }

    [Header("Team Configurations")]
    [SerializeField] private TeamManager playerTeam;
    [SerializeField] private TeamManager enemyTeam;

    [Header("Battle Settings")]
    [SerializeField] private float turnDelay = 0.5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackDistance = 2.3f;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera defaultCamera;
    [SerializeField] private CinemachineCamera playerTeamCamera;
    [SerializeField] private CinemachineCamera enemyTeamCamera;

    [Header("UI References")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private BattleTargetIndicator targetIndicator;

    public TurnManager turnManager { get; private set; }
    private BattleState currentState;
    private bool isBattleActive = false;

    public static event Action<BattleState> OnBattleStateChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        turnManager = GetComponent<TurnManager>();
        if (turnManager == null)
        {
            turnManager = gameObject.AddComponent<TurnManager>();
        }
    }

    void Start()
    {
        StartBattle();
    }

    public void StartBattle()
    {
        defaultCamera.Prioritize();
        currentState = BattleState.Start;
        OnBattleStateChanged?.Invoke(currentState);

        // Hide UI panels
        if (victoryPanel) victoryPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);

        // Initialize teams
        playerTeam.InitializeTeam();
        enemyTeam.InitializeTeam();

        // Initialize combat actions for all crew members
        foreach (var member in playerTeam.GetTeamMembers())
        {
            InitializeDefaultCombatActions(member);
            member.OnDefeated += OnCrewMemberDefeated;
        }

        foreach (var member in enemyTeam.GetTeamMembers())
        {
            InitializeDefaultCombatActions(member);
            member.OnDefeated += OnCrewMemberDefeated;
        }

        // Random team starts
        bool playerStarts = UnityEngine.Random.Range(0, 2) == 0;

        // Initialize turn manager with both teams
        turnManager.InitializeTurns(playerTeam, enemyTeam, playerStarts);

        // Set all crew members to combat idle
        foreach (var member in playerTeam.GetTeamMembers())
        {
            if (member.IsAlive) member.OnStartFight();
        }

        foreach (var member in enemyTeam.GetTeamMembers())
        {
            if (member.IsAlive) member.OnStartFight();
        }

        isBattleActive = true;
        StartNextTurn();
    }

    // Initialize default combat actions if none are specified
    private void InitializeDefaultCombatActions(CrewMember member)
    {
        // Only initialize if no actions are defined
        if (member.combatActions == null || member.combatActions.Count == 0)
        {
            member.combatActions = new List<CombatAction>();

            // Add default close combat actions
            CloseCombatAction punch = new CloseCombatAction()
            {
                actionName = "Punch",
                animationTrigger = "punch",
                damageMultiplier = 100,
                animationDuration = 1.0f,
                hitFramePercent = 0.4f,
                attackRange = 2.3f,
                dashSpeed = 2.0f,
                // Hit reactions
                hitReactionAnimation = "hit",
                useImpactAnimation = false
            };

            CloseCombatAction kick = new CloseCombatAction()
            {
                actionName = "Kick",
                animationTrigger = "kick_0",
                damageMultiplier = 120,
                animationDuration = 1.2f,
                hitFramePercent = 0.5f,
                attackRange = 2.3f,
                dashSpeed = 2.0f,
                // Hit reactions
                hitReactionAnimation = "hit",
                useImpactAnimation = false,
                knocksBack = true,
                knockbackForce = 1.0f
            };

            CloseCombatAction punchKickCombo = new CloseCombatAction()
            {
                actionName = "Punch-Kick Combo",
                animationTrigger = "combat_punch_Kick",
                damageMultiplier = 150,
                animationDuration = 1.5f,
                hitFramePercent = 0.6f,
                canStun = true,
                stunChance = 20,
                attackRange = 2.3f,
                dashSpeed = 2.0f,
                // Hit reactions
                hitReactionAnimation = "hit",
                useImpactAnimation = true,
                impactAnimation = "combat_punch_Kick_Impact",
                deathImpactAnimation = "combat_punch_Kick_Impact_death",
                knocksBack = true,
                knockbackForce = 1.5f
            };

            CloseCombatAction hookKick = new CloseCombatAction()
            {
                actionName = "Hook Kick",
                animationTrigger = "combat_hook_kick",
                damageMultiplier = 140,
                animationDuration = 1.3f,
                hitFramePercent = 0.55f,
                canStun = true,
                stunChance = 15,
                attackRange = 2.3f,
                dashSpeed = 2.0f,
                // Hit reactions
                hitReactionAnimation = "hit",
                useImpactAnimation = true,
                impactAnimation = "combat_hook_kick_Impact",
                deathImpactAnimation = "combat_hook_kick_Impact_death",
                knocksBack = true,
                knockbackForce = 1.2f
            };

            CloseCombatAction kickCombo = new CloseCombatAction()
            {
                actionName = "Kick Combo",
                animationTrigger = "combat_kickCombo",
                damageMultiplier = 160,
                animationDuration = 1.7f,
                hitFramePercent = 0.65f,
                canStun = true,
                stunChance = 25,
                attackRange = 2.3f,
                dashSpeed = 2.0f,
                // Hit reactions
                hitReactionAnimation = "hit",
                useImpactAnimation = true,
                impactAnimation = "combat_kickCombo_Impact",
                deathImpactAnimation = "combat_kickCombo_Impact_death",
                knocksBack = true,
                knockbackForce = 2.0f
            };

            // Add default ranged combat action
            RangedCombatAction rangedAttack = new RangedCombatAction()
            {
                actionName = "Ranged Attack",
                animationTrigger = "combat_rangedAttack",
                damageMultiplier = 80,
                animationDuration = 1.0f,
                hitFramePercent = 0.7f,
                projectileSpeed = 10.0f,
                // Hit reactions
                hitReactionAnimation = "hit",
                useImpactAnimation = false,
                createImpactEffect = true
            };

            // Add actions to the character
            member.combatActions.Add(punch);
            member.combatActions.Add(kick);
            member.combatActions.Add(punchKickCombo);
            member.combatActions.Add(hookKick);
            member.combatActions.Add(kickCombo);
            member.combatActions.Add(rangedAttack);
        }
    }

    public void RestartBattle()
    {
        // Reset health of all team members
        foreach (var member in playerTeam.GetTeamMembers())
        {
            member.currentHealth = member.maxHealth;
        }

        foreach (var member in enemyTeam.GetTeamMembers())
        {
            member.currentHealth = member.maxHealth;
        }

        // Reset positions
        playerTeam.ResetPositions();
        enemyTeam.ResetPositions();

        StartBattle();
    }

    public void AutoWinBattle()
    {
        // Defeat all enemy team members
        foreach (var member in enemyTeam.GetTeamMembers())
        {
            if (member.IsAlive)
            {
                member.TakeDamage(member.currentHealth);
            }
        }
    }

    public void AutoSkipBattle()
    {
        // Determine winner randomly with slight player advantage
        bool playerWins = UnityEngine.Random.Range(0, 10) > 4; // 60% chance for player

        if (playerWins)
        {
            AutoWinBattle();
        }
        else
        {
            // Defeat all player team members
            foreach (var member in playerTeam.GetTeamMembers())
            {
                if (member.IsAlive)
                {
                    member.TakeDamage(member.currentHealth);
                }
            }
        }
    }

    private void StartNextTurn()
    {
        if (!isBattleActive) return;

        // Get the next crew member whose turn it is
        CrewMember currentMember = turnManager.GetNextTurn();

        if (currentMember == null)
        {
            Debug.LogError("No active crew members found for the current turn!");
            return;
        }

        // Set battle state based on whose turn it is
        if (System.Array.Exists(playerTeam.GetTeamMembers(), member => member == currentMember))
        {
            currentState = BattleState.PlayerTurn;
            enemyTeamCamera.Target.TrackingTarget = currentMember.transform;
            StartCoroutine(PrioritizeEnemyCamera());

            OnBattleStateChanged?.Invoke(currentState);

            // AI will handle NPC turns, for player we wait for input
            if (currentMember.characterName == "player")
            {
                OnBattleStateChanged?.Invoke(currentState);

                // Wait for player input through UI or controls
                // This will be handled through player input system
            }
            else
            {
                // For non-player characters on player team, use AI
                StartCoroutine(ExecuteAITurn(currentMember));
            }
        }
        else
        {
            currentState = BattleState.EnemyTurn;
            OnBattleStateChanged?.Invoke(currentState);

            StartCoroutine(ExecuteAITurn(currentMember));
            playerTeamCamera.Target.TrackingTarget = currentMember.transform;

            StartCoroutine(PrioritizePlayerCamera());
        }
    }

    private IEnumerator ExecuteAITurn(CrewMember member)
    {
        yield return new WaitForSeconds(turnDelay);

        // Determine target
        TeamManager opposingTeam = System.Array.Exists(playerTeam.GetTeamMembers(), m => m == member) ? enemyTeam : playerTeam;
        CrewMember target = GetRandomAliveTarget(opposingTeam);

        if (target == null)
        {
            // No valid targets, end turn
            EndTurn();
            yield break;
        }

        // Determine attack type - check if character has both types of actions
        bool hasCloseActions = HasActionOfType(member, true);
        bool hasRangedActions = HasActionOfType(member, false);

        bool isCloseAttack = true;

        // If character has both types, randomly choose (70% close, 30% ranged)
        if (hasCloseActions && hasRangedActions)
        {
            isCloseAttack = UnityEngine.Random.Range(0, 10) < 7;
        }
        // If character only has one type, use that
        else if (hasCloseActions)
        {
            isCloseAttack = true;
        }
        else if (hasRangedActions)
        {
            isCloseAttack = false;
        }
        // If no actions defined, default to close attack
        else
        {
            isCloseAttack = true;
        }

        if (isCloseAttack)
        {
            yield return StartCoroutine(ExecuteCloseAttack(member, target));
        }
        else
        {
            yield return StartCoroutine(ExecuteRangedAttack(member, target));
        }


        // End turn after attack
        EndTurn();
    }

    // Helper method to check if a crew member has a specific type of action
    private bool HasActionOfType(CrewMember member, bool isCloseCombat)
    {
        foreach (var action in member.combatActions)
        {
            if ((isCloseCombat && action.type == CombatAction.ActionType.CloseCombat) ||
                (!isCloseCombat && action.type == CombatAction.ActionType.RangedCombat))
            {
                return true;
            }
        }
        return false;
    }

    // Select a combat action from a crew member's list
    private CombatAction SelectCombatAction(CrewMember attacker, bool isCloseCombat)
    {
        // Filter actions by type
        List<CombatAction> availableActions = new List<CombatAction>();

        foreach (var action in attacker.combatActions)
        {
            if ((isCloseCombat && action.type == CombatAction.ActionType.CloseCombat) ||
                (!isCloseCombat && action.type == CombatAction.ActionType.RangedCombat))
            {
                availableActions.Add(action);
            }
        }

        // If no matching actions found, return null
        if (availableActions.Count == 0)
        {
            Debug.LogWarning($"No {(isCloseCombat ? "close combat" : "ranged")} actions found for {attacker.characterName}");
            return null;
        }

        // Select a random action from available ones
        return availableActions[UnityEngine.Random.Range(0, availableActions.Count)];
    }

    private IEnumerator ExecuteCloseAttack(CrewMember attacker, CrewMember target)
    {
        // Show target indicator
        if (targetIndicator != null)
        {
            targetIndicator.SetTarget(target.transform, true);
        }

        // Select a close combat action
        CombatAction selectedAction = SelectCombatAction(attacker, true);

        // If no action found, use a default
        if (selectedAction == null)
        {
            // Default values
            selectedAction = new CloseCombatAction()
            {
                actionName = "Default Punch",
                animationTrigger = "punch",
                hitReactionAnimation = "hit",
                damageMultiplier = 100,
                animationDuration = 1.0f
            };
        }

        // Get specific properties for close combat if available
        float attackRange = attackDistance; // Default
        float dashSpeed = moveSpeed; // Default

        if (selectedAction is CloseCombatAction closeCombatAction)
        {
            attackRange = closeCombatAction.attackRange;
            dashSpeed = closeCombatAction.dashSpeed;
        }

        // Store original position
        Vector3 originalPosition = attacker.transform.position;

        // Calculate position to attack from
        Vector3 direction = (target.transform.position - attacker.transform.position).normalized;
        Vector3 attackPosition = target.transform.position - (direction * attackRange);

        // Move to attack position
        attacker.Run();
        attacker.transform.DOMove(attackPosition, dashSpeed).SetEase(Ease.OutQuad);
        attacker.transform.DOLookAt(target.transform.position, 0.2f);


        yield return new WaitForSeconds(dashSpeed);

        // Set the current action and target on the attacker
        attacker.PrepareAction(selectedAction, target);

        // Subscribe to attacker's animation events
        attacker.OnAnimationEvent += OnAttackerAnimationEvent;

        // Play attack animation - hit will be triggered through animation event
        attacker.animator.CrossFade(selectedAction.animationTrigger, 0.1f);

        // Wait for animation to complete
        float animationWaitTime = selectedAction.animationDuration;
        yield return new WaitForSeconds(animationWaitTime);

        // Unsubscribe from animation events
        attacker.OnAnimationEvent -= OnAttackerAnimationEvent;

        // Clear the action
        attacker.ClearAction();

        // Return to original position
        attacker.JumpOut();
        attacker.transform.DOMove(originalPosition, dashSpeed).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(dashSpeed);

        // Return to idle
        attacker.SetCombatIdle();

        // Clear target indicator
        if (targetIndicator != null)
        {
            targetIndicator.ClearTarget();
        }
    }

    // Handle animation events from attackers
    private void OnAttackerAnimationEvent(string eventName)
    {

        // We don't need to do anything here as the CrewMember class now handles the hit logic directly
        // This could be used for additional battle-level effects triggered by animation events

        if (eventName == "attackComplete")
        {
            // Optional: Handle any battle-level effects when attack animations complete
        }
    }

    private IEnumerator ExecuteRangedAttack(CrewMember attacker, CrewMember target)
    {
        // Show target indicator
        if (targetIndicator != null)
        {
            targetIndicator.SetTarget(target.transform, false);
        }

        // Select a ranged combat action
        CombatAction selectedAction = SelectCombatAction(attacker, false);

        // If no action found, use a default
        if (selectedAction == null)
        {
            // Default values
            selectedAction = new RangedCombatAction()
            {
                actionName = "Default Ranged Attack",
                animationTrigger = "combat_rangedAttack",
                hitReactionAnimation = "hit",
                damageMultiplier = 80,
                animationDuration = 1.0f
            };
        }

        // Make sure attacker faces target
        attacker.transform.DOLookAt(target.transform.position, 0.2f);
        yield return new WaitForSeconds(0.2f);

        // Play ranged attack animation
        attacker.animator.CrossFade(selectedAction.animationTrigger, 0.1f);

        // Play attack sound if available
        if (selectedAction.attackSound != null)
        {
            AudioSource.PlayClipAtPoint(selectedAction.attackSound, attacker.transform.position);
        }

        GameObject projectileObj = null;

        // Handle projectile if applicable
        if (selectedAction is RangedCombatAction rangedAction && rangedAction.projectilePrefab != null)
        {
            // Wait until appropriate time in animation to spawn projectile
            yield return new WaitForSeconds(selectedAction.animationDuration * 0.3f);

            // Get spawn position
            Transform spawnPoint = rangedAction.projectileSpawnPoint;
            if (spawnPoint == null)
            {
                // If no specific spawn point, use a default position
                spawnPoint = attacker.transform;
            }

            // Instantiate and configure projectile
            projectileObj = Instantiate(rangedAction.projectilePrefab,
                spawnPoint.position,
                Quaternion.LookRotation(target.transform.position - spawnPoint.position));

            // If projectile has a Rigidbody, add force toward target
            Rigidbody rb = projectileObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = (target.transform.position - spawnPoint.position).normalized * rangedAction.projectileSpeed;
            }
            else
            {
                // Otherwise move it using DOTween
                projectileObj.transform.DOMove(target.transform.position,
                    Vector3.Distance(spawnPoint.position, target.transform.position) / rangedAction.projectileSpeed)
                    .SetEase(Ease.Linear);
            }

            // Wait for projectile to reach target
            float travelTime = Vector3.Distance(spawnPoint.position, target.transform.position) / rangedAction.projectileSpeed;
            yield return new WaitForSeconds(travelTime);

            // Destroy projectile
            if (projectileObj != null)
            {
                Destroy(projectileObj);
            }

            // Create impact effect if enabled
            if (rangedAction.createImpactEffect && rangedAction.impactEffectPrefab != null)
            {
                GameObject impact = Instantiate(rangedAction.impactEffectPrefab,
                    target.transform.position + Vector3.up * 1f,
                    Quaternion.identity);
                Destroy(impact, 2f); // Destroy after 2 seconds
            }
        }
        else
        {
            // No projectile defined, just wait for animation timing
            yield return new WaitForSeconds(selectedAction.animationDuration * 0.5f);
        }

        // Calculate damage
        int damage = selectedAction.CalculateDamage(attacker.baseDamage);

        // Check if this hit will be fatal
        bool willBeFatal = target.currentHealth <= damage;

        // Apply damage
        target.TakeDamage(damage);

        // Play hit reaction on target
        string hitReactionAnim = selectedAction.GetHitReactionAnimation(willBeFatal);
        target.animator.CrossFade(hitReactionAnim, 0.1f);

        // Play hit sound if available
        if (selectedAction.hitSound != null)
        {
            AudioSource.PlayClipAtPoint(selectedAction.hitSound, target.transform.position);
        }

        // Apply knockback if enabled
        if (selectedAction.knocksBack)
        {
            Vector3 knockbackDirection = (target.transform.position - attacker.transform.position).normalized;
            target.transform.DOMove(
                target.transform.position + knockbackDirection * selectedAction.knockbackForce,
                0.2f
            ).SetEase(Ease.OutQuad);
        }

        // Apply stun effect if applicable
        if (selectedAction.canStun && UnityEngine.Random.Range(1, 101) <= selectedAction.stunChance)
        {
            target.Stun();
        }

        // Wait for hit reaction to play
        yield return new WaitForSeconds(0.5f);

        // Return to idle
        attacker.Stop();

        // Clear target indicator
        if (targetIndicator != null)
        {
            targetIndicator.ClearTarget();
        }
    }

    private CrewMember GetRandomAliveTarget(TeamManager team)
    {
        List<CrewMember> aliveMembers = new List<CrewMember>();

        foreach (var member in team.GetTeamMembers())
        {
            if (member.IsAlive)
            {
                aliveMembers.Add(member);
            }
        }

        if (aliveMembers.Count == 0) return null;

        return aliveMembers[UnityEngine.Random.Range(0, aliveMembers.Count)];
    }

    public void ExecutePlayerAction(CrewMember target, bool isCloseAttack)
    {
        if (currentState != BattleState.PlayerTurn) return;

        CrewMember player = turnManager.GetCurrentTurn();
        if (player.characterName != "player") return;

        StartCoroutine(PrioritizeEnemyCamera());

        StartCoroutine(isCloseAttack ?
            ExecuteCloseAttack(player, target) :
            ExecuteRangedAttack(player, target));


        // End player turn after attack animation completes
        StartCoroutine(EndPlayerTurnAfterAnimation());
    }

    private IEnumerator EndPlayerTurnAfterAnimation()
    {
        // Wait for attack animation and movement to complete
        yield return new WaitForSeconds(3f); // Adjust time based on actual animation length
        EndTurn();
    }

    private void EndTurn()
    {
        if (!isBattleActive) return;

        // Advance turn in the turn manager
        bool newTeamTurn = turnManager.AdvanceTurn();

        defaultCamera.Prioritize();

        // If a new team's turn is starting, check if the battle should end
        if (newTeamTurn)
        {
            CheckBattleEnd();
        }

        if (isBattleActive)
        {
            StartNextTurn();
        }
    }

    IEnumerator PrioritizePlayerCamera()
    {
        yield return new WaitForSeconds(2f);
        playerTeamCamera.Prioritize();
    }

    IEnumerator PrioritizeEnemyCamera()
    {
        yield return new WaitForSeconds(2f);
        enemyTeamCamera.Prioritize();
    }

    private void OnCrewMemberDefeated(CrewMember member)
    {
        // Check if the battle should end
        CheckBattleEnd();
    }

    private void CheckBattleEnd()
    {
        bool playerTeamAlive = false;
        bool enemyTeamAlive = false;

        // Check if any player team members are alive
        foreach (var member in playerTeam.GetTeamMembers())
        {
            if (member.IsAlive)
            {
                playerTeamAlive = true;
                break;
            }
        }

        // Check if any enemy team members are alive
        foreach (var member in enemyTeam.GetTeamMembers())
        {
            if (member.IsAlive)
            {
                enemyTeamAlive = true;
                break;
            }
        }

        // Determine battle outcome
        if (!playerTeamAlive)
        {
            EndBattle(false);
        }
        else if (!enemyTeamAlive)
        {
            defaultCamera.Prioritize();
            EndBattle(true);
        }
    }

    private void EndBattle(bool playerVictory)
    {
        isBattleActive = false;

        if (playerVictory)
        {
            currentState = BattleState.Victory;
            if (victoryPanel) victoryPanel.SetActive(true);
        }
        else
        {
            currentState = BattleState.Defeat;
            if (defeatPanel) defeatPanel.SetActive(true);
        }

        OnBattleStateChanged?.Invoke(currentState);
    }
}