using UnityEngine;

[System.Serializable]
public class WaveSpawn
{
    public EnemyData enemy;
    public float delayAfterSpawn = 0.5f;
}

[CreateAssetMenu(menuName = "TD/Wave Data")]
public class WaveData : ScriptableObject
{
    public WaveSpawn[] spawns;
}