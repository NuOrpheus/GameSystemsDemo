using UnityEngine;
using UnityEngine.Tilemaps;

public class GridBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject nodePrefab;
    [SerializeField] Tilemap pathTilemap;

    [Header("Grid Settings")]
    [SerializeField] int width = 10;
    [SerializeField] int height = 10;
    [SerializeField] float spacing = 1f;
    [SerializeField] Vector2 startPosition = Vector2.zero;

    void Start()
    {
        BuildGrid();
    }

    void BuildGrid()
    {
        if (nodePrefab == null || pathTilemap == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CreateNode(x, y);
            }
        }
    }

    void CreateNode(int x, int y)
    {
        Vector3 worldPosition = GetWorldPosition(x, y);
        bool buildable = IsBuildable(worldPosition);

        GameObject nodeObject = Instantiate(
            nodePrefab,
            worldPosition,
            Quaternion.identity,
            transform
        );

        BuildNode node = nodeObject.GetComponent<BuildNode>();

        if (node != null)
            node.Init(buildable);
    }

    Vector3 GetWorldPosition(int x, int y)
    {
        return startPosition + new Vector2(
            x * spacing,
            y * spacing
        );
    }

    bool IsBuildable(Vector3 worldPosition)
    {
        Vector3Int cell = pathTilemap.WorldToCell(worldPosition);
        return !pathTilemap.HasTile(cell);
    }
}