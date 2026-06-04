using System.Text;
using TMPro;
using UnityEngine;

public class WaveAnalyticsUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text summaryText;

    void Start()
    {
        Hide();
    }

    public void ShowSummary()
    {
        WaveAnalyticsManager a = WaveAnalyticsManager.Instance;

        if (a == null || summaryText == null)
            return;

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Wave " + a.currentWaveNumber + " Summary");
        sb.AppendLine();

        sb.AppendLine("Enemies Killed: " + a.enemiesKilled);
        sb.AppendLine("Enemies Reached End: " + a.enemiesReachedEnd);
        sb.AppendLine("Lives Lost: " + a.livesLost);
        sb.AppendLine();

        sb.AppendLine("Resources From Kills: " + a.resourcesEarnedFromKills);
        sb.AppendLine("Resources From Towers: " + a.resourcesGeneratedByTowers);
        sb.AppendLine("Resources Spent: " + a.resourcesSpent);

        summaryText.text = sb.ToString();

        if (panel != null)
            panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}