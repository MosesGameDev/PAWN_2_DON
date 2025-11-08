using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private Button closeAttackButton;
    [SerializeField] private Button rangedAttackButton;
    [SerializeField] private Button autoWinButton;
    [SerializeField] private Button autoSkipButton;
    [SerializeField] private Button restartButton;

    [Space]
    [SerializeField] private TargetCrewMemberButton[] targetButtons;

    [Space]
    public TargetCrewMemberButton[] targetCrewMemberButtons;
    public RectTransform targetButtonContentHolder;

    [SerializeField] private GameObject playerTurnIndicator;
    [SerializeField] private GameObject enemyTurnIndicator;

    private BattleController battleController;
    private TeamManager enemyTeam;
    private List<Button> enemyTargetButtons = new List<Button>();

    private bool selectingTarget = false;
    private bool isCloseAttack = false;

    private void OnEnable()
    {
        BattleController.OnBattleStateChanged += OnBattleStateChanged;
    }

    void Start()
    {
        battleController = BattleController.Instance;

        closeAttackButton.onClick.AddListener(() => StartTargetSelection(true));
        rangedAttackButton.onClick.AddListener(() => StartTargetSelection(false));

        autoWinButton.onClick.AddListener(battleController.AutoWinBattle);
        autoSkipButton.onClick.AddListener(battleController.AutoSkipBattle);
        restartButton.onClick.AddListener(battleController.RestartBattle);

        // Disable attack buttons initially
        closeAttackButton.interactable = false;
        rangedAttackButton.interactable = false;
    }

    private void OnBattleStateChanged(BattleState state)
    {
        switch (state)
        {
            case BattleState.PlayerTurn:
                playerTurnIndicator.SetActive(true);
                enemyTurnIndicator.SetActive(false);

                // Only enable attack buttons if it's the player's character turn
                CrewMember currentTurn = BattleController.Instance.turnManager.GetCurrentTurn();
                bool isPlayerCharacter = currentTurn != null && currentTurn.characterName == "player";

                closeAttackButton.interactable = isPlayerCharacter;
                rangedAttackButton.interactable = isPlayerCharacter;

                break;

            case BattleState.EnemyTurn:
                playerTurnIndicator.SetActive(false);
                enemyTurnIndicator.SetActive(true);
                closeAttackButton.interactable = false;
                rangedAttackButton.interactable = false;
                break;

            case BattleState.Victory:
            case BattleState.Defeat:
                playerTurnIndicator.SetActive(false);
                enemyTurnIndicator.SetActive(false);
                closeAttackButton.interactable = false;
                rangedAttackButton.interactable = false;
                break;
        }
    }

    private void StartTargetSelection(bool closeAttack)
    {
        selectingTarget = true;
        isCloseAttack = closeAttack;

        print("START TARGET SELECTION");

        // Get enemy team from battle controller
        enemyTeam = BattleController.Instance.turnManager.GetWaitingTeam();

        // Create or update target buttons for each alive enemy
        CreateTargetButtons();
    }

    private void CreateTargetButtons()
    {
        // Clear existing buttons
        foreach (var targetButton in targetCrewMemberButtons)
        {
            targetButton.gameObject.SetActive(false);
        }
        enemyTargetButtons.Clear();

        // buttons for each alive enemy
        foreach (var enemy in enemyTeam.GetTeamMembers())
        {
            if (enemy.IsAlive)
            {
                // In a real implementation, you would create UI buttons over the enemies
                // or use a different selection mechanism
                DisplayTargetButton(enemy);
            }
        }
    }


    private void DisplayTargetButton(CrewMember target)
    {
        for (int i = 0; i < targetCrewMemberButtons.Length; i++)
        {
            targetCrewMemberButtons[i].gameObject.SetActive(true);
            targetCrewMemberButtons[i].UpdateUIElement(target);

            targetCrewMemberButtons[i].button.onClick.AddListener(() => {
                battleController.ExecutePlayerAction(target, isCloseAttack);
                EndTargetSelection();
            });
        }
    }

    private void EndTargetSelection()
    {
        selectingTarget = false;

        // Destroy target buttons
        foreach (var button in enemyTargetButtons)
        {
            Destroy(button.gameObject);
        }
        enemyTargetButtons.Clear();
    }

    private void OnDestroy()
    {
        if (battleController != null)
        {
            BattleController.OnBattleStateChanged -= OnBattleStateChanged;
        }
    }

    private void OnDisable()
    {
        BattleController.OnBattleStateChanged -= OnBattleStateChanged;
    }
}