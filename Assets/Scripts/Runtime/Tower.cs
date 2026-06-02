using UnityEngine;
using UnityEngine.InputSystem;

public class Tower : MonoBehaviour
{
    public TowerData data;
    public BuildNode buildNode;

    private float fireCooldown;

    void Update()
    {
        if (data == null) return;

        fireCooldown -= Time.deltaTime;

        Enemy target = FindTarget();
        if (target == null) return;

        if (fireCooldown <= 0f)
        {
            Shoot(target);
            fireCooldown = 1f / data.fireRate;
        }
    }

    public void Sell()
    {
        if (data == null) return;

        int refund = Mathf.RoundToInt(data.cost * 0.75f);

        if (ResourceManager.Instance != null)
            ResourceManager.Instance.Add(refund);

        if (buildNode != null)
            buildNode.SetOccupied(false);

        Destroy(gameObject);
    }

    Enemy FindTarget()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

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
            projectile.Init(
                target,
                data.damage,
                data.projectileSpeed,
                data.effectType,
                data.slowMultiplier,
                data.slowDuration
            );
        }
    }
    void OnMouseOver()
    {
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            Sell();
        }
    }
}