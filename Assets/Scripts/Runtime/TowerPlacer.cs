using UnityEngine;
using UnityEngine.InputSystem;

public class TowerPlacer : MonoBehaviour
{
    public GameObject[] towerPrefabs;
    public int selectedTowerIndex = 0;

    public Camera cam;
    public LayerMask nodeLayer;

    GameObject SelectedTowerPrefab => towerPrefabs[selectedTowerIndex];

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectTower(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectTower(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectTower(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectTower(3);
    }

    void SelectTower(int index)
    {
        if (index < 0 || index >= towerPrefabs.Length)
            return;

        selectedTowerIndex = index;
        Debug.Log("Selected tower: " + SelectedTowerPrefab.name);
    }

    public void OnPlaceTower(InputValue value)
    {
        if (!value.isPressed)
            return;

        TryPlaceTower();
    }

    void TryPlaceTower()
    {
        if (SelectedTowerPrefab == null)
            return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(worldPos, nodeLayer);

        if (hit == null)
            return;

        BuildNode node = hit.GetComponent<BuildNode>();

        if (node == null || !node.CanBuild())
            return;

        Tower tower = SelectedTowerPrefab.GetComponent<Tower>();

        if (tower == null || tower.data == null)
        {
            Debug.LogError("Selected tower prefab is missing TowerData.");
            return;
        }

        int cost = tower.data.cost;

        if (!ResourceManager.Instance.CanAfford(cost))
        {
            Debug.Log("Not enough resources.");
            return;
        }

        ResourceManager.Instance.Spend(cost);

        Instantiate(SelectedTowerPrefab, node.transform.position, Quaternion.identity);
        node.SetOccupied(true);
    }
}