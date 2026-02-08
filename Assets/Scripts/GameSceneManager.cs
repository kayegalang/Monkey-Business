using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{ 
    public enum State {Start, Battle, Pause, GameOver}
    public State state;
   
    public static GameSceneManager Instance { get; private set; }

    [Header("Data References")]
    [SerializeField] private BananaWallet wallet;
    [SerializeField] private UpgradeStat[] allUpgrades;

    private GameObject pauseShadePanel;
   
    private void Awake()
    {
        // Check if instance already exists
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate GameSceneManager found! Destroying this one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        // IMPORTANT: Move to root if nested
        if (transform.parent != null)
        {
            Debug.LogWarning("GameSceneManager was nested under another GameObject. Moving to root for DontDestroyOnLoad.");
            transform.SetParent(null);
        }
        
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        
        Debug.Log("✓ GameSceneManager initialized and set to DontDestroyOnLoad");
    }

    void Start()
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
                if (SceneManager.GetActiveScene().name != "StartScene")
                {
                    Debug.Log("Loading StartScene...");
                    SceneManager.LoadScene("StartScene");
                }
                break;
                
            case State.Battle:
                if (SceneManager.GetActiveScene().name != "BattleScene")
                {
                    Debug.Log("Loading BattleScene...");
                    ResetAllData();
                    SceneManager.LoadScene("BattleScene");
                }
                else
                {
                    SetPausePanel(false);
                }
                break;
                
            case State.Pause:
                SetPausePanel(true);
                break;
                
            case State.GameOver:
                Debug.Log("Loading GameOverScene...");
                SceneManager.LoadScene("GameOverScene");
                break;
        }
    }
   
    private void OnDestroy()
    {
        // Only clear instance if we're the actual singleton
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
            case "BattleScene":
                pauseShadePanel = GameObject.Find("PauseShadePanel");
                Invoke(nameof(HidePausePanel), 0.1f);
                break;

            case "StartScene":
            case "GameOverScene":
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
    
    // Add this method for setting up references from Bootstrap
    public void SetupReferences(BananaWallet walletRef, UpgradeStat[] upgradesRef)
    {
        wallet = walletRef;
        allUpgrades = upgradesRef;
        Debug.Log($"References set: Wallet={wallet != null}, Upgrades={upgradesRef?.Length ?? 0}");
    }
}