using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("End Screen")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI subtitleText;

    [Header("Effects")]
    public GameObject victoryEffectPrefab;
    public GameObject defeatEffectPrefab;
    public Transform effectSpawnPoint;

    [Header("Timing")]
    public float endDelay = 0.75f;

    private bool gameEnded;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

        if (MusicManager.Instance != null)
            MusicManager.Instance.Play(MusicManager.Instance.gameplayMusic);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void GameOver()
    {
        if (gameEnded) return;

        gameEnded = true;
        StartCoroutine(EndGameRoutine(false));
    }

    public void Victory()
    {
        if (gameEnded) return;

        gameEnded = true;
        StartCoroutine(EndGameRoutine(true));
    }

    IEnumerator EndGameRoutine(bool victory)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.Play(
                victory
                    ? MusicManager.Instance.victoryMusic
                    : MusicManager.Instance.defeatMusic,
                false
            );
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(
                victory ? 0.12f : 0.25f,
                victory ? 0.08f : 0.2f,
                victory ? 8 : 20
            );
        }

        GameObject effectPrefab = victory
            ? victoryEffectPrefab
            : defeatEffectPrefab;

        if (effectPrefab != null)
        {
            Vector3 spawnPosition = effectSpawnPoint != null
                ? effectSpawnPoint.position
                : Vector3.zero;

            Instantiate(effectPrefab, spawnPosition, Quaternion.identity);
        }

        yield return new WaitForSecondsRealtime(endDelay);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverText != null)
            gameOverText.text = victory ? "Victory!" : "Game Over";

        if (subtitleText != null)
        {
            subtitleText.text = victory
                ? "You survived every wave."
                : "The base was overrun.";
        }

        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}