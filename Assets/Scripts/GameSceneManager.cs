
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    public enum State { Start, Battle, Pause, Results }
    public State state;

    private GameObject pauseShadePanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // need this to find stuff in new scenes
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
                pauseShadePanel = GameObject.Find("PauseShadePanel");
                // small delay so UIButtonManager wires buttons first
                Invoke(nameof(HidePausePanel), 0.05f);
                break;

            case "StartScene":
                pauseShadePanel = null;
                break;
        }
    }

    void Start() => ChangeState(State.Start);

    public void ChangeState(State newState)
    {
        state = newState;
        switch (newState)
        {
            case State.Start:
                if (SceneManager.GetActiveScene().name != "StartScene")
                    SceneManager.LoadScene("StartScene");
                break;

            case State.Battle:
                if (SceneManager.GetActiveScene().name != "BattleScene")
                    SceneManager.LoadScene("BattleScene");
                else
                    SetPausePanel(false);  // resuming from pause
                break;

            case State.Pause:
                SetPausePanel(true);
                break;

            case State.Results:
                break;
        }
    }

    private void SetPausePanel(bool active)
    {
        if (pauseShadePanel == null)
        {
            Debug.LogWarning("PauseShadePanel not found!");
            return;
        }

        pauseShadePanel.SetActive(active);
    }
    
    private void HidePausePanel()
    {
        if (pauseShadePanel != null)
            pauseShadePanel.SetActive(false);
    }
}


