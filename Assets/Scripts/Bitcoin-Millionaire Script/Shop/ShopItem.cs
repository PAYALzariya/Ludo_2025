using UnityEngine;

[CreateAssetMenu(menuName = "Clicker/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public double basePrice = 10;
    public double priceMultiplier = 1.15; // exponential scale
    public int owned = 0;

    // production
    public double incomePerSecond = 0; // passive income added when purchased
    public double clickMultiplier = 1; // multiplies click value when purchased

    // runtime price calculation
    public double GetPrice()
    {
        // price = basePrice * priceMultiplier^owned
        return basePrice * System.Math.Pow(priceMultiplier, owned);
    }

    // buy operation (no currency logic here)
    public void OnBought()
    {
        owned++;
    }
}
