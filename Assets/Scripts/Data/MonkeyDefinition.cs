using UnityEngine;

public enum MonkeyStatId
{
    Health,
    Damage,
    AttacksPerSecond,
    MoveSpeed,
    Range
}

[System.Serializable]
public struct StatUpgradeRule
{
    [Header("Stat Growth Per Upgrade")]
    public float baseValue;        // value at upgrade level 0
    public float addPerLevel;      // linear growth per upgrade level
    public float multPerLevel;     // optional multiplicative growth (1 = none)

    [Header("Cost Curve")]
    public int baseCost;           // cost at upgrade level 0
    public float costGrowth;       // e.g. 1.15 means +15% per upgrade level

    public float ValueAt(int level)
    {
        float v = baseValue + addPerLevel * Mathf.Max(0, level);
        if (multPerLevel != 1f) v *= Mathf.Pow(multPerLevel, Mathf.Max(0, level));
        return v;
    }

    public int CostAt(int level)
    {
        float c = baseCost * Mathf.Pow(costGrowth, Mathf.Max(0, level));
        return Mathf.Max(0, Mathf.RoundToInt(c));
    }
}

[CreateAssetMenu(fileName = "Monkey_", menuName = "Game/Monkeys/Monkey Definition")]
public class MonkeyDefinition : ScriptableObject
{
    [Header("Identity")]
    public string monkeyId = "starter";  // stable key for saves
    public string displayName = "Starter Monkey";
    public Sprite icon;
    public GameObject prefab;            // optional: monkey visuals/controller prefab

    [Header("Class / Skin Multipliers (stat multiplier)")]
    [Tooltip("Applied after upgrades. 1 = no change.")]
    public float healthMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float apsMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float rangeMultiplier = 1f;

    [Header("Upgradeable Stats Rules (designer editable)")]
    public StatUpgradeRule health;
    public StatUpgradeRule damage;
    public StatUpgradeRule attacksPerSecond;
    public StatUpgradeRule moveSpeed;
    public StatUpgradeRule range;

    // Helper for runtime reads:
    public float GetStat(MonkeyStatId stat, int upgradeLevel)
    {
        switch (stat)
        {
            case MonkeyStatId.Health:            return health.ValueAt(upgradeLevel) * healthMultiplier;
            case MonkeyStatId.Damage:            return damage.ValueAt(upgradeLevel) * damageMultiplier;
            case MonkeyStatId.AttacksPerSecond:  return attacksPerSecond.ValueAt(upgradeLevel) * apsMultiplier;
            case MonkeyStatId.MoveSpeed:         return moveSpeed.ValueAt(upgradeLevel) * speedMultiplier;
            case MonkeyStatId.Range:             return range.ValueAt(upgradeLevel) * rangeMultiplier;
            default: return 0f;
        }
    }

    public int GetUpgradeCost(MonkeyStatId stat, int currentUpgradeLevel)
    {
        switch (stat)
        {
            case MonkeyStatId.Health:            return health.CostAt(currentUpgradeLevel);
            case MonkeyStatId.Damage:            return damage.CostAt(currentUpgradeLevel);
            case MonkeyStatId.AttacksPerSecond:  return attacksPerSecond.CostAt(currentUpgradeLevel);
            case MonkeyStatId.MoveSpeed:         return moveSpeed.CostAt(currentUpgradeLevel);
            case MonkeyStatId.Range:             return range.CostAt(currentUpgradeLevel);
            default: return 0;
        }
    }

    public float GetDerivedDps(int dmgLevel, int apsLevel)
    {
        float dmg = GetStat(MonkeyStatId.Damage, dmgLevel);
        float aps = GetStat(MonkeyStatId.AttacksPerSecond, apsLevel);
        return dmg * aps;
    }
}

