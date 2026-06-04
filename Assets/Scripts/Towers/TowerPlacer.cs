using UnityEngine;
using UnityEngine.InputSystem;

public class TowerPlacer : MonoBehaviour
{
    [Header("Selection")]
    public TowerData selectedTower;

    [Header("References")]
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
        RefreshNodeCache();
        ExitPlacementMode();
    }

    void Update()
    {
        HandleCancelInput();
        UpdatePlacementPreview();
    }

    void HandleCancelInput()
    {
        if (selectedTower == null)
            return;

        if (Mouse.current != null &&
            Mouse.current.rightButton.wasPressedThisFrame)
        {
            ClearSelectedTower();
        }

        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClearSelectedTower();
        }
    }

    public void SelectTower(TowerData towerData)
    {
        if (towerData == null)
            return;

        selectedTower = towerData;
        ShowBuildNodes();

        Debug.Log("Selected tower: " + selectedTower.displayName);
    }

    public void ClearSelectedTower()
    {
        selectedTower = null;
        ExitPlacementMode();
    }

    public void OnPlaceTower(InputValue value)
    {
        if (value.isPressed)
            TryPlaceTower();
    }

    void TryPlaceTower()
    {
        if (!CanAttemptPlacement())
            return;

        BuildNode node = GetNodeUnderMouse();

        if (node == null || !node.CanBuild())
            return;

        if (!ResourceManager.Instance.CanAfford(selectedTower.cost))
        {
            AudioManager.Instance?.PlayNotEnoughResources();
            Debug.Log("Not enough resources.");
            return;
        }

        PlaceTower(node);
    }

    bool CanAttemptPlacement()
    {
        if (selectedTower == null || selectedTower.prefab == null)
            return false;

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("No ResourceManager in scene.");
            return false;
        }

        return true;
    }

    void PlaceTower(BuildNode node)
    {
        ResourceManager.Instance.Spend(selectedTower.cost);
        WaveAnalyticsManager.Instance?.RecordResourcesSpent(selectedTower.cost);

        GameObject towerObj = Instantiate(
            selectedTower.prefab,
            node.transform.position,
            Quaternion.identity
        );

        Tower tower = towerObj.GetComponent<Tower>();

        if (tower != null)
        {
            tower.data = selectedTower;
            tower.buildNode = node;
        }

        node.SetOccupied(true);

        AudioManager.Instance?.PlayPlaceTower();

        ClearSelectedTower();
    }

    void UpdatePlacementPreview()
    {
        hoveredNode = GetNodeUnderMouse();

        ClearPreviousHoverIfChanged();

        if (selectedTower == null)
        {
            ExitPlacementMode();
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

        ShowValidPlacementPreview(hoveredNode);
        previousHoveredNode = hoveredNode;
    }

    void ShowValidPlacementPreview(BuildNode node)
    {
        node.SetHoverVisual(true);

        rangePreview?.Show(
            node.transform.position,
            selectedTower.range
        );

        ghostPreview?.Show(
            selectedTower,
            node.transform.position,
            true
        );
    }

    void ClearPreviousHoverIfChanged()
    {
        if (previousHoveredNode != null && previousHoveredNode != hoveredNode)
            previousHoveredNode.SetHoverVisual(false);
    }

    void ExitPlacementMode()
    {
        HidePreviews();
        HideBuildNodes();
    }

    void ShowBuildNodes()
    {
        EnsureNodesCached();

        foreach (BuildNode node in allNodes)
        {
            if (node != null)
                node.ShowBuildable();
        }
    }

    void HideBuildNodes()
    {
        EnsureNodesCached();

        foreach (BuildNode node in allNodes)
        {
            if (node != null)
                node.HideVisual();
        }
    }

    void HidePreviews()
    {
        rangePreview?.Hide();
        ghostPreview?.Hide();

        if (previousHoveredNode != null)
        {
            previousHoveredNode.SetHoverVisual(false);
            previousHoveredNode = null;
        }
    }

    void RefreshNodeCache()
    {
        allNodes = FindObjectsByType<BuildNode>();
    }

    void EnsureNodesCached()
    {
        if (allNodes == null || allNodes.Length == 0)
            RefreshNodeCache();
    }

    BuildNode GetNodeUnderMouse()
    {
        if (Mouse.current == null || cam == null)
            return null;

        Vector2 screenPos = Mouse.current.position.ReadValue();

        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(worldPos, nodeLayer);

        return hit != null
            ? hit.GetComponentInParent<BuildNode>()
            : null;
    }
}