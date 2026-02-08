using Enemy.Scripts;
using UnityEngine;

namespace Wave_System.Scripts
{
    [CreateAssetMenu(fileName = "Wave", menuName = "Monkey Business/New Wave")]
    public class WaveData : ScriptableObject
    {
        [System.Serializable]
        public class EnemySpawn
        {
            [Tooltip("Which enemy type to spawn")]
            public EnemyData enemyType;
        
            [Tooltip("How many of this enemy to spawn")]
            public int count = 1;
        
            [Tooltip("Delay between spawning each enemy of this type (seconds)")]
            public float spawnDelay = 0.5f;
        }
    
        [Header("Wave Configuration")]
        [Tooltip("Name of this wave")]
        public string waveName = "Wave 1";
    
        [Tooltip("All enemy types to spawn in this wave")]
        public EnemySpawn[] enemySpawns;
    
        [Header("Spawn Settings")]
    
        [Tooltip("How far to the LEFT of camera to spawn enemies")]
        public float spawnDistanceFromCamera = 15f;
    
        public int GetTotalEnemyCount()
        {
            int total = 0;
            foreach (var spawn in enemySpawns)
            {
                total += spawn.count;
            }
            return total;
        }
    }
}