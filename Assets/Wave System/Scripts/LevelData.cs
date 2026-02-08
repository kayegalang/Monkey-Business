using UnityEngine;

namespace Wave_System.Scripts
{
    [CreateAssetMenu(fileName = "Level", menuName = "Monkey Business/New Level")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Info")]
        [Tooltip("Level Number")]
        public string levelName = "Level 1";
    
        [Tooltip("Description of this level")]
        [TextArea(2, 4)]
        public string description = "First level - introduces basic enemies";
    
        [Header("Waves")]
        [Tooltip("All waves in this level, played in order")]
        public WaveData[] waves;
    
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