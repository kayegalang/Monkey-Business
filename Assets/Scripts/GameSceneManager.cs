using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    // Keep scene names in one place to avoid typos
    private const string START_SCENE = "StartScene";
    private const string BATTLE_SCENE = "BattleScene";
    private const string GAMEOVER_SCENE = "GameOverScene";
    private const string WIN_SCENE = "WinScene"; // <-- create/use this scene name

    public enum State { Start, Battle, Pause, GameOver, Win }
    public State state;

    public static GameSceneManager Instance { get; private set; }

    [Header("Data References")]
    [SerializeField] private BananaWallet wallet;
    [SerializeField] private UpgradeStat[] allUpgrades;

    private GameObject pauseShadePanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate GameSceneManager found! Destroying this one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (transform.parent != null)
        {
            Debug.LogWarning("GameSceneManager was nested under another GameObject. Moving to root for DontDestroyOnLoad.");
            transform.SetParent(null);
        }

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("✓ GameSceneManager initialized and set to DontDestroyOnLoad");
    }

    private void Start()
    {
        ChangeState(State.Start);
    }

    public void ChangeState(State newState)
    {
        Debug.Log($"ChangeState called: {state} → {newState}");
        state = newState;

        switch (newState)
        {
            case State.Start:
                LoadIfNotActive(START_SCENE);
                break;

            case State.Battle:
                if (SceneManager.GetActiveScene().name != BATTLE_SCENE)
                {
                    Debug.Log("Loading BattleScene...");
                    ResetAllData();
                    SceneManager.LoadScene(BATTLE_SCENE);
                }
                else
                {
                    // If already in battle, unpause
                    SetPausePanel(false);
                }
                break;

            case State.Pause:
                SetPausePanel(true);
                break;

            case State.GameOver:
                Debug.Log("Loading GameOverScene...");
                SceneManager.LoadScene(GAMEOVER_SCENE);
                break;

            case State.Win:
                Debug.Log("Loading WinScene...");
                SceneManager.LoadScene(WIN_SCENE);
                break;
        }
    }

    private void LoadIfNotActive(string sceneName)
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            Debug.Log($"Loading {sceneName}...");
            SceneManager.LoadScene(sceneName);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Debug.Log("GameSceneManager destroyed!");
            Instance = null;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        switch (scene.name)
        {
            case BATTLE_SCENE:
                pauseShadePanel = GameObject.Find("PauseShadePanel");
                Invoke(nameof(HidePausePanel), 0.1f);
                break;

            // Any non-battle scenes should not keep a pause panel reference
            case START_SCENE:
            case GAMEOVER_SCENE:
            case WIN_SCENE:
                pauseShadePanel = null;
                break;
        }
    }

    private void HidePausePanel()
    {
        if (pauseShadePanel != null)
            pauseShadePanel.SetActive(false);
    }

    private void ResetAllData()
    {
        if (wallet != null)
            wallet.Reset();

        // If you later want “upgrades reset on battle start”, do it here too.
        // (Your current code only resets wallet.)
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

    // Convenience methods so other scripts don’t need to know enum names
    public void WinGame() => ChangeState(State.Win);
    public void LoseGame() => ChangeState(State.GameOver);

    public void SetupReferences(BananaWallet walletRef, UpgradeStat[] upgradesRef)
    {
        wallet = walletRef;
        allUpgrades = upgradesRef;
        Debug.Log($"References set: Wallet={wallet != null}, Upgrades={upgradesRef?.Length ?? 0}");
    }
}
