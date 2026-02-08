// UIButtonManager.cs
using UnityEditor;
using UnityEngine;

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
    }

    // --- Main Menu ---
    public void OnStartButtonClick()
    {
        gameSceneManager.ChangeState(GameSceneManager.State.Battle);
    }

    // --- Pause Menu ---
    public void OnResumeButtonClick()
    {
        //what state to put it in to resume game
        //gameSceneManager.ChangeState(GameSceneManager.State.Battle);
    }

    public void OnPauseButtonClick()
    {
        Debug.Log("Game is in paused state");
        gameSceneManager.ChangeState(GameSceneManager.State.Pause);
        
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