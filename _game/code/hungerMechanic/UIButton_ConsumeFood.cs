using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Coffee.UIExtensions;
public class UIButton_ConsumeFood : MonoBehaviour
{
    [SerializeField] private ConsumerbleFood food;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI foodNameText;
    [SerializeField] private TextMeshProUGUI foodValueText;

    [Space]
    [SerializeField] private ParticleSystem particles;

    [Space]
    public Button buttonConsume;
    [SerializeField] private TextMeshProUGUI buttonTextConsume;

    [Space]
    public  Button buttonBuy;
    [SerializeField] private TextMeshProUGUI buttonTextBuy;


    public static event System.Action<ConsumerbleFood> OnFoodConsumed;

    public void Initialize(ConsumerbleFood _food)
    {
        gameObject.SetActive(true);

        food = _food;
        food.onUpdated += UpdateUIElement;
        UpdateUIElement(food);
    }

    private void OnDestroy()
    {
        if (food != null)
        {
            food.onUpdated -= UpdateUIElement;
        }
    }

    bool particlesInitialized = false;

    public void UpdateUIElement(ConsumerbleFood _food)
    {
        int cash = ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").currentValue;

        icon.sprite = _food.icon;
        foodNameText.text = _food.foodName;
        foodValueText.text = $"{_food.foodValue}%";

        if (food.foodCost >= cash)
        {
            if (food.foodCount > 0)
            {
                buttonBuy.gameObject.SetActive(false);
                buttonConsume.gameObject.SetActive(true);

                buttonConsume.interactable = true;

                return;
            }

            buttonConsume.gameObject.SetActive(false);
            buttonBuy.gameObject.SetActive(true);
            buttonBuy.interactable = false;

            buttonTextBuy.text = $"<color=red>Buy {food.foodCost}$</color>";
        }
        else
        {
            if (food.foodCount > 0)
            {
                buttonBuy.gameObject.SetActive(false);
                buttonConsume.gameObject.SetActive(true);

                buttonConsume.interactable = true;


            }
            else
            {
                buttonBuy.interactable = true;
                buttonBuy.gameObject.SetActive(true);

                buttonTextBuy.text = $"<color=white>Buy {food.foodCost}$</color>";
            }
        }
    }

    public void ConsumeFood()
    {
        ScriptRegistry.Instance.hungerHandler.ConsumeFood(food);
        food.foodCount--;
        UpdateUIElement(food);

        if (!particlesInitialized)
        {
            UIParticleAttractor uIParticleAttractor = ScriptRegistry.Instance.textGameController.GetVariableUIElement("HP").UIParticleAttractor;
            uIParticleAttractor.AddParticleSystem(particles);

            uIParticleAttractor.onAttracted.AddListener(() =>
            {
                int currentHP = ScriptRegistry.Instance.textGameController.GetVariableUIElement("HP").currentValue;
                int newHP = (food.foodValue) / 5;
            });

            particlesInitialized = true;
        }

        particles.Play();

        UIDialogueElement dialogue = ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("foodAnimatedPointer");

        if(dialogue.isOpen)
        {
            dialogue.Hide();
        }

        OnFoodConsumed?.Invoke(food);

    }

    public void BuyFood()
    {
        ScriptRegistry.Instance.hungerHandler.PurchaseFood(food, true);
        UpdateUIElement(food);
        ConsumeFood();

        UIDialogueElement dialogue = ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("foodAnimatedPointer");

        if (dialogue.isOpen)
        {
            dialogue.Hide();
        }
    }
}
