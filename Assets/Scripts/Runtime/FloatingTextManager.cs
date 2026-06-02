using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;

    public FloatingText prefab;

    void Awake()
    {
        Instance = this;
    }

    public void Spawn(string value, Vector3 position)
    {
        Vector3 offset = new Vector3(
            Random.Range(-0.25f, 0.25f),
            Random.Range(0f, 0.25f),
            0f
        );

        FloatingText text = Instantiate(
      prefab,
      position + offset,
      Quaternion.identity
  );

        text.Setup(value);
    }
}