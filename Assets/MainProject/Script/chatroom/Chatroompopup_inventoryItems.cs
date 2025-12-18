using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GiftItem
{
    public string id;
    public string name;
    public string description;
    public int categoryId;
    public string imageUrl;
    public string priceType;   // e.g. "coin" or "diamond"
    public string priceValue;
    public string soundUrl;
    public bool isActive;
    public string createdAt;
    public string updatedAt;
    public string quantity;
    public LoadedSpriteAsset giftePictureAsset;
    public Currency currencyData;
    public async UniTask LoadAllAssets()
    {
        giftePictureAsset._sourceUrl = imageUrl; // Set the source URL for the asset
        giftePictureAsset.SpriteAssset = await DataManager.instance.LoadSprite(giftePictureAsset);
        if (giftePictureAsset.SpriteAssset == null)
        {
            giftePictureAsset.SpriteAssset = DataManager.instance.spriteMaganer.default_GiftSprite;
        }
       currencyData= DataManager.instance.GetCurrency(priceType);
    }
}

public class Chatroompopup_inventoryItems : MonoBehaviour
{
    public GiftItem giftItem;
    public Image selectionimage;
    public Image productimage;
    public TMP_Text productname;
    public TMP_Text giftprice;
    public Image giftpriceimage;
    public Toggle giftSendBtn;
    public string receiverid;

    void OnEnable()
    {
        
 
   
        giftItem.LoadAllAssets().Forget();
        productname.text=giftItem.name;
        productimage.sprite =giftItem.giftePictureAsset.SpriteAssset;
        giftprice.text = giftItem.priceValue;
        giftpriceimage.sprite =giftItem.currencyData.CurrencySprite;
        giftSendBtn.onValueChanged.AddListener(onClickMyTogglebtn);

        
    }
    public void onClickMyTogglebtn(bool ison)
    {
        SelectedGift();
    }
    void SelectedGift()
    {
        Debug.Log("____________  " + giftItem.priceType);
       
                GiftPanelManager.Instance.selectedGiftID = giftItem.id.ToString();
            
           GiftPanelManager.Instance.giftpriceType=giftItem.priceType;
        GiftPanelManager.Instance.giftPrice=giftItem.priceValue;
    }

}
