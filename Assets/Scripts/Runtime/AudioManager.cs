using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource sfxSource;

    public AudioClip placeTower;
    public AudioClip shoot;
    public AudioClip enemyDeath;
    public AudioClip notEnoughResources;
    public AudioClip waveStart;
    public AudioClip sellTower;

    [Header("Shoot Sound Settings")]
    public float shootSoundCooldown = 0.05f;

    private float lastShootSoundTime;

    void Awake()
    {
        Instance = this;
    }

    public void Play(AudioClip clip, float volume = 1f, bool randomPitch = false)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.pitch = randomPitch
            ? Random.Range(0.95f, 1.05f)
            : 1f;

        sfxSource.PlayOneShot(clip, volume);

        // Reset pitch so it doesn't affect later sounds
        sfxSource.pitch = 1f;
    }

    public void PlayShoot()
    {
        if (Time.time - lastShootSoundTime < shootSoundCooldown)
            return;

        lastShootSoundTime = Time.time;

        Play(shoot, 0.15f, true);
    }
}