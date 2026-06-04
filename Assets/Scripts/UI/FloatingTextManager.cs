using UnityEngine;
using static FloatingText;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    [SerializeField] FloatingText prefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Spawn(
        string value,
        Vector3 position,
        FloatingTextType type = FloatingTextType.Normal)
    {
        if (prefab == null)
            return;

        FloatingText floatingText = Instantiate(
            prefab,
            position,
            Quaternion.identity
        );

        floatingText.Setup(value, type);
    }

    public void SpawnDamage(int amount, Vector3 position)
    {
        Spawn(
            amount.ToString(),
            position,
            FloatingTextType.Normal
        );
    }

    public void SpawnPoison(int amount, Vector3 position)
    {
        Spawn(
            amount.ToString(),
            position,
            FloatingTextType.Poison
        );
    }

    public void SpawnBurn(int amount, Vector3 position)
    {
        Spawn(
            amount.ToString(),
            position,
            FloatingTextType.Burn
        );
    }

    public void SpawnResource(int amount, Vector3 position)
    {
        Spawn(
            "+" + amount,
            position,
            FloatingTextType.Resource
        );
    }

    public void SpawnHeal(int amount, Vector3 position)
    {
        Spawn(
            "+" + amount,
            position,
            FloatingTextType.Heal
        );
    }

    public void SpawnCritical(int amount, Vector3 position)
    {
        Spawn(
            amount.ToString(),
            position,
            FloatingTextType.Critical
        );
    }
}