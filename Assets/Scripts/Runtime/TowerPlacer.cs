using UnityEngine;
using UnityEngine.InputSystem;

public class TowerPlacer : MonoBehaviour
{
    public TowerData selectedTower;

    public Camera cam;
    public LayerMask nodeLayer;

    [Header("Placement Feedback")]
    public RangePreview rangePreview;
    public TowerGhostPreview ghostPreview;

    private BuildNode hoveredNode;
    private BuildNode previousHoveredNode;

    private BuildNode[] allNodes;

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Start()
    {
        allNodes = FindObjectsByType<BuildNode>();
        HideBuildNodes();
        HidePreviews();
    }

    void Update()
    {
        UpdatePlacementPreview();
    }

    public void SelectTower(TowerData towerData)
    {
        if (towerData == null)
            return;

        selectedTower = towerData;

        ShowBuildNodes();
        UpdatePlacementPreview();

        Debug.Log("Selected tower: " + selectedTower.displayName);
    }

    public void ClearSelectedTower()
    {
        selectedTower = null;

        HidePreviews();
        HideBuildNodes();
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

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("No ResourceManager in scene.");
            return;
        }

        if (!ResourceManager.Instance.CanAfford(selectedTower.cost))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play(AudioManager.Instance.notEnoughResources);

            Debug.Log("Not enough resources.");
            return;
        }

        ResourceManager.Instance.Spend(selectedTower.cost);

        if (WaveAnalyticsManager.Instance != null)
        {
            WaveAnalyticsManager.Instance?.RecordResourcesSpent(selectedTower.cost);
        }

        GameObject towerObj = Instantiate(
            selectedTower.prefab,
            node.transform.position,
            Quaternion.identity
        );

        Tower placedTower = towerObj.GetComponent<Tower>();

        if (placedTower != null)
        {
            placedTower.data = selectedTower;
            placedTower.buildNode = node;
        }

        node.SetOccupied(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioManager.Instance.placeTower);

        ClearSelectedTower();
    }

    void UpdatePlacementPreview()
    {
        hoveredNode = GetNodeUnderMouse();

        if (previousHoveredNode != null && previousHoveredNode != hoveredNode)
            previousHoveredNode.SetHoverVisual(false);

        if (selectedTower == null)
        {
            HidePreviews();
            HideBuildNodes();
            previousHoveredNode = hoveredNode;
            return;
        }

        ShowBuildNodes();

        if (hoveredNode == null || !hoveredNode.CanBuild())
        {
            HidePreviews();
            previousHoveredNode = hoveredNode;
            return;
        }

        hoveredNode.SetHoverVisual(true);

        if (rangePreview != null)
        {
            rangePreview.Show(
                hoveredNode.transform.position,
                selectedTower.range
            );
        }

        if (ghostPreview != null)
        {
            ghostPreview.Show(
                selectedTower,
                hoveredNode.transform.position,
                true
            );
        }

        previousHoveredNode = hoveredNode;
    }

    void ShowBuildNodes()
    {
        if (allNodes == null || allNodes.Length == 0)
            allNodes = FindObjectsByType<BuildNode>();

        foreach (BuildNode node in allNodes)
        {
            if (node != null)
                node.ShowBuildable();
        }
    }

    void HideBuildNodes()
    {
        if (allNodes == null)
            return;

        foreach (BuildNode node in allNodes)
        {
            if (node != null)
                node.HideVisual();
        }
    }

    void HidePreviews()
    {
        if (rangePreview != null)
            rangePreview.Hide();

        if (ghostPreview != null)
            ghostPreview.Hide();

        if (previousHoveredNode != null)
        {
            previousHoveredNode.SetHoverVisual(false);
            previousHoveredNode = null;
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