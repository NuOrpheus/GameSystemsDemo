using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("End Screen")]
    [SerializeField] GameObject endPanel;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI subtitleText;

    [Header("End Effects")]
    [SerializeField] GameObject victoryEffectPrefab;
    [SerializeField] GameObject defeatEffectPrefab;
    [SerializeField] Transform effectSpawnPoint;

    [Header("Timing")]
    [SerializeField] float endDelay = 0.75f;

    private bool gameEnded;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

        endPanel?.SetActive(false);
    }

    void Start()
    {
        AudioManager.Instance?.PlayGameplayMusic();
    }

    public void GameOver()
    {
        EndGame(false);
    }

    public void Victory()
    {
        EndGame(true);
    }

    void EndGame(bool victory)
    {
        if (gameEnded)
            return;

        gameEnded = true;
        StartCoroutine(EndGameRoutine(victory));
    }

    IEnumerator EndGameRoutine(bool victory)
    {
        PlayEndAudio(victory);
        PlayEndShake(victory);
        SpawnEndEffect(victory);

        yield return new WaitForSecondsRealtime(endDelay);

        ShowEndPanel(victory);
        Time.timeScale = 0f;
    }

    void PlayEndAudio(bool victory)
    {
        if (victory)
        {
            AudioManager.Instance?.PlayVictoryMusic();
            AudioManager.Instance?.PlayVictorySFX();
        }
        else
        {
            AudioManager.Instance?.PlayDefeatMusic();
            AudioManager.Instance?.PlayGameOver();
        }
    }

    void PlayEndShake(bool victory)
    {
        CameraShake.Instance?.Shake(
            victory ? 0.12f : 0.25f,
            victory ? 0.08f : 0.2f,
            victory ? 8 : 20
        );
    }

    void SpawnEndEffect(bool victory)
    {
        GameObject prefab = victory ? victoryEffectPrefab : defeatEffectPrefab;

        if (prefab == null)
            return;

        Instantiate(
            prefab,
            GetEffectSpawnPosition(),
            Quaternion.identity
        );
    }

    Vector3 GetEffectSpawnPosition()
    {
        if (effectSpawnPoint != null)
            return effectSpawnPoint.position;

        if (Camera.main == null)
            return Vector3.zero;

        Vector3 position = Camera.main.transform.position;
        position.z = 0f;

        return position;
    }

    void ShowEndPanel(bool victory)
    {
        endPanel?.SetActive(true);

        if (titleText != null)
            titleText.text = victory ? "Victory!" : "Game Over";

        if (subtitleText != null)
        {
            subtitleText.text = victory
                ? "You survived every wave."
                : "The base was overrun.";
        }
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