using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class FloatingBTC : MonoBehaviour
{
    public float floatDistance = 150f;
    public float floatDuration = 1f;
    public float fadeDuration = 0.8f;
    public float startScale = 0.8f;
    public float endScale = 1.2f;

    private RectTransform rect;
    public TMP_Text tmpText;
    public Image img;
    public bool isRandom = false;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
       // tmpText = GetComponent<TMP_Text>();
      //  img = GetComponent<Image>();
    }

    void Start()
    {      // Randomized movement direction a bit
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * floatDistance;
       /* if (isRandom)
        {
            endPos = startPos + Vector2.down * floatDistance;
        }
        else {
             endPos = startPos + Vector2.up * floatDistance;
        }*/
        // Randomized movement direction a bit
       // Vector2 startPos = rect.anchoredPosition;
        //Vector2 endPos = startPos + Vector2.up * floatDistance;
     //   endPos.x += Random.Range(-40f, 40f);
        endPos += new Vector2(Random.Range(-100f, 150f), Random.Range(100f, 180f));
        rect.localScale = Vector3.one * startScale;

        // Animation: scale bounce
        rect.DOScale(endScale, 0.25f).SetEase(Ease.OutBack);

        // Animation: move up
        rect.DOAnchorPos(endPos, floatDuration).SetEase(Ease.OutQuad);

        // Fade out by changing alpha color
        if (tmpText != null)
        {
            Color c = tmpText.color;
            tmpText.DOColor(new Color(c.r, c.g, c.b, 0f), fadeDuration)
                   .SetEase(Ease.InSine)
                   .SetDelay(floatDuration - fadeDuration);
        }
        else if (img != null)
        {
            Color c = img.color;
            img.DOColor(new Color(c.r, c.g, c.b, 0f), fadeDuration)
               .SetEase(Ease.InSine)
               .SetDelay(floatDuration - fadeDuration);
        }

        Destroy(gameObject, floatDuration + 0.2f);
    }

    public void SetValue(string text)
    {
        if (tmpText != null)
            tmpText.text = text;
    }
}
