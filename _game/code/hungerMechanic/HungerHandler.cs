using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using PixelCrushers.DialogueSystem;
using System;
using UnityEngine.UI;
using MoreMountains.Tools;


[System.Serializable]
public class ConsumerbleFood
{
    public Sprite icon;
    public string foodName;
    public  int foodValue;
    public  int foodCount;
    public  int foodCost;
    public Action<ConsumerbleFood> onUpdated;
}

public class HungerHandler : MonoBehaviour
{
    [Header("Hunger Settings")]

    [SerializeField] private float maxHunger = 100f;
    [SerializeField, ReadOnly] private int currentHunger;
    [SerializeField] private int hungerIncreaseAmount = 5;
    [SerializeField] private float hungerCheckInterval = 5f;

    [Header("Effects")]
    [SerializeField] private float criticalHungerThreshold = 30f;

    [Space]
    [SerializeField] private TextDialogueUIElement textDialogueUIElement;
    [SerializeField] private Button closeFoodPanelButton;
    [SerializeField] private RectTransform contentHolder;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private string[] dialogueLines;

    [Space]
    [SerializeField] private Button openFoodMenuButton;

    [Header("Foods")]
    public ConsumerbleFood[] foods;
    public UIButton_ConsumeFood[] foodButtons;

    private Coroutine hungerCoroutine;


    // Debug properties
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool visualizeInEditor = true;
    //[SerializeField, ReadOnly] private string hungerStatus = "None";
    [SerializeField, ReadOnly] private float timeSinceLastHungerIncrease = 0f;
    [SerializeField, ReadOnly] private int hungerDamageDealt = 0;
    bool isInitialized;
    

    [Button("Reset Hunger")]
    public void ResetHunger()
    {
        currentHunger = 0;
    }

    [Button("Add Hunger")]
    public void AddHunger(int amount = 10)
    {
        currentHunger += amount;
        CheckHungerEffects();
    }


    private void OnEnable()
    {
        UIButton_ConsumeFood.OnFoodConsumed += OnConsumeFood;
    }

    private void OnConsumeFood(ConsumerbleFood food)
    {
        if(isFtuePlaying && !hasPlayedFTUE)
        {
            foods[0].foodCount = 0;
            ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").UpdateUIElementDirect(-foods[0].foodCost);
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("foodAnimatedPointer_exitFoods").Show();
            print("Food consumed during FTUE, showing exit button.");
            DisableButtons();
            closeFoodPanelButton.interactable = true;
            return;
        }

        closeFoodPanelButton.interactable = true;

    }

    private void Start()
    {

        openFoodMenuButton.onClick.AddListener(() =>
        {
            OpenFoodMenu();
        });

    }

    [Button]
    public void Initialize()
    {
        if (isInitialized)
        {
            return;
        }
        if (!PlayerPrefs.HasKey("HUNGER_INITIALIZED"))
        {
            PlayerPrefs.SetInt("HUNGER_INITIALIZED", 1);
        }

        if (hungerCoroutine != null)
        {
            StopCoroutine(hungerCoroutine);
        }
        hungerCoroutine = StartCoroutine(IncreaseHungerOverTime());
        isInitialized = true;

        for (int i = 0; i < foods.Length; i++)
        {
            foodButtons[i].Initialize(foods[i]);
        }


        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("Food").onShow.RemoveAllListeners();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("Food").onShow.AddListener(() =>
        {
            if (isFtuePlaying && !hasPlayedFTUE)
            {
                foods[0].foodCount = 1; // Ensure the first food is available for FTUE
                ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("foodAnimatedPointer").Show();
                scrollView.enabled = false;
                InitializeFoodButtons();
            }
        });

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("Food").onHide.AddListener(() =>
        {
            EnableButtons();
        });


        PlayerPrefs.SetInt("HUNGER_INITIALIZED", 1);
    }

    [Button]
    public void ClearSavedData()
    {
        hasPlayedFTUE = false;
        PlayerPrefs.DeleteKey("HUNGER_INITIALIZED");    
    }



