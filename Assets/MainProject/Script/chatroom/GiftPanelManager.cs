
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GiftCategory
{
    public int id;
    public string name;
    //  public string description;
    public bool isActive;
}
public class GiftPanelManager : MonoBehaviour
{
    public static GiftPanelManager Instance;
    public GameObject Inventory_content, SelectPlayer_content, selectedP_Gift_prefab,g_notif_parent;
    public GiftNotificationPopup g_notificationPopup;
    public List<Chatroompopup_inventoryType> inventorylist = new List<Chatroompopup_inventoryType>();
    public ScrollRect scrollRect;
    public TMP_Dropdown quantityDD;
    public string selectedGiftID;
    public Image giftImage, senderProfile;
    public string allReciversName,giftpriceType,giftPrice;
      public Dictionary<string, Chatroompopup_inventoryItems> GiftDataList = new Dictionary<string, Chatroompopup_inventoryItems>();  
    // public List<string> ReceiverIds = new List<string>();
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        GiftsGetCategories(DataManager.instance.userId);

        foreach (Transform item in SelectPlayer_content.transform)
            Destroy(item.gameObject);
        Debug.Log("<color=red> ChatManager.instance.ChatParticipantDataList cout </color>" + ChatManager.instance.ChatParticipantDataList.Count);
        if (ChatManager.instance.CurrentRoom.data.host.isOnline == true)
        {
            GameObject spg = Instantiate(selectedP_Gift_prefab, SelectPlayer_content.transform, false);
            //  spg.GetComponent<Toggle>().group = SelectPlayer_content.GetComponent<ToggleGroup>();

            // kvp.Key is the dictionary key (string ID)
            // kvp.Value is the ChatParticipantData object
            spg.GetComponent<PlayerSendGift>().reciverID = ChatManager.instance.CurrentRoom.data.host.id;
            spg.GetComponent<PlayerSendGift>().myProfile.sprite = ChatManager.instance.CurrentRoom.data.host.profilePictureAsset.SpriteAssset;

        }
        foreach (var kvp in ChatManager.instance.ChatParticipantDataList)
        {
            GameObject spg = Instantiate(selectedP_Gift_prefab, SelectPlayer_content.transform, false);
            //  spg.GetComponent<Toggle>().group = SelectPlayer_content.GetComponent<ToggleGroup>();

            // kvp.Key is the dictionary key (string ID)
            // kvp.Value is the ChatParticipantData object

            spg.GetComponent<PlayerSendGift>().reciverID = kvp.Value.id;
            spg.GetComponent<PlayerSendGift>().PlayerProfile = kvp.Value.profilePicture;
            spg.GetComponent<PlayerSendGift>().myProfile.sprite = kvp.Value.profilePictureAsset.SpriteAssset;
        }


    }

    public void popUPGiftNotification(string sendername, string recivername, string quntity, Sprite profile_s, Sprite gift_s)
    {
        Debug.Log("<color=red>Call popupgift ...</color>");

        GiftNotificationPopup not = Instantiate(g_notificationPopup, g_notif_parent.transform, false);
        not.senderName.text = sendername;
        not.recivername.text = recivername;
        not.quntityText.text = quntity + "X";
        not.profileimage.sprite = profile_s;
        not.giftimage.sprite = gift_s;

        // Reset initial transform (start position and transparency)
        RectTransform rt = not.GetComponent<RectTransform>();
        CanvasGroup cg = not.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = not.gameObject.AddComponent<CanvasGroup>();

        Vector2 startPos = rt.anchoredPosition;
        cg.alpha = 0;

        // DOTween Animation Sequence
        DG.Tweening.Sequence seq = DOTween.Sequence();

        // Appear: fade in + small pop from right to original position
        rt.anchoredPosition = startPos + new Vector2(200f, 0); // start slightly right
        seq.Append(rt.DOAnchorPosX(startPos.x, 0.6f).SetEase(DG.Tweening.Ease.OutBack));
        seq.Join(cg.DOFade(1, 0.5f));

        // Stay visible for 3 seconds
        seq.AppendInterval(3f);

        // Exit: fade out + move to left side
        seq.Append(cg.DOFade(0, 0.5f));
        seq.Join(rt.DOAnchorPosX(startPos.x - 400f, 0.5f).SetEase(DG.Tweening.Ease.InSine));

        seq.OnComplete(() =>
        {
            Destroy(not.gameObject);
        });
        this.gameObject.SetActive(false);
    }


