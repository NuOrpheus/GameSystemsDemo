using UnityEngine;

public class TowerGhostPreview : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] Color validColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] Color invalidColor = new Color(1f, 0f, 0f, 0.5f);

    private GameObject currentGhost;
    private TowerData currentTowerData;
    private SpriteRenderer[] ghostRenderers;

    public void Show(TowerData towerData, Vector3 position, bool valid)
    {
        if (towerData == null || towerData.prefab == null)
        {
            Hide();
            return;
        }

        if (towerData != currentTowerData)
            CreateGhost(towerData);

        currentGhost.transform.position = position;
        currentGhost.SetActive(true);

        SetGhostColor(valid ? validColor : invalidColor);
    }

    public void Hide()
    {
        if (currentGhost != null)
            currentGhost.SetActive(false);
    }

    void CreateGhost(TowerData towerData)
    {
        ClearGhost();

        currentTowerData = towerData;
        currentGhost = Instantiate(towerData.prefab, transform);
        currentGhost.name = towerData.displayName + " Ghost";

        RemoveRuntimeComponents(currentGhost);

        ghostRenderers = currentGhost.GetComponentsInChildren<SpriteRenderer>();
        SetGhostColor(validColor);
    }

    void ClearGhost()
    {
        if (currentGhost != null)
            Destroy(currentGhost);

        currentGhost = null;
        currentTowerData = null;
        ghostRenderers = null;
    }

    void RemoveRuntimeComponents(GameObject obj)
    {
        Tower tower = obj.GetComponent<Tower>();

        if (tower != null)
            Destroy(tower);

        Collider2D[] colliders = obj.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
            Destroy(collider);
    }

    void SetGhostColor(Color color)
    {
        if (ghostRenderers == null)
            return;

        foreach (SpriteRenderer renderer in ghostRenderers)
        {
            if (renderer != null)
                renderer.color = color;
        }
    }
}