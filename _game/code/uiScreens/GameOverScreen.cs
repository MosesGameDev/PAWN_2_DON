using UnityEngine;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    public void Show(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("gameOver").Show();
    }

    public void Hide()
    {
        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("gameOver").Hide();
    }
}
