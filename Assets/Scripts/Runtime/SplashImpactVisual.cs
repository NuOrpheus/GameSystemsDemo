using UnityEngine;

public class SplashImpactVisual : MonoBehaviour
{
    public float duration = 0.35f;
    public float maxScale = 2f;

    private float timer;
    private SpriteRenderer spriteRenderer;
    private Color startColor;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
    }

    void Update()
    {
        timer += Time.deltaTime;

        float t = timer / duration;

        transform.localScale = Vector3.one * Mathf.Lerp(0.1f, maxScale, t);

        if (spriteRenderer != null)
        {
            Color c = startColor;
            c.a = Mathf.Lerp(startColor.a, 0f, t);
            spriteRenderer.color = c;
        }

        if (timer >= duration)
            Destroy(gameObject);
    }
}