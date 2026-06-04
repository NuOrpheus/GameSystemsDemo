using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public enum FloatingTextType
    {
        Normal,
        Poison,
        Burn,
        Resource,
        Heal,
        Critical
    }

    [Header("Movement")]
    [SerializeField] float moveUpAmount = 1f;
    [SerializeField] float sideDriftAmount = 0.3f;
    [SerializeField] float duration = 0.8f;

    [Header("Scale")]
    [SerializeField] float startScale = 0.6f;
    [SerializeField] float popScale = 1.25f;

    [Header("Rotation")]
    [SerializeField] float randomRotation = 12f;

    [Header("Text")]
    [SerializeField] float normalFontSize = 5f;
    [SerializeField] float criticalFontSize = 7f;
    [SerializeField] int sortingOrder = 999;

    private TextMeshPro text;
    private Sequence sequence;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();

        if (text == null)
            Debug.LogError("FloatingText needs a TextMeshPro component.");
    }

    public void Setup(string value, FloatingTextType type)
    {
        if (text == null)
            return;

        ApplyTextStyle(value, type);
        PlayAnimation();
    }

    void ApplyTextStyle(string value, FloatingTextType type)
    {
        text.text = value;
        text.alpha = 1f;
        text.alignment = TextAlignmentOptions.Center;
        text.sortingOrder = sortingOrder;
        text.fontSize = type == FloatingTextType.Critical
            ? criticalFontSize
            : normalFontSize;

        text.color = GetColor(type);
    }

    Color GetColor(FloatingTextType type)
    {
        return type switch
        {
            FloatingTextType.Poison => Color.green,
            FloatingTextType.Burn => new Color(1f, 0.5f, 0f),
            FloatingTextType.Resource => Color.magenta,
            FloatingTextType.Heal => Color.cyan,
            FloatingTextType.Critical => Color.red,
            _ => Color.white
        };
    }

    void PlayAnimation()
    {
        sequence?.Kill();

        float driftX = Random.Range(-sideDriftAmount, sideDriftAmount);
        float rotationZ = Random.Range(-randomRotation, randomRotation);

        transform.localScale = Vector3.one * startScale;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        Vector3 endPosition = transform.position + new Vector3(
            driftX,
            moveUpAmount,
            0f
        );

        sequence = DOTween.Sequence();

        sequence
            .Append(transform.DOScale(popScale, 0.15f).SetEase(Ease.OutBack))
            .Append(transform.DOScale(1f, 0.1f))
            .Join(transform.DOMove(endPosition, duration).SetEase(Ease.OutCubic))
            .Join(text.DOFade(0f, duration).SetDelay(0.2f))
            .OnComplete(() => Destroy(gameObject));
    }

    void OnDestroy()
    {
        sequence?.Kill();
    }
}