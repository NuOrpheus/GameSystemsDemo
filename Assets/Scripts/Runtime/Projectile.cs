using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    private Tower tower;
    private TowerData data;

    public void Init(Enemy target, Tower sourceTower)
    {
        this.target = target;
        tower = sourceTower;
        data = sourceTower.data;
    }

    void Update()
    {
        if (target == null || tower == null || data == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.transform.position,
            data.projectileSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
            HitTarget();
    }

    void HitTarget()
    {
        switch (data.effectType)
        {
            case TowerEffectType.Damage:
                DealDirectDamage(target);
                break;

            case TowerEffectType.Slow:
                DealDirectDamage(target);
                target.ApplySlow(
                    tower.GetSlowMultiplier(),
                    tower.GetSlowDuration()
                );
                break;

            case TowerEffectType.Poison:
                DealDirectDamage(target);
                target.ApplyPoison(
                    tower.GetPoisonDamagePerTick(),
                    tower.GetPoisonDuration(),
                    tower.GetPoisonTickRate()
                );
                break;

            case TowerEffectType.Burn:
                DealDirectDamage(target);
                target.ApplyBurn(
                    tower.GetBurnDamagePerTick(),
                    tower.GetBurnDuration(),
                    tower.GetBurnTickRate()
                );
                break;

            case TowerEffectType.Stun:
                DealDirectDamage(target);
                target.ApplyStun(tower.GetStunDuration());
                break;

            case TowerEffectType.Splash:
                ApplySplash();
                break;
        }

        Destroy(gameObject);
    }

    void DealDirectDamage(Enemy enemy)
    {
        if (enemy == null)
            return;

        int damage = tower.GetDamage();

        enemy.TakeDamage(damage);

    }

    void ApplySplash()
    {
        if (data.splashImpactPrefab != null)
        {
            GameObject splash = Instantiate(
                data.splashImpactPrefab,
                transform.position,
                Quaternion.identity
            );

            SplashImpactVisual visual = splash.GetComponent<SplashImpactVisual>();

            if (visual != null)
                visual.maxScale = tower.GetSplashRadius() * 2f;
        }

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.15f, 0.15f, 20);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            tower.GetSplashRadius()
        );

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy == null)
                continue;

            int damage = tower.GetSplashDamage();

            enemy.TakeSplashDamage(damage);
        }
    }
}