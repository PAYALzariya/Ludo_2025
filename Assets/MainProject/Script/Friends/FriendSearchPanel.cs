using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FriendSearchPanel : MonoBehaviour
{
    public FriendSearchPanel_Item friendSearchPanel_Item;
    public TMP_InputField  search_inputField;
    public void OnSearchButtonClick()

    {
        Debug.Log("OnSearchButtonClick " + search_inputField.text);
        if (!string.IsNullOrEmpty(search_inputField.text))
        {


            var data = new Dictionary<string, object> { { "userId", search_inputField.text } };
            LudoSocketManager.Instance.EmitEvent_With_responseEventHanbleBothCallBack<friendsearchuserResponse>(emitEvent: "friend_search_user", responseEvent: "friend_search_result", payloadData: data
            , onAckResponse: (response) =>
            {
                Debug.Log("Emit:: OnSearchButtonClick::" + response.success);
                if (response != null && response.success)
                {

                    Debug.Log($" onAckResponse received: {response} items");




                }
                else
                {
                    Debug.Log("Failed to ");
                    FailedEmitResponse failedApiResponse = JsonUtility.FromJson<FailedEmitResponse>(response.ToString());
                    DataManager.instance.spriteMaganer.DisplayWarningPanel(failedApiResponse.error);
                }
            },
       onPushResponse: (response) =>
       {
           if (response != null && response.success)
           {

               /*
                          var user = JsonUtility.FromJson<UserModel>(JsonUtility.ToJson(response.data));
                          Debug.Log($" onPushResponse received: {user.id} items");*/
               OnSuccessFunction(response);

           }
           else
           {
               FailedEmitResponse failedApiResponse = JsonUtility.FromJson<FailedEmitResponse>(response.ToString());
               DataManager.instance.spriteMaganer.DisplayWarningPanel(failedApiResponse.error);
               Debug.LogWarning("Failed to  ");
           }
       }
    );

        }
        else
        {
            DataManager.instance.spriteMaganer.DisplayWarningPanel("Search by your friend's ID ");
        }
    }
  internal  async void OnSuccessFunction(friendsearchuserResponse user)
    {

      await  user.data.LoadAllAssets();
        print("User Found: " + user.data.username);
        friendSearchPanel_Item.name_txt.text = user.data.username;
        friendSearchPanel_Item.userid_text.text = "ID: " + user.data.uniqueId;
        friendSearchPanel_Item.proile_img.sprite = user.data.profilePictureAsset.SpriteAssset;
        friendSearchPanel_Item.frame_img.sprite = user.data.frameData.frameSprite;
        friendSearchPanel_Item.level_img.sprite = user.data.levelData.levelSprite;
        friendSearchPanel_Item.country_img.sprite = user.data.countryData.flagSprite;
        friendSearchPanel_Item.country_txt.text = user.data.countryData.countryname;
         friendSearchPanel_Item.gameObject.SetActive(true);

    }
 
 

      
 

    internal void OnFailureFunction(string message)
    {
        print("User Not Found: " + message);
        friendSearchPanel_Item.gameObject.SetActive(false);
    }
}
[System.Serializable]
public class UserDataResponse
{
    public bool success;
    public UserData data;
    public long timestamp;
}