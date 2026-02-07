using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonkeyDatabase", menuName = "Game/Monkeys/Monkey Database")]
public class MonkeyDatabase : ScriptableObject
{
    public List<MonkeyDefinition> monkeys = new();

    public MonkeyDefinition GetById(string monkeyId)
    {
        for (int i = 0; i < monkeys.Count; i++)
        {
            if (monkeys[i] != null && monkeys[i].monkeyId == monkeyId)
                return monkeys[i];
        }
        return null;
    }
}