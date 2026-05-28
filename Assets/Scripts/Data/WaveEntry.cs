using UnityEngine;

[System.Serializable]
public class WaveEntry
{
    public EnemyData enemy;
    public int count = 5;
    public float spawnInterval = 0.5f;
}

[CreateAssetMenu(menuName = "TD/Wave Data")]
public class WaveData : ScriptableObject
{
    public WaveEntry[] enemies;
}