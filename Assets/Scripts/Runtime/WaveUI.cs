using DG.Tweening;
using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI waveText;

    public float fadeDuration = 0.4f;
    public float showDuration = 1.2f;
    public float scalePunch = 0.25f;

    void Awake()
    {
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one;
    }

    public void ShowMessage(string message)
    {
        waveText.text = message;

        DOTween.Kill(transform);
        DOTween.Kill(canvasGroup);

        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.8f;

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1f, fadeDuration));
        seq.Join(transform.DOScale(1f, fadeDuration).SetEase(Ease.OutBack));
        seq.Append(transform.DOPunchScale(Vector3.one * scalePunch, 0.3f));
        seq.AppendInterval(showDuration);
        seq.Append(canvasGroup.DOFade(0f, fadeDuration));
        seq.Join(transform.DOScale(0.8f, fadeDuration).SetEase(Ease.InBack));
    }
}