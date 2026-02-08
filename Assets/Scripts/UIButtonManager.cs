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
        DontDestroyOnLoad(gameObject);
        
        // Make sure it's at root level
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

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
        WireButton("ResumeButton", OnResumeButtonClick);
        WireButton("QuitButton", OnQuitButtonClick);
        WireButton("MenuButton", OnMenuButtonClick);
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

        // Remove existing listeners to avoid duplicates
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
        
        // Add animation component
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
        
        // Extra safety check with detailed logging
        if (GameSceneManager.Instance == null)
        {
            Debug.LogError("❌ GameSceneManager.Instance is NULL!");
            Debug.LogError("Searching for GameSceneManager in scene...");
            
            // Try to find it manually
            GameSceneManager manager = FindObjectOfType<GameSceneManager>();
            if (manager != null)
            {
                Debug.LogError("Found GameSceneManager but Instance is not set! This is a bug in GameSceneManager.Awake()");
            }
            else
            {
                Debug.LogError("GameSceneManager does not exist in the scene at all!");
            }
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