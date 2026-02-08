using UnityEngine;

/// <summary>
/// Real-time upgrade tester - Add to scene to verify upgrades work!
/// Press keys 1-6 to test each upgrade
/// </summary>
public class UpgradeTester : MonoBehaviour
{
    [Header("Auto-give bananas on start")]
    [SerializeField] private bool giveStartingBananas = true;
    [SerializeField] private int startingBananas = 10000;
    
    [Header("References")]
    [SerializeField] private BananaWallet wallet;
    
    void Start()
    {
        if (giveStartingBananas && wallet != null)
        {
            wallet.Add(startingBananas);
            Debug.Log($"💰 Gave {startingBananas} starting bananas for testing!");
        }
        
        Debug.Log("=== UPGRADE TESTER READY ===");
        Debug.Log("Press 1-6 to test upgrades:");
        Debug.Log("1 = Health");
        Debug.Log("2 = Damage");
        Debug.Log("3 = APS");
        Debug.Log("4 = DPS (same as APS)");
        Debug.Log("5 = Speed");
        Debug.Log("6 = Range");
        Debug.Log("7 = Monkey Level Up");
        Debug.Log("8 = Class Up");
        Debug.Log("R = Reset All Progress");
        Debug.Log("S = Show Current Stats");
    }
    
    void Update()
    {
        if (MonkeyStatsManager.Instance == null) return;
        
        // Test upgrades with number keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestUpgrade("Health", () => MonkeyStatsManager.Instance.TryUpgradeHealth());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestUpgrade("Damage", () => MonkeyStatsManager.Instance.TryUpgradeDamage());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestUpgrade("APS", () => MonkeyStatsManager.Instance.TryUpgradeAttackSpeed());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TestUpgrade("DPS", () => MonkeyStatsManager.Instance.TryUpgradeDPS());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TestUpgrade("Speed", () => MonkeyStatsManager.Instance.TryUpgradeMoveSpeed());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            TestUpgrade("Range", () => MonkeyStatsManager.Instance.TryUpgradeRange());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            TestUpgrade("Monkey Level", () => MonkeyStatsManager.Instance.TryLevelUp());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            TestUpgrade("Class Up", () => MonkeyStatsManager.Instance.TryEvolve());
        }
        
        // Reset progress
        if (Input.GetKeyDown(KeyCode.R))
        {
            MonkeyStatsManager.Instance.ResetAllProgress();
            Debug.Log("🔄 Progress reset!");
        }
        
        // Show current stats
        if (Input.GetKeyDown(KeyCode.S))
        {
            ShowCurrentStats();
        }
    }
    
    void TestUpgrade(string name, System.Func<bool> upgradeFunc)
    {
        Debug.Log($"");
        Debug.Log($"=== Testing {name} Upgrade ===");
        
        // Show stats BEFORE
        Debug.Log("BEFORE:");
        ShowCurrentStats();
        
        // Try upgrade
        bool success = upgradeFunc();
        
        if (success)
        {
            Debug.Log($"✅ {name} upgraded successfully!");
            
            // Show stats AFTER
            Debug.Log("AFTER:");
            ShowCurrentStats();
        }
        else
        {
            Debug.LogWarning($"❌ {name} upgrade failed! (Not enough bananas or maxed out?)");
            
            if (wallet != null)
            {
                Debug.Log($"Current bananas: {wallet.Bananas}");
            }
        }
    }
    
    void ShowCurrentStats()
    {
        if (MonkeyStatsManager.Instance == null)
        {
            Debug.LogError("MonkeyStatsManager.Instance is NULL!");
            return;
        }
        
        Debug.Log("──────────────────────────────");
        Debug.Log($"Form: {MonkeyStatsManager.Instance.GetCurrentFormName()}");
        Debug.Log($"Monkey Level: {MonkeyStatsManager.Instance.GetMonkeyLevel()}");
        Debug.Log($"");
        Debug.Log($"Health:  {MonkeyStatsManager.Instance.GetCurrentHealth():F1} (Lv {MonkeyStatsManager.Instance.GetHealthLevel()}/{MonkeyStatsManager.Instance.GetHealthMaxLevel()})");
        Debug.Log($"Damage:  {MonkeyStatsManager.Instance.GetCurrentDamage():F1} (Lv {MonkeyStatsManager.Instance.GetDamageLevel()}/{MonkeyStatsManager.Instance.GetDamageMaxLevel()})");
        Debug.Log($"APS:     {MonkeyStatsManager.Instance.GetCurrentAttackSpeed():F2} (Lv {MonkeyStatsManager.Instance.GetAttackSpeedLevel()}/{MonkeyStatsManager.Instance.GetAttackSpeedMaxLevel()})");
        Debug.Log($"DPS:     {MonkeyStatsManager.Instance.GetCurrentDPS():F1} (calculated)");
        Debug.Log($"Speed:   {MonkeyStatsManager.Instance.GetCurrentMoveSpeed():F1} (Lv {MonkeyStatsManager.Instance.GetMoveSpeedLevel()}/{MonkeyStatsManager.Instance.GetMoveSpeedMaxLevel()})");
        Debug.Log($"Range:   {MonkeyStatsManager.Instance.GetCurrentRange():F1} (Lv {MonkeyStatsManager.Instance.GetRangeLevel()}/{MonkeyStatsManager.Instance.GetRangeMaxLevel()})");
        
        if (wallet != null)
        {
            Debug.Log($"");
            Debug.Log($"Bananas: {wallet.Bananas} 🍌");
        }
        Debug.Log("──────────────────────────────");
    }
    
    [ContextMenu("Give 10,000 Bananas")]
    void Give10000Bananas()
    {
        if (wallet != null)
        {
            wallet.Add(10000);
            Debug.Log("💰 Added 10,000 bananas!");
        }
    }
    
    [ContextMenu("Max All Stats")]
    void MaxAllStats()
    {
        if (MonkeyStatsManager.Instance == null) return;
        
        Debug.Log("🚀 Maxing all stats...");
        
        // Give lots of bananas
        if (wallet != null)
        {
            wallet.Add(100000);
        }
        
        // Max each stat
        while (MonkeyStatsManager.Instance.TryUpgradeHealth()) { }
        while (MonkeyStatsManager.Instance.TryUpgradeDamage()) { }
        while (MonkeyStatsManager.Instance.TryUpgradeAttackSpeed()) { }
        while (MonkeyStatsManager.Instance.TryUpgradeMoveSpeed()) { }
        while (MonkeyStatsManager.Instance.TryUpgradeRange()) { }
        
        Debug.Log("✅ All stats maxed!");
        ShowCurrentStats();
    }
}