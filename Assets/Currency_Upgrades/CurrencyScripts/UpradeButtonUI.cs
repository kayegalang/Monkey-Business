using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonUI : MonoBehaviour
{
    [Header("Which Stat?")]
    [SerializeField] private StatType statType;
    
    [Header("References")]
    [SerializeField] private BananaWallet wallet;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button button;
    
    public enum StatType
    {
        Health,
        Damage,
        APS,
        DPS,
        Speed,
        Range
    }
    
    void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(OnClick);
        
        if (wallet != null)
            wallet.OnBananasChanged += OnBananasChanged;
        
        if (MonkeyStatsManager.Instance != null)
            MonkeyStatsManager.Instance.OnStatsChanged += RefreshUI;
        
        RefreshUI();
    }
    
    void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClick);
        
        if (wallet != null)
            wallet.OnBananasChanged -= OnBananasChanged;
        
        if (MonkeyStatsManager.Instance != null)
            MonkeyStatsManager.Instance.OnStatsChanged -= RefreshUI;
    }
    
    void OnClick()
    {
        if (MonkeyStatsManager.Instance == null) return;
        
        bool success = statType switch
        {
            StatType.Health => MonkeyStatsManager.Instance.TryUpgradeHealth(),
            StatType.Damage => MonkeyStatsManager.Instance.TryUpgradeDamage(),
            StatType.APS => MonkeyStatsManager.Instance.TryUpgradeAttackSpeed(),
            StatType.DPS => MonkeyStatsManager.Instance.TryUpgradeDPS(),
            StatType.Speed => MonkeyStatsManager.Instance.TryUpgradeMoveSpeed(),
            StatType.Range => MonkeyStatsManager.Instance.TryUpgradeRange(),
            _ => false
        };
        
        if (success)
        {
            RefreshUI();
        }
    }
    
    void OnBananasChanged(int _) => RefreshUI();
    
    void RefreshUI()
    {
        if (MonkeyStatsManager.Instance == null) return;
        
        // Get current level, cost, and max from MonkeyStatsManager (not ScriptableObject!)
        int currentLevel = GetCurrentLevel();
        int maxLevel = GetMaxLevel();
        int cost = GetCost();
        string statName = GetStatName();
        
        // Update UI
        if (nameText != null)
            nameText.text = statName;
        
        if (levelText != null)
            levelText.text = $"Lv {currentLevel}/{maxLevel}";
        
        bool isMaxed = currentLevel >= maxLevel;
        
        if (costText != null)
        {
            if (isMaxed)
                costText.text = "MAX";
            else
                costText.text = $"{cost} 🍌";
        }
        
        if (button != null)
        {
            if (isMaxed)
            {
                button.interactable = false;
            }
            else
            {
                int bananas = wallet != null ? wallet.Bananas : 0;
                button.interactable = bananas >= cost;
            }
        }
    }
    
    int GetCurrentLevel()
    {
        return statType switch
        {
            StatType.Health => MonkeyStatsManager.Instance.GetHealthLevel(),
            StatType.Damage => MonkeyStatsManager.Instance.GetDamageLevel(),
            StatType.APS => MonkeyStatsManager.Instance.GetAttackSpeedLevel(),
            StatType.DPS => MonkeyStatsManager.Instance.GetDPSLevel(),
            StatType.Speed => MonkeyStatsManager.Instance.GetMoveSpeedLevel(),
            StatType.Range => MonkeyStatsManager.Instance.GetRangeLevel(),
            _ => 0
        };
    }
    
    int GetMaxLevel()
    {
        return statType switch
        {
            StatType.Health => MonkeyStatsManager.Instance.GetHealthMaxLevel(),
            StatType.Damage => MonkeyStatsManager.Instance.GetDamageMaxLevel(),
            StatType.APS => MonkeyStatsManager.Instance.GetAttackSpeedMaxLevel(),
            StatType.DPS => MonkeyStatsManager.Instance.GetDPSMaxLevel(),
            StatType.Speed => MonkeyStatsManager.Instance.GetMoveSpeedMaxLevel(),
            StatType.Range => MonkeyStatsManager.Instance.GetRangeMaxLevel(),
            _ => 0
        };
    }
    
    int GetCost()
    {
        return statType switch
        {
            StatType.Health => MonkeyStatsManager.Instance.GetHealthCost(),
            StatType.Damage => MonkeyStatsManager.Instance.GetDamageCost(),
            StatType.APS => MonkeyStatsManager.Instance.GetAttackSpeedCost(),
            StatType.DPS => MonkeyStatsManager.Instance.GetDPSCost(),
            StatType.Speed => MonkeyStatsManager.Instance.GetMoveSpeedCost(),
            StatType.Range => MonkeyStatsManager.Instance.GetRangeCost(),
            _ => 0
        };
    }
    
    string GetStatName()
    {
        return statType switch
        {
            StatType.Health => "Health",
            StatType.Damage => "Damage",
            StatType.APS => "APS",
            StatType.DPS => "DPS",
            StatType.Speed => "Speed",
            StatType.Range => "Range",
            _ => "Unknown"
        };
    }
}