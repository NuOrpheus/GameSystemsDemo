using UnityEngine;

public enum TowerTargetMode { First, Last, Closest, Strongest }
public enum TowerEffectType { Damage, Slow }

[CreateAssetMenu(menuName = "TD/Tower Data")]
public class TowerData : ScriptableObject
{
    public string displayName;
    public GameObject prefab;
    public GameObject projectilePrefab;

    public int cost = 100;
    public float range = 4f;
    public float fireRate = 1f;
    public int damage = 10;
    public float projectileSpeed = 10f;

    public TowerTargetMode targetMode = TowerTargetMode.First;

    public TowerEffectType effectType = TowerEffectType.Damage;

    [Header("Slow Settings")]
    public float slowMultiplier = 0.5f;
    public float slowDuration = 2f;
}