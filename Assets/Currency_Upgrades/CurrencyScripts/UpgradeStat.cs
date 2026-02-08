// UpgradeStat.cs
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Game/Upgrade Stat")]
public class UpgradeStat : ScriptableObject
{
    [Header("Info")]
    public string upgradeName;
    public string description;

    [Header("Cost")]
    [SerializeField] private int baseCost = 10;
    [SerializeField] private float costMultiplier = 1.5f;
    // e.g. base 10 → 15 → 22 → 33 → 50...

    [Header("State")]
    [SerializeField] private int currentLevel;
    [SerializeField] private int maxLevel = 10;

    [Header("Value")]
    [SerializeField] private float baseValue = 1f;
    [SerializeField] private float valuePerLevel = 0.5f;

    // events
    public event Action<UpgradeStat> OnUpgraded;

    // --- properties ---
    public int CurrentLevel => currentLevel;
    public int MaxLevel => maxLevel;
    public bool IsMaxed => currentLevel >= maxLevel;

    public int CurrentCost
    {
        get
        {
            // cost increases with each purchase
            return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel));
        }
    }

    public float CurrentValue
    {
        get
        {
            return baseValue + (valuePerLevel * currentLevel);
        }
    }

    // --- methods ---
    public bool TryUpgrade(BananaWallet wallet)
    {
        if (IsMaxed)
        {
            Debug.Log($"{upgradeName} is already maxed!");
            return false;
        }

        int cost = CurrentCost;

        if (!wallet.TrySpend(cost))
        {
            Debug.Log($"Not enough bananas! Need {cost}");
            return false;
        }

        currentLevel++;
        OnUpgraded?.Invoke(this);
        Debug.Log($"{upgradeName} upgraded to level {currentLevel}!");
        return true;
    }

    public void Reset()
    {
        currentLevel = 0;
    }
}