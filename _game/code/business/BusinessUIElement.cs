using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BusinessUIElement : MonoBehaviour
{
    public GameObject tapToUnlockGraphic;
    public GameObject businessLockedGraphic;

    [Space]
    public Button unlockButton;
    public Button upgradeButton;

    [Space]
    public TextMeshProUGUI unlockButtonText;
    public TextMeshProUGUI upgradeButtonText;
    public TextMeshProUGUI[] businessTitleText;
}
