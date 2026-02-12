using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int startingWave = 0; // Just to make it easier to test later waves
    public float spawnDelay = .2f; // To stagger enemy spawns. Should this be exposed in Wavelet?

    [System.Serializable]
    public class Wavelet {
        public float time;
        public Transform enemy;
        public int enemyCount;
        public Transform spawnPoint;
    }

    [System.Serializable]
    public class Wave {
        public Wavelet[] wavelets;
    }

    private int waveCounter;
    public Wave[] waves;
    
    void Start() {
        waveCounter = 0;
        TriggerWave();
    }

    // if there are no remaining enemies
    public void WaveHasEnded() {
        this.waveCounter++;
        
        //if (this.waveCounter >= allWaves.count) {
            // Game is over
       // }
    }
    
    // This function babysits the current wave
    public void TriggerWave() {
        Wave currentWave = this.waves[waveCounter];
        Wavelet[] wavelets = currentWave.wavelets;

        // TODO – This should probably not start timers for EVERY wavelet at once, lmao.
        foreach (Wavelet wavelet in wavelets) {
            IEnumerator coroutine = WaveletCoroutine(wavelet);
            StartCoroutine(coroutine);
        }
    }

    private IEnumerator WaveletCoroutine(Wavelet wavelet) {
        yield return new WaitForSeconds(wavelet.time);
        IEnumerator coroutine = EnemySpawnCoroutine(wavelet);
        StartCoroutine(coroutine);
    }

    // every 2 seconds perform the print()
    private IEnumerator EnemySpawnCoroutine(Wavelet wavelet) {
        int enemyCount = 0;
        while (enemyCount <= wavelet.enemyCount)
        {
            yield return new WaitForSeconds(spawnDelay);
            Transform spawnPoint = wavelet.spawnPoint;
            Vector3 spawnPosition = spawnPoint.position;
            enemyCount++;
            Transform enemy = Instantiate(wavelet.enemy, spawnPosition, Quaternion.identity);
        }
    }
}
