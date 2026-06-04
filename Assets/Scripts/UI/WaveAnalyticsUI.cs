using TMPro;
using UnityEngine;

public class WaveAnalyticsUI : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TMP_Text summaryText;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (summaryText == null)
            summaryText = GetComponentInChildren<TMP_Text>();

        Hide();
    }

    public void ShowSummary()
    {
        WaveAnalyticsManager analytics = WaveAnalyticsManager.Instance;

        if (analytics == null || summaryText == null)
            return;

        summaryText.text =
            $"Wave {analytics.currentWaveNumber} Summary\n\n" +
            $"Enemies Killed: {analytics.enemiesKilled}\n" +
            $"Enemies Reached End: {analytics.enemiesReachedEnd}\n" +
            $"Lives Lost: {analytics.livesLost}\n\n" +
            $"Resources From Kills: {analytics.resourcesEarnedFromKills}\n" +
            $"Resources From Towers: {analytics.resourcesGeneratedByTowers}\n" +
            $"Resources Spent: {analytics.resourcesSpent}";

        Show();
    }

    public void Show()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}