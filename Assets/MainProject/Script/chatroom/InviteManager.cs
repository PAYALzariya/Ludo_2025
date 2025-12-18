using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InviteManager : MonoBehaviour
{
    public static InviteManager Instance;
    public SeatSettings_HostOnly seatsettingOnlyForhostPanel;
    public GameObject sendRequestToHostTojoinRoomPanel;
    public GameObject hostSendRequestToOtherPanel;
    public Transform invitePopUp, invitePopupparent;

   public string seatnumber;
    private void Awake()
    {
        Instance = this;
    }
    public void InvitePopUpRecived(SeatInvitationReceived seatInvitationReceived)
    {
        GameObject gameObject = Instantiate(invitePopUp.gameObject, invitePopupparent, false);
        gameObject.GetComponent<SeatinvitationReceived>().seatInvitationReceived = seatInvitationReceived;

    }
    internal async void InviteByHost(string roomid)
    {
        Debug.Log("InviteByHost...");


        var requestData = new RequestDataByRoomIDOnly
        {
            roomId = roomid


        };

        string requestJson = JsonUtility.ToJson(requestData);
        string urlTemplate = GameConstants.GAME_URLS[(int)RequestType.ChatRoom_InviteByHostByRoomID];
        string apiUrl = GameConstants.FormatUrl(urlTemplate, ("roomId", roomid));
        // Use apiUrl for your API call
        try
        {
            Debug.Log("Sending request to LeaveRoom...+" + requestJson);

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.ChatRoom_InviteByHostByRoomID,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: true,
                 customUrl: apiUrl
            );

            Debug.Log($"GGOT LeaveRoom '{response}'");

            if (response.IsSuccess)
            {




            }
            else
            {

                Debug.LogError($"SERVER ERROR ({response.StatusCode}): {response.Text}");

            }
        }
        catch (WebServiceException e)
        {
            // This catches network failures (no internet, timeout, etc.).
            Debug.LogError($"REQUEST FAILED ({e.StatusCode}): {e.ErrorMessage}");
            // Show a generic "Could not connect" error popup.
        }
        finally
        {

        }
    }
    internal async void SendJoinChatRoomRequestToHost(int uid)
    {
        Debug.Log("InviteByHost...");


        var requestData = new RequestDataByUserIDOnly
        {
            uid = uid


        };

        string requestJson = JsonUtility.ToJson(requestData);
        string urlTemplate = GameConstants.GAME_URLS[(int)RequestType.ChatRoom_SendRequestToHost];
        string apiUrl = GameConstants.FormatUrl(urlTemplate, ("uid", uid));
        // Use apiUrl for your API call
        try
        {
            Debug.Log("Sending request to LeaveRoom...+" + requestJson);

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.ChatRoom_SendRequestToHost,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: true,
                 customUrl: apiUrl
            );

            Debug.Log($"GGOT LeaveRoom '{response}'");

            if (response.IsSuccess)
            {




            }
            else
            {

                Debug.LogError($"SERVER ERROR ({response.StatusCode}): {response.Text}");

            }
        }
        catch (WebServiceException e)
        {
            // This catches network failures (no internet, timeout, etc.).
            Debug.LogError($"REQUEST FAILED ({e.StatusCode}): {e.ErrorMessage}");
            // Show a generic "Could not connect" error popup.
        }
        finally
        {

        }
    }
    //========sockt====
 internal void InviteUSer(string roomid,string seatNo,string targetUserId)
    {
        Debug.Log("InviteUSer..."+ roomid+"::"+ seatNo+"::"+ targetUserId);
        var data = new Dictionary<string, object> { { "roomId", roomid },{"seatNumber",seatNo},{"targetUserId", targetUserId } };
        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.EmitEvents.seatinvite, payloadData: data
              , onAckResponse: (response) =>
         {
             if (response != null && response.success)
             {
                 

                 Debug.Log($" onAckResponse  received: {response.success} items");
             }
             else
             {
                 Debug.LogWarning("Failed to get  ");
             }
         }
      );
    }
    //On SeatSettingsHost_Only_ButtonsClick event



    internal void ResetIniviteManagerData()
    {
        Debug.Log("ResetIniviteManagerData...");
        seatsettingOnlyForhostPanel.gameObject.SetActive(false);
        sendRequestToHostTojoinRoomPanel.gameObject.SetActive(false);
        hostSendRequestToOtherPanel.gameObject.SetActive(false);
        WailtingList.Clear();
        foreach (var item in SendRequestToHost_List)
        {
            Destroy(item.Value.gameObject);
        }
        SendRequestToHost_List.Clear();
    }
    //-------------SendRequestTojoinroomToHostPopup_OthersOnly Buttonclick And other Funtion
  
    public Dictionary<ChatParticipantData, CommonUserProfileItem> SendRequestToHost_List = new Dictionary<ChatParticipantData, CommonUserProfileItem>();
    public Dictionary<string, ChatParticipantData> WailtingList = new Dictionary<string, ChatParticipantData>();
    public Transform SendRequestToHostPatentTransform;

    internal void OpenSendRequestToHostScreen(string seatNo)
    {
        DataManager.instance.spriteMaganer.ShowLoaderWithProgress(" Fetching Wailtinglist ");
        SeatWaltingList(seatNo);
       
    }
    public void OnApplyNowButtonClick()
    {
        if (SendRequestToHost_List.ContainsKey(HomePanel.instance.chatManager.OwnProfileData))
        {
            DataManager.instance.spriteMaganer.DisplayWarningPanel("You have already sent a request to join the room.");
            Debug.Log("You have already sent a request to join the room.");
            return;
        }
        Debug.Log("newitem::"+HomePanel.instance.chatManager.OwnProfileData.username);

        AddParticiantToWailtingListandDisplay(HomePanel.instance.chatManager.OwnProfileData.id);
        ChatManager.instance.Emituserapplyseat(HomePanel.instance.chatManager.chatPanel.Selectdseatitem.seatNo.ToString()); 
 

    }
  internal void AddParticiantToWailtingListandDisplay(string userid)
    {
        Debug.Log("AddParticiantToWailtingListandDisply");
        if (WailtingList == null)
        {
            WailtingList = new Dictionary<string, ChatParticipantData>();
        }
        if (SendRequestToHost_List == null)
        {
                        SendRequestToHost_List = new Dictionary<ChatParticipantData, CommonUserProfileItem>();
        }
        if (ChatManager.instance.ChatParticipantDataList.ContainsKey(userid))
        {
            Debug.Log("AddParticiantToWailtingListandDisply::" + userid);
            if (WailtingList.Count > 0)
            {
                Debug.Log("WailtingList.ContainsKey::" + WailtingList.ContainsKey(userid));
                if (!WailtingList.ContainsKey(userid))
                {
                    CloneParticiantToWailtingList(userid);
                    
                }
            }
            else
            {
                CloneParticiantToWailtingList(userid);
            }
           
        }
        
    }
    void CloneParticiantToWailtingList(string userid)
    {
        Debug.Log("CloneParticiantToWailtingList::" + userid);
        ChatParticipantData chatParticipantData = ChatManager.instance.ChatParticipantDataList[userid];
        Debug.Log("chatParticipantData::" + chatParticipantData.username);
        CommonUserProfileItem newitem = ChatManager.instance.chatPanel.CloneOnlineUserItem(chatParticipantData);
      Debug.Log("newitem::" + newitem);
        HomePanel.instance.chatManager.chatPanel.DisplayOnlineUserItem(SendRequestToHostPatentTransform, newitem);
        Debug.Log("WailtingList.Add::" + chatParticipantData.username);
        WailtingList.Add(userid, chatParticipantData);
        SendRequestToHost_List.Add(chatParticipantData, newitem);
    }


    //========================Host Ivite Funcation==================

    internal void SeatWaltingList(string seatNo)
    {
        var data = new Dictionary<string, object> { { "roomId", HomePanel.instance.chatManager.CurrentRoom.data.room.id }, { "seatNumber", seatNo }};
        LudoSocketManager.Instance.EmitWithAck<GetSeatwaitinglistResponse>(GameConstants.EmitEvents.getseatwaitinglist, payloadData: data
              , onAckResponse: (response) =>
              {
                  if (response != null && response.success)
                  {

                  Debug.Log("getseatwaitinglist onAckResponse..."+response.data.waitingList.applications.Length);
                      if (!HomePanel.instance.chatManager.OwnProfileData.isHost)
                      {
                          Debug.Log("getseatwaitinglist applications..." + response.data.waitingList.applications.Length);
                          foreach (var item in response.data.waitingList.applications)
                            {
                                AddParticiantToWailtingListandDisplay(item.userId.ToString());
                          }
                      }
                      else
                      { 
                      }
                          Debug.Log($" onAckResponse  received: {response.success} items");
                  }
                  else
                  {
                      Debug.LogWarning("Failed to get  ");
                  }
                  DataManager.instance.spriteMaganer.loaderManager.HideLoader();
                 HomePanel.instance.OpenPanel
       (sendRequestToHostTojoinRoomPanel.gameObject);
              }
      );
    }

   
    public  void OnInvite()
    {
      //  InviteUSer(HomePanel.instance.chatManager.CurrentRoom.data.room.id,);

    }


}
