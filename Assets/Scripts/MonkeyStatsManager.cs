using UnityEngine;
using System;

public class MonkeyStatsManager : MonoBehaviour
{
    public static MonkeyStatsManager Instance { get; private set; }
    
    [Header("References")]
    [SerializeField] private BananaWallet wallet;
    [SerializeField] private MonkeyDatabase monkeyDatabase;
    
    [Header("Monkey Level Costs")]
    [SerializeField] private int baseLevelUpCost = 20;
    [SerializeField] private float levelUpCostMultiplier = 1.25f;
    
    [Header("Evolution Costs")]
    [SerializeField] private int baseEvolutionCost = 300;
    [SerializeField] private float evolutionCostMultiplier = 1.5f;
    
    [Header("Current State (Debug Only - Don't Edit)")]
    [SerializeField] private int debugHealthLevel;
    [SerializeField] private int debugDamageLevel;
    [SerializeField] private int debugMonkeyLevel;
    [SerializeField] private string debugCurrentForm;
    
    private PlayerMonkeysSave saveData;
    private MonkeyProgress currentMonkey;
    private MonkeyDefinition currentDefinition;
    
    [Header("Session Settings")]
    [SerializeField] private bool resetOnRestart = true; 
    
    // Events
    public event Action OnStatsChanged;
    public event Action OnMonkeyEvolved;
    public event Action OnMonkeyLevelUp;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (resetOnRestart)
        {
            PlayerPrefs.DeleteKey("MonkeyProgress");
            saveData = new PlayerMonkeysSave();
        }
        
