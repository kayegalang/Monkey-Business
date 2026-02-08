// UIButtonManager.cs
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
        WireButton("MenuButton", OnMenuButtonClick);
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

    public void OnMenuButtonClick()
    {
        GameSceneManager.Instance.ChangeState(GameSceneManager.State.Start);
    }

    // --- Main Menu ---
    public void OnStartButtonClick()
    {
        GameSceneManager.Instance.ChangeState(GameSceneManager.State.Battle);
    }

    // --- Pause Menu ---
    public void OnPauseButtonClick()
    {
        Debug.Log("Game is in paused state");
        GameSceneManager.Instance.ChangeState(GameSceneManager.State.Pause);
    }

    public void OnResumeButtonClick()
    {
        Debug.Log("Game resumed");
        GameSceneManager.Instance.ChangeState(GameSceneManager.State.Battle);
    }

    // --- Shared ---
    public void OnQuitButtonClick()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}