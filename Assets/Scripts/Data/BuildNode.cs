using UnityEngine;

public class BuildNode : MonoBehaviour
{
    public int x;
    public int y;

    public bool isBuildable = true;
    public bool occupied = false;

    public void Init(int gridX, int gridY, bool buildable)
    {
        x = gridX;
        y = gridY;
        isBuildable = buildable;
        occupied = false;
    }

    public bool CanBuild()
    {
        return isBuildable && !occupied;
    }

    public void SetOccupied(bool value)
    {
        occupied = value;
    }
}