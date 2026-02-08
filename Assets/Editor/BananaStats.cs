using UnityEditor;
using UnityEngine;

public static class BananaStats
{
    [MenuItem("Tools/Bananas/Write All Banana Stats")]
    static void WriteAll()
    {


        Write(
            id: "bananemy",
            name: "bananemy",
            path: "Assets/Enemy/EnemyData/bananemy.asset",
            health: 15f,
            damage: 2f,
            cooldown: 0.5f,
            speed: 1f,
            range: 2f
        );

        Write(
            id: "ripe_banana",
            name: "ripe banana",
            path: "Assets/Enemy/EnemyData/ripe_banana.asset",
            health: 200f,
            damage: 25f,
            cooldown: 0.4f,
            speed: 1.2f,
            range: 3f
        );

        Write(
            id: "rotton_bananemy",
            name: "rotton bananemy",
            path: "Assets/Enemy/EnemyData/rotton_bananemy.asset",
            health: 250f,
            damage: 30f,
            cooldown: 0.5f,
            speed: 1.3f,
            range: 3.5f
        );

        Write(
            id: "bananemy_boxer_chad",
            name: "bananemy boxer chad",
            path: "Assets/Enemy/EnemyData/bananemy_boxer_chad.asset",
            health: 300f,
            damage: 35f,
            cooldown: 0.3f,
            speed: 1.4f,
            range: 4f
        );

        Write(
            id: "golden_bananemy",
            name: "golden bananemy",
            path: "Assets/Enemy/EnemyData/golden_bananemy.asset",
            health: 400f,
            damage: 45f,
            cooldown: 0.2f,
            speed: 1.5f,
            range: 5f
        );

        Write(
            id: "big_bad_bananemy",
            name: "big bad bananemy",
            path: "Assets/Enemy/EnemyData/big_bad_bananemy.asset",
            health: 500f,
            damage: 60f,
            cooldown: 0.15f,
            speed: 1.6f,
            range: 6f
        );

        AssetDatabase.SaveAssets();
    }

    static void Write(
        string id,
        string name,
        string path,
        float health,
        float damage,
        float cooldown,
        float speed,
        float range
    )
    {
        var banana = AssetDatabase.LoadAssetAtPath<Enemy.Scripts.EnemyData>(path);
        if (!banana)
        {
            banana = ScriptableObject.CreateInstance<Enemy.Scripts.EnemyData>();
            AssetDatabase.CreateAsset(banana, path);
        }

        banana.enemyId = id;
        banana.displayName = name;
        banana.health = health;
        banana.damage = damage;
        banana.cooldown = cooldown;
        banana.speed = speed;
        banana.range = range;

        EditorUtility.SetDirty(banana);
    }
}
