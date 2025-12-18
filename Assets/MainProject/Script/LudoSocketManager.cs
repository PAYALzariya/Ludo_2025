using BestHTTP.JSON;
using BestHTTP.JSON.LitJson;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
public class LudoSocketManager : MonoBehaviour
{


    public static LudoSocketManager Instance;

    internal Socket socket;

    // EventName -> List of callbacks
    private Dictionary<string, Action<string>> eventHandlers = new Dictionary<string, Action<string>>();
    private SocketManager Manager;
    private void Awake()
    {
        Application.runInBackground = true;
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void OnEnable()
    {
        ConnectWithAuthObject();
        ConncetWithSocket();
        SetupAllListeners();
    }
    void ConnectWithAuthObject()
    {
   
        var options = new SocketOptions();
//PlatformSupport.Collections.ObjectModel.ObservableDictionary
        // Send token as auth object
        options.AdditionalQueryParams = new PlatformSupport.Collections.ObjectModel.ObservableDictionary<string, string>
        {
            { "auth[token]",DataManager.instance.AccessToken} // Server reads "auth.token"
        };

        Manager = new SocketManager(new Uri(GameConstants.Socket_URL), options);
        socket = Manager.GetSocket();
        Debug.Log(Manager.Uri);
        Manager.Open();

    }





    void ConncetWithSocket()
    {
        socket.On(SocketIOEventTypes.Connect, OnConnect);
        socket.On(SocketIOEventTypes.Disconnect, OndisConnect);
    }
     private void OnConnect(Socket socket, Packet packet, object[] args)
    {
        Debug.Log(">>> SocketManager: WE ARE CONNECTED! <<<");
      

    }
    private void OndisConnect(Socket socket, Packet packet, object[] args)
    {

        Debug.Log(">>> SocketManager: WE ARE DisCONNECTED! <<<");
        HomePanel.instance.chatManager.LeavRoom();
        SendDisConncetRequest();

    }

    #region  Disconncet APi
    internal async void SendDisConncetRequest()
    {
        Debug.Log("SendDisConncetRequest...");



  
        string urlTemplate = GameConstants.GAME_URLS[(int)RequestType.socketdisconnection];
        string apiUrl = GameConstants.FormatUrl(urlTemplate, ("userId", DataManager.instance.userId));
        // Use apiUrl for your API call
        try
        {
            Debug.Log("Sending request to SendDisConncetRequest...+" );

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.socketdisconnection,
                httpMethod: "POST",
                requestData: urlTemplate,
                addAuthHeader: true,
                 customUrl: apiUrl
            );

        

        

            Debug.Log($"GGOT SendDisConncetRequest '{response}'");

            if (response.IsSuccess)
            {

                Debug.Log(" socket-disconnection::" + response.Text);


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
            // This block always runs, whether the request succeeded or failed.
            // It's the perfect place to re-enable the UI.
            //createRoomButton.interactable = true;
        }
    }

#endregion

    #region emit methods


    //---------------- EMIT WITH emitEvent OR responseEvent ----------------
    public void EmitEvent_With_responseEventHandleinSingleCallBack<T>(string emitEvent, string responseEvent = null, object payloadData = null, Action<T> onResponse = null) where T : class
    {
        if (socket == null)
        {
            Debug.LogError("Socket not connected!");
            return;
        }

        Debug.Log($"Emitting event: {emitEvent}");
        //   Debug.Log($"Payload: {payloadData.ToString()}");
        if (payloadData != null)
            Debug.Log($"Payload: {JsonUtility.ToJson(payloadData)}");
        // Ack callback (if server calls ack)
        SocketIOAckCallback ackCallback = (s, p, ackArgs) =>
        {
            if (ackArgs.Length == 0) return;

            try
            {
                string jsonResponse = Json.Encode(ackArgs[0]);
                Debug.Log($"[Ack] Response from {emitEvent}: {jsonResponse}");
                T responseObj = ParseSocketResponse<T>(jsonResponse);
                onResponse?.Invoke(responseObj);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse ack response for {emitEvent}: {ex.Message}");
            }
        };

        // Subscribe to separate response event if provided
        if (!string.IsNullOrEmpty(responseEvent))
        {
            socket.Off(responseEvent); // remove previous listener to avoid duplicates
            socket.On(responseEvent, (s, p, args) =>
            {
                if (args.Length == 0) return;
                try
                {
                    string jsonResponse = Json.Encode(args[0]);
                    Debug.Log($"[Push] Response from {responseEvent}: {jsonResponse}");
                    T responseObj = ParseSocketResponse<T>(jsonResponse);
                    onResponse?.Invoke(responseObj);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to parse push response for {responseEvent}: {ex.Message}");
                }
            });
        }

        // Emit the event
        if (payloadData == null)
            socket.Emit(emitEvent, ackCallback);
        else
            socket.Emit(emitEvent, ackCallback, payloadData);

    }

    public void EmitEvent_With_responseEventHanbleBothCallBack<T>(string emitEvent, string responseEvent = null, object payloadData = null, Action<T> onAckResponse = null, Action<T> onPushResponse = null
) where T : class
    {
        if (socket == null)
        {
            Debug.LogError("Socket not connected!");
            return;
        }

        Debug.Log($"[Emit] Event: {emitEvent}");
        if (payloadData != null)
            Debug.Log($"[Emit] Payload: {JsonUtility.ToJson(payloadData)}");

        // --- ACK callback (fires once immediately after server acknowledges emit)
        SocketIOAckCallback ackCallback = (s, p, ackArgs) =>
    {
        if (ackArgs.Length == 0) return;
        try
        {
            string jsonResponse = Json.Encode(ackArgs[0]);
            Debug.Log($"[Ack] Response from {emitEvent}: {jsonResponse}");
            T responseObj = ParseSocketResponse<T>(jsonResponse);
            onAckResponse?.Invoke(responseObj);
        }
        catch (Exception ex)
        {
              string jsonResponse = Json.Encode(ackArgs[0]);
                       FailedEmitResponse failedResponse = ParseSocketResponse<FailedEmitResponse>(jsonResponse);

                       if(!string.IsNullOrEmpty(failedResponse.error))
                       {
                           DataManager.instance.spriteMaganer.DisplayWarningPanel(failedResponse.error);
                       }
                       else
                       {
                           string concatMessage ="'"+ emitEvent +"'" +ex.Message;
                           DataManager.instance.spriteMaganer.DisplayWarningPanel(concatMessage);
                       }
          //  Debug.LogError($"Failed to parse ack response for {emitEvent}: {ex.Message}");
        }
    };



        // --- Push listener (fires whenever server emits this event)
        if (!string.IsNullOrEmpty(responseEvent) && onPushResponse != null)
        {
            //  socket.Off(responseEvent); // remove previous listener to avoid duplicates
            socket.On(responseEvent, (s, p, args) =>
            {
                if (args.Length == 0) return;
                try
                {
                    string jsonResponse = Json.Encode(args[0]);
                    Debug.Log($"[Push] Response from {responseEvent}: {jsonResponse}");
                    T responseObj = ParseSocketResponse<T>(jsonResponse);
                    onPushResponse?.Invoke(responseObj);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to parse push response for {responseEvent}: {ex.Message}");
                }
            });
        }
        if (payloadData == null)
            socket.Emit(emitEvent, ackCallback);
        else
            socket.Emit(emitEvent, ackCallback, payloadData);

    }
    #endregion
    // ---------------- SETUP ALL LISTENERS ----------------

    private void SetupAllListeners()
    {
        if (socket == null) return;

        /*
                // Debug: Catch-all listener
                socket.On(SocketIOEventTypes.Event, (s, p, args) =>
            {
                string eventName = p.EventName;
                string data = (args != null && args.Length > 0 && args[0] != null)
                    ? args[0].ToString()
                    : "no args";
                Debug.Log($"[SOCKET DEBUG] Event: {eventName} | Data: {data}");
                if( eventHandlers.ContainsKey(eventName))
                {
                    eventHandlers[eventName]?.Invoke(data);
                }
            });*/
        //JoinRoom Listener

        On_joinRoom();

        //gift_received Listener
        On_Gift_Received();
        //gift send for all yours
        On_GIFT_SENT_IN_ROOM();
        //ReceivedGroupText message Listener
        On_Received_GroupTextmessage();
        //userapplyseatsuccess Listener
        On_UserApplySeatSuccess();
        //OnlyforHost_seatapplicationreceived Listener
        On_OnlyforHost_seatapplicationreceived();
        //seatlockmic
        On_Seatlockmicsuccess();
        //seatlocked bordcast
        On_Seatlocked();
        //seatinloced bordcast
        On_SeatUnlocked();
        //left room
        On_LeftRoom();

        On_SeatInvitationReceived();        
        On_user_respond_seat_invitation_success();
        On_useracceptedseatinvitation();
        On_seatapplicationresponsebroadcast();
        On_SeatWaitinglistUpdated();
        // ⚠️ When disconnected
        socket.On(SocketIOEventTypes.Disconnect, (s, p, a) =>
        {
            string reason = (a.Length > 0 ? a[0].ToString() : "No reason");
            Debug.LogWarning("<color=yellow>⚠️ Socket disconnected: </color>" + reason);
        });

        // ❌ Error during communication
      
     


    }
    void 
    On_joinRoom()
    {
        socket.On(GameConstants.BroadcastEvents.joinroom, (s, p, args) =>
        {
            if (args.Length == 0) return;

            string jsonResponse = Json.Encode(args[0]);


            Debug.Log($"<color=green> Response from {"GameConstants.ONEvents.joinroom</color>"}: {jsonResponse}");
            // ON_JoinRoomResponse responseObj = ParseSocketResponse<ON_JoinRoomResponse>(jsonResponse);
            ON_JoinRoomResponse responseObj = JsonConvert.DeserializeObject<ON_JoinRoomResponse>(jsonResponse);

            if (responseObj.success)
            {

                Debug.Log("On_joinRoom Response from:: " + responseObj.data);
                Debug.Log("On_joinRoom Response from:: " + responseObj.data.participants.Length);
                if (responseObj.data.participants.Length > 0)
                {
                    Debug.Log("On_joinRoom Response participants:: " + responseObj.data.participants.Length);
                    HomePanel.instance.chatManager.AddParticipantInList(responseObj.data.participants);
                }
                DataManager.instance.spriteMaganer.DisplayWarningPanel( responseObj.data.message);
                /*foreach (var participant in responseObj.participants)
                        { Debug.Log("  participant id:: " + participant.id);
                            if (!participant.isHost)
                        {

                            HomePanel.instance.chatManager.AddPrarticipant(participant);
                        }
                        else
                        {
                            Debug.Log("  participants Is Host Plz Update Host Data:: " + participant.username);
                        }

                        }*/


                Debug.Log("ON SetupAllListeners Response from:: " + responseObj.data.participants.Length);
            }

        });
    }
    void On_Gift_Received()
    {
        socket.On(GameConstants.ONEvents.gift_received, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                // {"senderId":"39676cea-2dbe-42f5-9e65-9f8aa246c435", "receiverId":"633d8985-babe-4022-b9c4-1c079d5321d8", "giftId":"1", "quantity":1, "roomId":"3", "id":7, "sentAt":"2025-09-26T12:17:08.561Z"}
                // Debug.Log($"[Push] Response from {"GameConstants.ONEvents.joinroom"}: {jsonResponse}");
                Debug.Log("<color=red>Response from gift_received  </color>" + jsonResponse);
                /* GiftReceivedResponse response = ParseSocketResponse<GiftReceivedResponse>(jsonResponse);
                 if (jsonResponse != null )
                 {
                     Debug.Log("<color=green>Response senderId </color>" + response.senderId);
                     Debug.Log("<color=green>Response OwnProfileData </color>" + HomePanel.instance.chatManager.OwnProfileData.id);
                     Debug.Log("<color=green>Response reciverid </color>" + response.receiverId);


                     if (response.senderId != HomePanel.instance.chatManager.OwnProfileData.id)
                     {
                    //   response.data.sender.LoadAllAssets().Forget();
                         HomePanel.instance.chatManager.groupTextChatManager.
                         AddNewGiftMessage(response, false);
                     }
                     else
                     {
                         HomePanel.instance.chatManager.groupTextChatManager.
                         AddNewGiftMessage(response, true);
                     }

                  //   Debug.Log($" onAckResponse received_GroupTextmessage received: {response.data.message} items");
                 }
                 else
                 {
                     Debug.LogWarning("Failed to get received_GroupTextmessage ");
                 }*/

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push response for {"new-user-joined"}: {ex.Message}");
            }
        });

    }
    void On_GIFT_SENT_IN_ROOM()
    {
        socket.On(GameConstants.ONEvents.gift_sent_in_room, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                // {"senderId":"39676cea-2dbe-42f5-9e65-9f8aa246c435", "receiverId":"633d8985-babe-4022-b9c4-1c079d5321d8", "giftId":"1", "quantity":1, "roomId":"3", "id":7, "sentAt":"2025-09-26T12:17:08.561Z"}
                // Debug.Log($"[Push] Response from {"GameConstants.ONEvents.joinroom"}: {jsonResponse}");
                Debug.Log("<color=green>Response from gift_sent_in_room  </color>" + jsonResponse);
                GiftDataReciveRoot response = ParseSocketResponse<GiftDataReciveRoot>(jsonResponse);
                GiftPanelManager.Instance.allReciversName = "";
                if (jsonResponse != null)
                {
                    for (int i = 0; i < response.data.gifts.Length; i++)
                    {

                        Debug.Log($"  received: {response.data.gifts.Length} items");
                        HomePanel.instance.chatManager.groupTextChatManager.AddNewGiftMessage(response.data.gifts[i], true);
                    }
                  
                    if (ChatManager.instance.ChatParticipantDataList.TryGetValue(response.data.senderId, out var senderData))
                    {
                       
                    GiftPanelManager.Instance.popUPGiftNotification(response.data.gifts[0].senderName,
                        GiftPanelManager.Instance.allReciversName, response.data.gifts[0].quantity.ToString(),
                         senderData.profilePictureAsset.SpriteAssset,null
                         
                         );
                    }
                    else if (ChatManager.instance.OwnProfileData.id == response.data.senderId)
                    {
                      
                        GiftPanelManager.Instance.popUPGiftNotification(response.data.gifts[0].senderName,
                            GiftPanelManager.Instance.allReciversName, response.data.gifts[0].quantity.ToString(),
                            ChatManager.instance.OwnProfileData.profilePictureAsset.SpriteAssset,null
                            );
                    }
                    /* if (response.data.roomId != HomePanel.instance.chatManager.OwnProfileData.id)
                     {
                         HomePanel.instance.chatManager.groupTextChatManager.AddNewGiftMessage(response.data, false);
                         //   response.data.sender.LoadAllAssets().Forget();
                     }
                     else
                     {
                         HomePanel.instance.chatManager.groupTextChatManager.
                         AddNewGiftMessage(response.data, true);
                     }*/
                //    HomePanel.instance.chatManager.chatPanel.GiftPopupPanel.gameObject.SetActive(false);
                    //   Debug.Log($" onAckResponse received_GroupTextmessage received: {response.data.message} items");
                }
                else
                {
                    Debug.LogWarning("Failed to get received_GroupTextmessage ");
                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push response for {"new-user-joined"}: {ex.Message}");
            }
        });

    }
    void On_Received_GroupTextmessage()
    {
        socket.On(GameConstants.BroadcastEvents.newmessagerealtime, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                Debug.Log("recived msg::" +jsonResponse);
                GrouptextchatResponse response = ParseSocketResponse<GrouptextchatResponse>(jsonResponse);
                if (jsonResponse != null &&response!=null)
                {
                    if (response.data. sender.id != HomePanel.instance.chatManager.OwnProfileData.id)
                    {
                    Debug.Log("recived msg by other::" +jsonResponse);
                       // response.data.sender.LoadAllAssets().Forget();
                        HomePanel.instance.chatManager.groupTextChatManager.
                        AddNewTextChatMessage(response.data, false);
                    }
                    else
                    {
                        HomePanel.instance.chatManager.groupTextChatManager.
                        AddNewTextChatMessage(response.data, true);
                    }

                    Debug.Log($" onAckResponse received_GroupTextmessage received: {response.data.message} items");
                }
                else
                {
                    Debug.LogWarning("Failed to get received_GroupTextmessage ");
                }
                Debug.Log("<color=green>Response from received_GroupTextmessage  </color>" + jsonResponse);

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push received_GroupTextmessage response for : {ex.Message}");
            }
        });
    }
    void On_UserApplySeatSuccess()
    {
        socket.On(GameConstants.ONEvents.userapplyseatsuccess, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {

                string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(args[0]);
                 JObject myObject = ParseData(jsonResponse);

                    Debug.Log("jsono jbd:::" + myObject);
                user_apply_seatSuccessResponse response =
                ParseSocketResponse<user_apply_seatSuccessResponse>(myObject.ToString());

                if (jsonResponse != null && response.success)
                {
                Debug.Log("<color=green>Response from userapplyseatsuccess </color>" + jsonResponse);
                 //  HomePanel.instance.chatManager.chatPanel.OnUserapplyseatsuccess(response);
                }


            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push userapplyseatsuccess response for : {ex.Message}");
            }
        });
    }

    void On_OnlyforHost_seatapplicationreceived()
    {
        socket.On(GameConstants.ONEvents.OnlyforHost_seatapplicationreceived, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                if(HomePanel.instance.chatManager.CurrentRoom!=null)
                {
                    SeatInvitationReceived responseObj = ParseSocketResponse<SeatInvitationReceived>(jsonResponse);
                
                Debug.Log("On_SeatInvitationReceived roomid ::"+responseObj.data.roomId+"ChatManager.instance.CurrentRoom.data.room.id:"+ChatManager.instance.CurrentRoom.data.room.id);
                
                if (ChatManager.instance.isJoined)
                {
                    if (responseObj.data. roomId.ToString() == ChatManager.instance.CurrentRoom.data.room.id)
                    {
                        InviteManager.Instance.InvitePopUpRecived(responseObj);
                    }
                }
                }
                Debug.Log("<color=green>Response from OnlyforHost_seatapplicationreceived </color>" + jsonResponse);

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push OnlyforHost_seatapplicationreceived response for : {ex.Message}");
            }
        });
    }
    void On_Seatlocked()
    {
        socket.On(GameConstants.BroadcastEvents.seatlocked, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                 
                Debug.Log("recived msg::" +jsonResponse+"::::current room:::"+HomePanel.instance.chatManager.CurrentRoom.data.room.id);
                LockedSeat response = ParseSocketResponse<LockedSeat>(jsonResponse);
                if (jsonResponse != null&&response.success)
                {
                    
                    if (HomePanel.instance.chatManager.CurrentRoom != null)
                    {

                        if (response.data. roomId == HomePanel.instance.chatManager.CurrentRoom.data.room.id)
                        {

                            HomePanel.instance.chatManager.inviteManager.seatsettingOnlyForhostPanel.OnSeatLockedAndUnlock(response,true);
                            Debug.Log("<color=green>Response from seatlocked </color>" + jsonResponse);
                        }
                        else
                        {
                            Debug.Log("youare not is same room");
                        }
                    }
                    else
                    {
                         Debug.LogWarning("you are  not in room");
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to get seatlocked ");
                }
    
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push seatlocked for : {ex.Message}");
            }
        });
    }
       void On_SeatUnlocked()
    {
        socket.On(GameConstants.BroadcastEvents.seatunlocked, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                 
                Debug.Log("recived msg::" +jsonResponse);
                
                if (jsonResponse != null)
                {
                    LockedSeat response = ParseSocketResponse<LockedSeat>(jsonResponse);
                    if (HomePanel.instance.chatManager.CurrentRoom != null)
                    {

                        if (response.data. roomId == HomePanel.instance.chatManager.CurrentRoom.data.room.id)
                        {

                            HomePanel.instance.chatManager.inviteManager.seatsettingOnlyForhostPanel.OnSeatLockedAndUnlock(response,false);
                            Debug.Log("<color=green>Response from seatlocked </color>" + jsonResponse);
                        }
                        else
                        {
                            Debug.Log("youare not is same room");
                        }
                    }
                    else
                    {
                         Debug.LogWarning("you are  not in room");
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to get seatlocked ");
                }
    
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push seatlocked for : {ex.Message}");
            }
        });
    }
    void On_Seatlockmicsuccess()
    {
        socket.On(GameConstants.ONEvents.seatlockmicsuccess, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                
                Debug.Log("<color=green>Response from seatlockmicsuccess </color>" + jsonResponse);
    
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push seatlockmicsuccess for : {ex.Message}");
            }
        });
    }
    void On_LeftRoom()
    {
        Debug.Log("On_LeftRoom");
        socket.On(GameConstants.BroadcastEvents.userleftrealtime, (s, p, args) =>
        {
          //  Debug.Log("<color=green>Response from userleftrealtime </color>" + args[0]);
            if (args.Length == 0) return;
            try
            {string jsonResponse = Json.Encode(args[0]);
                Debug.Log("<color=green>Response from userleftrealtime </color>" + jsonResponse);
                if (HomePanel.instance.chatManager.isJoined)
                {



                    ON_LeavRoomResponse responseObj = ParseSocketResponse<ON_LeavRoomResponse>(jsonResponse);

                    if (responseObj.success)
                    {
                        HomePanel.instance.chatManager.ChatParticipantDataList = new Dictionary<string, ChatParticipantData>();
                        
                        HomePanel.instance.chatManager.voiceChatManager.RemoveSingleWebRtcConncetionAndData(responseObj.data.leftUser.id);
                        HomePanel.instance.chatManager.chatPanel.ResetSeatDataWithUserid(responseObj.data.leftUser.id);
                        if (responseObj.data.participants.Length > 0)
                        {

                            HomePanel.instance.chatManager.UpdateChatParticpintdata(responseObj.data.participants);
                        }
                        DataManager.instance.spriteMaganer.DisplayWarningPanel(responseObj.data.leftUser.username + "Is Left room");

                    }
                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push userleftrealtime for : {ex.Message}");
            }
        });
    }
    void On_SeatInvitationReceived()
    {
        socket.On(GameConstants.ONEvents.seatInvitationReceived, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                SeatInvitationReceived responseObj = ParseSocketResponse<SeatInvitationReceived>(jsonResponse);
                
                Debug.Log("On_SeatInvitationReceived roomid ::"+responseObj.data.roomId+"ChatManager.instance.CurrentRoom.data.room.id:"+ChatManager.instance.CurrentRoom.data.room.id);
                
                if (ChatManager.instance.isJoined)
                {
                    if (responseObj.data. roomId.ToString() == ChatManager.instance.CurrentRoom.data.room.id)
                    {
                        InviteManager.Instance.InvitePopUpRecived(responseObj);
                    }
                }
                Debug.Log($" Response from {"GameConstants.ONEvents.joinroom"}: {jsonResponse}");

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push userleftrealtime for : {ex.Message}");
            }
        });
    }
    void On_user_respond_seat_invitation_success()
    {
        socket.On(GameConstants.ONEvents.user_respond_seat_invitation_success, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                SeatInvitationReceived responseObj = ParseSocketResponse<SeatInvitationReceived>(jsonResponse);
                /*if (ChatManager.instance.isJoined)
                {
                    if (responseObj.roomId.ToString() == ChatManager.instance.CurrentRoom.data.room.id)
                    {
                        InviteManager.Instance.InvitePopUpRecived(responseObj);
                    }
                }*/
                Debug.Log($" Response from {"On_user_respond_seat_invitation_success"}: {jsonResponse}");

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse On_user_respond_seat_invitation_success for : {ex.Message}");
            }
        });
    }
    void On_useracceptedseatinvitation()
    {
        socket.On(GameConstants.BroadcastEvents.useracceptedseatinvitation, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                Debug.Log("<color=red> On_useracceptedseatinvitation      ....</color>" + jsonResponse);
                //SeatInvitationReceived responseObj = ParseSocketResponse<SeatInvitationReceived>(jsonResponse);
                Rootuseracceptedseatinvitation re = ParseSocketResponse<Rootuseracceptedseatinvitation>(jsonResponse);
                Debug.Log(" Response........  "+ re.data.userId);
                //add for seat 

                if (ChatManager.instance.isJoined)
                {
                    if (re.data.roomId.ToString() == ChatManager.instance.CurrentRoom.data.room.id)
                    {
                        HomePanel.instance.chatManager.chatPanel.UpdateSeat(re.data.seatNumber,re.data.userId);
                Debug.Log($" Response from {"responseObj.data.userId  "}: {re.data.userId}");
                    }
                }
                Debug.Log($" Response from {"GameConstants.user_accepted_seat_invitation"}: {jsonResponse}");

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push On_useracceptedseatinvitation for : {ex.Message}");
            }
        });
    }

 void On_seatapplicationresponsebroadcast()
    {
        socket.On(GameConstants.BroadcastEvents.seatapplicationresponsebroadcast, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                Debug.Log("<color=red> seatapplicationresponsebroadcast      ....</color>" + jsonResponse);
                //SeatInvitationReceived responseObj = ParseSocketResponse<SeatInvitationReceived>(jsonResponse);
                Rootuseracceptedseatinvitation re = ParseSocketResponse<Rootuseracceptedseatinvitation>(jsonResponse);
                Debug.Log(" Response........  "+ re.data.userId);
                //add for seat 

                if (ChatManager.instance.isJoined)
                {
                    if (re.data.roomId.ToString() == ChatManager.instance.CurrentRoom.data.room.id)
                    {
                        HomePanel.instance.chatManager.chatPanel.UpdateSeat(re.data.seatNumber,re.data.userId);
                Debug.Log($" Response from {"responseObj.data.userId  "}: {re.data.userId}");
                    }
                }
                Debug.Log($" Response from {"GameConstants.seatapplicationresponsebroadcast"}: {jsonResponse}");

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push seatapplicationresponsebroadcast for : {ex.Message}");
            }
        });
    }

    void On_SeatWaitinglistUpdated()
    {
        socket.On(GameConstants.ONEvents.seatwaitinglistupdated, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                Debug.Log("recived msg::" + jsonResponse);

                SeatwaitinglistUpdatedResponse response = JsonUtility.FromJson<SeatwaitinglistUpdatedResponse>(jsonResponse);
                Debug.Log("<color=green>Response from On_SeatWaitinglistUpdated  </color>" + response.data);
                if (response.success)
                {
                  HomePanel.instance.chatManager.inviteManager.AddParticiantToWailtingListandDisplay(response.data.userId);
                    
                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push On_SeatWaitinglistUpdated response for : {ex.Message}");
            }
        });
    }

    public void EmitWithAck<T>(string eventName, object payloadData, Action<T> onAckResponse) where T : class
    {
        if (socket == null || !socket.IsOpen)
        {
            Debug.LogError($"Socket not connected! Cannot emit event '{eventName}'.");
            
            return;
        }
        SocketIOAckCallback ackCallback = (s, p, ackArgs) =>
               {
                   if (ackArgs.Length == 0) return;
                   try
                   {
                       string jsonResponse = Json.Encode(ackArgs[0]);
                       Debug.Log($"[Ack] Response from {eventName}: {jsonResponse}");
                       
                       T responseObj = ParseSocketResponse<T>(jsonResponse);
                     
                       onAckResponse?.Invoke(responseObj);
                   }
                   catch (Exception ex)
                   {
                       string jsonResponse = Json.Encode(ackArgs[0]);
                       FailedEmitResponse failedResponse = ParseSocketResponse<FailedEmitResponse>(jsonResponse);

                       if(!string.IsNullOrEmpty(failedResponse.error))
                       {
                           DataManager.instance.spriteMaganer.DisplayWarningPanel(failedResponse.error);
                       }
                       else
                       {
                           string concatMessage ="'"+ eventName +"'" +ex.Message;
                           DataManager.instance.spriteMaganer.DisplayWarningPanel(concatMessage);
                       }
                       Debug.Log($"Failed to parse ack response for {eventName}: {ex.Message}");
                   }
               };


        Debug.Log($"[Emit] Firing event '{eventName}' with payload.");
        // Emit the event with the data and the acknowledgment callback
        socket.Emit(eventName, ackCallback, payloadData);
    }


    void OnDestroy()
    {
        if (Manager != null)
        {
            Debug.Log(" Closing socket...");
            Manager.Close();
        }
    }
    // ---------------- COMMON PARSER ----------------
    private T ParseSocketResponse<T>(string json) where T : class
    {
        if (string.IsNullOrEmpty(json)) return null;

        try
        {
            // Automatically wrap root arrays in "data" if needed
            string trimmed = json.Trim();
            if (trimmed.StartsWith("["))
            {
                json = $"{{\"data\":{json}}}";
            }

            return JsonUtility.FromJson<T>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"ParseSocketResponse failed for {typeof(T).Name}: {ex.Message}\nJSON: {json}");
            return null;
        }
    }
        public static JObject ParseData(string jsonString)
    {
        try { return JObject.Parse(jsonString); }
        catch { return null; }
    }


}


