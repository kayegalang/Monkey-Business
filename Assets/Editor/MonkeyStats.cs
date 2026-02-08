using UnityEditor;
using UnityEngine;

public static class MonkeyStats
{
    [MenuItem("Tools/Monkeys/Write All Monkey Stats")]
    static void WriteAll()
    {
        Write(
            id: "monkee_1",
            name: "The Monkee",
            path: "Assets/Monkeys/The_Monkee.asset",

            health: (100, 15, 1f, 50, 1.2f),
            damage: (5,   2,  1f, 40, 1.25f),
            aps:    (1f,  0.1f,1f, 30, 1.2f),
            speed:  (1f,  0f, 1f, 0,  1f),
            range:  (5f,  0.2f,1f, 35, 1.2f)

        );

        Write(
            id: "monkee_2",
            name: "Chiller Monkee",
            path: "Assets/Monkeys/Chiller_Monkee.asset",

            health: (130, 18, 1f, 70, 1.2f),
            damage: (7,   2.5f,1f, 55, 1.25f),
            aps:    (1.1f, 0.12f,1f,40, 1.2f),
            speed:  (1.05f, 0f,1f, 0,  1f),
            range:  (5f,   0.2f,1f,35, 1.2f)
        );
        
        Write (
            id: "monkee_3",
            name: "Lit Monkee",
            path: "Assets/Monkeys/Lit_Monkee.asset",

            health: (160, 20, 1f, 90, 1.2f),
            damage: (9,   3,  1f, 75, 1.25f),
            aps:    (1.2f, 0.15f,1f,55, 1.2f),
            speed:  (1.1f, 0f, 1f, 0,  1f),
            range:  (5f,  0.2f,1f,35, 1.2f)
        );

        Write (
            id: "kong_1",
            name: "Battle Kong",
            path: "Assets/Monkeys/Battle_Kong.asset",

            health: (220, 25, 1f, 130, 1.25f),
            damage: (14,  4,  1f, 110, 1.25f),
            aps:    (1.35f,0.18f,1f,80,  1.2f),
            speed:  (1.15f,0f, 1f, 0,   1f),
            range:  (5f,   0.2f,1f,35,  1.2f)

        );

        Write ( 
            id: "kong_2",
            name: "Golden Kong",
            path: "Assets/Monkeys/Golden_Kong.asset",

            health: (270, 30, 1f, 170, 1.25f),
            damage: (18,  5,  1f, 145, 1.25f),
            aps:    (1.5f, 0.2f,1f,110, 1.2f),
            speed:  (1.2f, 0f, 1f, 0,   1f),
            range:  (5f,  0.2f,1f,35,  1.2f)

        );

        Write (
            id: "kong_3",
            name: "Big Chungus Kong",
            path: "Assets/Monkeys/Big_Chungus_Kong.asset",

            health: (330, 35, 1f, 220, 1.25f),
            damage: (23,  6,  1f, 185, 1.25f),
            aps:    (1.65f, 0.22f,1f,150, 1.2f),
            speed:  (1.25f, 0f, 1f, 0,   1f),
            range:  (5f,   0.2f,1f,35,  1.2f)

        );

        AssetDatabase.SaveAssets();
    }

    static void Write(
        string id,
        string name,
        string path,
        (float b, float a, float m, int c, float g) health,
        (float b, float a, float m, int c, float g) damage,
        (float b, float a, float m, int c, float g) aps,
        (float b, float a, float m, int c, float g) speed,
        (float b, float a, float m, int c, float g) range
    )
    {
        var mky = AssetDatabase.LoadAssetAtPath<MonkeyDefinition>(path);
        if (!mky)
        {
            mky = ScriptableObject.CreateInstance<MonkeyDefinition>();
            AssetDatabase.CreateAsset(mky, path);
        }

        mky.monkeyId = id;
        mky.displayName = name;

        Apply(ref mky.health, health);
        Apply(ref mky.damage, damage);
        Apply(ref mky.attacksPerSecond, aps);
        Apply(ref mky.moveSpeed, speed);
        Apply(ref mky.range, range);

        EditorUtility.SetDirty(mky);
    }

    static void Apply(ref StatUpgradeRule r, (float b, float a, float m, int c, float g) v)
    {
        r.baseValue = v.b;
        r.addPerLevel = v.a;
        r.multPerLevel = v.m;
        r.baseCost = v.c;
        r.costGrowth = v.g;
    }
}
