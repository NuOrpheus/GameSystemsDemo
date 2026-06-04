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
        if (startWaveButton != null)
        {
            startWaveButton.onClick.AddListener(StartNextWave);
            startWaveButton.gameObject.SetActive(true);
        }
    }

    public void StartNextWave()
    {
        if (waveRunning) return;
        if (currentWaveIndex >= waves.Length) return;

        if (startWaveButton != null)
            startWaveButton.gameObject.SetActive(false);

        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioManager.Instance.waveStart);

        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        waveRunning = true;
        enemiesAlive = 0;

        int waveNumber = currentWaveIndex + 1;

        if (WaveAnalyticsManager.Instance != null)
            WaveAnalyticsManager.Instance.StartWave(waveNumber);

        if (waveUI != null)
            waveUI.ShowMessage("Wave " + waveNumber);

        yield return new WaitForSeconds(delayBeforeWaveStarts);

        yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));

        yield return new WaitUntil(() => enemiesAlive <= 0);

        if (WaveAnalyticsManager.Instance != null)
            WaveAnalyticsManager.Instance.EndWave();

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

        if (startWaveButton != null)
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

            if (enemy != null)
            {
                enemy.Init(
                    spawn.enemy,
                    pathPoints,
                    this
                );
            }

            yield return new WaitForSeconds(spawn.delayAfterSpawn);
        }
    }

    public void EnemyFinished()
    {
        enemiesAlive--;

        if (enemiesAlive < 0)
            enemiesAlive = 0;
    }
}