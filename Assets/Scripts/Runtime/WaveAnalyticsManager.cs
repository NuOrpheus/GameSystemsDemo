using UnityEngine;

public class WaveAnalyticsManager : MonoBehaviour
{
    public static WaveAnalyticsManager Instance;

    public int currentWaveNumber;

    public int enemiesKilled;
    public int enemiesReachedEnd;
    public int livesLost;

    public int resourcesEarnedFromKills;
    public int resourcesGeneratedByTowers;
    public int resourcesSpent;

    void Awake()
    {
        Instance = this;
    }

    public void StartWave(int waveNumber)
    {
        currentWaveNumber = waveNumber;

        enemiesKilled = 0;
        enemiesReachedEnd = 0;
        livesLost = 0;

        resourcesEarnedFromKills = 0;
        resourcesGeneratedByTowers = 0;
        resourcesSpent = 0;

        WaveAnalyticsUI ui = FindAnyObjectByType<WaveAnalyticsUI>();

        if (ui != null)
            ui.Hide();
    }

    public void EndWave()
    {
        WaveAnalyticsUI ui = FindAnyObjectByType<WaveAnalyticsUI>();

        if (ui != null)
            ui.ShowSummary();
    }

    public void RecordEnemyKilled(int reward)
    {
        enemiesKilled++;
        resourcesEarnedFromKills += reward;
    }

    public void RecordEnemyReachedEnd(int livesDamage)
    {
        enemiesReachedEnd++;
        livesLost += livesDamage;
    }

    public void RecordTowerGeneratedResources(int amount)
    {
        resourcesGeneratedByTowers += amount;
    }

    public void RecordResourcesSpent(int amount)
    {
        resourcesSpent += amount;
    }
}