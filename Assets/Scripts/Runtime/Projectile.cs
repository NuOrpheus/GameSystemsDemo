using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    private int damage;
    private float speed;

    public void Init(Enemy targetEnemy, int projectileDamage, float projectileSpeed)
    {
        target = targetEnemy;
        damage = projectileDamage;
        speed = projectileSpeed;
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
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}