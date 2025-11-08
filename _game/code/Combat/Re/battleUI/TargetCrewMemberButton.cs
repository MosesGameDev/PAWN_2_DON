using UnityEngine;
using UnityEngine.UI;

public class TargetCrewMemberButton : MonoBehaviour
{
    public Button button;
    [SerializeField] private Image characterImage;
    [SerializeField] private Image healthFillImage;
    

    public void UpdateUIElement(CrewMember crewMember)
    {
        characterImage.sprite = crewMember.characterImage;
        healthFillImage.fillAmount = crewMember.currentHealth / crewMember.maxHealth;
    }
}
