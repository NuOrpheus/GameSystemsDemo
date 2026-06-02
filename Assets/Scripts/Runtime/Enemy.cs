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

    public float Progress => pathIndex;

    public void Init(EnemyData enemyData, Transform[] pathPoints)
    {
        data = enemyData;
        health = data.maxHealth;
        path = pathPoints;
        pathIndex = 0;

        baseSpeed = data.moveSpeed;
        currentSpeed = baseSpeed;
        slowTimer = 0f;
    }

    void Update()
    {
        if (data == null) return;
        if (path == null || pathIndex >= path.Length) return;

        UpdateSlow();

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

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.Add(data.reward);
            }
            Destroy(gameObject);
        }
    }

    public void ApplySlow(float multiplier, float duration)
    {
        multiplier = Mathf.Clamp(multiplier, 0.05f, 1f);

        currentSpeed = baseSpeed * multiplier;
        slowTimer = duration;
    }
}