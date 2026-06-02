using UnityEngine;
using static FloatingText;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;

    public FloatingText prefab;

    void Awake()
    {
        Instance = this;
    }

    public void Spawn(
        string value,
        Vector3 position,
        FloatingTextType type = FloatingTextType.Normal)
    {
        FloatingText text = Instantiate(
            prefab,
            position,
            Quaternion.identity
        );

        text.Setup(value, type);
    }
}