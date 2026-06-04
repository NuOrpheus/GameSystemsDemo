using UnityEngine;

[System.Serializable]
public class WaveSpawn
{
    [SerializeField] EnemyData enemy;
    [SerializeField] float delayAfterSpawn = 0.5f;

    public EnemyData Enemy => enemy;
    public float DelayAfterSpawn => delayAfterSpawn;
}

[CreateAssetMenu(menuName = "TD/Wave Data")]
public class WaveData : ScriptableObject
{
    [SerializeField] WaveSpawn[] spawns;

    public WaveSpawn[] Spawns => spawns;
}