using UnityEngine;

public class TowerGhostPreview : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color validColor = new Color(1f, 1f, 1f, 0.5f);
    public Color invalidColor = new Color(1f, 0f, 0f, 0.5f);

    private GameObject currentGhost;

    public void Show(TowerData towerData, Vector3 position, bool valid)
    {
        if (towerData == null || towerData.prefab == null)
        {
            Hide();
            return;
        }

        if (currentGhost == null)
        {
            currentGhost = Instantiate(towerData.prefab);
            DestroyRuntimeScripts(currentGhost);
            SetTransparent(currentGhost);
        }

        currentGhost.transform.position = position;
        currentGhost.SetActive(true);

        SetGhostColor(currentGhost, valid ? validColor : invalidColor);
    }

    public void Hide()
    {
        if (currentGhost != null)
            currentGhost.SetActive(false);
    }

    void DestroyRuntimeScripts(GameObject obj)
    {
        Tower tower = obj.GetComponent<Tower>();
        if (tower != null)
            Destroy(tower);

        Collider2D[] colliders = obj.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
            Destroy(col);
    }

    void SetTransparent(GameObject obj)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer r in renderers)
            r.color = validColor;
    }

    void SetGhostColor(GameObject obj, Color color)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer r in renderers)
            r.color = color;
    }
}