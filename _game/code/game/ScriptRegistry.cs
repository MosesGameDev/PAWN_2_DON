using Buildings;
using Sirenix.OdinInspector;
using System;
using System.ComponentModel;
using Unity.Cinemachine;
using UnityEngine;

public class ScriptRegistry : MonoBehaviour
{
    public static ScriptRegistry Instance { get; private set; }

    [Title("Camera References")]
    public CinemachineCamera gameCam;
    public CinemachineCamera customizationCamera;

    [Title("Script References")]
    public InitializationManager initializationManager;
    public TextGameUIController textGameController;
    public UIDialoguesManager uiDialoguesManager;
    public ScreenFade screenFade;
    public DeliveryMinigameLoader deliveryMinigameLoader;
    public CutSceneManager cutSceneManager;
    public PlayerCharacterController player;
    public WalkingSimulator walkingSimulator;
    public HomeScreenManager homeScreenManager;
    public DayCompleteHandler dayCompleteHandler;
    public CharacterCustomizationHandler characterCustomizationHandler;
    public FeelVibrationManager feelVibrationManager;
    public RewardedVideoPanel rewardedVideoPanel;

    [Space]


    [Space]
    public HungerHandler hungerHandler;
    public BusinessHandler businessHandler;
    public CharacterSelectionHandler characterSelectionHandler;
    public GameOverScreen gameOverScreen;

    public static event Action<PlayerDeliveryController> onControllerAssigned;
    public static event Action<int> onDayCompleted;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnDeliveryControllerAssigned(PlayerDeliveryController _playerDeliveryController)
    {
        onControllerAssigned?.Invoke(_playerDeliveryController);
    }
}
