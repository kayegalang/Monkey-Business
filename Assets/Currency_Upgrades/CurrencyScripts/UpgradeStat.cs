using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Game/Upgrade Stat")]
public class UpgradeStat : ScriptableObject
{
    [Header("Info")]
    public string upgradeName;
    public string description;

    [Header("Cost Configuration")]
    public int baseCost = 30;
    public float costMultiplier = 1.4f;

    [Header("Value Configuration")]
    public float baseValue = 100f;
    public float valuePerLevel = 15f;
    
    [Header("Limits")]
    public int maxLevel = 12;

    // Calculate cost at a specific level
    public int GetCostAtLevel(int level)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, level));
    }

    // Calculate value at a specific level
    public float GetValueAtLevel(int level)
    {
        return baseValue + (valuePerLevel * level);
    }
}