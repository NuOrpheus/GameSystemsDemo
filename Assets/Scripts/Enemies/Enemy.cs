using System.Collections;
using UnityEngine;
using static FloatingText;

[System.Serializable]
public class StatusEffectVisual
{
    public GameObject prefab;
    [HideInInspector] public GameObject instance;
}

public class Enemy : MonoBehaviour
{
    public EnemyData data;

    [Header("Status Effect Visuals")]
    public StatusEffectVisual fire;
    public StatusEffectVisual poison;
    public StatusEffectVisual slow;
    public StatusEffectVisual stun;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private int health;
    private Transform[] path;
    private int pathIndex;

    private float baseSpeed;
    private float currentSpeed;
    private float slowTimer;

    private bool isStunned;
    private float stunTimer;

    private WaveSpawner spawner;
    private bool isDead;

    public float Progress => pathIndex;
    public int CurrentHealth => health;

    public static readonly System.Collections.Generic.List<Enemy> ActiveEnemies = new();

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void Init(
        EnemyData enemyData,
        Transform[] pathPoints,
        WaveSpawner waveSpawner)
    {
        data = enemyData;
        health = data.maxHealth;

        path = pathPoints;
        pathIndex = 0;

        baseSpeed = data.moveSpeed;
        currentSpeed = baseSpeed;

        slowTimer = 0f;
        stunTimer = 0f;
        isStunned = false;
        isDead = false;

        spawner = waveSpawner;
    }

    void Update()
    {
        if (isDead || data == null || path == null || pathIndex >= path.Length)
            return;

        UpdateSlow();
        UpdateStun();

        if (isStunned)
            return;

        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        Transform target = path[pathIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            currentSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) >= 0.05f)
            return;

        pathIndex++;

        if (pathIndex >= path.Length)
            ReachEnd();
    }

    void ReachEnd()
    {
        if (isDead)
            return;

        isDead = true;

        CleanupEffects();

        HealthManager.Instance?.TakeDamage(data.livesDamage);
        WaveAnalyticsManager.Instance?.RecordEnemyReachedEnd(data.livesDamage);

        spawner?.EnemyFinished();

        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        ApplyDamage(amount, FloatingTextType.Normal);
    }

    public void TakeSplashDamage(int amount)
    {
        ApplyDamage(amount, FloatingTextType.Normal);
    }

    void ApplyDamage(int amount, FloatingTextType textType)
    {
        if (isDead)
            return;

        FloatingTextManager.Instance?.Spawn(
            amount.ToString(),
            transform.position + Vector3.up,
            textType
        );

        health -= amount;

        if (health <= 0)
            Die();
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        CleanupEffects();

        ResourceManager.Instance?.Add(data.reward);
        WaveAnalyticsManager.Instance?.RecordEnemyKilled(data.reward);

        spawner?.EnemyFinished();

        AudioManager.Instance?.PlayEnemyDeath();
        CameraShake.Instance?.Shake(0.08f, 0.05f, 10);

        Destroy(gameObject);
    }

    public void ApplySlow(float multiplier, float duration)
    {
        if (isDead)
            return;

        currentSpeed = baseSpeed * Mathf.Clamp(multiplier, 0.05f, 1f);
        slowTimer = duration;

        StartEffect(slow, Vector3.zero, new Color(0.6f, 0.8f, 1f));
    }

    void UpdateSlow()
    {
        if (slowTimer <= 0f)
            return;

        slowTimer -= Time.deltaTime;

        if (slowTimer > 0f)
            return;

        currentSpeed = baseSpeed;
        StopEffect(slow);
    }

    public void ApplyStun(float duration)
    {
        if (isDead)
            return;

        isStunned = true;
        stunTimer = duration;

        StartEffect(stun, Vector3.up * 0.5f, new Color(1f, 1f, 0.5f));
    }

    void UpdateStun()
    {
        if (!isStunned)
            return;

        stunTimer -= Time.deltaTime;

        if (stunTimer > 0f)
            return;

        isStunned = false;
        StopEffect(stun);
    }

    public void ApplyPoison(
        int damagePerTick,
        float duration,
        float tickRate)
    {
        if (isDead)
            return;

        StartEffect(poison, Vector3.zero, new Color(0.6f, 1f, 0.6f));
        StartCoroutine(DamageOverTimeRoutine(
            damagePerTick,
            duration,
            tickRate,
            FloatingTextType.Poison,
            poison
        ));
    }

    public void ApplyBurn(
        int damagePerTick,
        float duration,
        float tickRate)
    {
        if (isDead)
            return;

        StartEffect(fire, Vector3.zero, new Color(1f, 0.65f, 0.65f));
        StartCoroutine(DamageOverTimeRoutine(
            damagePerTick,
            duration,
            tickRate,
            FloatingTextType.Burn,
            fire
        ));
    }

    IEnumerator DamageOverTimeRoutine(
        int damagePerTick,
        float duration,
        float tickRate,
        FloatingTextType textType,
        StatusEffectVisual effect)
    {
        float elapsed = 0f;

        while (elapsed < duration && !isDead)
        {
            ApplyDamage(damagePerTick, textType);

            yield return new WaitForSeconds(tickRate);

            elapsed += tickRate;
        }

        StopEffect(effect);
    }

    void StartEffect(
        StatusEffectVisual effect,
        Vector3 localPosition,
        Color tint)
    {
        if (effect == null)
            return;

        if (effect.prefab != null && effect.instance == null)
        {
            effect.instance = Instantiate(
                effect.prefab,
                transform.position,
                Quaternion.identity,
                transform
            );

            effect.instance.transform.localPosition = localPosition;
        }

        SetColor(tint);
    }

    void StopEffect(StatusEffectVisual effect)
    {
        if (effect == null)
            return;

        if (effect.instance != null)
        {
            Destroy(effect.instance);
            effect.instance = null;
        }

        ResetColorIfNoEffects();
    }

    void CleanupEffects()
    {
        StopEffect(fire);
        StopEffect(poison);
        StopEffect(slow);
        StopEffect(stun);
    }

    void SetColor(Color color)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }

    void ResetColorIfNoEffects()
    {
        if (
            HasEffect(fire) ||
            HasEffect(poison) ||
            HasEffect(slow) ||
            HasEffect(stun)
        )
        {
            return;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    bool HasEffect(StatusEffectVisual effect)
    {
        return effect != null && effect.instance != null;
    }
    void OnEnable()
    {
        if (!ActiveEnemies.Contains(this))
            ActiveEnemies.Add(this);
    }

    void OnDisable()
    {
        ActiveEnemies.Remove(this);
    }
}