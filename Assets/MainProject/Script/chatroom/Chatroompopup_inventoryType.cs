using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
public class Chatroompopup_inventoryType : MonoBehaviour
{
    public GiftCategory MygiftCategory;
    public Image sideimage;
    public Image underimage;
    public Image backgroundimage;
    public TMP_Text mynametext;
    public Toggle myToggle;
    public Chatroompopup_inventoryItems InventoryTypeItems;
    public Transform InventoryTypeItemsPranet;
    public TMP_ColorGradient selctedinventorycolor;
    public TMP_ColorGradient defaultinventorycolor;
//    public Transform myPopUpContent;

    public void Start()
    {
        Debug.Log("............MyCat is" + MygiftCategory.name);

        //  myToggle.isOn = true;
        //   underimage.gameObject.SetActive(false);

        //  InventoryTypeItemsPranet.gameObject.SetActive(false);

        // Add runtime listener
        myToggle.onValueChanged.AddListener(OnClickGetMyCategory);
    }
    public void OnClickGetMyCategory(bool isOn)
    {
        GiftsGetCategorieByID(MygiftCategory.id.ToString());
    }
    internal async void GiftsGetCategorieByID(string catid)
    {
        Debug.Log("GiftsGetCategorieByID..." + catid);


        //string requestJson = JsonUtility.ToJson(requestData);
        // / api / gifts ? categoryId = 1
        // string apiUrl = "https://ludobaar.com/api/gifts?categoryId=" + catid;
        string urlTemplate = GameConstants.GAME_URLS[(int)RequestType.GiftsGetCategoriesByID];

        string apiUrl = GameConstants.FormatUrl(urlTemplate, ("categoryId", catid));

        string requestJson = "";
        Debug.Log("<color=green> GiftsGetCategorieByID apiUrl:: </color>" + apiUrl);

        try
        {
            Debug.Log("Sending request to PopularChatListRequest...");

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.GiftsGetCategoriesByID,
                httpMethod: "GET",
                requestData: requestJson,
                addAuthHeader: true,
                customUrl: apiUrl
            );


            if (response.IsSuccess)
            {

                //GiftsGetCategories GiftsGetCategories = JsonUtility.FromJson<GiftsGetCategories>(response.Text);
                Debug.Log("GetCategorie    " + response.Text);
                foreach (Transform item in InventoryTypeItemsPranet.transform)
                    Destroy(item.gameObject);

                GiftItem[] giftCategoryLists = JsonHelper.FromJson<GiftItem>(response.Text);
                if (GiftPanelManager.Instance.GiftDataList != null)
                {
                    
                GiftPanelManager.Instance.GiftDataList = new Dictionary<string, Chatroompopup_inventoryItems>();
                }
                for (int i = 0; i < giftCategoryLists.Length; i++)
                {
                    Chatroompopup_inventoryItems item = Instantiate(InventoryTypeItems);
                    item.giftItem = giftCategoryLists[i];
                    item.transform.SetParent(InventoryTypeItemsPranet, false);
                    item.gameObject.GetComponent<Toggle>().group = InventoryTypeItemsPranet.GetComponent<ToggleGroup>();
                    GiftPanelManager.Instance.GiftDataList.Add(item.giftItem.id ,item);
                    //   item.transform.transform.position = inventory.InventoryTypeItemsPranet.transform.position;
                    //  item.transform.localScale = Vector3.one;
                    // item.GetComponent<Chatroompopup_inventoryItems>().giftItem = giftCategoryLists
                }
                GiftPanelManager.Instance.OnHideContentOFGifts();
              InventoryTypeItemsPranet.gameObject.SetActive(true);
                GiftPanelManager.Instance.scrollRect.content = InventoryTypeItemsPranet.GetComponent<RectTransform>();
                //SendRequestToSendGift("39676cea-2dbe-42f5-9e65-9f8aa246c435", giftCategoryLists[0].id.ToString(),"1", "1");              

            }
            else
            {

                Debug.LogError($"SERVER ERROR ({response.StatusCode}): {response.Text}");

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

}