using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    public GameObject nodePrefab;

    public int width = 10;
    public int height = 10;
    public float spacing = 1f;

    public Vector2 startPosition = Vector2.zero;

    void Start()
    {
        BuildGrid();
    }

    void BuildGrid()
    {
        if (nodePrefab == null)
        {
            Debug.LogError("GridBuilder2D: No nodePrefab assigned.");
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = startPosition + new Vector2(x * spacing, y * spacing);
                Instantiate(nodePrefab, position, Quaternion.identity, transform);
            }
        }
    }
}