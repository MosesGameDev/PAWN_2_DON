using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;


public class SimpleSceneManager : MonoBehaviour
{

    // Singleton instance
    private static SimpleSceneManager _instance;

    [SerializeField] private Image loadingProgress;

    public static event Action<string> OnSceneLoaded;
    public static event Action<string> OnSceneUnloaded;

    public static SimpleSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SimpleSceneManager");
                _instance = go.AddComponent<SimpleSceneManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // Track which scenes we've loaded additively
    private System.Collections.Generic.List<string> loadedScenes = new System.Collections.Generic.List<string>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Load a scene additively
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    /// <param name="showLoadingScreen">Whether to show loading screen</param>
    /// <param name="setAsActive">Whether to set the loaded scene as active</param>
    public void LoadSceneAdditive(string sceneName, bool setAsActive = false)
    {
        StartCoroutine(LoadSceneAdditiveCoroutine(sceneName,  setAsActive));
    }

    /// <summary>
    /// Unload a scene that was loaded additively
    /// </summary>
    /// <param name="sceneName">Name of the scene to unload</param>
    public void UnloadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            StartCoroutine(UnloadSceneCoroutine(sceneName));
        }
        else
        {
            Debug.LogWarning($"Scene {sceneName} is not loaded, cannot unload.");
        }
    }

    /// <summary>
    /// Unload the most recently loaded additive scene
    /// </summary>
    public void UnloadLastScene()
    {
        if (loadedScenes.Count > 0)
        {
            string lastScene = loadedScenes[loadedScenes.Count - 1];
            UnloadScene(lastScene);
        }
        else
        {
            Debug.LogWarning("No additively loaded scenes to unload.");
        }
    }

    /// <summary>
    /// Load a new scene and unload the current active scene
    /// </summary>
    /// <param name="sceneToLoad">Scene to load</param>
    /// <param name="unloadCurrentScene">Whether to unload the current scene</param>
    public void SwapScenes(string sceneToLoad, bool unloadCurrentScene = true)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(SwapScenesCoroutine(sceneToLoad, currentScene, unloadCurrentScene));
    }

    /// <summary>
    /// Unload all additively loaded scenes
    /// </summary>
    public void UnloadAllAdditiveScenes()
    {
        StartCoroutine(UnloadAllScenesCoroutine());
    }

    private IEnumerator LoadSceneAdditiveCoroutine(string sceneName,  bool setAsActive)
    {

        // Start loading the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Wait while the scene loads
        while (!asyncLoad.isDone)
        {
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("loadingBar").Show();
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Normalize to 0-1 range
            loadingProgress.fillAmount = progress;
            yield return null;
        }

        // Add the scene to our tracking list
        if (!loadedScenes.Contains(sceneName))
        {
            loadedScenes.Add(sceneName);
        }

        // Set as active scene if requested
        if (setAsActive)
        {
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(loadedScene);
        }

        ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("loadingBar").Hide();
        OnSceneLoaded?.Invoke(sceneName);
    }

    private IEnumerator UnloadSceneCoroutine(string sceneName)
    {
        // Start unloading the scene
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

        // Wait for the unload to complete
        while (asyncUnload != null && !asyncUnload.isDone)
        {
            float progress = Mathf.Clamp01(asyncUnload.progress / 0.9f);
            loadingProgress.fillAmount = progress;

            yield return null;
        }


        // Remove from our tracking list
        loadedScenes.Remove(sceneName);
        OnSceneUnloaded?.Invoke(sceneName);
        //Debug.Log($"Scene {sceneName} unloaded.");
    }

    private IEnumerator SwapScenesCoroutine(string sceneToLoad, string currentScene, bool unloadCurrentScene)
    {
        // First load the new scene additively
        yield return StartCoroutine(LoadSceneAdditiveCoroutine(sceneToLoad,true));

        // Then unload the current scene if requested
        if (unloadCurrentScene)
        {
            yield return StartCoroutine(UnloadSceneCoroutine(currentScene));
        }
    }

    private IEnumerator UnloadAllScenesCoroutine()
    {
        // Make a copy of the list since we'll be modifying it during iteration
        string[] scenesToUnload = loadedScenes.ToArray();

        foreach (string scene in scenesToUnload)
        {
            yield return StartCoroutine(UnloadSceneCoroutine(scene));
        }

        Debug.Log("All additive scenes unloaded.");
    }
}
