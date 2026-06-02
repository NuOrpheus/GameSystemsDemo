using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public WaveData[] waves;
    public Transform spawnPoint;
    public Transform[] pathPoints;

    public float delayBetweenWaves = 5f;
    public WaveUI waveUI;

    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;

    void Start()
    {
        StartCoroutine(SpawnAllWaves());
    }

    IEnumerator SpawnAllWaves()
    {
        while (currentWaveIndex < waves.Length)
        {
            int waveNumber = currentWaveIndex + 1;

            if (waveUI != null)
                waveUI.ShowMessage("Wave " + waveNumber);

            yield return new WaitForSeconds(2f);

            WaveData wave = waves[currentWaveIndex];

            yield return StartCoroutine(SpawnWave(wave));

            yield return new WaitUntil(() => enemiesAlive <= 0);

            currentWaveIndex++;

            if (currentWaveIndex < waves.Length)
            {
                if (waveUI != null)
                    waveUI.ShowMessage("Wave Complete");

                yield return new WaitForSeconds(delayBetweenWaves);
            }
        }

        if (waveUI != null)
            waveUI.ShowMessage("All Waves Complete!");
        if (GameManager.Instance != null)
            GameManager.Instance.Victory();
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        foreach (WaveSpawn spawn in wave.spawns)
        {
            enemiesAlive++;

            GameObject obj = Instantiate(
                spawn.enemy.prefab,
                spawnPoint.position,
                Quaternion.identity
            );

            Enemy enemy = obj.GetComponent<Enemy>();
            enemy.Init(spawn.enemy, pathPoints, this);

            yield return new WaitForSeconds(spawn.delayAfterSpawn);
        }
    }

    public void EnemyFinished()
    {
        enemiesAlive--;
    }
}