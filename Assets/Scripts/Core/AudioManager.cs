using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip gameplayMusic;
    [SerializeField] AudioClip victoryMusic;
    [SerializeField] AudioClip defeatMusic;

    [Header("SFX")]
    [SerializeField] AudioClip placeTower;
    [SerializeField] AudioClip shoot;
    [SerializeField] AudioClip enemyDeath;
    [SerializeField] AudioClip notEnoughResources;
    [SerializeField] AudioClip waveStart;
    [SerializeField] AudioClip sellTower;
    [SerializeField] AudioClip upgradeTower;
    [SerializeField] AudioClip gameOver;
    [SerializeField] AudioClip victory;
    [SerializeField] AudioClip buttonClick;

    [Header("Shoot Settings")]
    [SerializeField] float shootCooldown = 0.05f;
    [SerializeField] float shootVolume = 0.15f;

    private float lastShootTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayMenuMusic() => PlayMusic(menuMusic, true);
    public void PlayGameplayMusic() => PlayMusic(gameplayMusic, true);
    public void PlayVictoryMusic() => PlayMusic(victoryMusic, false);
    public void PlayDefeatMusic() => PlayMusic(defeatMusic, false);

    void PlayMusic(AudioClip clip, bool loop)
    {
        if (clip == null || musicSource == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource?.Stop();
    }

    void PlaySFX(AudioClip clip, float volume = 1f, bool randomPitch = false)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.pitch = randomPitch
            ? Random.Range(0.95f, 1.05f)
            : 1f;

        sfxSource.PlayOneShot(clip, volume);
        sfxSource.pitch = 1f;
    }

    public void PlayPlaceTower() => PlaySFX(placeTower);
    public void PlayEnemyDeath() => PlaySFX(enemyDeath, 0.5f, true);
    public void PlayNotEnoughResources() => PlaySFX(notEnoughResources);
    public void PlayWaveStart() => PlaySFX(waveStart);
    public void PlaySellTower() => PlaySFX(sellTower);
    public void PlayUpgradeTower() => PlaySFX(upgradeTower);
    public void PlayGameOver() => PlaySFX(gameOver);
    public void PlayVictorySFX() => PlaySFX(victory);
    public void PlayButtonClick() => PlaySFX(buttonClick);

    public void PlayShoot()
    {
        if (Time.time - lastShootTime < shootCooldown)
            return;

        lastShootTime = Time.time;
        PlaySFX(shoot, shootVolume, true);
    }
}