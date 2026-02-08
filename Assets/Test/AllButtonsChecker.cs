using UnityEngine;

/// <summary>
/// Add this to scene to check ALL upgrade buttons at once
/// Finds duplicate StatTypes!
/// </summary>
public class AllButtonsChecker : MonoBehaviour
{
    [ContextMenu("Check All Buttons")]
    void CheckAllButtons()
    {
        Debug.Log("===== CHECKING ALL UPGRADE BUTTONS =====");
        
        UpgradeButtonUI[] buttons = FindObjectsOfType<UpgradeButtonUI>();
        
        Debug.Log($"Found {buttons.Length} UpgradeButtonUI components");
        
        if (buttons.Length == 0)
        {
            Debug.LogError("No UpgradeButtonUI components found!");
            return;
        }
        
        // Check each button
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            
            // Use reflection to get statType
            var statTypeField = typeof(UpgradeButtonUI).GetField("statType", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var statType = statTypeField?.GetValue(button);
            
            Debug.Log($"{i + 1}. '{button.gameObject.name}' → StatType: {statType}");
        }
        
        // Check for duplicates
        Debug.Log("");
        Debug.Log("===== CHECKING FOR DUPLICATES =====");
        
        bool foundDuplicate = false;
        
        for (int i = 0; i < buttons.Length; i++)
        {
            var statTypeField = typeof(UpgradeButtonUI).GetField("statType", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var statType1 = statTypeField?.GetValue(buttons[i]);
            
            for (int j = i + 1; j < buttons.Length; j++)
            {
                var statType2 = statTypeField?.GetValue(buttons[j]);
                
                if (statType1.ToString() == statType2.ToString())
                {
                    Debug.LogError($"❌ DUPLICATE! '{buttons[i].gameObject.name}' and '{buttons[j].gameObject.name}' both have StatType: {statType1}");
                    foundDuplicate = true;
                }
            }
        }
        
        if (!foundDuplicate)
        {
            Debug.Log("✅ No duplicates found! All buttons have unique StatTypes.");
        }
        
        Debug.Log("");
        Debug.Log("===== EXPECTED CONFIGURATION =====");
        Debug.Log("You should have 6 buttons with these StatTypes:");
        Debug.Log("1. Health");
        Debug.Log("2. Damage");
        Debug.Log("3. APS");
        Debug.Log("4. DPS");
        Debug.Log("5. Speed");
        Debug.Log("6. Range");
        
        Debug.Log("========================================");
    }
    
    [ContextMenu("Test Each Button")]
    void TestEachButton()
    {
        if (MonkeyStatsManager.Instance == null)
        {
            Debug.LogError("MonkeyStatsManager.Instance is null!");
            return;
        }
        
        Debug.Log("===== INITIAL STAT LEVELS =====");
        PrintAllStats();
        
        Debug.Log("");
        Debug.Log("Now click each button ONE TIME and watch which stat increases!");
    }
    
    void PrintAllStats()
    {
        Debug.Log($"Health: {MonkeyStatsManager.Instance.GetHealthLevel()}");
        Debug.Log($"Damage: {MonkeyStatsManager.Instance.GetDamageLevel()}");
        Debug.Log($"APS: {MonkeyStatsManager.Instance.GetAttackSpeedLevel()}");
        Debug.Log($"DPS: {MonkeyStatsManager.Instance.GetDPSLevel()}");
        Debug.Log($"Speed: {MonkeyStatsManager.Instance.GetMoveSpeedLevel()}");
        Debug.Log($"Range: {MonkeyStatsManager.Instance.GetRangeLevel()}");
    }
}