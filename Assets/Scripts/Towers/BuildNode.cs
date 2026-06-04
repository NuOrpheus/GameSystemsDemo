using UnityEngine;

public class BuildNode : MonoBehaviour
{
    [Header("State")]
    [SerializeField] bool isBuildable = true;
    [SerializeField] bool occupied;

    [Header("Visuals")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color hoverColor = Color.green;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        HideVisual();
    }

    public void Init(bool buildable)
    {
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
        SetVisual(false);
    }

    public void SetHoverVisual(bool hovering)
    {
        SetVisual(hovering);
    }

    public void HideVisual()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    void SetVisual(bool hovering)
    {
        if (spriteRenderer == null || !CanBuild())
        {
            HideVisual();
            return;
        }

        spriteRenderer.enabled = true;
        spriteRenderer.color = hovering
            ? hoverColor
            : normalColor;
    }
}