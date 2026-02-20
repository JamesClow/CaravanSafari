using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages deterministic enemy waves. Spawns enemies, tracks them,
/// and progresses to the next wave when all enemies in the current wave are dead.
///
/// Setup:
/// 1. Add this to a GameObject in your scene.
/// 2. Define waves in the inspector (each wave has spawn groups).
/// 3. Create spawn point Transforms in the scene and assign them.
/// 4. Ensure enemy prefabs have: NavMeshAgent, Health, UnitAI, OffensiveTargeting, Attack, Collider (IsTrigger), Tag="Enemy".
/// 5. Ensure a NavMesh is baked in the scene.
/// </summary>
public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnGroup
    {
        [Tooltip("Enemy prefab to spawn")]
        public GameObject enemyPrefab;

        [Tooltip("Number of enemies in this group")]
        public int count = 5;

        [Tooltip("Where to spawn this group")]
        public Transform spawnPoint;

        [Tooltip("Delay before this group starts spawning (seconds from wave start)")]
        public float startDelay = 0f;

        [Tooltip("Time between individual enemy spawns within this group")]
        public float spawnInterval = 0.3f;
    }

    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave";
        public SpawnGroup[] spawnGroups;

        [Tooltip("Delay before this wave starts after being triggered (seconds)")]
        public float preWaveDelay = 2f;
    }

    [Header("Wave Configuration")]
    public Wave[] waves;
    public int startingWave = 0;

    [Header("Auto-Progression")]
    [Tooltip("Automatically start the next wave when the current one is cleared")]
    public bool autoProgressWaves = true;

    [Tooltip("Delay between waves when auto-progressing (seconds)")]
    public float timeBetweenWaves = 5f;

    [Header("Runtime State (Read Only)")]
    [SerializeField] private int currentWaveIndex = 0;
    [SerializeField] private int enemiesAlive = 0;
    [SerializeField] private int totalEnemiesSpawned = 0;
    [SerializeField] private bool waveInProgress = false;
    [SerializeField] private bool allWavesComplete = false;

    // Events for other systems to listen to
    public event Action<int> OnWaveStarted;           // Passes wave index
    public event Action<int> OnWaveComplete;           // Passes wave index
    public event Action OnAllWavesComplete;
    public event Action<int, int> OnEnemyCountChanged; // (alive, totalSpawned)

    // Track living enemies
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        currentWaveIndex = startingWave;

        if (waves == null || waves.Length == 0)
        {
            Debug.LogWarning("WaveManager: No waves defined!");
            return;
        }

        // Start the first wave
        StartCoroutine(StartWaveSequence());
    }

    /// <summary>
    /// Call this from an Enemy's OnDeath event to notify WaveManager.
    /// </summary>
    public void OnEnemyKilled(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            enemiesAlive = activeEnemies.Count;
            OnEnemyCountChanged?.Invoke(enemiesAlive, totalEnemiesSpawned);

            Debug.Log($"[WaveManager] Enemy killed. {enemiesAlive} remaining.");

            // Check if wave is cleared
            if (waveInProgress && enemiesAlive <= 0)
            {
                WaveCleared();
            }
        }
    }

    private IEnumerator StartWaveSequence()
    {
        while (currentWaveIndex < waves.Length)
        {
            yield return StartCoroutine(RunWave(currentWaveIndex));

            // Wait for wave to be cleared (enemies all dead)
            while (waveInProgress)
            {
                yield return null;
            }

            // Check if there are more waves
            currentWaveIndex++;
            if (currentWaveIndex >= waves.Length)
            {
                allWavesComplete = true;
                Debug.Log("[WaveManager] All waves complete!");
                OnAllWavesComplete?.Invoke();
                yield break;
            }

            if (autoProgressWaves)
            {
                Debug.Log($"[WaveManager] Next wave in {timeBetweenWaves} seconds...");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
            else
            {
                // Wait for manual trigger
                yield break;
            }
        }
    }

    private IEnumerator RunWave(int waveIndex)
    {
        Wave wave = waves[waveIndex];

        Debug.Log($"[WaveManager] Starting {wave.waveName} (Wave {waveIndex + 1}/{waves.Length})");

        // Pre-wave delay
        if (wave.preWaveDelay > 0f)
        {
            yield return new WaitForSeconds(wave.preWaveDelay);
        }

        waveInProgress = true;
        totalEnemiesSpawned = 0;
        enemiesAlive = 0;
        activeEnemies.Clear();

        OnWaveStarted?.Invoke(waveIndex);

        // Spawn all groups (each group runs its own coroutine)
        foreach (SpawnGroup group in wave.spawnGroups)
        {
            StartCoroutine(SpawnGroupCoroutine(group));
        }
    }

    private IEnumerator SpawnGroupCoroutine(SpawnGroup group)
    {
        if (group.enemyPrefab == null || group.spawnPoint == null)
        {
            Debug.LogWarning("[WaveManager] SpawnGroup has null prefab or spawn point, skipping.");
            yield break;
        }

        // Initial delay for this group
        if (group.startDelay > 0f)
        {
            yield return new WaitForSeconds(group.startDelay);
        }

        for (int i = 0; i < group.count; i++)
        {
            // Spawn enemy with slight random offset to avoid stacking
            Vector3 spawnPos = group.spawnPoint.position + UnityEngine.Random.insideUnitSphere * 1.5f;
            spawnPos.y = group.spawnPoint.position.y; // Keep on ground level

            GameObject enemy = Instantiate(group.enemyPrefab, spawnPos, Quaternion.identity);

            // Track this enemy
            activeEnemies.Add(enemy);
            totalEnemiesSpawned++;
            enemiesAlive = activeEnemies.Count;
            OnEnemyCountChanged?.Invoke(enemiesAlive, totalEnemiesSpawned);

            // Subscribe to enemy death via Health component
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.OnDeath += (deadObj) => OnEnemyKilled(deadObj);
            }

            // Wait between spawns
            if (group.spawnInterval > 0f && i < group.count - 1)
            {
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }

    private void WaveCleared()
    {
        waveInProgress = false;

        Debug.Log($"[WaveManager] Wave {currentWaveIndex + 1} cleared!");
        OnWaveComplete?.Invoke(currentWaveIndex);
    }

    /// <summary>
    /// Manually trigger the next wave (when autoProgressWaves is false).
    /// </summary>
    public void TriggerNextWave()
    {
        if (!waveInProgress && !allWavesComplete)
        {
            StartCoroutine(StartWaveSequence());
        }
    }

    // --- Public Accessors ---

    public int GetCurrentWaveIndex() => currentWaveIndex;
    public int GetTotalWaves() => waves != null ? waves.Length : 0;
    public int GetEnemiesAlive() => enemiesAlive;
    public bool IsWaveInProgress() => waveInProgress;
    public bool AreAllWavesComplete() => allWavesComplete;

    /// <summary>
    /// Periodic cleanup of null entries (enemies destroyed by other means).
    /// </summary>
    void LateUpdate()
    {
        if (!waveInProgress) return;

        // Clean up any enemies that were destroyed without going through OnEnemyKilled
        int removedCount = activeEnemies.RemoveAll(e => e == null);
        if (removedCount > 0)
        {
            enemiesAlive = activeEnemies.Count;
            OnEnemyCountChanged?.Invoke(enemiesAlive, totalEnemiesSpawned);

            if (enemiesAlive <= 0)
            {
                WaveCleared();
            }
        }
    }
}
