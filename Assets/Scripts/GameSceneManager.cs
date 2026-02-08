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
        DontDestroyOnLoad(gameObject);
        
        // Make sure the GameObject won't be destroyed
        if (transform.parent != null)
        {
            Debug.LogWarning("GameSceneManager should not have a parent! Moving to root.");
            transform.SetParent(null);
        }

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
            Debug.LogError("GameSceneManager Instance is being destroyed! This should not happen!");
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
 
        if (allUpgrades != null)
        {
            foreach (var upgrade in allUpgrades)
            {
                if (upgrade != null)
                    upgrade.Reset();
            }
        }
 
        Debug.Log("All data reset!");
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
}