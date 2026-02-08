using UnityEngine;

/// <summary>
/// Resets all upgrades when you press Play - FOR TESTING ONLY!
/// Add to MonkeyStatsManager GameObject and check "Reset On Play"
/// </summary>
public class TestingResetHelper : MonoBehaviour
{
    [Header("Testing Controls")]
    [SerializeField] private bool resetOnPlay = true;
    [SerializeField] private bool resetOnSceneLoad = false;
    [SerializeField] private KeyCode resetHotkey = KeyCode.R;
    
    [Header("What to Reset")]
    [SerializeField] private bool resetMonkeyProgress = true;
    [SerializeField] private bool resetBananaWallet = true;
    [SerializeField] private bool giveStartingBananas = true;
    [SerializeField] private int startingBananas = 10000;
    
    [Header("References")]
    [SerializeField] private BananaWallet wallet;
    
    void Start()
    {
        if (resetOnPlay)
        {
            ResetEverything();
        }
    }
    
    void Update()
    {
        // Press hotkey to reset during play
        if (Input.GetKeyDown(resetHotkey))
        {
            Debug.Log($"🔄 Reset hotkey ({resetHotkey}) pressed!");
            ResetEverything();
        }
    }
    
    [ContextMenu("Reset Everything Now")]
    public void ResetEverything()
    {
        Debug.Log("🔄 RESETTING ALL PROGRESS FOR TESTING...");
        
        // Reset monkey progress
        if (resetMonkeyProgress && MonkeyStatsManager.Instance != null)
        {
            MonkeyStatsManager.Instance.ResetAllProgress();
            Debug.Log("✓ Monkey progress reset");
        }
        
        // Reset bananas
        if (resetBananaWallet && wallet != null)
        {
            wallet.Reset();
            Debug.Log("✓ Bananas reset to 0");
            
            if (giveStartingBananas)
            {
                wallet.Add(startingBananas);
                Debug.Log($"✓ Added {startingBananas} starting bananas");
            }
        }
        
        Debug.Log("🔄 RESET COMPLETE!");
    }
    
    [ContextMenu("Give 10,000 Bananas")]
    public void GiveBananas()
    {
        if (wallet != null)
        {
            wallet.Add(10000);
            Debug.Log("💰 Added 10,000 bananas!");
        }
    }
    
    [ContextMenu("Give 100,000 Bananas")]
    public void GiveLotsOfBananas()
    {
        if (wallet != null)
        {
            wallet.Add(100000);
            Debug.Log("💰 Added 100,000 bananas!");
        }
    }
    
    [ContextMenu("Max Level (10)")]
    public void MaxLevel()
    {
        if (MonkeyStatsManager.Instance != null)
        {
            for (int i = 0; i < 9; i++)
            {
                MonkeyStatsManager.Instance.TryLevelUp();
            }
            Debug.Log("⬆️ Leveled to 10!");
        }
    }
    
    [ContextMenu("Unlock Evolution")]
    public void UnlockEvolution()
    {
        if (wallet != null)
        {
            wallet.Add(100000);
        }
        
        if (MonkeyStatsManager.Instance != null)
        {
            for (int i = 0; i < 9; i++)
            {
                MonkeyStatsManager.Instance.TryLevelUp();
            }
            Debug.Log("🎉 Ready to evolve! (Level 10 + bananas)");
        }
    }
}