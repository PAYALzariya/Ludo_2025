using TMPro;
using UnityEngine;

public class BCGUIManager : MonoBehaviour
{
    public static BCGUIManager Instance { get; private set; }

    public TMP_Text btcText;
    public TMP_Text bpsText;
    public TMP_Text clickValueText;
    public TMP_Text errorDialogText;
   
    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else Destroy(gameObject);
    }

    public void UpdateCurrency(double amount)
    {
        if (btcText) btcText.text = Format(amount) ;
    }

    public void UpdateBPS(double bps)
    {
        if (bpsText) bpsText.text = Format(bps) + " BITS PER SECOND";
    }

    public void UpdateClickValue(double clickVal)
    {
        if (clickValueText) clickValueText.text = "+" + Format(clickVal);
    }

    string Format(double v)
    {
        if (v >= 1e12) return (v / 1e12).ToString("0.##") + "T";
        if (v >= 1e9) return (v / 1e9).ToString("0.##") + "B";
        if (v >= 1e6) return (v / 1e6).ToString("0.##") + "M";
        if (v >= 1e3) return (v / 1e3).ToString("0.##") + "K";
        return v.ToString("0.##");
    }
}
