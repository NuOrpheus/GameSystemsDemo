using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerData data;

    private float fireCooldown;

    void Update()
    {
        if (data == null) return;

        fireCooldown -= Time.deltaTime;

        Enemy target = FindTarget();
        if (target == null) return;

        //RotateTowards(target);

        if (fireCooldown <= 0f)
        {
            Shoot(target);
            fireCooldown = 1f / data.fireRate;
        }
    }

    Enemy FindTarget()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>();

        Enemy bestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= data.range && distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    void RotateTowards(Enemy target)
    {
        Vector3 direction = target.transform.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Use angle - 90 if your tower sprite points upward by default.
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    void Shoot(Enemy target)
    {
        if (data.projectilePrefab == null) return;

        GameObject projectileObj = Instantiate(
            data.projectilePrefab,
            transform.position + Vector3.up,
            Quaternion.identity
        );

        Projectile projectile = projectileObj.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Init(target, data.damage, data.projectileSpeed);
        }
    }
}