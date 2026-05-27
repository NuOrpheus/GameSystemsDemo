using UnityEngine;

public enum TowerTargetMode { First, Last, Closest, Strongest }

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
}