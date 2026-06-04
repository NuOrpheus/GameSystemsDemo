using UnityEngine;

public class BuildNode : MonoBehaviour
{
    public int x;
    public int y;

    public bool isBuildable = true;
    public bool occupied = false;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.green;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        HideVisual();
    }

    public void Init(int gridX, int gridY, bool buildable)
    {
        x = gridX;
        y = gridY;
        isBuildable = buildable;
        occupied = false;

        HideVisual();
    }

    public bool CanBuild()
    {
        return isBuildable && !occupied;
    }

    public void SetOccupied(bool value)
    {
        occupied = value;
        HideVisual();
    }

    public void ShowBuildable()
    {
        if (spriteRenderer == null)
            return;

        if (!CanBuild())
        {
            HideVisual();
            return;
        }

        spriteRenderer.enabled = true;
        spriteRenderer.color = normalColor;
    }

    public void SetHoverVisual(bool hovering)
    {
        if (spriteRenderer == null)
            return;

        if (!CanBuild())
        {
            HideVisual();
            return;
        }

        spriteRenderer.enabled = true;
        spriteRenderer.color = hovering ? hoverColor : normalColor;
    }

    public void HideVisual()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }
}