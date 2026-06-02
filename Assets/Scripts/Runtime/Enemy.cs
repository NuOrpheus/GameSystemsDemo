using System.Collections;
using UnityEngine;
using static FloatingText;

public class Enemy : MonoBehaviour
{
    public EnemyData data;

    [Header("Effects")]
    public GameObject fireEffectPrefab;
    public GameObject poisonEffectPrefab;

    private GameObject activeFireEffect;
    private GameObject activePoisonEffect;

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
        if (isDead) return;
        if (data == null) return;
        if (path == null || pathIndex >= path.Length) return;

        UpdateSlow();
        UpdateStun();

        if (isStunned)
            return;

        Transform target = path[pathIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            currentSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            pathIndex++;

            if (pathIndex >= path.Length)
                ReachEnd();
        }
    }

    void ReachEnd()
    {
        if (isDead) return;
        isDead = true;

        if (HealthManager.Instance != null)
            HealthManager.Instance.TakeDamage(data.livesDamage);

        if (spawner != null)
            spawner.EnemyFinished();

        Destroy(gameObject);
    }

    void UpdateSlow()
    {
        if (slowTimer <= 0f)
            return;

        slowTimer -= Time.deltaTime;

        if (slowTimer <= 0f)
            currentSpeed = baseSpeed;
    }

    void UpdateStun()
    {
        if (!isStunned)
            return;

        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0f)
            isStunned = false;
    }

    public void TakeDamage(int amount)
    {
        ApplyDamage(amount, FloatingTextType.Normal);
    }

    void ApplyDamage(int amount, FloatingTextType textType)
    {
        if (isDead)
            return;

        if (FloatingTextManager.Instance != null && amount > 0)
        {
            FloatingTextManager.Instance.Spawn(
                amount.ToString(),
                transform.position + Vector3.up,
                textType
            );
        }

        health -= amount;

        if (health <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (ResourceManager.Instance != null)
            ResourceManager.Instance.Add(data.reward);

        if (spawner != null)
            spawner.EnemyFinished();

        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioManager.Instance.enemyDeath, 0.5f);
        
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(
                0.08f,
                0.05f,
                10
            );
        }

        Destroy(gameObject);
    }

    public void ApplySlow(float multiplier, float duration)
    {
        if (isDead) return;

        multiplier = Mathf.Clamp(multiplier, 0.05f, 1f);

        currentSpeed = baseSpeed * multiplier;
        slowTimer = duration;
    }

    public void ApplyStun(float duration)
    {
        if (isDead) return;

        isStunned = true;
        stunTimer = duration;
    }

    public void ApplyPoison(
        int damagePerTick,
        float duration,
        float tickRate)
    {
        if (isDead) return;

        StartPoisonEffect();
        StartCoroutine(PoisonRoutine(damagePerTick, duration, tickRate));
    }

    IEnumerator PoisonRoutine(
        int damagePerTick,
        float duration,
        float tickRate)
    {
        float elapsed = 0f;

        while (elapsed < duration && !isDead)
        {
            ApplyDamage(damagePerTick, FloatingTextType.Poison);

            yield return new WaitForSeconds(tickRate);

            elapsed += tickRate;
        }

        StopPoisonEffect();
    }

    public void ApplyBurn(
        int damagePerTick,
        float duration,
        float tickRate)
    {
        if (isDead) return;

        StartFireEffect();
        StartCoroutine(BurnRoutine(damagePerTick, duration, tickRate));
    }

    IEnumerator BurnRoutine(
        int damagePerTick,
        float duration,
        float tickRate)
    {
        float elapsed = 0f;

        while (elapsed < duration && !isDead)
        {
            ApplyDamage(damagePerTick, FloatingTextType.Burn);

            yield return new WaitForSeconds(tickRate);

            elapsed += tickRate;
        }

        StopFireEffect();
    }

    void StartFireEffect()
    {
        if (fireEffectPrefab != null && activeFireEffect == null)
        {
            activeFireEffect = Instantiate(
                fireEffectPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );

            activeFireEffect.transform.localPosition = Vector3.zero;
        }

        SetColor(new Color(1f, 0.65f, 0.65f));
    }

    void StopFireEffect()
    {
        if (activeFireEffect != null)
        {
            Destroy(activeFireEffect);
            activeFireEffect = null;
        }

        ResetColorIfNoEffects();
    }

    void StartPoisonEffect()
    {
        if (poisonEffectPrefab != null && activePoisonEffect == null)
        {
            activePoisonEffect = Instantiate(
                poisonEffectPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );

            activePoisonEffect.transform.localPosition = Vector3.zero;
        }

        SetColor(new Color(0.6f, 1f, 0.6f));
    }

    void StopPoisonEffect()
    {
        if (activePoisonEffect != null)
        {
            Destroy(activePoisonEffect);
            activePoisonEffect = null;
        }

        ResetColorIfNoEffects();
    }

    void SetColor(Color color)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }

    void ResetColorIfNoEffects()
    {
        if (activeFireEffect == null && activePoisonEffect == null)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = originalColor;
        }
    }
}