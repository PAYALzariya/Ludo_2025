using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
    public ShopItem item;
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public TMP_Text ownedText;
    public Button buyButton;

    void Start()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(OnBuyPressed);

        RefreshUI();
    }

    void RefreshUI()
    {
        if (item == null) return;
        if (iconImage) iconImage.sprite = item.icon;
        if (nameText) nameText.text = item.displayName;
        if (priceText) priceText.text ="$" + FormatNumber(item.GetPrice());
        if (ownedText) ownedText.text = $"x{item.owned}";
    }

    public void OnBuyPressed()
    {
        if (item == null) return;
        double price = item.GetPrice();
        if (CurrencyManager.Instance.TrySpend(price))
        {
            item.OnBought();
            // apply effects
            if (item.incomePerSecond != 0)
            {
                CurrencyManager.Instance.AddIncomePerSecond(item.incomePerSecond);
            }
            if (item.clickMultiplier > 1)
            {
                CurrencyManager.Instance.MultiplyClick(item.clickMultiplier);
            }

            RefreshUI();
        }
        else
        {
            // optional: UI feedback for not enough money
            Debug.Log("Not enough bitcoins!");
        }
    }

    string FormatNumber(double v)
    {
        if (v >= 1e12) return (v / 1e12).ToString("0.##") + "T";
        if (v >= 1e9) return (v / 1e9).ToString("0.##") + "B";
        if (v >= 1e6) return (v / 1e6).ToString("0.##") + "M";
        if (v >= 1e3) return (v / 1e3).ToString("0.##") + "K";
        return v.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
