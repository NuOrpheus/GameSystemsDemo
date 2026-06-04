using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    private TowerData data;

    public void Init(Enemy target, TowerData towerData)
    {
        this.target = target;
        data = towerData;
    }

    void Update()
    {
        if (target == null)
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
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        switch (data.effectType)
        {
            case TowerEffectType.Damage:
                target.TakeDamage(data.damage);
                break;

            case TowerEffectType.Slow:
                target.TakeDamage(data.damage);
                target.ApplySlow(data.slowMultiplier, data.slowDuration);
                break;

            case TowerEffectType.Poison:
                target.TakeDamage(data.damage);
                target.ApplyPoison(
                    data.poisonDamagePerTick,
                    data.poisonDuration,
                    data.poisonTickRate
                );
                break;

            case TowerEffectType.Burn:
                target.TakeDamage(data.damage);
                target.ApplyBurn(
                    data.burnDamagePerTick,
                    data.burnDuration,
                    data.burnTickRate
                );
                break;

            case TowerEffectType.Stun:
                target.TakeDamage(data.damage);
                target.ApplyStun(data.stunDuration);
                break;

            case TowerEffectType.Splash:
                ApplySplash();
                break;
        }

        Destroy(gameObject);
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

            Destroy(splash, 1f);
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(
                0.15f,
                0.15f,
                20
            );
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            data.splashRadius
        );

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy != null)
                enemy.TakeDamage(data.splashDamage);
        }
    }
}