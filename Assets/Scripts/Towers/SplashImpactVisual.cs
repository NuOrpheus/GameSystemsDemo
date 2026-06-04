using UnityEngine;

public class SplashImpactVisual : MonoBehaviour
{
    public float maxScale = 2f;

    [SerializeField] float duration = 0.35f;
    [SerializeField] float startScale = 0.1f;

    private float timer;
    private SpriteRenderer spriteRenderer;
    private Color startColor;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            startColor = spriteRenderer.color;

        transform.localScale = Vector3.one * startScale;
    }

    void Update()
    {
        timer += Time.deltaTime;

        float progress = Mathf.Clamp01(timer / duration);

        transform.localScale = Vector3.one *
            Mathf.Lerp(startScale, maxScale, progress);

        FadeOut(progress);

        if (progress >= 1f)
            Destroy(gameObject);
    }

    void FadeOut(float progress)
    {
        if (spriteRenderer == null)
            return;

        Color color = startColor;
        color.a = Mathf.Lerp(startColor.a, 0f, progress);

        spriteRenderer.color = color;
    }
}