internal async void GiftsGetCategories(string userid)
    {
       
        string requestJson = "";


        try
        {
            Debug.Log("Sending request to PopularChatListRequest...");

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.GiftsGetCategories,
                httpMethod: "GET",
                requestData: requestJson,
                addAuthHeader: true
            );


            Debug.Log("SUCCESS! GiftsGetCategories::" + response.Text);
            if (response.IsSuccess)
            {
                GiftCategory[] giftCategoryLists = JsonHelper.FromJson<GiftCategory>(response.Text);
                print(giftCategoryLists.Length);

                /*  foreach (Transform item in Inventory_content.transform)
                      Destroy(item.gameObject);
  */
                for (int i = 0; i < inventorylist.Count; i++)
                {
                    inventorylist[i].GetComponent<Chatroompopup_inventoryType>().MygiftCategory = giftCategoryLists[i];
                    inventorylist[i].mynametext.text = inventorylist[i].GetComponent<Chatroompopup_inventoryType>().MygiftCategory.name;
                    inventorylist[i].mynametext.colorGradientPreset = inventorylist[i].defaultinventorycolor;
                    inventorylist[i].InventoryTypeItemsPranet.gameObject.SetActive(false);
                    
                   /* if (!string.IsNullOrEmpty(giftCategoryLists[0].name))
                     {
                         inventorylist[i].GiftsGetCategorieByID(giftCategoryLists[i].id.ToString());
                     }*/
                }
                inventorylist[0].InventoryTypeItemsPranet.gameObject.SetActive(true);
                 if (giftCategoryLists.Length != 0 && !string.IsNullOrEmpty(giftCategoryLists[0].name))
                  {
                      inventorylist[0].GiftsGetCategorieByID(giftCategoryLists[0].id.ToString());
                      inventorylist[0].InventoryTypeItemsPranet.gameObject.SetActive(true);
                  }
                
               

            }
        }

        catch (WebServiceException e)
        {
            // This catches network failures (no internet, timeout, etc.).
            Debug.LogError($"REQUEST FAILED ({e.StatusCode}): {e.ErrorMessage}" + DataManager.instance.AccessToken);
            // Show a generic "Could not connect" error popup.
        }
        finally
        {
            // This block always runs, whether the request succeeded or failed.
            // It's the perfect place to re-enable the UI.
            //createRoomButton.interactable = true;
        }
    }

    public void OnHideContentOFGifts()
    {
        for (int i = 0; i < inventorylist.Count; i++)
        {
            inventorylist[i].InventoryTypeItemsPranet.gameObject.SetActive(false);
        }
    }
    public void OnclickALL()
    {
        foreach (Transform child in SelectPlayer_content.transform)
        {
            var toggle = child.GetComponent<Toggle>();
            //  var playerGift = child.GetComponent<PlayerSendGift>();
            toggle.isOn = true;

        }
    }

    public void OnSendGiftButtonClick()
    {
        Debug.Log("OnSendGiftButtonClick");
        // Collect all selected receiver IDs from toggles
        List<string> receiverIds = new List<string>();

        foreach (Transform child in SelectPlayer_content.transform)
        {
            var toggle = child.GetComponent<Toggle>();
            var playerGift = child.GetComponent<PlayerSendGift>();

            if (toggle != null && toggle.isOn && playerGift != null)
            {
                receiverIds.Add(playerGift.reciverID);
            }
        }

        // Ensure no duplicates
        receiverIds = receiverIds.Distinct().ToList();

        // Gift details
        string giftId = selectedGiftID;

        int selectedIndex = quantityDD.value; //  gives you the selected number
        string quantity = quantityDD.options[selectedIndex].text;
        //string quantity = "1";

        string roomId = ChatManager.instance.CurrentRoom.data.room.id.ToString();
        Debug.Log(receiverIds + "   g  " + giftId + "  q         " + quantity + "      r   " + roomId);
        SendRequestToSendGift(receiverIds, giftId, quantity, roomId);
    }
    //--------Sockt Functions-----------------
    void SendRequestToSendGift(List<string> receiverIds, string giftId, string quantity, string roomId)
    {
        Debug.Log("==== SendRequestToSendGift START ====");

        if (receiverIds == null || receiverIds.Count == 0)
        {
            Debug.LogError("❌ receiverIds is null or empty!");
            return;
        }

        // Log all receiver IDs
        Debug.Log($"Receiver Count: {receiverIds.Count}");
        for (int i = 0; i < receiverIds.Count; i++)
        {
            Debug.Log($"Receiver[{i}] ID: {receiverIds[i]}");
        }
        if (giftpriceType == "coin")
        {
            if (DataManager.instance.coin >= int.Parse(giftPrice))
            {
                DataManager.instance.coin -= int.Parse(giftPrice);
                HomePanel.instance.cointxt.text = DataManager.instance.coin.ToString();
                HomePanel.instance.dimondtxt.text = DataManager.instance.dimond.ToString();
                PlayerPrefs.SetInt("Coin", DataManager.instance.coin);
            }
            else
            {
                DataManager.instance.spriteMaganer.DisplayWarningPanel("You don't have coin");
                return;
            }
        }
        else
        {
            if (DataManager.instance.dimond >= int.Parse(giftPrice))
            {
                DataManager.instance.dimond -= int.Parse(giftPrice);
                HomePanel.instance.cointxt.text = DataManager.instance.coin.ToString();
                HomePanel.instance.dimondtxt.text = DataManager.instance.dimond.ToString();

                PlayerPrefs.SetInt("Dimonad", DataManager.instance.dimond);
            }
            else
            {
                DataManager.instance.spriteMaganer.DisplayWarningPanel("You don't have Dimond");
                return;
            }
        }
Debug.Log($"GiftId: {giftId}, Quantity: {quantity}, RoomId: {roomId}");

        // Prepare payload
        var data = new Dictionary<string, object>
    {
        { "receiverIds", receiverIds },
        { "giftId", giftId },
        { "quantity", quantity },
        { "roomId", roomId }
    };

        // Serialize for visibility in logs
        string jsonPayload = JsonUtility.ToJson(new SerializableSendGiftData(receiverIds, giftId, quantity, roomId));
        Debug.Log($"📤 Sending Payload JSON: {jsonPayload}");

        // Emit event
        LudoSocketManager.Instance.EmitEvent_With_responseEventHanbleBothCallBack<SendGiftResponse>(
            emitEvent: "send_gift",
            responseEvent: "gift_sent",
            payloadData: data,
            onAckResponse: (response) =>
            {
                if (response != null && response.success)
                {
                  //  DataManager.instance.coin-= response.data.
                  HomePanel.instance.  cointxt.text = DataManager.instance.coin.ToString();
                    HomePanel.instance.dimondtxt.text = DataManager.instance.dimond.ToString();
                    Debug.Log($"✅ onAckResponse received: RoomId={response.data.roomId}, GiftId={response.data.giftId}, ReceiverCount={receiverIds.Count}");
                }
                else
                {
                    Debug.LogWarning("⚠️ onAckResponse failed or null response");
                }
            },
            onPushResponse: (response) =>
            {
                if (response != null && response.success)
                {
                    Debug.Log($"📩 onPushResponse received: RoomId={response.data.roomId}, GiftId={response.data.giftId}");
                }
                else
                {
                    Debug.LogWarning("⚠️ onPushResponse failed or null response");
                }
            }
        );

        Debug.Log("==== SendRequestToSendGift END ====");
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    internal static List<T> FromJsonArray<T>(object source)
    {
        throw new NotImplementedException();
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
[System.Serializable]
public class GiftsGetCategories
{
    public bool success;

}
[System.Serializable]
public class SendGiftResponse
{
    public bool success;
    public long timestamp;
    public GiftTransaction data;
}

[System.Serializable]
public class GiftTransaction
{
    public string senderId;     // who sent
    public string receiverId;   // who received
    public string giftId;       // "1"
    public int quantity;        // 1
    public string roomId;       // "1"
    public int id;              // transaction ID
    public string sentAt;       // "2025-09-25T07:00:23.315Z"
}
[System.Serializable]
public class GiftDataReciveRoot
{
    public bool success;
    public GiftDataRecive data;
    public long timestamp;
}

[System.Serializable]
public class GiftDataRecive
{
    public string roomId;
    public string senderId;
    public string[] receiverIds;
    public string giftId;
    public string quantity;
    public GiftEntry[] gifts;
    public GiftSender sender;
    public GiftRecive gift;
    public int totalSent;
    public string action;
}

[System.Serializable]
public class GiftEntry
{
    public string senderId;
    public string receiverId;
    public string giftId;
    public int quantity;
    public string roomId;
    public int id;
    public string sentAt;
    public string senderName;
    public int senderLevel;
    public string giftName;
    public int giftCategoryId;
    public string giftCategoryName;
}

[System.Serializable]
public class GiftSender
{
    public string id;
    public string username;
    public int level;
}

[System.Serializable]
public class GiftRecive
{
    public int id;
    public string name;
    public int categoryId;
    public string categoryName;
}
// Helper class for debugging JSON payload easily
[System.Serializable]
public class SerializableSendGiftData
{
    public List<string> receiverIds;
    public string giftId;
    public string quantity;
    public string roomId;

    public SerializableSendGiftData(List<string> receiverIds, string giftId, string quantity, string roomId)
    {
        this.receiverIds = receiverIds;
        this.giftId = giftId;
        this.quantity = quantity;
        this.roomId = roomId;
    }
}