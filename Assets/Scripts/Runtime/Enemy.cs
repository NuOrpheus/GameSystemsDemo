using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;

    private int health;
    private Transform[] path;
    private int pathIndex;

    private float baseSpeed;
    private float currentSpeed;
    private float slowTimer;

    private bool isStunned;
    private float stunTimer;

    private WaveSpawner spawner;

    public float Progress => pathIndex;

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

        spawner = waveSpawner;
    }

    void Update()
    {
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
            {
                if (HealthManager.Instance != null)
                {
                    HealthManager.Instance.TakeDamage(data.livesDamage);
                }

                if (spawner != null)
                {
                    spawner.EnemyFinished();
                }

                Destroy(gameObject);
            }
        }
    }

    void UpdateSlow()
    {
        if (slowTimer <= 0f)
            return;

        slowTimer -= Time.deltaTime;

        if (slowTimer <= 0f)
        {
            currentSpeed = baseSpeed;
        }
    }

    void UpdateStun()
    {
        if (!isStunned)
            return;

        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0f)
        {
            isStunned = false;
        }
    }

    public void TakeDamage(int amount)
    {
        if (FloatingTextManager.Instance != null && amount > 0)
        {
            FloatingTextManager.Instance.Spawn(
                amount.ToString(),
                transform.position + Vector3.up
            );
        }

        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.Add(data.reward);
        }

        if (spawner != null)
        {
            spawner.EnemyFinished();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(
                AudioManager.Instance.enemyDeath,
                0.5f
            );
        }

        Destroy(gameObject);
    }

    public void ApplySlow(float multiplier, float duration)
    {
        multiplier = Mathf.Clamp(multiplier, 0.05f, 1f);

        currentSpeed = baseSpeed * multiplier;
        slowTimer = duration;
    }

    public void ApplyStun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
    }

    public void ApplyPoison(
        int damagePerTick,
        float duration,
        float tickRate)
    {
        StartCoroutine(
            PoisonRoutine(
                damagePerTick,
                duration,
                tickRate
            )
        );
    }

    IEnumerator PoisonRoutine(
        int damagePerTick,
        float duration,
        float tickRate)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            TakeDamage(damagePerTick);

            yield return new WaitForSeconds(tickRate);

            elapsed += tickRate;
        }
    }

    public void ApplyBurn(
        int damagePerTick,
        float duration,
        float tickRate)
    {
        StartCoroutine(
            BurnRoutine(
                damagePerTick,
                duration,
                tickRate
            )
        );
    }

    IEnumerator BurnRoutine(
        int damagePerTick,
        float duration,
        float tickRate)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            TakeDamage(damagePerTick);

            yield return new WaitForSeconds(tickRate);

            elapsed += tickRate;
        }
    }
}