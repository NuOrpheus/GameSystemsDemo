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
        foreach (WaveEntry entry in wave.enemies)
        {
            for (int i = 0; i < entry.count; i++)
            {
                GameObject obj = Instantiate(entry.enemy.prefab, spawnPoint.position, Quaternion.identity);
                Enemy enemy = obj.GetComponent<Enemy>();
                enemy.Init(entry.enemy, pathPoints);

                yield return new WaitForSeconds(entry.spawnInterval);
            }
        }
    }
}