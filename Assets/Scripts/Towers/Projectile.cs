using UnityEngine;

public class Projectile : MonoBehaviour
{
    private const float HitDistance = 0.1f;

    private Enemy target;
    private Tower tower;
    private TowerData data;

    public void Init(Enemy targetEnemy, Tower sourceTower)
    {
        target = targetEnemy;
        tower = sourceTower;
        data = sourceTower != null ? sourceTower.data : null;
    }

    void Update()
    {
        if (!IsValid())
        {
            Destroy(gameObject);
            return;
        }

        MoveTowardsTarget();

        if (HasReachedTarget())
            HitTarget();
    }

    bool IsValid()
    {
        return target != null && tower != null && data != null;
    }

    void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.transform.position,
            data.projectileSpeed * Time.deltaTime
        );
    }

    bool HasReachedTarget()
    {
        return Vector3.Distance(
            transform.position,
            target.transform.position
        ) < HitDistance;
    }

    void HitTarget()
    {
        switch (data.effectType)
        {
            case TowerEffectType.Damage:
                DealDirectDamage();
                break;

            case TowerEffectType.Slow:
                DealDirectDamage();
                target.ApplySlow(
                    tower.GetSlowMultiplier(),
                    tower.GetSlowDuration()
                );
                break;

            case TowerEffectType.Poison:
                DealDirectDamage();
                target.ApplyPoison(
                    tower.GetPoisonDamagePerTick(),
                    tower.GetPoisonDuration(),
                    tower.GetPoisonTickRate()
                );
                break;

            case TowerEffectType.Burn:
                DealDirectDamage();
                target.ApplyBurn(
                    tower.GetBurnDamagePerTick(),
                    tower.GetBurnDuration(),
                    tower.GetBurnTickRate()
                );
                break;

            case TowerEffectType.Stun:
                DealDirectDamage();
                target.ApplyStun(tower.GetStunDuration());
                break;

            case TowerEffectType.Splash:
                ApplySplash();
                break;
        }

        Destroy(gameObject);
    }

    void DealDirectDamage()
    {
        target.TakeDamage(tower.GetDamage());
    }

    void ApplySplash()
    {
        SpawnSplashVisual();
        ShakeCamera();

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            tower.GetSplashRadius()
        );

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy != null)
                enemy.TakeSplashDamage(tower.GetSplashDamage());
        }
    }

    void SpawnSplashVisual()
    {
        if (data.splashImpactPrefab == null)
            return;

        GameObject splash = Instantiate(
            data.splashImpactPrefab,
            transform.position,
            Quaternion.identity
        );

        SplashImpactVisual visual = splash.GetComponent<SplashImpactVisual>();

        if (visual != null)
            visual.maxScale = tower.GetSplashRadius() * 2f;
    }

    void ShakeCamera()
    {
        CameraShake.Instance?.Shake(
            0.15f,
            0.15f,
            20
        );
    }
}