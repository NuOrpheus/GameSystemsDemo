using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Setup")]
    [SerializeField] WaveData[] waves;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform[] pathPoints;

    [Header("UI")]
    [SerializeField] WaveUI waveUI;
    [SerializeField] Button startWaveButton;
    [SerializeField] GameObject panel;

    [Header("Timing")]
    [SerializeField] float delayBeforeWaveStarts = 1f;

    private int currentWaveIndex;
    private int enemiesAlive;
    private bool waveRunning;

    public bool IsWaveRunning => waveRunning;

    void Start()
    {
        if (startWaveButton == null)
            return;

        startWaveButton.onClick.AddListener(StartNextWave);
        startWaveButton.gameObject.SetActive(true);
        panel.gameObject.SetActive(true);
    }

    public void StartNextWave()
    {
        if (waveRunning || currentWaveIndex >= waves.Length)
            return;

        startWaveButton?.gameObject.SetActive(false);
        panel?.SetActive(false);
        AudioManager.Instance?.PlayWaveStart();

        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        waveRunning = true;
        enemiesAlive = 0;

        int waveNumber = currentWaveIndex + 1;

        WaveAnalyticsManager.Instance?.StartWave(waveNumber);
        waveUI?.ShowMessage($"Wave {waveNumber}");

        yield return new WaitForSeconds(delayBeforeWaveStarts);
        yield return SpawnWave(waves[currentWaveIndex]);
        yield return new WaitUntil(() => enemiesAlive <= 0);

        WaveAnalyticsManager.Instance?.EndWave();

        currentWaveIndex++;
        waveRunning = false;

        if (currentWaveIndex >= waves.Length)
        {
            waveUI?.ShowMessage("All Waves Complete!");
            GameManager.Instance?.Victory();
            yield break;
        }

        waveUI?.ShowMessage("Wave Complete");
        startWaveButton?.gameObject.SetActive(true);
        panel?.SetActive(true);
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        if (wave == null || wave.Spawns == null)
            yield break;

        foreach (WaveSpawn spawn in wave.Spawns)
        {
            SpawnEnemy(spawn);
            yield return new WaitForSeconds(spawn.DelayAfterSpawn);
        }
    }

    void SpawnEnemy(WaveSpawn spawn)
    {
        EnemyData enemyData = spawn.Enemy;

        if (enemyData == null || enemyData.Prefab == null || spawnPoint == null)
            return;

        enemiesAlive++;

        GameObject obj = Instantiate(
            enemyData.Prefab,
            spawnPoint.position,
            Quaternion.identity
        );

        Enemy enemy = obj.GetComponent<Enemy>();

        if (enemy != null)
            enemy.Init(enemyData, pathPoints, this);
    }

    public void EnemyFinished()
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
    }

    void OnDestroy()
    {
        if (startWaveButton != null)
            startWaveButton.onClick.RemoveListener(StartNextWave);
    }
}