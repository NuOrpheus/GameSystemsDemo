using UnityEngine;
using UnityEngine.InputSystem;

public class TowerPlacer : MonoBehaviour
{
    public GameObject[] towerPrefabs;
    public int selectedTowerIndex = 0;

    public Camera cam;
    public LayerMask nodeLayer;
    public RangePreview rangePreview;

    private BuildNode hoveredNode;

    GameObject SelectedTowerPrefab
    {
        get
        {
            if (towerPrefabs == null || towerPrefabs.Length == 0)
                return null;

            if (selectedTowerIndex < 0 || selectedTowerIndex >= towerPrefabs.Length)
                return null;

            return towerPrefabs[selectedTowerIndex];
        }
    }

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Start()
    {
        SelectTower(selectedTowerIndex);
    }

    void Update()
    {
        HandleTowerSelection();
        UpdateRangePreview();
    }

    void HandleTowerSelection()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectTower(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectTower(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectTower(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectTower(3);
    }

    void UpdateRangePreview()
    {
        hoveredNode = GetNodeUnderMouse();

        GameObject prefab = SelectedTowerPrefab;

        if (hoveredNode == null || prefab == null)
        {
            if (rangePreview != null)
                rangePreview.Hide();

            return;
        }

        Tower tower = prefab.GetComponent<Tower>();

        if (tower == null || tower.data == null)
        {
            if (rangePreview != null)
                rangePreview.Hide();

            return;
        }

        if (rangePreview != null)
        {
            rangePreview.Show(
                hoveredNode.transform.position,
                tower.data.range
            );
        }
    }

    void SelectTower(int index)
    {
        if (towerPrefabs == null)
            return;

        if (index < 0 || index >= towerPrefabs.Length)
            return;

        if (towerPrefabs[index] == null)
            return;

        selectedTowerIndex = index;
        Debug.Log("Selected tower: " + towerPrefabs[index].name);
    }

    public void OnPlaceTower(InputValue value)
    {
        if (!value.isPressed)
            return;

        TryPlaceTower();
    }

    void TryPlaceTower()
    {
        GameObject prefab = SelectedTowerPrefab;

        if (prefab == null)
            return;

        BuildNode node = GetNodeUnderMouse();

        if (node == null || !node.CanBuild())
            return;

        Tower tower = prefab.GetComponent<Tower>();

        if (tower == null || tower.data == null)
        {
            Debug.LogError("Selected tower prefab is missing TowerData.");
            return;
        }

        int cost = tower.data.cost;

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("No ResourceManager in scene.");
            return;
        }

        if (!ResourceManager.Instance.CanAfford(cost))
        {
            Debug.Log("Not enough resources.");
            return;
        }

        ResourceManager.Instance.Spend(cost);

        GameObject towerObj = Instantiate(
            prefab,
            node.transform.position,
            Quaternion.identity
        );

        Tower placedTower = towerObj.GetComponent<Tower>();

        if (placedTower != null)
        {
            placedTower.buildNode = node;
        }

        node.SetOccupied(true);

        if (rangePreview != null)
            rangePreview.Hide();
    }

    BuildNode GetNodeUnderMouse()
    {
        if (Mouse.current == null || cam == null)
            return null;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(worldPos, nodeLayer);

        if (hit == null)
            return null;

        return hit.GetComponent<BuildNode>();
    }
}