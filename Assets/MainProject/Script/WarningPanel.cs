using DG.Tweening;

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WarningPanel : MonoBehaviour
{
    
    [Header("UI References")]
    [SerializeField] private Image backgroundImage;   
    [SerializeField] private RectTransform popupBox; 
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button closeButton;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float scaleDuration = 0.3f;
    [SerializeField] private float showScale = 1.1f;

    private Color bgColorHidden;
    private Color bgColorVisible;

    private void Awake()
    {
        closeButton.onClick.AddListener(HidePopup);
        gameObject.SetActive(false);

        bgColorVisible = backgroundImage.color;
        bgColorHidden = new Color(bgColorVisible.r, bgColorVisible.g, bgColorVisible.b, 0);
        backgroundImage.color = bgColorHidden;
    }

    public void ShowPopup(string message)
    {
        messageText.text = message;
        gameObject.SetActive(true);

        popupBox.localScale = Vector3.zero;
        backgroundImage.color = bgColorHidden;


        DOTween.Kill(popupBox);
        DOTween.Kill(backgroundImage);


        backgroundImage.DOFade(bgColorVisible.a, fadeDuration);
        popupBox.DOScale(showScale, scaleDuration).SetEase(Ease.OutBack)
                 .OnComplete(() => popupBox.DOScale(1f, 0.1f));
    }

    public void HidePopup()
    {
        DOTween.Kill(popupBox);
        DOTween.Kill(backgroundImage);

        Sequence seq = DOTween.Sequence();
        seq.Append(popupBox.DOScale(0.8f, 0.2f).SetEase(Ease.InBack))
           .Join(backgroundImage.DOFade(0f, 0.2f))
           .OnComplete(() => gameObject.SetActive(false));
    }
}
