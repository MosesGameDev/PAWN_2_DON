using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lean.Pool;
using Sirenix.OdinInspector;
using DG.Tweening;
using PixelCrushers.DialogueSystem;

public class BusinessHandler : MonoBehaviour
{

    #region Enums
    public enum BusinessClass
    {
        CounterfeitGoods,
        MoneyLaundering,
        IllegalLotteries,
        UndergroundBetting,
        CreditCardFraud,
        DocumentForgery,
        NightClub
    }
    #endregion

    #region UI Components
    [Header("UI Panel Controls")]
    public GameObject unlockIcon;
    public Button OpenPanel;
    public Button ClosePanel;
    public GameObject panelUI;
    public GameObject shadowMaskShape;
    #endregion

    #region Business Management
    [Header("Business Configuration")]
    public List<BusinessType> businesses = new List<BusinessType>();
    public bool startEarningsOnStart;
    #endregion

    #region interactable (First Time User Experience)
    [Header("FTUE Settings")]
    public bool isPlayingFTUE;
    public bool hasPlayedFTUE;
    #endregion

    #region Events
    public Action<BusinessClass> onBusinessUnlocked;
    #endregion

    #region Unity Lifecycle

    public void OnEnable()
    {
        foreach (var business in businesses)
        {
            business.AssignEvents();
            business.onUnlocked += ShowUnlockIcon;
        }
    }

    private void OnDisable()
    {
        foreach (var business in businesses)
        {
            business.DisableEvents();
            business.onUnlocked -= ShowUnlockIcon;
        }
    }

