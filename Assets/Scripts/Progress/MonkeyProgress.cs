using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonkeyProgress
{
    // Track current monkey form
    public string currentFormId;
    
    // Monkey level
    public int monkeyLevel;
    
    // Upgrade levels (6 stats total)
    public int healthLvl;
    public int damageLvl;
    public int apsLvl;
    public int dpsLvl;      // NEW! 6th stat
    public int speedLvl;
    public int rangeLvl;
    
    public string monkeyId;
    
    // Evolution thresholds
    private static readonly int[] evolutionLevels = { 10, 20, 30, 40, 50 };
    
    // List of monkey forms in order
    private static readonly string[] monkeyForms = {
        "monkee_1", // The Monkee
        "monkee_2", // Chiller Monkee
        "monkee_3", // Lit Monkee
        "kong_1",   // Battle Kong
        "kong_2",   // Golden Kong
        "kong_3"    // Big Chungus Kong
    };
    
    // Evolution cost values
    public int baseEvolutionCost = 300;
    public float evolutionCostMultiplier = 1.5f;
    
    // Returns the cost to evolve to the next form
    public int GetEvolutionCost()
    {
        int formIndex = Array.IndexOf(monkeyForms, currentFormId);
        return (int)(baseEvolutionCost * Mathf.Pow(evolutionCostMultiplier, formIndex));
    }
    
    public void EvolveIfReady()
    {
        int formIndex = Array.IndexOf(monkeyForms, currentFormId);
        
        // Only evolve if not at last form
        if (formIndex < monkeyForms.Length - 1 && monkeyLevel >= evolutionLevels[formIndex])
        {
            currentFormId = monkeyForms[formIndex + 1];
            monkeyLevel = 1;
            ResetUpgrades();
        }
    }
    
    public int GetLevel(MonkeyStatId stat) => stat switch
    {
        MonkeyStatId.Health => healthLvl,
        MonkeyStatId.Damage => damageLvl,
        MonkeyStatId.AttacksPerSecond => apsLvl,
        MonkeyStatId.MoveSpeed => speedLvl,
        MonkeyStatId.Range => rangeLvl,
        _ => 0
    };
    
    public void SetLevel(MonkeyStatId stat, int value)
    {
        value = Math.Max(0, value);
        switch (stat)
        {
            case MonkeyStatId.Health: healthLvl = value; break;
            case MonkeyStatId.Damage: damageLvl = value; break;
            case MonkeyStatId.AttacksPerSecond: apsLvl = value; break;
            case MonkeyStatId.MoveSpeed: speedLvl = value; break;
            case MonkeyStatId.Range: rangeLvl = value; break;
        }
    }
    
    public void ResetUpgrades()
    {
        healthLvl = damageLvl = apsLvl = dpsLvl = speedLvl = rangeLvl = 0;
    }
}

[Serializable]
public class PlayerMonkeysSave
{
    public string activeMonkeyId = "starter";
    public List<MonkeyProgress> perMonkey = new();
    
    public MonkeyProgress GetOrCreate(string monkeyId)
    {
        var found = perMonkey.Find(m => m.monkeyId == monkeyId);
        if (found != null) return found;
        
        var created = new MonkeyProgress { monkeyId = monkeyId };
        perMonkey.Add(created);
        return created;
    }
}