        Initialize();
    }
    
    void Initialize()
    {
        saveData = LoadSaveData();
        currentMonkey = saveData.GetOrCreate(saveData.activeMonkeyId);
        
        if (string.IsNullOrEmpty(currentMonkey.currentFormId))
        {
            currentMonkey.currentFormId = "monkee_1";
            currentMonkey.monkeyLevel = 1;
        }
        
        RefreshDefinition();
        UpdateDebugDisplay();
        
        Debug.Log($"MonkeyStatsManager initialized. Form: {currentMonkey.currentFormId}, Level: {currentMonkey.monkeyLevel}");
    }
    
    void RefreshDefinition()
    {
        if (monkeyDatabase == null)
        {
            Debug.LogError("MonkeyDatabase is null! Assign it in Inspector!");
            return;
        }
        
        currentDefinition = monkeyDatabase.GetById(currentMonkey.currentFormId);
        
        if (currentDefinition == null)
        {
            Debug.LogError($"MonkeyDefinition not found for ID: {currentMonkey.currentFormId}");
        }
        else
        {
            Debug.Log($"Loaded MonkeyDefinition: {currentDefinition.displayName}");
        }
    }
    
    void UpdateDebugDisplay()
    {
        debugHealthLevel = currentMonkey.healthLvl;
        debugDamageLevel = currentMonkey.damageLvl;
        debugMonkeyLevel = currentMonkey.monkeyLevel;
        debugCurrentForm = currentMonkey.currentFormId;
    }
    
    // ===== STAT QUERIES (from MonkeyDefinition!) =====
    
    public float GetCurrentHealth()
    {
        if (currentDefinition == null) return 100f;
        return currentDefinition.GetStat(MonkeyStatId.Health, currentMonkey.healthLvl);
    }
    
    public float GetCurrentDamage()
    {
        if (currentDefinition == null) return 5f;
        return currentDefinition.GetStat(MonkeyStatId.Damage, currentMonkey.damageLvl);
    }
    
    public float GetCurrentAttackSpeed()
    {
        if (currentDefinition == null) return 1f;
        return currentDefinition.GetStat(MonkeyStatId.AttacksPerSecond, currentMonkey.apsLvl);
    }
    
    public float GetCurrentDPS()
    {
        // DPS = calculated from Damage × APS
        if (currentDefinition == null) return 5f;
        return currentDefinition.GetDerivedDps(currentMonkey.damageLvl, currentMonkey.apsLvl);
    }
    
    public float GetCurrentMoveSpeed()
    {
        if (currentDefinition == null) return 5f;
        return currentDefinition.GetStat(MonkeyStatId.MoveSpeed, currentMonkey.speedLvl);
    }
    
    public float GetCurrentRange()
    {
        if (currentDefinition == null) return 2f;
        return currentDefinition.GetStat(MonkeyStatId.Range, currentMonkey.rangeLvl);
    }
    
    // Level getters
    public int GetHealthLevel() => currentMonkey.healthLvl;
    public int GetDamageLevel() => currentMonkey.damageLvl;
    public int GetAttackSpeedLevel() => currentMonkey.apsLvl;
    public int GetDPSLevel() => currentMonkey.apsLvl; // DPS uses APS level
    public int GetMoveSpeedLevel() => currentMonkey.speedLvl;
    public int GetRangeLevel() => currentMonkey.rangeLvl;
    
    // Cost getters
    public int GetHealthCost()
    {
        if (currentDefinition == null) return 30;
        return currentDefinition.GetUpgradeCost(MonkeyStatId.Health, currentMonkey.healthLvl);
    }
    
    public int GetDamageCost()
    {
        if (currentDefinition == null) return 35;
        return currentDefinition.GetUpgradeCost(MonkeyStatId.Damage, currentMonkey.damageLvl);
    }
    
    public int GetAttackSpeedCost()
    {
        if (currentDefinition == null) return 40;
        return currentDefinition.GetUpgradeCost(MonkeyStatId.AttacksPerSecond, currentMonkey.apsLvl);
    }
    
    public int GetDPSCost()
    {
        // DPS cost is same as APS cost (since they're linked)
        return GetAttackSpeedCost();
    }
    
    public int GetMoveSpeedCost()
    {
        if (currentDefinition == null) return 30;
        return currentDefinition.GetUpgradeCost(MonkeyStatId.MoveSpeed, currentMonkey.speedLvl);
    }
    
    public int GetRangeCost()
    {
        if (currentDefinition == null) return 35;
        return currentDefinition.GetUpgradeCost(MonkeyStatId.Range, currentMonkey.rangeLvl);
    }
    
    // Max levels (from definitions)
    public int GetHealthMaxLevel() => 12;
    public int GetDamageMaxLevel() => 12;
    public int GetAttackSpeedMaxLevel() => 10;
    public int GetDPSMaxLevel() => 10;
    public int GetMoveSpeedMaxLevel() => 12;
    public int GetRangeMaxLevel() => 10;
    
    // ===== STAT UPGRADES =====
    
    public bool TryUpgradeHealth()
    {
        if (currentMonkey.healthLvl >= GetHealthMaxLevel()) return false;
        int cost = GetHealthCost();
        if (!wallet.TrySpend(cost)) return false;
        
        currentMonkey.healthLvl++;
        SaveProgress();
        OnStatsChanged?.Invoke();
        UpdateDebugDisplay();
        return true;
    }
    
    public bool TryUpgradeDamage()
    {
        if (currentMonkey.damageLvl >= GetDamageMaxLevel()) return false;
        int cost = GetDamageCost();
        if (!wallet.TrySpend(cost)) return false;
        
        currentMonkey.damageLvl++;
        SaveProgress();
        OnStatsChanged?.Invoke();
        UpdateDebugDisplay();
        return true;
    }
    
    public bool TryUpgradeAttackSpeed()
    {
        if (currentMonkey.apsLvl >= GetAttackSpeedMaxLevel()) return false;
        int cost = GetAttackSpeedCost();
        if (!wallet.TrySpend(cost)) return false;
        
        currentMonkey.apsLvl++;
        SaveProgress();
        OnStatsChanged?.Invoke();
        UpdateDebugDisplay();
        return true;
    }
    
    public bool TryUpgradeDPS()
    {
        // DPS upgrade is same as APS upgrade
        return TryUpgradeAttackSpeed();
    }
    
    public bool TryUpgradeMoveSpeed()
    {
        if (currentMonkey.speedLvl >= GetMoveSpeedMaxLevel()) return false;
        int cost = GetMoveSpeedCost();
        if (!wallet.TrySpend(cost)) return false;
        
        currentMonkey.speedLvl++;
        SaveProgress();
        OnStatsChanged?.Invoke();
        UpdateDebugDisplay();
        return true;
    }
    
    public bool TryUpgradeRange()
    {
        if (currentMonkey.rangeLvl >= GetRangeMaxLevel()) return false;
        int cost = GetRangeCost();
        if (!wallet.TrySpend(cost)) return false;
        
        currentMonkey.rangeLvl++;
        SaveProgress();
        OnStatsChanged?.Invoke();
        UpdateDebugDisplay();
        return true;
    }
    
    // ===== MONKEY LEVEL =====
    
    public int GetLevelUpCost()
    {
        return Mathf.RoundToInt(baseLevelUpCost * Mathf.Pow(levelUpCostMultiplier, currentMonkey.monkeyLevel - 1));
    }
    
    public bool TryLevelUp()
    {
        int cost = GetLevelUpCost();
        if (!wallet.TrySpend(cost)) return false;
        
        currentMonkey.monkeyLevel++;
        SaveProgress();
        OnStatsChanged?.Invoke();
        OnMonkeyLevelUp?.Invoke();
        UpdateDebugDisplay();
        
        return true;
    }
    
    public int GetMonkeyLevel() => currentMonkey.monkeyLevel;
    
    // ===== EVOLUTION (Class Up) =====
    
    public bool CanEvolve()
    {
        string[] forms = { "monkee_1", "monkee_2", "monkee_3", "kong_1", "kong_2", "kong_3" };
        int formIndex = Array.IndexOf(forms, currentMonkey.currentFormId);
        return formIndex < forms.Length - 1;
    }
    
    public int GetEvolutionCost()
    {
        string[] forms = { "monkee_1", "monkee_2", "monkee_3", "kong_1", "kong_2", "kong_3" };
        int formIndex = Array.IndexOf(forms, currentMonkey.currentFormId);
        return Mathf.RoundToInt(baseEvolutionCost * Mathf.Pow(evolutionCostMultiplier, formIndex));
    }
    
    public bool TryEvolve()
    {
        if (!CanEvolve())
        {
            Debug.Log("Already at max form!");
            return false;
        }
        
        int cost = GetEvolutionCost();
        if (!wallet.TrySpend(cost))
        {
            Debug.Log($"Not enough bananas for class up! Need {cost}");
            return false;
        }
        
        string oldForm = currentMonkey.currentFormId;
        
        // RESET ALL UPGRADE LEVELS TO 0
        currentMonkey.ResetUpgrades();
        
        // EVOLVE TO NEXT FORM (which has higher base stats!)
        string[] forms = { "monkee_1", "monkee_2", "monkee_3", "kong_1", "kong_2", "kong_3" };
        int formIndex = Array.IndexOf(forms, currentMonkey.currentFormId);
        if (formIndex < forms.Length - 1)
        {
            currentMonkey.currentFormId = forms[formIndex + 1];
        }
        
        // LOAD THE NEW DEFINITION (with higher base stats!)
        RefreshDefinition();
        
        SaveProgress();
        OnStatsChanged?.Invoke();
        OnMonkeyEvolved?.Invoke();
        UpdateDebugDisplay();
        
        Debug.Log($"🎉 CLASS UP from {oldForm} to {currentMonkey.currentFormId}!");
        Debug.Log($"✨ All upgrades reset to 0, now using {currentDefinition.displayName} base stats!");
        
        return true;
    }
    
    public string GetNextFormName()
    {
        string[] forms = { "monkee_1", "monkee_2", "monkee_3", "kong_1", "kong_2", "kong_3" };
        string[] names = { "The Monkee", "Chiller Monkee", "Lit Monkee", "Battle Kong", "Golden Kong", "Big Chungus Kong" };
        int formIndex = Array.IndexOf(forms, currentMonkey.currentFormId);
        
        if (formIndex >= 0 && formIndex < forms.Length - 1)
            return names[formIndex + 1];
        
        return "MAX FORM";
    }
    
    public string GetCurrentFormName()
    {
        if (currentDefinition != null)
            return currentDefinition.displayName;
        
        return "Unknown";
    }
    
    // ===== SAVE/LOAD =====
    
    void SaveProgress()
    {
        string json = JsonUtility.ToJson(saveData, true);
        PlayerPrefs.SetString("MonkeyProgress", json);
        PlayerPrefs.Save();
    }
    
    PlayerMonkeysSave LoadSaveData()
    {
        if (PlayerPrefs.HasKey("MonkeyProgress"))
        {
            string json = PlayerPrefs.GetString("MonkeyProgress");
            return JsonUtility.FromJson<PlayerMonkeysSave>(json);
        }
        
        return new PlayerMonkeysSave();
    }
    
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteKey("MonkeyProgress");
        Initialize();
        OnStatsChanged?.Invoke();
        Debug.Log("All progress reset!");
    }
    
    public MonkeyDefinition GetCurrentDefinition() => currentDefinition;
}