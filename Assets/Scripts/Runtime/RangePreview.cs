using UnityEngine;

public class RangePreview : MonoBehaviour
{
    public LineRenderer line;
    public int segments = 64;

    void Awake()
    {
        Hide();
    }

    public void Show(Vector3 position, float radius)
    {
        transform.position = position;

        line.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 point = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );

            line.SetPosition(i, point);
        }

        line.enabled = true;
    }

    public void Hide()
    {
        line.enabled = false;
    }
}