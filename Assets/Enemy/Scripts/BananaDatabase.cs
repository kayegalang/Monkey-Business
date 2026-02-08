using System.Collections.Generic;
using UnityEngine;
using Enemy.Scripts;

[CreateAssetMenu(fileName = "BananaDatabase", menuName = "Game/Bananas/Banana Database")]
public class BananaDatabase : ScriptableObject
{
    public List<EnemyData> bananas = new();

    public EnemyData GetById(string bananaId)
    {
        for (int i = 0; i < bananas.Count; i++)
        {
            if (bananas[i] != null && bananas[i].enemyId == bananaId)
                return bananas[i];
        }
        return null;
    }
}
