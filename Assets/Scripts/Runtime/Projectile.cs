using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    private int damage;
    private float speed;

    private TowerEffectType effectType;
    private float slowMultiplier;
    private float slowDuration;

    public void Init(Enemy target,
        int damage,
        float speed,
        TowerEffectType effectType,
        float slowMultiplier,
        float slowDuration)
    {
        this.target = target;
        this.damage = damage;
        this.speed = speed;
        this.effectType = effectType;
        this.slowMultiplier = slowMultiplier;
        this.slowDuration = slowDuration;
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
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
        {
            HitTarget();
        }
    }
    void HitTarget()
    {
        if (effectType == TowerEffectType.Damage)
        {
            target.TakeDamage(damage);
        }
        else if (effectType == TowerEffectType.Slow)
        {
            target.ApplySlow(slowMultiplier, slowDuration);
        }

        Destroy(gameObject);
    }
}