    [Button("Open Food Menu")]
    public void OpenFoodMenu()
    {
        Initialize();

        if (ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
        {
            ScriptRegistry.Instance.dayCompleteHandler.nextDayButton.gameObject.SetActive(false);
        }


        for (int i = 0; i < foods.Length; i++)
        {
            foodButtons[i].Initialize(foods[i]);
        }

        ScriptRegistry.Instance.textGameController.ShowTextArea();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("Food").Show();

    }

    public bool isFtuePlaying = false;
    public bool hasPlayedFTUE = false;


    [Button("Show FTUE")]
    public void ShowFtue()
    {
        if(hasPlayedFTUE)
        {
            //print("FTUE has already been played. Skipping FTUE.");
            //return;
        }

        isFtuePlaying = true;
        closeFoodPanelButton.interactable = false;
        int day = DialogueLua.GetVariable("DAY").asInt;

        if (day != 2)
        {
            return;
        }

        ScriptRegistry.Instance.textGameController.GetVariableUIElement("HP").UpdateUIElementDirect(50);

        CutSceneManager cutSceneManager = ScriptRegistry.Instance.cutSceneManager;
        cutSceneManager.PauseCutscene();

        ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(true);
        DisableButtons();

        StartCoroutine(UnlockFoodEnum());
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("settings_button").Hide();

    }

    void DisableButtons()
    {
        for (int i = 0; i < foodButtons.Length; i++)
        {
            foodButtons[i].buttonBuy.interactable = false;
            foodButtons[i].buttonConsume.interactable = false;
        }

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("settings_button").canvasGroup.interactable = false;

    }

    void EnableButtons(int startIndex = 0)
    {
        for (int i = startIndex; i < foodButtons.Length; i++)
        {
            foodButtons[i].buttonBuy.interactable = true;
            foodButtons[i].buttonConsume.interactable = true;
        }

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("settings_button").canvasGroup.interactable = true;

    }

    IEnumerator UnlockFoodEnum()
    {
        yield return new WaitForSeconds(2f);
        ScriptRegistry.Instance.homeScreenManager.UnlockButton("Food");
        AnalyticsEvents.instance.UniqueEvent("ftue_food_start");
        yield return new WaitForSeconds(.5f);
    }

    public void CloseFoodMenu()
    {
        if (ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
        {
            ScriptRegistry.Instance.dayCompleteHandler.nextDayButton.gameObject.SetActive(true);
            ScriptRegistry.Instance.textGameController.MinimizeTextArea();
        }


        hasPlayedFTUE = true;
        isFtuePlaying = false;

        scrollView.enabled = true;

        CutSceneManager cutSceneManager = ScriptRegistry.Instance.cutSceneManager;
        cutSceneManager.Unpause();

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("hungerAlert").Hide();
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("Food").Hide();
        CheckHungerEffects();
    }

    public void DisableButtonsForFTUE()
    {
        if (!isFtuePlaying)
        {
            return;
        }
        DisableButtons();
    }


    private void OnDisable()
    {
        // Stop coroutine when object is disabled
        if (hungerCoroutine != null)
        {
            StopCoroutine(hungerCoroutine);
            hungerCoroutine = null;
        }

        UIButton_ConsumeFood.OnFoodConsumed -= OnConsumeFood;

    }

    //editor visualization
    private void Update()
    {
        if(!isInitialized)
        {
            return;
        }
        if (visualizeInEditor && hungerCoroutine != null)
        {
            timeSinceLastHungerIncrease += Time.deltaTime;
            if (timeSinceLastHungerIncrease > hungerCheckInterval)
            {
                timeSinceLastHungerIncrease = 0;
            }
        }
    }

    //increase hunger at regular intervals
    private IEnumerator IncreaseHungerOverTime()
    {
        while (true)
        {
            timeSinceLastHungerIncrease = 0;
            yield return new WaitForSeconds(hungerCheckInterval);

            int prevHunger = currentHunger;
            currentHunger += hungerIncreaseAmount;

            CheckHungerEffects();
        }
    }



    public void CheckHungerEffects()
    {
        UpdateFoodButtons();
        int playerHealth = ScriptRegistry.Instance.textGameController.GetVariableUIElement("HP").currentValue;

        // Critical hunger level - apply effects
        if (isInitialized && playerHealth > criticalHungerThreshold)
        {
            int previousHealth = playerHealth;
            int newHealth = ScriptRegistry.Instance.textGameController.GetVariableUIElement("HP").currentValue;
            ScriptRegistry.Instance.textGameController.GetVariableUIElement("HP").UpdateUIElement(-5);
            int damageDealt = previousHealth - newHealth;
            hungerDamageDealt += damageDealt;

            //TextDialogueUIElement _textDialogueUIElement = Instantiate(textDialogueUIElement, contentHolder);
            // _textDialogueUIElement.SetText($"<color=red>Hunger Damage!</color>\nYou are {hungerStatusEnum.ToString()!} You lost {damageDealt} health.");
        }
        else
        {
            if(ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
            {
                if(ScriptRegistry.Instance.deliveryMinigameLoader.isPlayingDeliveryMinigameReplay)
                {
                    return;
                }
                ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("hungerAlert").Show();
            }

        }

    }

    [Button("Consume Food")]
    public void ConsumeFood(ConsumerbleFood food)
    {
        print($"Consuming food: {food.foodName}");

        int CurrentHP = ScriptRegistry.Instance.textGameController.GetVariableUIElement("HP").currentValue;
        int newHP = CurrentHP + food.foodValue;
        print("new hpp" +  newHP );

        if (newHP > 100)
        {
            newHP = 100;
        }

        ScriptRegistry.Instance.textGameController.GetVariableUIElement("HP").UpdateUIElementDirect(newHP);

        AnalyticsEvents.instance.UniqueEvent($"food_Consumed_{food.foodName}");


        if (!ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
        {
            TextDialogueUIElement _textDialogueUIElement = Instantiate(textDialogueUIElement, contentHolder);
            ScriptRegistry.Instance.textGameController.textDialogueUIElements.Add(_textDialogueUIElement);

            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("hungerAlert").Hide();

            _textDialogueUIElement.EnableParameterUIElement("HP", food.foodValue);
            _textDialogueUIElement.EnableParameterUIElement("CASH", food.foodCost);
            _textDialogueUIElement.SetText($"<color=green>You ate {food.foodName}!</color>\nHunger reduced");
            ScriptRegistry.Instance.textGameController.ScrollToBottomSmooth();

        }


        if (ScriptRegistry.Instance.homeScreenManager.homeScreenOpen)
        {
            SaveGameManager.instance.SaveVariables();
        }

        for (int i = 0; i < foods.Length; i++)
        {
            foodButtons[i].Initialize(foods[i]);
        }
    }

    bool foodButtonsInitialized;
    public void InitializeFoodButtons()
    {
        if (foodButtonsInitialized)
        {
            return;
        }

        for (int i = 0; i < foods.Length; i++)
        {
            foodButtons[i].Initialize(foods[i]);
        }

        if (isFtuePlaying)
        {
            DisableButtons();
            foodButtons[0].Initialize(foods[0]);
        }

        foodButtonsInitialized = true;
    }

    public void UpdateFoodButtons()
    {
        for (int i = 0; i < foods.Length; i++)
        {
            foodButtons[i].UpdateUIElement(foods[i]);
        }
    }

    public float GetCurrentHunger()
    {
        return currentHunger;
    }

    public bool IsHungerPastTheshold()
    {
        return currentHunger >= criticalHungerThreshold;
    }


    public float GetHungerPercentage()
    {
        return currentHunger / maxHunger;
    }

    public void PurchaseFood(ConsumerbleFood food, bool consume)
    {
        int cash = ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").currentValue;

        if(cash >= food.foodCost)
        {
            food.foodCount++;
            cash-= food.foodCost;
            ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").UpdateUIElementDirect(cash);

            if(consume)
            {
                print($"Consuming food: {food.foodName}");
                ConsumeFood(food);
            }

            food.onUpdated?.Invoke(food);

        }


    }





}