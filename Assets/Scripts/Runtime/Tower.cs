using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerData data;

    private float fireCooldown;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        Enemy target = FindTarget();
        if (target == null) return;

        transform.LookAt(target.transform);

        if (fireCooldown <= 0f)
        {
            Shoot(target);
            fireCooldown = 1f / data.fireRate;
        }
    }

    Enemy FindTarget()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        Enemy best = null;
        float bestDistance = Mathf.Infinity;

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance > data.range) continue;

            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = enemy;
            }
        }

        return best;
    }

    void Shoot(Enemy target)
    {
        GameObject projectileObj = Instantiate(
            data.projectilePrefab,
            transform.position + Vector3.up,
            Quaternion.identity
        );

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.Init(target, data.damage, data.projectileSpeed);
    }
}
