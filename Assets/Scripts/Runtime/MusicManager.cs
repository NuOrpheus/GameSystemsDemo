using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource audioSource;

    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Play(AudioClip clip, bool loop = true)
    {
        if (clip == null || audioSource == null)
            return;

        if (audioSource.clip == clip)
            return;

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }
}