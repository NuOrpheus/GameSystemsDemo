using DG.Tweening;
using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI waveText;

    [Header("Animation")]
    [SerializeField] float fadeDuration = 0.4f;
    [SerializeField] float showDuration = 1.2f;
    [SerializeField] float punchScale = 0.25f;

    private Sequence currentSequence;

    void Awake()
    {
        HideInstant();
    }

    public void ShowMessage(string message)
    {
        if (canvasGroup == null || waveText == null)
            return;

        waveText.text = message;

        currentSequence?.Kill();

        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.8f;

        currentSequence = DOTween.Sequence();

        currentSequence
            .Append(canvasGroup.DOFade(1f, fadeDuration))
            .Join(transform.DOScale(1f, fadeDuration).SetEase(Ease.OutBack))
            .Append(transform.DOPunchScale(Vector3.one * punchScale, 0.3f))
            .AppendInterval(showDuration)
            .Append(canvasGroup.DOFade(0f, fadeDuration))
            .Join(transform.DOScale(0.8f, fadeDuration).SetEase(Ease.InBack));
    }

    void HideInstant()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        transform.localScale = Vector3.one;
    }

    void OnDestroy()
    {
        currentSequence?.Kill();
    }
}