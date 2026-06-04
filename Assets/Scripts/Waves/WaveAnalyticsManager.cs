using UnityEngine;

public class WaveAnalyticsManager : MonoBehaviour
{
    public static WaveAnalyticsManager Instance { get; private set; }

    [Header("Wave")]
    public int currentWaveNumber;

    [Header("Enemies")]
    public int enemiesKilled;
    public int enemiesReachedEnd;
    public int livesLost;

    [Header("Resources")]
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
        ResetStats();
        HideUI();
    }

    public void EndWave()
    {
        ShowUI();
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

    void ResetStats()
    {
        enemiesKilled = 0;
        enemiesReachedEnd = 0;
        livesLost = 0;

        resourcesEarnedFromKills = 0;
        resourcesGeneratedByTowers = 0;
        resourcesSpent = 0;
    }

    void HideUI()
    {
        WaveAnalyticsUI ui = FindAnyObjectByType<WaveAnalyticsUI>();
        ui?.Hide();
    }

    void ShowUI()
    {
        WaveAnalyticsUI ui = FindAnyObjectByType<WaveAnalyticsUI>();
        ui?.ShowSummary();
    }
}