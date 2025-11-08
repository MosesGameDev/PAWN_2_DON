using DG.Tweening;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static GameAnalyticsController;
public class ResetGame : MonoBehaviour
{
    string[] objectNames = {
"DIALOGUE_MANAGER",
"AdsManager",
"REGISTRY",
"Vibrations",
"SCENEMANAGER",
"[Debug Updater]",
"[DOTween]",
"AdsManager"
    };

    private void Start()
    {
        // Optionally, you can call ResetAll() here or bind it to a UI button click
        Invoke(nameof(ResetAll), 1.5f);
    }

    int resetCount;
    int completed;

    public void ResetAll()
    {
        // Read before wiping
        resetCount = PlayerPrefs.GetInt("RESET_COUNT", 0);
        completed = PlayerPrefs.GetInt("COMPLETED", 0);

        print($"<color=red>reset count {resetCount}</color>");

        // Delete everything EXCEPT the count
        PlayerPrefs.DeleteAll();

        // Restore the count and increment
        resetCount++;
        PlayerPrefs.SetInt("RESET_COUNT", resetCount);

        // Keep your other data logic
        PlayerPrefs.SetInt("INITIALIZED", 1);
        PlayerPrefs.SetInt("COMPLETED", completed + 1);

        foreach (string objectName in objectNames)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj != null)
                Destroy(obj);
        }

        SceneManager.LoadScene(1);
    }
}
