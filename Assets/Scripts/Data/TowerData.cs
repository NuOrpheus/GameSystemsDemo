using UnityEngine;

public enum TowerTargetMode
{
    First,
    Last,
    Closest,
    Strongest
}

public enum TowerEffectType
{
    Damage,
    Slow,
    Poison,
    Splash,
    Stun,
    Burn,
    Resource
}

[CreateAssetMenu(menuName = "TD/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Basic Info")]
    public string displayName;
    public GameObject prefab;
    public GameObject projectilePrefab;

    [Header("Economy")]
    public int cost = 100;
    public int sellValue = 75;

    [Header("Combat")]
    public float range = 4f;
    public float fireRate = 1f;
    public int damage = 10;
    public float projectileSpeed = 10f;

    [Header("Targeting")]
    public TowerTargetMode targetMode = TowerTargetMode.First;

    [Header("Effect")]
    public TowerEffectType effectType = TowerEffectType.Damage;

    [Header("Slow")]
    public float slowMultiplier = 0.5f;
    public float slowDuration = 2f;

    [Header("Poison")]
    public int poisonDamagePerTick = 2;
    public float poisonDuration = 4f;
    public float poisonTickRate = 1f;

    [Header("Splash")]
    public float splashRadius = 1.5f;
    public int splashDamage = 10;

    [Header("Stun")]
    public float stunDuration = 1f;

    [Header("Burn")]
    public int burnDamagePerTick = 5;
    public float burnDuration = 2f;
    public float burnTickRate = 0.5f;

    [Header("Resource Generator")]
    public int resourceGain = 5;
    public float resourceInterval = 5f;
}