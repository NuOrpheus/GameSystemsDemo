using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public WaveData[] waves;
    public Transform spawnPoint;
    public Transform[] pathPoints;

    public float delayBeforeWaveStarts = 1f;
    public WaveUI waveUI;
    public Button startWaveButton;

    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private bool waveRunning = false;

    void Start()
    {
        startWaveButton.onClick.AddListener(StartNextWave);
        startWaveButton.gameObject.SetActive(true);
    }

    public void StartNextWave()
    {
        if (waveRunning) return;
        if (currentWaveIndex >= waves.Length) return;

        startWaveButton.gameObject.SetActive(false);
        AudioManager.Instance.Play(AudioManager.Instance.waveStart);
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        waveRunning = true;

        int waveNumber = currentWaveIndex + 1;

        if (waveUI != null)
            waveUI.ShowMessage("Wave " + waveNumber);

        yield return new WaitForSeconds(delayBeforeWaveStarts);

        yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));

        yield return new WaitUntil(() => enemiesAlive <= 0);

        currentWaveIndex++;
        waveRunning = false;

        if (currentWaveIndex >= waves.Length)
        {
            if (waveUI != null)
                waveUI.ShowMessage("All Waves Complete!");

            if (GameManager.Instance != null)
                GameManager.Instance.Victory();

            yield break;
        }

        if (waveUI != null)
            waveUI.ShowMessage("Wave Complete");

        startWaveButton.gameObject.SetActive(true);
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