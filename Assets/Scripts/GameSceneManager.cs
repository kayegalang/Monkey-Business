using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private const string START_SCENE = "StartScene";
    private const string BATTLE_SCENE = "BattleScene";
    private const string GAMEOVER_SCENE = "GameOverScene";
    private const string WIN_SCENE = "WinScene";

    public enum State { Start, Battle, Pause, GameOver, Win }
    public State state;

    public static GameSceneManager Instance { get; private set; }

    [Header("Data References")]
    [SerializeField] private BananaWallet wallet;
    [SerializeField] private UpgradeStat[] allUpgrades;

    [Header("UI References - Assign in Inspector")]
    [SerializeField] private GameObject pauseShadePanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("✓ GameSceneManager initialized");
    }

    private void Start()
    {
        ChangeState(State.Start);
    }

    public void ChangeState(State newState)
    {
        Debug.Log($"ChangeState: {state} → {newState}");
        state = newState;

        switch (newState)
        {
            case State.Start:
                Time.timeScale = 1f;
                LoadIfNotActive(START_SCENE);
                break;

            case State.Battle:
                if (SceneManager.GetActiveScene().name != BATTLE_SCENE)
                {
                    Time.timeScale = 1f;
                    ResetAllData();
                    SceneManager.LoadScene(BATTLE_SCENE);
                }
                else
                {
                    // Unpause
                    Time.timeScale = 1f;
                    SetPausePanel(false);
                }
                break;

            case State.Pause:
                Time.timeScale = 0f;
                SetPausePanel(true);
                break;

            case State.GameOver:
                Time.timeScale = 1f;
                SceneManager.LoadScene(GAMEOVER_SCENE);
                break;

            case State.Win:
                Time.timeScale = 1f;
                SceneManager.LoadScene(WIN_SCENE);
                break;
        }
    }

    private void LoadIfNotActive(string sceneName)
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        // Find pause panel in BattleScene
        if (scene.name == BATTLE_SCENE)
        {
            if (pauseShadePanel == null)
            {
                pauseShadePanel = GameObject.Find("PauseShadePanel");
                if (pauseShadePanel != null)
                {
                    Debug.Log("✓ Found PauseShadePanel");
                }
            }
            
            // Make sure it starts hidden
            if (pauseShadePanel != null)
            {
                pauseShadePanel.SetActive(false);
            }
        }
    }

    private void ResetAllData()
    {
        if (wallet != null)
            wallet.Reset();
    }

    private void SetPausePanel(bool active)
    {
        if (pauseShadePanel == null)
        {
            Debug.LogWarning("PauseShadePanel reference is null!");
            return;
        }

        pauseShadePanel.SetActive(active);
        Debug.Log($"PauseShadePanel set to: {active}");
    }

    // Public methods for buttons to call
    public void StartGame() => ChangeState(State.Battle);
    public void PauseGame() => ChangeState(State.Pause);
    public void ResumeGame() => ChangeState(State.Battle);
    public void GoToMenu() => ChangeState(State.Start);
    public void WinGame() => ChangeState(State.Win);
    public void LoseGame() => ChangeState(State.GameOver);
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void SetupReferences(BananaWallet walletRef, UpgradeStat[] upgradesRef)
    {
        wallet = walletRef;
        allUpgrades = upgradesRef;
    }
}