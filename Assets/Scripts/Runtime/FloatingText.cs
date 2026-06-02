using TMPro;
using UnityEngine;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    private TextMeshPro text;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();

        if (text == null)
        {
            Debug.LogError("FloatingText needs a TextMeshPro component.");
        }
    }

    public void Setup(string value)
    {
        if (text == null) return;

        text.text = value;
        text.alpha = 1f;

        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;

        text.fontSize = 5;
        text.alignment = TextAlignmentOptions.Center;
        text.sortingOrder = 999;

        Sequence seq = DOTween.Sequence();

        seq.Join(transform.DOMoveY(transform.position.y + 1f, 0.8f));
        seq.Join(text.DOFade(0f, 0.8f));
        seq.OnComplete(() => Destroy(gameObject));
    }
}