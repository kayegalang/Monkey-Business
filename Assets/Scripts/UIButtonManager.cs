



// UIButtonManager.cs
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtonManager : MonoBehaviour
{
    [Header("Script References")]
    public GameSceneManager gameSceneManager;

    public static UIButtonManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- find and wire buttons when a new scene loads ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "BattleScene":
                WireBattleButtons();
                break;
        }
    }

    private void WireBattleButtons()
    {
        WireButton("PauseButton", OnPauseButtonClick);
        WireButton("ResumeButton", OnResumeButtonClick);
        WireButton("QuitButton", OnQuitButtonClick);
    }

    private void WireButton(string buttonName, UnityEngine.Events.UnityAction action)
    {
        GameObject obj = GameObject.Find(buttonName);
        if (obj == null)
        {
            Debug.LogWarning($"Button '{buttonName}' not found!");
            return;
        }

        Button button = obj.GetComponent<Button>();
        button.onClick.AddListener(action);
        Debug.Log($"Wired: {buttonName}");
    }

    // --- Main Menu ---
    public void OnStartButtonClick()
    {
        gameSceneManager.ChangeState(GameSceneManager.State.Battle);
    }

    // --- Pause Menu (wired by code in BattleScene) ---
    public void OnPauseButtonClick()
    {
        Debug.Log("Game is in paused state");
        gameSceneManager.ChangeState(GameSceneManager.State.Pause);
    }

    public void OnResumeButtonClick()
    {
        Debug.Log("Game resumed");
        gameSceneManager.ChangeState(GameSceneManager.State.Battle);
    }

    // --- Shared ---
    public void OnQuitButtonClick()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        Debug.Log("Application Quit!");
        #endif
    }
}


