using UnityEngine;

namespace Wave_System.Scripts
{
    [CreateAssetMenu(fileName = "Level", menuName = "Monkey Business/New Level")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Info")]
        [Tooltip("Level number")]
        public string levelName = "Level 1";
    
        [Tooltip("Description of this level")]
        [TextArea(2, 4)]
        public string description = "First level - introduces basic enemies";
    
        [Header("Visual Theme")]
        [Tooltip("Background prefab for this level")]
        public GameObject backgroundPrefab;
    
        [Header("Waves")]
        [Tooltip("All waves in this level, played in order")]
        public WaveData[] waves;
    
        [Header("Victory Conditions")]
        [Tooltip("What happens when level is complete")]
        public bool showVictoryScreen = true;
    
        [Tooltip("Next level to load (leave empty for last level)")]
        public LevelData nextLevel;
    
        public int GetTotalWaves()
        {
            return waves != null ? waves.Length : 0;
        }
    
        public int GetTotalEnemiesInLevel()
        {
            int total = 0;
            if (waves != null)
            {
                foreach (var wave in waves)
                {
                    total += wave.GetTotalEnemyCount();
                }
            }
            return total;
        }
    }
}