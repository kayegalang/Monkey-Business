using System;
using System.Collections.Generic;

[Serializable]
public class MonkeyProgress
{
    public string monkeyId;

    // upgrade levels (per monkey type)
    public int healthLvl;
    public int damageLvl;
    public int apsLvl;
    public int speedLvl;
    public int rangeLvl;
    public int cooldownLvl;

    // optional "prestige" / monkey level concept that resets progress
    public int monkeyLevel;

    public int GetLevel(MonkeyStatId stat) => stat switch
    {
        MonkeyStatId.Health => healthLvl,
        MonkeyStatId.Damage => damageLvl,
        MonkeyStatId.AttacksPerSecond => apsLvl,
        MonkeyStatId.MoveSpeed => speedLvl,
        MonkeyStatId.Range => rangeLvl,
        MonkeyStatId.Cooldown => cooldownLvl,
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
            case MonkeyStatId.Cooldown: cooldownLvl = value; break;
        }
    }

    public void ResetUpgrades()
    {
        healthLvl = damageLvl = apsLvl = speedLvl = rangeLvl = cooldownLvl = 0;
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