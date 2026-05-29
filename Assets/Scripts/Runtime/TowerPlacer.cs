using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    public GameObject towerPrefab;
    public Camera cam;
    public LayerMask nodeLayer;

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
        Vector2 mousePosition = mouseWorld;

        RaycastHit2D hit = Physics2D.Raycast(
            mousePosition,
            Vector2.zero,
            0f,
            nodeLayer
        );

        if (hit.collider == null)
            return;

        BuildNode node = hit.collider.GetComponent<BuildNode>();

        if (node == null)
            return;

        if (!node.CanBuild())
            return;

        Instantiate(towerPrefab, node.transform.position, Quaternion.identity);

        node.SetOccupied(true);
    }
}