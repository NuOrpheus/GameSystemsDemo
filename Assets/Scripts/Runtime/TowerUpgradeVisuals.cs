using System.Collections;
using UnityEngine;

public class TowerUpgradeVisuals : MonoBehaviour
{
    [Header("Upgrade Burst")]
    public GameObject upgradeBurstPrefab;
    public Transform burstPoint;

    [Header("Level Stars")]
    public GameObject starPrefab;
    public Transform starContainer;
    public float starSpacing = 0.25f;

    [Header("Scale Pulse")]
    public float pulseScale = 1.2f;
    public float pulseDuration = 0.15f;

    [Header("Range Flash")]
    public LineRenderer rangeLine;
    public int rangeSegments = 64;
    public float rangeFlashDuration = 0.6f;

    private Vector3 originalScale;
    private Coroutine pulseRoutine;
    private Coroutine rangeRoutine;

    void Awake()
    {
        originalScale = transform.localScale;

        if (burstPoint == null)
            burstPoint = transform;

        if (rangeLine != null)
            rangeLine.enabled = false;

        UpdateStars(0);
    }

    public void PlayUpgradeVisuals(int level, float range)
    {
        PlayBurst();
        UpdateStars(level);
        PlayPulse();
        FlashRange(range);
    }

    void PlayBurst()
    {
        if (upgradeBurstPrefab == null)
            return;

        GameObject burst = Instantiate(
            upgradeBurstPrefab,
            burstPoint.position,
            Quaternion.identity
        );

        Destroy(burst, 1f);
    }

    public void UpdateStars(int level)
    {
        if (starContainer == null || starPrefab == null)
            return;

        foreach (Transform child in starContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < level; i++)
        {
            GameObject star = Instantiate(
                starPrefab,
                starContainer.position,
                Quaternion.identity,
                starContainer
            );

            float offset = (i - (level - 1) * 0.5f) * starSpacing;
            star.transform.localPosition = new Vector3(offset, 0f, 0f);
        }
    }

    void PlayPulse()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(PulseRoutine());
    }

    IEnumerator PulseRoutine()
    {
        float timer = 0f;

        while (timer < pulseDuration)
        {
            timer += Time.deltaTime;
            float t = timer / pulseDuration;

            transform.localScale = Vector3.Lerp(
                originalScale,
                originalScale * pulseScale,
                t
            );

            yield return null;
        }

        timer = 0f;

        while (timer < pulseDuration)
        {
            timer += Time.deltaTime;
            float t = timer / pulseDuration;

            transform.localScale = Vector3.Lerp(
                originalScale * pulseScale,
                originalScale,
                t
            );

            yield return null;
        }

        transform.localScale = originalScale;
    }

    void FlashRange(float range)
    {
        if (rangeLine == null)
            return;

        if (rangeRoutine != null)
            StopCoroutine(rangeRoutine);

        rangeRoutine = StartCoroutine(RangeFlashRoutine(range));
    }

    IEnumerator RangeFlashRoutine(float range)
    {
        DrawRange(range);

        rangeLine.enabled = true;

        yield return new WaitForSeconds(rangeFlashDuration);

        rangeLine.enabled = false;
    }

    void DrawRange(float radius)
    {
        rangeLine.positionCount = rangeSegments + 1;
        rangeLine.useWorldSpace = false;

        for (int i = 0; i <= rangeSegments; i++)
        {
            float angle = i * Mathf.PI * 2f / rangeSegments;

            Vector3 point = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );

            rangeLine.SetPosition(i, point);
        }
    }
}