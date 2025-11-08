using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;
using Unity.Cinemachine;
using System;
using Sirenix.OdinInspector;
using static CharacterSelectionHandler;
using UnityEngine.Events;

public enum EncounterType
{
    ThugEncounter,
    PoliceEncounter
}

public class PlayerDeliveryController : MonoBehaviour
{
    [Header("Current Path Completion")]
    [Space(20)]
    public float pathCompletion;

    public bool fullScreenCam;

    private void Update()
    {
        if (movementTween != null && movementTween.IsActive())
        {
            pathCompletion = movementTween.ElapsedPercentage();
        }
    }
    [Header("Prop Handler")]
    [Space(20)]
    public PropVisibilityManager GetPropVisibilityManager;

    void PrepPropHandler(Tween tween)
    {
        if (GetPropVisibilityManager != null)
        {
            GetPropVisibilityManager.AcceptTween(tween);
        }
    }


    [Header("Player Move Tutorial")]
    [Space(20)]
    public GameObject playerTapPrompt;

    public bool SetPlayerTapPrompt(bool state)
    {
        if (playerTapPrompt == null) return false;

        playerTapPrompt.SetActive(state);
        return true;
    }

    [Header("Player Encounter Set Up")]
    [Space(20)]

    #region Encounter
    public GameObject moveButtonObj;
    public bool encounterActive;
    public CinemachineCamera encounterCam;

    public GameObject mainCanvas;
    public Vector3 mainCanvasEncounterPos, mainCanvasMovementPos;

    public GameObject EncounterButtonsObj;
    public Button showStrengthButton, foldUnderPressureButton;
    public Text strengthText, foldText;

    public string ThugThreatSentence, PoliceThreatSentence;
    public string ThugLoseSentence, PoliceLoseSentence;

    public void ShowEncounterButtons(bool s)
    {
        EncounterButtonsObj.SetActive(s);
    }

    void UpdateButtonTexts(EncounterType type)
    {
        string intro = "";
        string loseText = "";
        if (type == EncounterType.ThugEncounter)
        {
            intro = ThugThreatSentence; // fight back thug
            loseText = ThugLoseSentence; // lose to thug

        }
        else if (type == EncounterType.PoliceEncounter)
        {
            intro = PoliceThreatSentence; // intimidate cop
            loseText = PoliceLoseSentence; // lose to cop 

        }
        strengthText.text = intro;
        foldText.text = loseText;
    }

    void PrepEncounterButtons()
    {
        if (showStrengthButton != null)
        {
            showStrengthButton.onClick.AddListener(OnShowStrengthClicked);

        }
        if (foldUnderPressureButton != null)
        {
            foldUnderPressureButton.onClick.AddListener(OnFoldUnderPressureClicked);
        }
    }

    public bool HasEnoughStrength()
    {
        return true;
    }

    void OnShowStrengthClicked()
    {

    }

    void OnFoldUnderPressureClicked()
    {

    }

    public void ToggleEncounter(bool s)
    {
        encounterActive = s;
        moveButtonObj.SetActive(s);
        encounterCam.gameObject.SetActive(s);

        if (s)
        {
            OnMovementButtonUp();
        }
        else
        {
            ShowEncounterButtons(s); // false
        }

        EncounterCanvasPositions(s);


    }

    void EncounterCanvasPositions(bool s)
    {
        if (s)
        {
            mainCanvas.transform.localPosition = mainCanvasEncounterPos;
        }
        else
        {
            mainCanvas.transform.localPosition = mainCanvasMovementPos;
        }
    }

    #endregion


    [Header("Player Cam Set Up")]
    [Space(20)]
    public bool initializeOnStart = true;
    public CinemachineCamera playerStartCam;
    bool disableStartCamCondition;


    void DisableStartCam()
    {
        if (!disableStartCamCondition)
        {
            disableStartCamCondition = true;
            playerStartCam.Priority = 0;
        }
    }

    [System.Serializable]
    public class PlayerModel
    {
        public Transform transform;
        public GameObject model;
        public Animator animator;

        public RotationLooper GetRotationLooper;
    }

    private void Awake()
    {
        // bikeVisual = currentPlayerModel.model.transform;
        if (mainCanvas != null)
        {
            mainCanvasMovementPos = mainCanvas.transform.localPosition;
        }

        PrepEncounterButtons();

        SetPlayerTapPrompt(true);

    }

