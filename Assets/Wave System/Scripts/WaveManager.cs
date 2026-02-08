using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Background.Scripts;
using Enemy.Scripts;

namespace Wave_System.Scripts
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Level Configuration")]
        [SerializeField] private LevelData currentLevel;
        
        [Header("Spawning")]
        [SerializeField] private GameObject enemyPrefab; 
        [SerializeField] private Transform enemyParent; 
        
        [Header("Background")]
        [SerializeField] private Vector3 backgroundPosition = new Vector3(0, 0, 10); 
        
        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;
        
        // State tracking
        private int currentWaveIndex = 0;
        private List<GameObject> activeEnemies = new List<GameObject>();
        private bool isSpawningWave = false;
        private Camera mainCamera;
        private GameObject currentBackground; // Track spawned background
        
        // Events
        public delegate void WaveEvent(int waveNumber, int totalWaves);
        public event WaveEvent OnWaveStarted;
        public event WaveEvent OnWaveCompleted;
        public event System.Action OnLevelCompleted;
        
        void Start()
        {
            mainCamera = Camera.main;
            
            if (currentLevel == null)
            {
                Debug.LogError("WaveManager: No level assigned!");
                return;
            }
            
            if (enemyPrefab == null)
            {
                Debug.LogError("WaveManager: No enemy prefab assigned!");
                return;
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"<color=cyan>Starting Level: {currentLevel.levelName}</color>");
                Debug.Log($"Total Waves: {currentLevel.GetTotalWaves()}");
                Debug.Log($"Total Enemies: {currentLevel.GetTotalEnemiesInLevel()}");
            }
            
            SpawnBackground();
            StartNextWave();
        }
        
        void Update()
        {
            // Check if current wave is complete
            if (!isSpawningWave && activeEnemies.Count > 0)
            {
                // Remove any null references (dead enemies)
                activeEnemies.RemoveAll(enemy => enemy == null);
                
                // If all enemies dead, start next wave
                if (activeEnemies.Count == 0)
                {
                    OnWaveComplete();
                }
            }
        }
        
        void StartNextWave()
        {
            if (FinishedAllWaves())
            {
                OnLevelComplete();
                return;
            }
            
            WaveData wave = currentLevel.waves[currentWaveIndex];
            
            if (showDebugLogs)
            {
                Debug.Log($"<color=yellow>═══ WAVE {currentWaveIndex + 1}/{currentLevel.GetTotalWaves()} START ═══</color>");
                Debug.Log($"Wave: {wave.waveName}");
                Debug.Log($"Enemies: {wave.GetTotalEnemyCount()}");
            }
            
            OnWaveStarted?.Invoke(currentWaveIndex + 1, currentLevel.GetTotalWaves());
            
            StartCoroutine(SpawnWave(wave));
        }

        private bool FinishedAllWaves()
        {
            return currentWaveIndex >= currentLevel.GetTotalWaves();
        }

        IEnumerator SpawnWave(WaveData wave)
        {
            isSpawningWave = true;
            
            foreach (var enemySpawn in wave.enemySpawns)
            {
                for (int i = 0; i < enemySpawn.count; i++)
                {
                    SpawnEnemy(enemySpawn.enemyType, wave);
                    
                    if (enemySpawn.spawnDelay > 0)
                    {
                        yield return new WaitForSeconds(enemySpawn.spawnDelay);
                    }
                }
            }
            
            isSpawningWave = false;
            
            if (showDebugLogs)
            {
                Debug.Log($"<color=green>Wave spawning complete! {activeEnemies.Count} enemies active</color>");
            }
        }
        
        void SpawnEnemy(EnemyData enemyData, WaveData wave)
        {
            if (mainCamera == null || enemyPrefab == null || enemyData == null)
            {
                Debug.LogWarning("Cannot spawn enemy - missing references");
                return;
            }
            
            Vector3 spawnPosition = CalculateSpawnPosition(wave);
            
            var enemy = SpawnEnemy(spawnPosition);
            
            if (enemyParent != null)
            {
                enemy.transform.SetParent(enemyParent);
            }
            
            Enemy.Scripts.Enemy enemyScript = enemy.GetComponent<Enemy.Scripts.Enemy>();
            if (enemyScript != null)
            {
                enemyScript.enemyData = enemyData;
            }
            else
            {
                Debug.LogWarning("Enemy prefab doesn't have Enemy script!");
            }
            
            activeEnemies.Add(enemy);
            
            if (showDebugLogs)
            {
                Debug.Log($"Spawned: {enemyData.name} at {spawnPosition}");
            }
        }

        private GameObject SpawnEnemy(Vector3 spawnPosition)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            return enemy;
        }

        Vector3 CalculateSpawnPosition(WaveData wave)
        {
            Vector3 cameraPos = mainCamera.transform.position;
            
            float spawnX = cameraPos.x - wave.spawnDistanceFromCamera;
            
            return new Vector3(spawnX, -4, 0);
        }
        
        void OnWaveComplete()
        {
            if (showDebugLogs)
            {
                Debug.Log($"<color=lime>✓ WAVE {currentWaveIndex + 1} COMPLETE!</color>");
            }
            
            OnWaveCompleted?.Invoke(currentWaveIndex + 1, currentLevel.GetTotalWaves());
            
            currentWaveIndex++;
            
            StartNextWave();
        }
        
        void OnLevelComplete()
        {
            if (showDebugLogs)
            {
                Debug.Log($"<color=yellow>★★★ LEVEL COMPLETE: {currentLevel.levelName} ★★★</color>");
            }
            
            OnLevelCompleted?.Invoke();
            
            // Handle victory
            if (currentLevel.showVictoryScreen)
            {
                // TODO: Show victory UI
                Debug.Log("VICTORY! (Show victory screen here)");
            }
            
            // TODO: Load next level if exists
            if (currentLevel.nextLevel != null)
            {
                Debug.Log($"Next level available: {currentLevel.nextLevel.levelName}");
                // You can call StartLevel(currentLevel.nextLevel) here to auto-progress
            }
        }
        
        void SpawnBackground()
        {
            if (currentBackground != null)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"<color=orange>Destroying old background</color>");
                }
                Destroy(currentBackground);
            }
            
            if (currentLevel.backgroundPrefab != null)
            {
                currentBackground = Instantiate(currentLevel.backgroundPrefab, backgroundPosition, Quaternion.identity);
                
                
                InfiniteBackground bgScript = currentBackground.GetComponent<InfiniteBackground>();
                if (bgScript == null)
                {
                    Debug.LogWarning($"Background prefab '{currentLevel.backgroundPrefab.name}' doesn't have StationaryInfiniteBackground script! Adding it...");
                    bgScript = currentBackground.AddComponent<InfiniteBackground>();
                }
                
                if (showDebugLogs)
                {
                    Debug.Log($"<color=cyan>Spawned background: {currentLevel.backgroundPrefab.name}</color>");
                }
            }
            else
            {
                if (showDebugLogs)
                {
                    Debug.Log("<color=yellow>No background assigned to this level</color>");
                }
            }
        }
        
        public void StartLevel(LevelData level)
        {
            currentLevel = level;
            currentWaveIndex = 0;
            activeEnemies.Clear();
            
            // Spawn the new level's background
            SpawnBackground();
            
            StartNextWave();
        }
        
        public int GetCurrentWave()
        {
            return currentWaveIndex + 1;
        }
        
        public int GetTotalWaves()
        {
            return currentLevel != null ? currentLevel.GetTotalWaves() : 0;
        }
        
        public int GetActiveEnemyCount()
        {
            activeEnemies.RemoveAll(enemy => enemy == null);
            return activeEnemies.Count;
        }
    }
}