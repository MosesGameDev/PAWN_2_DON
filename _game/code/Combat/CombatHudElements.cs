using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatHudElements : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image healthFillImage;

    [Header("UI Dialogue")]
    [SerializeField] private UIDialogueElement healthUIElement;

    [Header("Follow Settings")]
    public Transform targetToFollow;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0); // Offset above character

    private Camera mainCamera;
    private RectTransform rectTransform;

    [Space]
    [SerializeField] private float maxHealth;

    private void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }


    private void Update()
    {
        if (targetToFollow == null) return;

        // Convert world position to screen position
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetToFollow.position + offset);

        // Update UI position
        if (screenPos.z > 0) // Only show if target is in front of camera
        {
            rectTransform.position = screenPos;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void ShowHealth()
    {
        healthUIElement.Show();
    }

    public void HideHealth()
    {
        healthUIElement.Hide();
    }


    // Public method to update the health bar fill amount
    public void SetHealthFill(float currentHealth)
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        targetToFollow = newTarget;
    }

    private void OnBattleStart()
    {
        gameObject.SetActive(true);
        //maxHealth = isPlayer ? combatController.playerCharacter.currentHealth : combatController.npcCharacter.c_crewMember.currentHealth;
        SetHealthFill(maxHealth); // Set initial health
    }

    public void OnBattleEnd()
    {
        gameObject.SetActive(false);
    }
}