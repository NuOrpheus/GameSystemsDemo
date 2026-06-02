using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public WaveData[] waves;
    public Transform spawnPoint;
    public Transform[] pathPoints;

    public float delayBetweenWaves = 5f;

    private int currentWaveIndex = 0;

    void Start()
    {
        StartCoroutine(SpawnAllWaves());
    }

    IEnumerator SpawnAllWaves()
    {
        while (currentWaveIndex < waves.Length)
        {
            WaveData wave = waves[currentWaveIndex];

            Debug.Log("Starting wave " + (currentWaveIndex + 1));

            yield return StartCoroutine(SpawnWave(wave));

            currentWaveIndex++;

            if (currentWaveIndex < waves.Length)
                yield return new WaitForSeconds(delayBetweenWaves);
        }

        Debug.Log("All waves finished");
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        foreach (WaveSpawn spawn in wave.spawns)
        {
            GameObject obj = Instantiate(
                spawn.enemy.prefab,
                spawnPoint.position,
                Quaternion.identity
            );

            Enemy enemy = obj.GetComponent<Enemy>();
            enemy.Init(spawn.enemy, pathPoints);

            yield return new WaitForSeconds(spawn.delayAfterSpawn);
        }
    }
}