    #region bike recoil

    [SerializeField] Transform bikeVisual;
    [SerializeField] private float raiseAngle = -220f;
    [SerializeField] private float raiseDuration = 0.5f;
    [SerializeField] private float downDuration = 0.3f;

    private Tween currentRotationTween;

    public void RaiseFront()
    {
        if (bikeVisual == null)
        {
            return;
        }
        if (currentRotationTween != null && currentRotationTween.IsActive())
            currentRotationTween.Kill();

        Vector3 targetRotation = new Vector3(raiseAngle, bikeVisual.localEulerAngles.y, bikeVisual.localEulerAngles.z);
        currentRotationTween = bikeVisual.DOLocalRotate(targetRotation, raiseDuration).SetEase(Ease.OutBack);
    }

    public void LowerFront()
    {
        if (bikeVisual == null)
        {
            return;
        }
        if (currentRotationTween != null && currentRotationTween.IsActive())
            currentRotationTween.Kill();

        Vector3 resetRotation = new Vector3(0f, bikeVisual.localEulerAngles.y, bikeVisual.localEulerAngles.z);
        currentRotationTween = bikeVisual.DOLocalRotate(resetRotation, downDuration).SetEase(Ease.InOutSine);
    }

    #endregion


    public PlayerModel currentPlayerModel;
    public PlayerModel caucasianPlayerModel;
    public PlayerModel africanPlayerModel;

    [Space]
    [SerializeField] private CanvasGroup ftueInstructions;

    [Space]
    public Transform playerPathParent;
    public Button movementButton;
    public AdvanceButton advanceButton;

    public float moveDuration;

    List<Vector3> playerPathPoints;
    public Tween movementTween;



    private void OnEnable()
    {
        SimpleSceneManager.OnSceneLoaded += SimpleSceneManager_OnSceneLoaded;
    }

    private void SimpleSceneManager_OnSceneLoaded(string obj)
    {
        if (ScriptRegistry.Instance != null)
        {
            ScriptRegistry.Instance.deliveryMinigameLoader.deliveryController = this;
            ScriptRegistry.Instance.deliveryMinigameLoader.deliveryController.GetDeliveryAgent.feelVibrationManager = ScriptRegistry.Instance.feelVibrationManager;
            ScriptRegistry.Instance.OnDeliveryControllerAssigned(this);
        }
    }

    private void OnDisable()
    {
        SimpleSceneManager.OnSceneLoaded -= SimpleSceneManager_OnSceneLoaded;
    }


    void Start()
    {
        if (initializeOnStart)
        {
            Initialize();
        }
    }

    public bool loadCharacterModel;

    public void Initialize(Button button = null)
    {
        if (loadCharacterModel)
        {
            LoadCharacterModel();

        }


        playerPathPoints = CollectPathPoints(playerPathParent);
        InitialisePlayerPath();

        if (button != null)
        {
            SetupButtonEvents(button);
        }
        else
        {
            SetupButtonEvents();
        }

        StartCoroutine(SetBicycleStateWithDelay(BicycleStates.Stop, 1.5f, true));

        StartCoroutine(ShowFtue());
    }

    IEnumerator ShowFtue()
    {
        yield return new WaitForSeconds(2f);
        if (ftueInstructions!= null)
        {
            ftueInstructions.alpha = 1f;
        }
    }

    List<Vector3> CollectPathPoints(Transform pathParent)
    {
        List<Vector3> points = new List<Vector3>();
        foreach (Transform child in pathParent)
        {
            points.Add(child.position);
        }
        return points;
    }
    public int debugBodyTypeValue;
    public void LoadCharacterModel()
    {
        if (debugBodyTypeValue != -1)
        {
           // SetPlayerModel(debugBodyTypeValue == 0 ? BodyType.CAUCASIAN : BodyType.AFRICAN);

        }
        else
        {
            //SetPlayerModel(PlayerPrefs.GetInt("BODY-TYPE") == 0 ? BodyType.CAUCASIAN : BodyType.AFRICAN);
        }
        // SetPlayerModel(debugBodyTypeValue == 0 ? BodyType.CAUCASIAN : BodyType.AFRICAN);
        // PlayerPrefs.SetInt("BODY-TYPE", 1);
    }

    public void CameraFullScreen()
    {
        Camera.main.rect = new Rect(0, 0, 1, 1);
    }

