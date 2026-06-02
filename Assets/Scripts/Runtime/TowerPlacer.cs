using UnityEngine;
using UnityEngine.InputSystem;

public class TowerPlacer : MonoBehaviour
{
    public TowerData[] towers;
    public TowerData selectedTower;

    public Camera cam;
    public LayerMask nodeLayer;
    public RangePreview rangePreview;

    private BuildNode hoveredNode;

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Start()
    {
        if (selectedTower == null && towers.Length > 0)
            SelectTower(0);
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
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectTower(4);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SelectTower(5);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SelectTower(6);
    }

    public void SelectTower(int index)
    {
        if (towers == null)
            return;

        if (index < 0 || index >= towers.Length)
            return;

        if (towers[index] == null)
            return;

        selectedTower = towers[index];

        Debug.Log("Selected tower: " + selectedTower.displayName);
    }

    public void SelectTower(TowerData towerData)
    {
        if (towerData == null)
            return;

        selectedTower = towerData;

        Debug.Log("Selected tower: " + selectedTower.displayName);
    }

    public void OnPlaceTower(InputValue value)
    {
        if (!value.isPressed)
            return;

        TryPlaceTower();
    }

    void TryPlaceTower()
    {
        if (selectedTower == null || selectedTower.prefab == null)
            return;

        BuildNode node = GetNodeUnderMouse();

        if (node == null || !node.CanBuild())
            return;

        int cost = selectedTower.cost;

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("No ResourceManager in scene.");
            return;
        }

        if (!ResourceManager.Instance.CanAfford(cost))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play(AudioManager.Instance.notEnoughResources);

            Debug.Log("Not enough resources.");
            return;
        }

        ResourceManager.Instance.Spend(cost);

        GameObject towerObj = Instantiate(
            selectedTower.prefab,
            node.transform.position,
            Quaternion.identity
        );

        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioManager.Instance.placeTower);

        Tower placedTower = towerObj.GetComponent<Tower>();

        if (placedTower != null)
        {
            placedTower.data = selectedTower;
            placedTower.buildNode = node;
        }

        node.SetOccupied(true);

        if (rangePreview != null)
            rangePreview.Hide();
    }

    void UpdateRangePreview()
    {
        hoveredNode = GetNodeUnderMouse();

        if (hoveredNode == null || selectedTower == null)
        {
            if (rangePreview != null)
                rangePreview.Hide();

            return;
        }

        if (rangePreview != null)
        {
            rangePreview.Show(
                hoveredNode.transform.position,
                selectedTower.range
            );
        }
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

        return hit.GetComponentInParent<BuildNode>();
    }
}