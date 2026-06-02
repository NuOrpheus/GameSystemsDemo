using TMPro;
using UnityEngine;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    private TextMeshPro text;

    [Header("Movement")]
    public float moveUpAmount = 1f;
    public float sideDriftAmount = 0.3f;
    public float duration = 0.8f;

    [Header("Scale")]
    public float startScale = 0.6f;
    public float popScale = 1.25f;

    [Header("Rotation")]
    public float randomRotation = 12f;

    public enum FloatingTextType
    {
        Normal,
        Poison,
        Burn,
        Resource,
        Heal,
        Critical
    }

    void Awake()
    {
        text = GetComponent<TextMeshPro>();

        if (text == null)
            Debug.LogError("FloatingText needs a TextMeshPro component.");
    }

    public void Setup(string value, FloatingTextType type)
    {
        if (text == null) return;

        text.text = value;

        switch (type)
        {
            case FloatingTextType.Normal:
                text.color = Color.white;
                break;

            case FloatingTextType.Poison:
                text.color = Color.green;
                break;

            case FloatingTextType.Burn:
                text.color = new Color(1f, 0.5f, 0f);
                break;

            case FloatingTextType.Resource:
                text.color = Color.yellow;
                break;

            case FloatingTextType.Heal:
                text.color = Color.cyan;
                break;

            case FloatingTextType.Critical:
                text.color = Color.red;
                text.fontSize = 7;
                break;
        }
        
    text.alpha = 1f;

        text.fontSize = 5;
        text.alignment = TextAlignmentOptions.Center;
        text.sortingOrder = 999;

        float driftX = Random.Range(-sideDriftAmount, sideDriftAmount);
        float rotationZ = Random.Range(-randomRotation, randomRotation);

        transform.localScale = Vector3.one * startScale;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        Vector3 endPos = transform.position + new Vector3(
            driftX,
            moveUpAmount,
            0f
        );

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(popScale, 0.15f).SetEase(Ease.OutBack));
        seq.Append(transform.DOScale(1f, 0.1f));

        seq.Join(transform.DOMove(endPos, duration).SetEase(Ease.OutCubic));
        seq.Join(text.DOFade(0f, duration).SetDelay(0.2f));

        seq.OnComplete(() => Destroy(gameObject));
    }
}