    void SetNewMainAnimator(BodyType _bodyType)
    {
        if (_bodyType == BodyType.CAUCASIAN)
        {
            currentPlayerModel.animator = caucasianPlayerModel.animator;
        }
        else
        {
            currentPlayerModel.animator = africanPlayerModel.animator;

        }
    }
    public bool usingNewMinigamePrefab;
    public GameObject minigamePrefab_White, minigamePrefab_Black;
    public Animator mainVehicleAnimator;

    public void ToggleMinigamePrefab(BodyType type)
    {
        bool isCaucasian = type == BodyType.CAUCASIAN;

        minigamePrefab_White.SetActive(isCaucasian);
        minigamePrefab_Black.SetActive(!isCaucasian);
    }

    [Button]
    public void SetPlayerModel(BodyType _bodyType)
    {
        //BodyType selectedBodyType = PlayerPrefs.GetInt("BODY-TYPE") == 0 ? BodyType.CAUCASIAN : BodyType.AFRICAN;
        print($"BODY: {_bodyType}");


        if (usingNewMinigamePrefab)
        {
            ToggleMinigamePrefab(_bodyType);
            return;
        }

        caucasianPlayerModel.animator.gameObject.SetActive(false);
        africanPlayerModel.animator.gameObject.SetActive(false);

        string selection = PlayerPrefs.GetString("SELECTED_CHARACTER");

        if (selection == "CAUCASIAN") // 
        {
            _bodyType = BodyType.CAUCASIAN;
            // currentPlayerModel = caucasianPlayerModel;
            currentPlayerModel.animator = caucasianPlayerModel.animator;
            currentPlayerModel.animator.gameObject.SetActive(true);

            currentPlayerModel.transform = caucasianPlayerModel.transform;
            currentPlayerModel.model = caucasianPlayerModel.model;
            currentPlayerModel.GetRotationLooper = caucasianPlayerModel.GetRotationLooper;


            caucasianPlayerModel.model.SetActive(true);
        }
        else if (selection == "AFRICAN") //
        {
            _bodyType = BodyType.AFRICAN;
            // currentPlayerModel = africanPlayerModel;
            currentPlayerModel.animator = africanPlayerModel.animator;
            currentPlayerModel.animator.gameObject.SetActive(true);


            currentPlayerModel.transform = africanPlayerModel.transform;
            currentPlayerModel.model = africanPlayerModel.model;
            currentPlayerModel.GetRotationLooper = africanPlayerModel.GetRotationLooper;

            africanPlayerModel.model.SetActive(true);


        }
        SetNewMainAnimator(_bodyType);


        //switch (_bodyType)
        //{
        //    case BodyType.CAUCASIAN:
        //        currentPlayerModel = caucasianPlayerModel;
        //        currentPlayerModel.animator = caucasianPlayerModel.animator;
        //        currentPlayerModel.animator.gameObject.SetActive(true);
        //        break;

        //    case BodyType.AFRICAN:
        //        currentPlayerModel = africanPlayerModel;
        //        currentPlayerModel.animator = africanPlayerModel.animator;
        //        currentPlayerModel.animator.gameObject.SetActive(true);

        //        break;
        //}
    }


    void InitialisePlayerPath()
    {
        if (currentPlayerModel != null && playerPathPoints != null && playerPathPoints.Count > 0)
        {
            /*
            movementTween = currentPlayerModel.transform
                .DOPath(playerPathPoints.ToArray(), moveDuration, PathType.CatmullRom, PathMode.Full3D)
                .SetEase(Ease.InOutSine)
                .SetLookAt(0.01f, true)
                .SetAutoKill(false)
                .Pause();
            */
            movementTween = currentPlayerModel.transform
           .DOPath(playerPathPoints.ToArray(), moveDuration, PathType.CatmullRom, PathMode.Full3D)
           .SetEase(Ease.InOutSine)
           .SetLookAt(0.01f, true)
           .SetAutoKill(false)
           .OnComplete(() =>
           {
               //CheckForTimeline();
               OnComplete.Invoke();
           });

            movementTween.Goto(moveDuration * 0.01f);
            movementTween.Pause();

            PrepPropHandler(movementTween);

        }
    }
    public bool playTimeLineOnDeliveryCompletion;
    public TimelineManager GetTimelineManager;
    public TimelineClipName timelineClipToPlay;
    public CinemachineCamera timelineTransistionCam;

