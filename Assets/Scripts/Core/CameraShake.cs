using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Shake(
        float duration = 0.15f,
        float strength = 0.15f,
        int vibrato = 15)
    {
        transform.DOKill();

        transform.DOShakePosition(
            duration,
            strength,
            vibrato,
            90f,
            false,
            true
        );
    }
}