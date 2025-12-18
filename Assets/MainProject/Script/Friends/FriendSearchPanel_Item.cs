using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class FriendSearchPanel_Item : MonoBehaviour
{
    public Image proile_img;
    public Image frame_img;
    public Image level_img;
    public Image country_img;
    public TMP_Text name_txt,userid_text,country_txt;
    public string receiverId;
    public  void OnButonClick()
    {
        //  TestLudoGameSocketManager.instance.Emit_On_SendRequestTO_friend_request_sent(userid);
        var payload = new Dictionary<string, object> { { "receiverId", receiverId } };
        Debug.Log($"[EMIT] Sending friend request to: {receiverId}");

        LudoSocketManager.Instance.EmitEvent_With_responseEventHanbleBothCallBack<BackendResponse>(
             emitEvent: "friend_send_request",
             responseEvent: "friend_request_sent",
             payloadData: payload,
             onPushResponse: (response) =>
             {
                 if (response != null && response.success)
                 {
                     Debug.Log($"Friend list received: {response.data} items");
                     //  OnSuccessFunction(response.data);
                     FriendsManager.instance.FriendList();
                     FriendsManager.instance.GetAllFriendsListRequest();
                 }
                 else
                 {   FailedEmitResponse failedApiResponse = JsonUtility.FromJson<FailedEmitResponse>(response.ToString());
            DataManager.instance.spriteMaganer.DisplayWarningPanel(failedApiResponse.error);
                     Debug.LogWarning("Failed to get friends list");
                 }
             }
         );
    }
    internal void OnSuccessFunction(UserModel user)
    {
        print("User Found: " + user.username);
      
    }
}