    [Button]
    public void CheckForTimeline()
    {

        /*
        if (playTimeLineOnDeliveryCompletion)
        {
            Debug.Log("call main transistion");

            GetTimelineManager.PlayTimelineByName(timelineClipToPlay);
        }
        */

        if (GetTimelineManager == null)
        {
            return;
        }

        StartCoroutine(HandleTimeline());
    }
    IEnumerator HandleTimeline()
    {
        if (timelineTransistionCam != null)
        {
            timelineTransistionCam.Priority = 99;
            //Debug.Log("start cam transistion");

            GetTimelineManager.FadeInImage();
            //Debug.Log("cue cam transistion");

            yield return new WaitForSeconds(1.5f);
            //Debug.Log("end cam transistion");

            //timelineTransistionCam.Priority = 0;

        }

        if (playTimeLineOnDeliveryCompletion)
        {
            //Debug.Log("call main transistion");

            GetTimelineManager.PlayTimelineByName(timelineClipToPlay);
        }
    }

    void SetupButtonEvents()
    {
        if (movementButton == null) return;

        EventTrigger trigger = movementButton.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = movementButton.gameObject.AddComponent<EventTrigger>();
        }

        AddEventTrigger(trigger, EventTriggerType.PointerDown, OnMovementButtonDown);
        AddEventTrigger(trigger, EventTriggerType.PointerUp, OnMovementButtonUp);
    }


    Button moveButton;
    void SetupButtonEvents(Button _movementButton)
    {
        if (_movementButton == null) return;

        moveButton = _movementButton;

        EventTrigger trigger = _movementButton.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = _movementButton.gameObject.AddComponent<EventTrigger>();
        }

        AddEventTrigger(trigger, EventTriggerType.PointerDown, OnMovementButtonDown);
        AddEventTrigger(trigger, EventTriggerType.PointerUp, OnMovementButtonUp);
    }


    void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = eventType
        };
        entry.callback.AddListener((eventData) => action.Invoke());
        trigger.triggers.Add(entry);
    }



    public void RemoveButtonEvents()
    {
        if (moveButton == null) return;

        EventTrigger trigger = moveButton.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) return;

        // Find and remove specific event entries
        List<EventTrigger.Entry> entriesToRemove = new List<EventTrigger.Entry>();

        foreach (EventTrigger.Entry entry in trigger.triggers)
        {
            // Check if this is one of your event entries
            if (entry.eventID == EventTriggerType.PointerDown ||
                entry.eventID == EventTriggerType.PointerUp)
            {
                entriesToRemove.Add(entry);
            }
        }

        // Remove the entries from the triggers list
        foreach (EventTrigger.Entry entry in entriesToRemove)
        {
            trigger.triggers.Remove(entry);
        }

        moveButton = null;
    }

    IEnumerator SmoothTimeScale(Tween tween, float targetScale, float duration)
    {
        float startScale = tween.timeScale;
        float timeElapsed = 0f;

        if (targetScale > 0f && !tween.IsPlaying())
            tween.Play();

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / duration;
            tween.timeScale = Mathf.Lerp(startScale, targetScale, t);
            yield return null;
        }

        tween.timeScale = targetScale;

        if (Mathf.Approximately(targetScale, 0f))
            tween.Pause();

    }


    [SerializeField] float smoothDuration = 0.5f;

    Coroutine smoothTimeScaleRoutine;

    public DeliveryAgent GetDeliveryAgent;

    bool IsDead()
    {
        if (GetDeliveryAgent == null)
        {
            GetDeliveryAgent = GetComponent<DeliveryAgent>();
            if (GetDeliveryAgent == null)
            {
                return false;
            }
        }
        return GetDeliveryAgent.dying;
    }

    void OnMovementButtonDown()
    {
        if (encounterActive || IsDead())
        {
            return;
        }
        if (movementTween != null)
        {
            if (smoothTimeScaleRoutine != null)
            {
                StopCoroutine(smoothTimeScaleRoutine);
            }
            smoothTimeScaleRoutine = StartCoroutine(SmoothTimeScale(movementTween, 1f, smoothDuration));
        }

        SetBicycleState(BicycleStates.Ride);
        currentPlayerModel.GetRotationLooper.ResumeTween();
        RaiseFront();

        DisableStartCam();

        SetPlayerTapPrompt(false);

        ToggleWheelRotation(true);

        if (ftueInstructions!=null)
        {
            if (ftueInstructions.alpha > 0)
            {
                ftueInstructions.alpha = 1f;

            }
        }

       
    }

    public void OnMovementButtonUp()
    {
        if (movementTween != null)
        {
            if (smoothTimeScaleRoutine != null)
            {
                if (gameObject.activeInHierarchy)
                    StopCoroutine(smoothTimeScaleRoutine);
            }

            if (gameObject.activeInHierarchy)
                smoothTimeScaleRoutine = StartCoroutine(SmoothTimeScale(movementTween, 0f, smoothDuration / 2));
        }

        SetBicycleState(BicycleStates.Stop);
        currentPlayerModel.GetRotationLooper.PauseTween();
        LowerFront();

        ToggleWheelRotation(false);

    }
    /*
    void OnMovementButtonDown()
    {
        movementTween?.Play();
        SetBicycleState(BicycleStates.Ride);

        currentPlayerModel.GetRotationLooper.ResumeTween();

        RaiseFront();
    }

    void OnMovementButtonUp()
    {
        movementTween?.Pause();
        SetBicycleState(BicycleStates.Stop);

        currentPlayerModel.GetRotationLooper.PauseTween();

        LowerFront();
    }
    */


    /*
    public float moveStartDelay, brakeDelay;
    Coroutine movementDownRoutine;
    Coroutine movementUpRoutine;

    void OnMovementButtonDown()
    {
        if (movementDownRoutine != null) StopCoroutine(movementDownRoutine);
        movementDownRoutine = StartCoroutine(HandleMovementButtonDown());
    }

    void OnMovementButtonUp()
    {
        if (movementUpRoutine != null) StopCoroutine(movementUpRoutine);
        movementUpRoutine = StartCoroutine(HandleMovementButtonUp());
    }

    IEnumerator HandleMovementButtonDown()
    {
        yield return new WaitForSeconds(moveStartDelay);

        movementTween?.Play();
        SetBicycleState(BicycleStates.Ride);
        currentPlayerModel.GetRotationLooper.ResumeTween();
        RaiseFront();

        movementDownRoutine = null;
    }

    IEnumerator HandleMovementButtonUp()
    {
        yield return new WaitForSeconds(brakeDelay);

        movementTween?.Pause();
        SetBicycleState(BicycleStates.Stop);
        currentPlayerModel.GetRotationLooper.PauseTween();
        LowerFront();

        movementUpRoutine = null;
    }
    */


    IEnumerator SetBicycleStateWithDelay(BicycleStates state, float delay, bool checkState = false)
    {
        yield return new WaitForSeconds(delay);
        if (checkState)
        {
            if (currentBicycleState != BicycleStates.Ride)
            {
                SetBicycleState(state);
            }

        }
        else
        {
            SetBicycleState(state);

        }
        // SetBicycleState(state);
    }


    public BicycleStates currentBicycleState;

    public UnityEvent OnComplete;

    public Animator vehicleAnimator;


    public void SetBicycleState(BicycleStates state)
    {

        currentBicycleState = state;

        if (currentPlayerModel.animator != null)
        {
            currentPlayerModel.animator.SetTrigger(state.ToString());
        }
        if (vehicleAnimator != null)
        {
            // vehicleAnimator.SetTrigger("cycle_" + state);
            vehicleAnimator.SetTrigger(state.ToString());

        }
    }

    public enum BicycleStates
    {
        Ride,
        Mount,
        Stop
    }

    #region Wheels rotator


    public List<Transform> wheelTransforms;
    public float rotationSpeed = 360f;

    private List<Tweener> wheelTweens = new List<Tweener>();

    bool wheelTweensPrepped;

    void PrepWheelTweens()
    {
        if (wheelTweensPrepped)
        {
            return;
        }

        foreach (Transform wheel in wheelTransforms)
        {
            if (wheel == null) continue;

            Tweener t = wheel.DOLocalRotate(Vector3.right * 360f, 1f / (rotationSpeed / 360f), RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear)
                .Pause();

            wheelTweens.Add(t);
        }

        wheelTweensPrepped = true;
    }

    public void ToggleWheelRotation(bool shouldRotate)
    {
        if (!wheelTweensPrepped)
        {
            PrepWheelTweens();
        }
        foreach (Tweener t in wheelTweens)
        {
            if (t == null) continue;

            if (shouldRotate) t.Play();
            else t.Pause();
        }
    }


    #endregion

}