    void ShowUnlockIcon()
    {
        if (unlockIcon == null)
        {
            return;
        }
        if (!unlockIcon.activeInHierarchy)
        {
            unlockIcon.SetActive(true);
            unlockIcon.transform.DOScale(Vector3.one * 1.05f, 0.45f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void Start()
    {
        if (startEarningsOnStart)
        {
            StartBusinessEarning();
        }

        AssignBusinessRefrences();
    }

    [Button]
    void AddCash()
    {
        ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").UpdateUIElement(10000);
    }

    public void CheckSaveData()
    {
        if (PlayerPrefs.HasKey("BUSINESS_UNLOCKED"))
        {
            isPlayingFTUE = false;
            hasPlayedFTUE = true;
            ScriptRegistry.Instance.homeScreenManager.GetButton("Business").EnableButton();

            foreach (var business in businesses)
            {
                if (business.CheckIfBusinessHasBeenUnlocked())
                {
                    business.LoadBusinessData();
                    business.UnlockMain();
                    business.SetupUpgradeButton();
                    business.lockedBusinessBanner.SetActive(false);
                }
            }
        }
    }

    [Button]
    public void DeleteUnlockBusiness()
    {
        hasPlayedFTUE = false;
        PlayerPrefs.DeleteKey("BUSINESS_UNLOCKED");
    }

    #endregion

    #region Initialization

    void AssignBusinessRefrences()
    {
        foreach (var business in businesses)
        {
            business.businessHandler = this;

            business.CostCheck();
            business.unlockButton.onClick.AddListener(() =>
            {
                business.UnlockMain();
            });

        }
    }
    #endregion

    #region UI Management
    public void OpenBusinessUI()
    {
        CheckSaveData();

        foreach (var business in businesses)
        {
            business.CostCheck();
        }

        if(ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
        {
            ScriptRegistry.Instance.dayCompleteHandler.nextDayButton.gameObject.SetActive(false);
        }
        ScriptRegistry.Instance.homeScreenManager.homeScreenButtonParent.SetActive(false);
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("Business").Show();

        if(unlockIcon)
        {
            unlockIcon.transform.DOKill();
            unlockIcon.transform.localScale = Vector3.one;
            unlockIcon.SetActive(false);
        }

        SoundManager.Instance.PlaySFX("click");
        FeelVibrationManager.Instance.PlayVibration();

    }

    public void CloseUI()
    {
        HandleFTUEClose();

        print("<color=green>Business UI closed.</color>");

        if (!ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
        {
            ScriptRegistry.Instance.homeScreenManager.homeScreenButtonParent.SetActive(false);
        }
        else
        {
            ScriptRegistry.Instance.homeScreenManager.homeScreenButtonParent.SetActive(true);
        }

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("softMask").Hide();

        SoundManager.Instance.PlaySFX("click");
        FeelVibrationManager.Instance.PlayVibration();

        if (ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
        {
            return;
        }
        ScriptRegistry.Instance.cutSceneManager.GetCutsceneHandler(5).GetComponent<Day05CutSceneEventHandler>().Close();

    }

    private void HandleFTUEClose()
    {
        isPlayingFTUE = false;
        hasPlayedFTUE = true;
        ClosePanel.interactable = true;
        shadowMaskShape.SetActive(false);

        ScriptRegistry.Instance.homeScreenManager.EnableButtonsFTUE();


        if (ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
        {
            ScriptRegistry.Instance.dayCompleteHandler.nextDayButton.gameObject.SetActive(true);
        }

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("foodAnimatedPointer_business_unlock_03").Hide();
        ToggleBusinessButtons(true);

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("Business").Hide();

        PlayerPrefs.SetInt("BUSINESS_UNLOCKED", 1);
        print("<color=green>Business FTUE completed and saved.</color>");
    }
    #endregion

    public void ToggleBusinessButtons(bool interactible)
    {
        foreach (var business in businesses)
        {
            business.ToggleButtonInteractable(interactible);
        }
    }

    #region Business Operations
    public void StartBusinessEarning()
    {
        foreach (var business in businesses)
        {
            StartBusiness(business);
        }
    }

    public void ShowBusinessEarnings()
    {
        if (!HasActiveEarnings())
        {
            print("No active earnings to show.");
            return;
        }

        foreach (var business in businesses)
        {
            if (business.businessUnlocked && business.earningActive)
            {
                business.ShowDayEarnings();
            }
        }

        //StartCoroutine(ShowBusinessEarningsCoroutine());
    }

    public bool HasActiveEarnings()
    {
        foreach (var business in businesses)
        {
            if (business.businessUnlocked && business.earningActive)
            {
                return true;
            }
        }
        return false;
    }

    public void StartBusiness(BusinessType business)
    {
        if (business != null && !business.earningActive)
        {
            business.Initialise();
            business.CostCheck();
            business.earningRoutine = StartCoroutine(business.EarningLoop());
            business.earningActive = true;

            //CreateTextDialogueElement($"Business {business.businessClass} is unlocked and started.");
        }
    }

    public void ToggleEarning(BusinessType business)
    {
        if (business.earningActive)
        {
            StopBusinessEarning(business);
        }
        else
        {
            StartBusinessEarning(business);
        }
    }

    private void StopBusinessEarning(BusinessType business)
    {
        if (business.earningRoutine != null)
        {
            StopCoroutine(business.earningRoutine);
            business.earningRoutine = null;
        }
        business.earningActive = false;
    }

    public void StartBusinessEarning(BusinessType business)
    {
        business.earningRoutine = StartCoroutine(business.EarningLoop());
        business.earningActive = true;
    }
    #endregion

    #region Business Unlock Management
    public void UnlockBusinessFromList(BusinessClass businessClass)
    {
        foreach (var business in businesses)
        {
            if (business != null && business.businessClass == businessClass)
            {
                business.UnlockBusiness();
                StartBusiness(business);
                break;
            }
        }
    }

    public void UnlockBusinessFromList_ViaLocalUnlockState()
    {
        foreach (var business in businesses)
        {
            if (business != null && business.businessUnlocked)
            {
                business.UnlockBusiness();
                StartBusiness(business);
                print($"Business {business.businessClass} is unlocked and started.");
            }
        }
    }

    public void CreateTextDialogueElement(string text)
    {
        ScriptRegistry.Instance.textGameController.CreateTextDialogueElement(text, out TextDialogueUIElement textDialogue);
    }

    public void DisableBusinessUnlockButtons()
    {
        foreach (var business in businesses)
        {
            if (business != null && business.unlockButton != null)
            {
                business.unlockButton.interactable = false;
            }
        }
    }
    #endregion

    #region Business Queries
    public BusinessType GetBusinessByClass(BusinessClass businessClass)
    {
        foreach (var business in businesses)
        {
            if (business != null && business.businessClass == businessClass)
            {
                return business;
            }
        }
        return null;
    }
    #endregion

    #region FTUE Management
    [Button("OpenBusinessFTUE")]
    public void OpenBusinessFTUE()
    {
        print("<color=red>Business FTUE is not active.</color>");
        if (hasPlayedFTUE) return;

        isPlayingFTUE = true;
        ScriptRegistry.Instance.homeScreenManager.DisableButtonsFTUE();
        ScriptRegistry.Instance.homeScreenManager.UnlockButton("Business");
        AnalyticsEvents.instance.UniqueEvent("ftue_businesses");

    }

    public void OnOpenBusinessFTUE()
    {
        if (isPlayingFTUE)
        {
            ClosePanel.interactable = false;
            shadowMaskShape.SetActive(true);
            ScriptRegistry.Instance.homeScreenManager.homeScreenButtonParent.SetActive(false);

            var uiDialogueManager = ScriptRegistry.Instance.uiDialoguesManager;
            uiDialogueManager.GetUIDialogue("softMask").Show();
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("foodAnimatedPointer_business_unlock_03").Hide();

            UnlockFTUEBusinesses();
        }
        else
        {
            //print in red
            print("<color=red>Business FTUE is not active.</color>");
        }
    }

    public void CheckForUnlockedBusinesses()
    {
        int startConversationId = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        foreach (var business in businesses)
        {
            business.CheckForUnlock(startConversationId);
        }

    }


    public void CheckUpgradeButtons()
    {
        foreach (var business in businesses)
        {
            business.CostCheckUpgrade();
        }
    }


    public void UnlockFTUEBusinesses()
    {
        var uiDialogueManager = ScriptRegistry.Instance.uiDialoguesManager;

        foreach (var business in businesses)
        {
            if (business.businessClass == BusinessClass.CounterfeitGoods || business.businessClass == BusinessClass.IllegalLotteries)
            {
                business.ShowBusinessUnlockBanner();
                business.unlockButton.onClick.AddListener(() =>
                {
                    business.UnlockMain();
                    uiDialogueManager.GetUIDialogue("softMask").Hide();
                    shadowMaskShape.SetActive(false);
                    AnalyticsEvents.instance.UniqueEvent($"business_{business.businessClass.ToString().ToLower()}_unlocked");
                });
            }
        }
    }
    #endregion

    #region Nested BusinessType Class
    [System.Serializable]
    public class BusinessType
    {
        #region Business Configuration
        [Header("Business Info")]
        public BusinessClass businessClass;
        public BusinessHandler businessHandler;
        #endregion

        #region Unlock System
        [Header("Unlock Variables")]
        [Space(20)]
        public bool businessAvailable; // Indicates if the business is available for unlocking
        public bool businessUnlocked;
        public GameObject lockedBusinessBanner;
        public GameObject unLockedBusinessBanner;
        public GameObject unlockFXPrefab;
        public RectTransform uiElementRectTransform;
        public Button unlockButton;
        public int unlockCost;
        public int unlockDay;
        #endregion

        #region Earnings Configuration
        [Header("Earnings Settings")]
        [Space(20)]
        public bool earningActive;
        public float earningDuration;
        public int dayEarnings;
        public int currentLevel;
        public int maxLevel;

        [Header("Earnings Configuration")]
        [Space(20)]
        public float baseEarning;
        public float earningIncrementPercent;

        [Header("Upgrade Configuration")]
        [Space(20)]
        public float baseUpgradeCost;
        public float upgradeCostIncrementPercent;
        #endregion

        #region UI Elements
        [Header("UI Elements")]
        [Space(20)]
        public TextMeshProUGUI currentIncome;
        public TextMeshProUGUI upgradeCostText;
        public TextMeshProUGUI unlockCostText;
        public TextMeshProUGUI unlockDayText;
        public GameObject maxLevelBanner;
        public Image fillImage;
        public Button upgradeButton;
        public Sprite businessIcon;
        #endregion

        #region Private Fields
        private float currentEarning;
        private float currentUpgradeCost;
        private bool initialised;
        public Coroutine earningRoutine;
        public bool infiniteCash; // Debug feature
        public Action onUnlocked;
        #endregion

        #region Initialization

        public void AssignEvents()
        {
            TextGameUIController.OnDayChange += CheckForUnlock;
        }

        public void DisableEvents()
        {
            TextGameUIController.OnDayChange -= CheckForUnlock;
        }

        public void CheckForUnlock(int day)
        {
            if (day >= unlockDay && !businessUnlocked)
            {
                Unlock();
                onUnlocked?.Invoke();
            }

            unlockDayText.SetText($"Unlocks on Day: {unlockDay}");
        }

        public void Initialise()
        {
            if (initialised) return;

            initialised = true;
            LoadBusinessData();
            SetupUpgradeButton();
            CheckForMaxLevel();
            UpdatePanelText();
        }

        public void Unlock()
        {
            if (CheckIfBusinessHasBeenUnlocked())
            {
                return;
            }

            if (!businessAvailable && !businessUnlocked)
            {
                businessAvailable = true;
                lockedBusinessBanner.SetActive(false);
                ShowUnlockBanner();
            }
        }

        public bool CheckIfBusinessHasBeenUnlocked()
        {
            if (PlayerPrefs.HasKey($"{businessClass}_UNLOCKED"))
            {
                return true;
            }

            return false;
        }

        public void LoadBusinessData()
        {
            string prefix = businessClass.ToString();
            currentEarning = PlayerPrefs.GetFloat($"{prefix}_currentEarning", baseEarning);
            currentUpgradeCost = PlayerPrefs.GetFloat($"{prefix}_currentUpgradeCost", baseUpgradeCost);
            currentLevel = PlayerPrefs.GetInt($"{prefix}_currentLevel", 0);
        }


        public void ShowUnlockBanner()
        {
            unLockedBusinessBanner.SetActive(true);
            unlockButton.transform.DOScale(Vector3.one * 1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }


        public void SetupUpgradeButton()
        {
            if (upgradeButton != null)
            upgradeButton.onClick.AddListener(CallForUpgrade);
        }

        #endregion

        #region Business Unlock
        [Button("Unlock Business")]
        public void UnlockBusiness()
        {
            unLockedBusinessBanner.SetActive(false);
            unlockButton.gameObject.SetActive(false);
            businessUnlocked = true;

            Initialise();
            SpawnUnlockFX();
        }

        public void CostCheck()
        {
            int cash = ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").currentValue;


            if (cash < unlockCost)
            {
                unlockButton.interactable = false;
                unlockCostText.text = $"<color=red>{unlockCost.ToString("N0")}</color>";
            }
            else
            {
                unlockButton.interactable = true;
                unlockCostText.text = $"<color=white>{unlockCost.ToString("N0")}</color>";
            }
        }

        public void ShowBusinessUnlockBanner()
        {
            unlockButton.transform.DOScale(Vector3.one * 1.05f, 0.45f).SetLoops(-1, LoopType.Yoyo);
            CostCheck();
        }

        public void UnlockMain()
        {
            CheckTransaction(unlockCost);
            SoundManager.Instance.PlaySFX("click");
            FeelVibrationManager.Instance.PlayVibration();

            unLockedBusinessBanner.gameObject.SetActive(false);
            businessUnlocked = true;
            businessHandler.UnlockBusinessFromList(businessClass);
            businessHandler.onBusinessUnlocked?.Invoke(businessClass);

            PlayerPrefs.SetInt($"{businessClass}_UNLOCKED", 1);
            SaveBusinessData();

            //AnalyticsEvents.instance.UniqueEvent($"business_{businessClass}_unlocked");
        }

        private void SpawnUnlockFX()
        {
            if (unlockFXPrefab != null && lockedBusinessBanner != null)
            {
                LeanPool.Spawn(unlockFXPrefab, lockedBusinessBanner.transform.position, lockedBusinessBanner.transform.rotation);
            }
        }
        #endregion

        #region Earnings System


        public IEnumerator EarningLoop()
        {
            while (true)
            {
                yield return businessHandler.StartCoroutine(AnimateFillBar());
                //yield return new WaitForSeconds(earningDuration);


                AddEarning(currentEarning);
                ResetFillBar();
                SaveBusinessData();
            }
        }

        private IEnumerator AnimateFillBar()
        {
            fillImage.fillAmount = 0f;
            float elapsedTime = 0f;

            while (elapsedTime < earningDuration)
            {
                elapsedTime += Time.deltaTime;
                fillImage.fillAmount = Mathf.Clamp01(elapsedTime / earningDuration);
                yield return null;
            }
        }

        public void ResetFillBar()
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = 0f;
            }
        }

        public void AddEarning(float value)
        {
            int v = Mathf.RoundToInt(value);
            int currentCash = ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").currentValue;
            currentCash += v;
            dayEarnings += v;
            //ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").UpdateUIElementDirect(currentCash);
        }
        #endregion

        #region Upgrade System
        public void CallForUpgrade()
        {
            if (CheckTransaction(currentUpgradeCost))
            {
                UpgradeEarning();
                SoundManager.Instance.PlaySFX("click");
                FeelVibrationManager.Instance.PlayVibration();
            }
        }

        public void ToggleButtonInteractable(bool interactable)
        {
            upgradeButton.interactable = interactable;
            unlockButton.interactable = interactable;
        }

        public void ShowDayEarnings()
        {
            if(dayEarnings <= 0)
            {
                return;
            }
            //string earningsText = $"{businessClass} earned {dayEarnings.ToString("N0")}";
            string earningsText = $"<color=#FFF294>{businessClass}</color> earned <color=#A5FF97>$ {dayEarnings.ToString("N0")}</color>";
            ScriptRegistry.Instance.textGameController.CreateBusinessDialogueElement(earningsText, this, out TextDialogueUIElement textDialogue);
            

            textDialogue.EnableParameterUIElement("CASH", dayEarnings);
            int value = ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").GetCurrentValue();

            ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").UpdateUIElement(dayEarnings);
            //print($"Business {businessClass} earnings added: {dayEarnings}. New cash value: {value + dayEarnings}");
        }


        public bool CheckTransaction(float cost)
        {
            if (infiniteCash) return true;

            int currentCash = ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").currentValue;

            if (currentCash >= cost)
            {
                int c = Mathf.RoundToInt(cost);
                ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").UpdateUIElementDirect(currentCash - c);
                return true;
            }
            else
            {
                //Debug.Log($"Insufficient funds. Current: {currentCash}, Required: {cost}");
                return false;
            }
        }

        public void CostCheckUpgrade()
        {
            int cash = ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").currentValue;
            upgradeButton.interactable = (cash >= currentUpgradeCost);
            upgradeCostText.gameObject.SetActive(true);
        }

        public void UpgradeEarning()
        {
            if (currentLevel >= maxLevel)
            {
                MaxLevelReached();
                return;
            }

            ProcessUpgrade();
            SaveBusinessData();
            UpdatePanelText();
            CheckForMaxLevel();
            SaveBusinessData();
            CostCheck();
            CostCheckUpgrade();

            ScriptRegistry.Instance.businessHandler.CheckUpgradeButtons();
        }

        private void ProcessUpgrade()
        {
            currentLevel++;
            currentEarning +=  (earningIncrementPercent);
            currentUpgradeCost += (upgradeCostIncrementPercent);
            //AnalyticsEvents.instance.UniqueEvent($"business_{businessClass}_upgrade_level_{currentLevel}");
        }

        private void SaveBusinessData()
        {
            string prefix = businessClass.ToString();
            PlayerPrefs.SetFloat($"{prefix}_currentEarning", currentEarning);
            PlayerPrefs.SetFloat($"{prefix}_currentUpgradeCost", currentUpgradeCost);
            PlayerPrefs.SetInt($"{prefix}_currentLevel", currentLevel);
        }
        #endregion

        #region Level Management
        private void CheckForMaxLevel()
        {
            bool isMaxed = currentLevel >= maxLevel;

            if (maxLevelBanner != null)
                maxLevelBanner.SetActive(isMaxed);

            if (upgradeButton != null)
                upgradeButton.interactable = !isMaxed;

            if (isMaxed)
                MaxLevelReached();
        }

        private void MaxLevelReached()
        {
            if (maxLevelBanner != null)
                maxLevelBanner.SetActive(true);

            if (upgradeButton != null)
                upgradeButton.interactable = false;
        }
        #endregion

        #region UI Updates
        private void UpdatePanelText()
        {
            if (currentIncome != null)
                currentIncome.text = $"{currentEarning:0} / min";

            if (upgradeCostText != null)
                upgradeCostText.text = currentUpgradeCost.ToString("0");


            upgradeCostText.gameObject.SetActive(true);
        }
        #endregion
    }
    #endregion
}