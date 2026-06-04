using System.Collections;
using UnityEngine;

public class TowerUpgradeVisuals : MonoBehaviour
{
    [Header("Upgrade Burst")]
    [SerializeField] GameObject upgradeBurstPrefab;
    [SerializeField] Transform burstPoint;
    [SerializeField] float burstLifetime = 1f;

    [Header("Level Stars")]
    [SerializeField] GameObject starPrefab;
    [SerializeField] Transform starContainer;
    [SerializeField] float starSpacing = 0.25f;
    [SerializeField] int maxStars = 5;

    [Header("Scale Pulse")]
    [SerializeField] float pulseScale = 1.2f;
    [SerializeField] float pulseDuration = 0.15f;

    [Header("Range Flash")]
    [SerializeField] LineRenderer rangeLine;
    [SerializeField] int rangeSegments = 64;
    [SerializeField] float rangeFlashDuration = 0.6f;

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

        Destroy(burst, burstLifetime);
    }

    public void UpdateStars(int level)
    {
        if (starContainer == null || starPrefab == null)
            return;

        ClearStars();

        level = Mathf.Clamp(level, 0, maxStars);

        for (int i = 0; i < level; i++)
            CreateStar(i, level);
    }

    void ClearStars()
    {
        for (int i = starContainer.childCount - 1; i >= 0; i--)
            Destroy(starContainer.GetChild(i).gameObject);
    }

    void CreateStar(int index, int totalStars)
    {
        GameObject star = Instantiate(starPrefab, starContainer);

        float offset = (index - (totalStars - 1) * 0.5f) * starSpacing;

        star.transform.localPosition = new Vector3(offset, 0f, 0f);
        star.transform.localRotation = Quaternion.identity;
        star.transform.localScale = Vector3.one;
    }

    void PlayPulse()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(PulseRoutine());
    }

    IEnumerator PulseRoutine()
    {
        yield return ScaleRoutine(originalScale, originalScale * pulseScale, pulseDuration);
        yield return ScaleRoutine(originalScale * pulseScale, originalScale, pulseDuration);

        transform.localScale = originalScale;
        pulseRoutine = null;
    }

    IEnumerator ScaleRoutine(Vector3 from, Vector3 to, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            transform.localScale = Vector3.Lerp(from, to, t);

            yield return null;
        }
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
        rangeRoutine = null;
    }

    void DrawRange(float radius)
    {
        radius = Mathf.Max(0.1f, radius);
        rangeSegments = Mathf.Max(8, rangeSegments);

        rangeLine.positionCount = rangeSegments + 1;
        rangeLine.useWorldSpace = false;

        for (int i = 0; i <= rangeSegments; i++)
        {
            float angle = i * Mathf.PI * 2f / rangeSegments;

            rangeLine.SetPosition(
                i,
                new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0f
                )
            );
        }
    }

    void OnDisable()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        if (rangeRoutine != null)
            StopCoroutine(rangeRoutine);

        if (rangeLine != null)
            rangeLine.enabled = false;

        transform.localScale = originalScale;
    }
}