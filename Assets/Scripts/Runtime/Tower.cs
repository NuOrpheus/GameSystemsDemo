using UnityEngine;
using UnityEngine.InputSystem;

public class Tower : MonoBehaviour
{
    public TowerData data;
    public BuildNode buildNode;

    private float fireCooldown;
    private float resourceTimer;

    void Update()
    {
        if (data == null)
            return;

        if (data.effectType == TowerEffectType.Resource)
        {
            UpdateResourceTower();
            return;
        }

        fireCooldown -= Time.deltaTime;

        Enemy target = FindTarget();

        if (target == null)
            return;

        if (fireCooldown <= 0f)
        {
            Shoot(target);
            fireCooldown = 1f / data.fireRate;
        }
    }

    void UpdateResourceTower()
    {
        resourceTimer += Time.deltaTime;

        if (resourceTimer >= data.resourceInterval)
        {
            resourceTimer = 0f;

            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.Add(data.resourceGain);
            }
        }
    }

    public void Sell()
    {
        if (data == null)
            return;

        if (ResourceManager.Instance != null)
            ResourceManager.Instance.Add(data.sellValue);

        if (buildNode != null)
            buildNode.SetOccupied(false);

        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioManager.Instance.sellTower);

        Destroy(gameObject);
    }

    Enemy FindTarget()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>();

        Enemy bestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

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
        if (data.projectilePrefab == null)
            return;

        GameObject projectileObj = Instantiate(
            data.projectilePrefab,
            transform.position + Vector3.up,
            Quaternion.identity
        );

        Projectile projectile = projectileObj.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Init(target, data);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShoot();
        }
    }

    void OnMouseOver()
    {
        if (Mouse.current != null &&
            Mouse.current.rightButton.wasPressedThisFrame)
        {
            Sell();
        }
    }
}