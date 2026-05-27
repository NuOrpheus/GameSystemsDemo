using UnityEngine;

[CreateAssetMenu(menuName = "TD/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string displayName;
    public GameObject prefab;

    public int maxHealth = 50;
    public float moveSpeed = 2f;
    public int reward = 10;
    public int livesDamage = 1;
}