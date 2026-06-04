using UnityEngine;

public class RangePreview : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    [SerializeField] int segments = 64;

    private Vector3[] unitCirclePoints;

    private Vector3 lastPosition;
    private float lastRadius = -1f;
    private bool visible;

    void Awake()
    {
        BuildUnitCircle();
        Hide();
    }

    public void Show(Vector3 position, float radius)
    {
        if (line == null)
            return;

        bool changed =
            !visible ||
            position != lastPosition ||
            !Mathf.Approximately(radius, lastRadius);

        if (!changed)
            return;

        visible = true;
        lastPosition = position;
        lastRadius = radius;

        transform.position = position;

        line.positionCount = unitCirclePoints.Length;

        for (int i = 0; i < unitCirclePoints.Length; i++)
        {
            line.SetPosition(
                i,
                unitCirclePoints[i] * radius
            );
        }

        line.enabled = true;
    }

    public void Hide()
    {
        visible = false;
        lastRadius = -1f;

        if (line != null)
            line.enabled = false;
    }

    void BuildUnitCircle()
    {
        segments = Mathf.Max(8, segments);

        unitCirclePoints = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            unitCirclePoints[i] = new Vector3(
                Mathf.Cos(angle),
                Mathf.Sin(angle),
                0f
            );
        }
    }
}