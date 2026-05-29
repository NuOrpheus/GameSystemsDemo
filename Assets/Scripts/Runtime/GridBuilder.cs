using UnityEngine;
using UnityEngine.Tilemaps;

public class GridBuilder : MonoBehaviour
{
    public GameObject nodePrefab;
    public Tilemap pathTilemap;

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
        if (nodePrefab == null || pathTilemap == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = startPosition + new Vector2(x * spacing, y * spacing);

                Vector3Int tileCell = pathTilemap.WorldToCell(worldPos);

                bool isPath = pathTilemap.HasTile(tileCell);
                bool buildable = !isPath;

                GameObject nodeObj = Instantiate(
                    nodePrefab,
                    worldPos,
                    Quaternion.identity,
                    transform
                );

                BuildNode node = nodeObj.GetComponent<BuildNode>();
                node.Init(x, y, buildable);
            }
        }
    }
}