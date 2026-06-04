using UnityEngine;

[CreateAssetMenu(menuName = "TD/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Info")]
    public string displayName;
    public GameObject prefab;
    public GameObject Prefab => prefab;

    [Header("Stats")]
    public int maxHealth = 50;
    public float moveSpeed = 2f;

    [Header("Rewards")]
    public int reward = 10;
    public int livesDamage = 1;
}