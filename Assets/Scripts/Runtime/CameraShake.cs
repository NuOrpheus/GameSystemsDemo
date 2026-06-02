using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    void Awake()
    {
        Instance = this;
    }

    public void Shake(
        float duration = 0.15f,
        float strength = 0.15f,
        int vibrato = 15)
    {
        transform.DOComplete();

        transform.DOShakePosition(
            duration,
            strength,
            vibrato,
            90,
            false,
            true
        );
    }
}