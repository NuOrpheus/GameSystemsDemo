using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public WaveData wave;
    public Transform spawnPoint;
    public Transform[] pathPoints;

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
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