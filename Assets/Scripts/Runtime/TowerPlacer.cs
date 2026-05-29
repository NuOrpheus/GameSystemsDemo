using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    public GameObject towerPrefab;
    public Camera cam;
    public LayerMask buildableLayer;

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceTower();
        }
    }

    void TryPlaceTower()
    {
        if (towerPrefab == null)
        {
            Debug.LogError("TowerPlacer2D: No towerPrefab assigned.");
            return;
        }

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition = new Vector2(mouseWorld.x, mouseWorld.y);

        RaycastHit2D hit = Physics2D.Raycast(
            mousePosition,
            Vector2.zero,
            0f,
            buildableLayer
        );

        if (hit.collider == null)
            return;

        BuildNode node = hit.collider.GetComponent<BuildNode>();

        if (node == null)
            return;

        if (node.occupied)
            return;

        Instantiate(towerPrefab, node.transform.position, Quaternion.identity);
        node.SetOccupied(true);
    }
}