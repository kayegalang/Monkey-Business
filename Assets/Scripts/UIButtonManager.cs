using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtonManager : MonoBehaviour
{
    public static UIButtonManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate UIButtonManager found! Destroying this one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        if (transform.parent != null)
        {
            Debug.LogWarning("UIButtonManager was nested under another GameObject. Moving to root for DontDestroyOnLoad.");
            transform.SetParent(null);
        }
        
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        Debug.Log("✓ UIButtonManager initialized and set to DontDestroyOnLoad");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"UIButtonManager wiring buttons in scene: {scene.name}");
        
        WireButton("StartButton", OnStartButtonClick);
        WireButton("PauseButton", OnPauseButtonClick);
        WireButton("QuitButton", OnQuitButtonClick);
        
        // NEW: Wire pause menu buttons immediately if we're in BattleScene
        if (scene.name == "BattleScene")
        {
            // Use Invoke to ensure hierarchy is fully loaded
            Invoke(nameof(WirePauseMenuButtons), 0.1f);
        }
    }

    // NEW METHOD: Wire pause menu buttons by searching within the pause panel
    public void WirePauseMenuButtons()
    {
        GameObject pausePanel = GameObject.Find("PauseShadePanel");
        if (pausePanel == null)
        {
            Debug.LogWarning("PauseShadePanel not found in scene!");
            return;
        }

        // Temporarily activate to find children, then restore state
        bool wasActive = pausePanel.activeSelf;
        pausePanel.SetActive(true);

        // Find buttons within the pause panel
        WireButtonInParent(pausePanel.transform, "ResumeButton", OnResumeButtonClick);
        WireButtonInParent(pausePanel.transform, "MenuButton", OnMenuButtonClick);
        WireButtonInParent(pausePanel.transform, "QuitButton", OnQuitButtonClick);

        // Restore original state
        pausePanel.SetActive(wasActive);
        
        Debug.Log("✓ Pause menu buttons wired");
    }

    // NEW HELPER: Find button within a parent transform
    private void WireButtonInParent(Transform parent, string buttonName, UnityEngine.Events.UnityAction action)
    {
        Transform buttonTransform = parent.Find(buttonName);
        if (buttonTransform == null)
        {
            // Try recursive search
            buttonTransform = FindInChildren(parent, buttonName);
        }

        if (buttonTransform == null)
        {
            Debug.LogWarning($"Button '{buttonName}' not found in pause menu!");
            return;
        }

        Button button = buttonTransform.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning($"Button component not found on '{buttonName}'!");
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
        
        ButtonAnimator animator = buttonTransform.GetComponent<ButtonAnimator>();
        if (animator == null)
        {
            buttonTransform.gameObject.AddComponent<ButtonAnimator>();
        }
        
        Debug.Log($"✓ Wired pause menu button: {buttonName}");
    }

    // Helper to recursively find a child by name
    private Transform FindInChildren(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            
            Transform found = FindInChildren(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    private void WireButton(string buttonName, UnityEngine.Events.UnityAction action)
    {
        GameObject obj = GameObject.Find(buttonName);
        if (obj == null)
        {
            return; // Button not in this scene
        }

        Button button = obj.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning($"Button component not found on '{buttonName}'!");
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
        
        ButtonAnimator animator = obj.GetComponent<ButtonAnimator>();
        if (animator == null)
        {
            obj.AddComponent<ButtonAnimator>();
        }
        
        Debug.Log($"✓ Wired button: {buttonName}");
    }

    public void OnMenuButtonClick()
    {
        Debug.Log("Menu button clicked");
        
        if (GameSceneManager.Instance == null)
        {
            Debug.LogError("❌ GameSceneManager.Instance is NULL!");
            return;
        }
        
        Debug.Log("Calling ChangeState(Start)...");
        GameSceneManager.Instance.ChangeState(GameSceneManager.State.Start);
    }

    public void OnStartButtonClick()
    {
        Debug.Log("Start button clicked");
        if (GameSceneManager.Instance != null)
            GameSceneManager.Instance.ChangeState(GameSceneManager.State.Battle);
        else
            Debug.LogError("❌ GameSceneManager.Instance is NULL!");
    }

    public void OnPauseButtonClick()
    {
        Debug.Log("Pause button clicked");
        if (GameSceneManager.Instance != null)
            GameSceneManager.Instance.ChangeState(GameSceneManager.State.Pause);
        else
            Debug.LogError("❌ GameSceneManager.Instance is NULL!");
    }

    public void OnResumeButtonClick()
    {
        Debug.Log("Resume button clicked");
        if (GameSceneManager.Instance != null)
            GameSceneManager.Instance.ChangeState(GameSceneManager.State.Battle);
        else
            Debug.LogError("❌ GameSceneManager.Instance is NULL!");
    }

    public void OnQuitButtonClick()
    {
        Debug.Log("Quit button clicked");
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}