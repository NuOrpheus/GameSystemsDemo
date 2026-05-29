using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;
    private int health;
    private Transform[] path;
    private int pathIndex;

    public float Progress => pathIndex;

    public void Init(EnemyData enemyData, Transform[] pathPoints)
    {
        data = enemyData;
        health = data.maxHealth;
        path = pathPoints;
        pathIndex = 0;
    }

    void Update()
    {
        if (path == null || pathIndex >= path.Length) return;

        Transform target = path[pathIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            data.moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            pathIndex++;

            if (pathIndex >= path.Length)
            {
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            // Add currency here later.
            Destroy(gameObject);
        }
    }
}