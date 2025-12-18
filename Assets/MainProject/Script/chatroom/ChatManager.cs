
using Cysharp.Threading.Tasks;
using LitJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;
    public CreateChatRoomPanel createChatRoomPanel;
    public PopularChatRoomListPanel popularChatRoomListPanel;
    public ChatPanel chatPanel;
    public ChatRoomSetting_HostOnly chatRoomSetting_HostOnlyPanel;
    public ChatRoomSetting_ParticipantOnly chatRoomSetting_ParticipantOnly;
    [SerializeField]
    public JoinChatRoomResponse CurrentRoom;
    public ChatRoomSettingResponse HostRoomsettings;
    public VoiceChatManager voiceChatManager;
    public InviteManager inviteManager;
    public GroupTextChatManager groupTextChatManager;
    public ChatParticipantData OwnProfileData;
    public Dictionary<string, ChatParticipantData> ChatParticipantDataList = new Dictionary<string, ChatParticipantData>();
    public bool isJoined = false;
    public void Awake()
    {
        instance = this;
    }
    
    internal void AddPrarticipant(ChatParticipantData prarticipant)
    {


        string totalParticipantname = "";
        //voiceChatManager.JoinVoiceSession(CurrentRoom.data.room.id);
        HomePanel.instance.chatManager.groupTextChatManager.OnNewUserEnterGroupMessage(prarticipant.username);
        prarticipant.LoadAllAssets().Forget();
        if (!ChatParticipantDataList.ContainsKey(prarticipant.id))
        {
            Debug.Log(" Added New Participant Name:: " + prarticipant.username);

            ChatParticipantDataList.Add(prarticipant.id, prarticipant);
            totalParticipantname+= prarticipant.username + ", ";
           
        }
        else
        {

            Debug.Log(" Participant already exists in the list: " + prarticipant.username);
        }

        Debug.Log("prarticipant.id::" + prarticipant.id + "::CurrentRoom.data.room.hostId::" + CurrentRoom.data.room.hostId +
        "::total ChatParticipantDataList:::" + ChatParticipantDataList.Count);
        if (prarticipant.id != CurrentRoom.data.room.hostId)
        {
            prarticipant.isHost = false;

        }
        else
        {
            prarticipant.isHost = true;

        }
        Debug.Log("prarticipant.isSeated::" + prarticipant.isSeated);   
        if (prarticipant.isSeated)
        {

        chatPanel.UpdateSeat(prarticipant.seatNumber,prarticipant.id);
        }
        

    }
    
    async UniTask  SetCurrentRoomData(JoinChatRoomResponse GetChatRoom)
    {


        CurrentRoom = GetChatRoom;
        await CurrentRoom.data.room.LoadAllAssets();
        await CurrentRoom.data.host.LoadAllAssets();
        Debug.Log("my ID::" + DataManager.instance.userId);
        Debug.Log("is host in the room ::" +CurrentRoom.data.room.roomImageAsset.SpriteAssset);
        Debug.Log("CurrentRoom room id:" + CurrentRoom.data.room.id);
        voiceChatManager.currentRoomId = CurrentRoom.data.room.id;
        voiceChatManager.OnQualityChanged(1);
       
        
  


        if (CurrentRoom.data.host.id == DataManager.instance.userId)
        {

            OwnProfileData = CurrentRoom.data.host;
            OwnProfileData.isHost = true;

            HomePanel.instance.chatManager.groupTextChatManager.OnNewUserEnterGroupMessage(OwnProfileData.username);
           
            Debug.Log("ownprofile::" + OwnProfileData.username + " levelData::" + OwnProfileData.levelData.levelSprite + "spire::" + OwnProfileData.profilePictureAsset.SpriteAssset);
        }
        else
        {
            

        }

            voiceChatManager.JoinVoiceSession(CurrentRoom.data.room.id);
        Debug.Log("StoreCurrentRoomData..." + CurrentRoom.data.room.name + " Host:" + CurrentRoom.data.host.username + " Participants:" + CurrentRoom.data.participants.Length);
       
    }
    internal void AddParticipantInList(ChatParticipantData[] Newparticipants)
    {
        if (ChatParticipantDataList == null)
        {

            ChatParticipantDataList = new Dictionary<string, ChatParticipantData>();
        }
        else
        {
            Debug.Log(" there is   ChatParticipantDataList  already assigned bofre here just add new ChatParticipantData ");
        }
        foreach (var item in Newparticipants)
        {

            AddPrarticipant(item);
        }

     

        if (ChatParticipantDataList.ContainsKey(DataManager.instance.userId))
        {
            OwnProfileData = ChatParticipantDataList[DataManager.instance.userId];
        }
    }
    internal void UpdateChatParticpintdata(ChatParticipantData[] Newparticipants)
    {
        foreach (var item in Newparticipants)
        {
            if (ChatParticipantDataList.ContainsKey(item.id))
            {
                ChatParticipantDataList[item.id] = item;
                Debug.Log(" Updated Participant Name:: " + item.username);
            }
            else
            {
                ChatParticipantDataList.Remove(item.id);
               
            }

               
        }
    }

    internal void ResetRoomData()
    {
        isJoined = false;
        Debug.Log("ResetRoomData...");

        ChatParticipantDataList.Clear();
        OwnProfileData = null;
        chatPanel.ClearOnlineAllUsers();
        ResetRoomSettingData();
        inviteManager.ResetIniviteManagerData();
        groupTextChatManager.ClearAllMessages();
        isJoined = false;
        chatPanel.ResetSeats();
        CurrentRoom = null;
    }

    void ResetRoomSettingData()
    {
        HostRoomsettings = null;
        chatRoomSetting_HostOnlyPanel.RestData();
      

    }
    internal void PickImageFromGallery(RawImage roomImage, string pickImagePath)
    {
        NativeGallery.GetImageFromGallery(
            (path) =>
            {
                if (path != null)
                {
                    // Create Texture from selected image
                    Texture texture = NativeGallery.LoadImageAtPath(path, 512, false); // Max size 1024
                    if (texture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        return;
                    }


                    if (roomImage != null)
                    {
                        roomImage.texture = texture;
                        
                        Texture2D tex2D = texture as Texture2D;
                        if (tex2D != null)
                        {
                            byte[] imageBytes = tex2D.EncodeToJPG(75);
                            pickImagePath = System.Convert.ToBase64String(imageBytes);
                            print("imagePAth::" + pickImagePath);
                           
                        }
                        else
                        {
                            Debug.LogWarning("Selected image is not a Texture2D and cannot be encoded to JPG.");

                        }
                    }
                    else
                    {
                        Debug.LogWarning("roomImage not assigned.");

                    }
                }
            },
            "Select an image", // Title for the picker
            "image/*" // Mime type (image/png, image/jpeg, etc.)
        );

        Debug.Log("Image picker launched.");
    }

    internal void NewPickImageFromGallery(RawImage roomImage)
    {
        NativeGallery.GetImageFromGallery(
            async (path) =>
            {
                if (path != null)
                {
                    // Create Texture from selected image
                    Texture texture = NativeGallery.LoadImageAtPath(path, 512, false);
                    if (texture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        
                        return;

                    }

                    if (roomImage != null)
                    {
                        roomImage.texture = texture;

                    }

                    Texture2D tex2D = texture as Texture2D;
                    if (tex2D != null)
                    {
                        byte[] imageBytes = tex2D.EncodeToJPG(75);
                        string base64ImageString = System.Convert.ToBase64String(imageBytes);
                        print("Image converted to Base64 string.");
                        await DataManager.instance.spriteMaganer.UploadImageToServer(imageBytes,
                         "profile-images", "image/jpeg", "2097152");
                       
                    }
                    else
                    {
                        Debug.LogWarning("Selected image is not a Texture2D.");
                       
                    }
                }
                else
                {
                    // User cancelled the picker
                    Debug.Log("User cancelled image selection.");
                  
                }
            },
            "Select an image",
            "image/*"
        );
    }

    public void OnChooseImageButtonClicked(RawImage roomImage)
    {

        NewPickImageFromGallery(roomImage);
    }
        internal async void SendGetPopularChatListRequest(bool openpanel)
    {
        if (openpanel)
        {

            DataManager.instance.spriteMaganer.ShowLoaderWithProgress("Fetching Available Rooms");
        }
        Debug.Log("GetPopularChatList...");
        

        string requestJson = "";


        try
        {
            Debug.Log("Sending request to PopularChatListRequest...");

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.ChatRoom_PopularChatList,
                httpMethod: "GET",
                requestData: requestJson,
                addAuthHeader: true
            );


            if (response.IsSuccess)
            {

                
               
               
              PopularChatListResponse tmppopularChatListResponse = JsonUtility.FromJson<PopularChatListResponse>(response.Text);

                Debug.Log($"SUCCESS! PopularChatListRequest '{response.Text}'");
                Debug.Log($"SUCCESS! PopularChatListRequest '{tmppopularChatListResponse.success}'");
                Debug.Log("SUCCESS! popularChatRoomListPanel.popularChatListResponse" + popularChatRoomListPanel.popularChatListResponse);


              if (tmppopularChatListResponse.data != null && tmppopularChatListResponse.success)
                {
                   
                    Debug.Log("popular chat rooms available." + tmppopularChatListResponse.data.Length);
                    SyncChatRoomLists(popularChatRoomListPanel.popularChatListResponse,tmppopularChatListResponse);
                    popularChatRoomListPanel.popularChatListResponse = tmppopularChatListResponse;
                    if (openpanel)
                    {

                    HomePanel.instance.OpenPanel(popularChatRoomListPanel.gameObject);
                    }


            
                    


                    
                }
                  else
                  {
                      Debug.Log("No popular chat rooms available.");
                      return;
                  }



            }
            else
            {

                Debug.LogError($"SERVER ERROR ({response.StatusCode}): {response.Text}");

            }
        }
        catch (WebServiceException e)
        {

            Debug.LogError($"REQUEST FAILED ({e.StatusCode}): {e.ErrorMessage}" + DataManager.instance.AccessToken);

        }
        finally
        {

        }
    }
    public void SyncChatRoomLists(PopularChatListResponse lastResponse, PopularChatListResponse currentResponse)
    {
    
        if (currentResponse == null || !currentResponse.success || currentResponse.data == null)
        {
            Debug.LogError("The new chat room response is invalid. Aborting sync.");
            DataManager.instance.spriteMaganer.DisplayWarningPanel("The new chat room response is invalid");
            return;
        }

        //if old list is empty
        if (lastResponse == null || lastResponse.data == null || lastResponse.data.Length == 0)
        {
            Debug.Log("No old data exists. Performing initial load of all items.");
            foreach (ChatRoomData newRoom in currentResponse.data)
            {
                LoadRoomData(newRoom);
            }
            return;
        }

        var oldRoomsById = lastResponse.data.ToDictionary(room => room.id, room => room);
        var newRoomIds = new HashSet<string>(currentResponse.data.Select(room => room.id));

      
        foreach (ChatRoomData newRoom in currentResponse.data)
        {
            if (oldRoomsById.TryGetValue(newRoom.id, out ChatRoomData oldRoom))
            {
                // ROOM EXISTS:  UPDATE IT.
               
                if (!newRoom.Equals(oldRoom))
                {
                    // DATA IS DIFFERENT must update the UI.
                    Debug.Log($"Data for room '{newRoom.name}' ({newRoom.roomImageAsset._isLoaded}) has changed. Updating UI.");
                    Debug.Log("old room::" + oldRoom.roomImageAsset._isLoaded);
                    LoadRoomData(newRoom);
                }
                else
                {
                    // DATA IS THE SAME.
                   
                    Debug.Log($"Data for room '{newRoom.name}' ({newRoom.id}) is unchanged. Skipping.");
                }
            }
            else
            {
                // ROOM IS NEW:
                Debug.Log($"Adding new room: '{newRoom.name}' ({newRoom.id}).");
                LoadRoomData(newRoom);
            }
        }

       
    }
    async void LoadRoomData(ChatRoomData chatRoomData)
    {
        Debug.Log("UpdatepopularChatRoomListData...");


        if (!chatRoomData.roomImageAsset._isLoaded)
        {

            await chatRoomData.LoadAllAssets();
            popularChatRoomListPanel.AddChartRoomItem(chatRoomData);
        }
     
        

       

       
    }
    internal async void SendCreateNewRoomRequest(string roomName, string roomDescription, int roomParticipants, string roomImageStr)
    {
        Debug.Log("CreateNewRoom...");



        var requestData = new CreateRoomRequest
        {
            name = roomName,
            description = roomDescription,
            isPrivate = false,
            maxParticipants = roomParticipants,
            roomImage = roomImageStr

        };

        string requestJson = JsonUtility.ToJson(requestData);

        try
        {
            Debug.Log("Sending request to CreateNewRoom...");

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.ChatRoom_CreatechatRoom,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: true
            );

            Debug.Log($"GGOT CreateNewRoomResponse '{response}'");

            if (response.IsSuccess)
            {

                JoinChatRoomResponse joinChatRoomDatatResponse = JsonUtility.FromJson<JoinChatRoomResponse>(response.Text);
                //  ChatRoomData joinChatRoomDatatResponse1 = JsonUtility.FromJson<ChatRoomData>(response.Text);
                DataManager.instance.IsRoomCreated = true;
                DataManager.instance.spriteMaganer.ResetprfoileurltosendServer();



                if (joinChatRoomDatatResponse.data != null)
                {
                    DataManager.instance.roomID = joinChatRoomDatatResponse.data.room.id;
                     JoinRoomBySockt(CurrentRoom.data.room.id);
                    /* chatPanel.SetChatArea(joinChatRoomDatatResponse);
                     StoreCurrentRoomData(joinChatRoomDatatResponse);
                     HomePanel.instance.OpenPanel(chatPanel.gameObject);*/

                    return;
                }
                else
                {
                    Debug.Log("No popular chat rooms available.");
                    return;
                }



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
    internal void JoinRoomBySockt(string roomid)
    {

        Debug.Log("JoinRoomBySockt::" + roomid);
        var data = new Dictionary<string, object> { { "roomId",roomid } };
        LudoSocketManager.Instance.EmitWithAck<JoinChatRoomResponse>(GameConstants.EmitEvents.joinroom, payloadData: data
              , onAckResponse: async (response) =>
         {
            
 Debug.Log($" before onAckResponse join-room-realtime received: {response.ToString()} items");
             if (response != null && response.success)
             {
                    isJoined = true;



             Debug.Log($" response join-room-realtime received: {response.data.room.name} items");
                 await OnJoinedoom(response);
                 
             }
             else
             {
                isJoined = false;
                 Debug.LogWarning("Failed to getjoin-room-realtime ");
             }
         }
      );
    }
    internal async UniTask OnJoinedoom(JoinChatRoomResponse responsedata)
    {


      await  SetCurrentRoomData(responsedata);

        // SendParticipantsRequest(CurrentRoom.data. id); new api resposne we get this in currentroom data.paqrticipants
        
        chatPanel.SetChatRoom(CurrentRoom);

       

        HomePanel.instance.OpenPanel(chatPanel.gameObject);
        Debug.Log("OnJoinedoom Room Name..." + responsedata.data.room.id);
        DataManager.instance.spriteMaganer.loaderManager.HideLoader();
    }
   internal void LeavRoom()
    {
        ResetRoomData();
       
     voiceChatManager.CleanupVoiceSession();
        chatPanel.gameObject.SetActive(false);
        //    voiceChatManager.OnUserLeftRoom(CurrentRoom.data.room.id.ToString());
    }
    void StartVoiceChatSession_onlyforHost()
    {
        /*var data = new Dictionary<string, object> { { "roomId", CurrentRoom.data.room.id } };

        LudoSocketManager.Instance.EmitEvent_With_responseEventHanbleBothCallBack<start_voice_chatResponse>(emitEvent: "start-voice-chat", responseEvent: "voice_chat_started", payloadData: data,
            onPushResponse: (response) =>
            {
                if (response != null && response.success)
                {

                    Debug.Log($"Handle onPushResponse  started-voice-chat received: {response.data.roomId} items");
                    //voiceChatManager. currentRoomId = response.data.roomId.ToString();
                   // voiceChatManager.WebRTC_forCreatedRoom(DataManager.instance.userId);
                }
                else
                {
                    Debug.LogWarning("Failed to get start-voice-chat ");
                }
            }
);*/
    }
    internal async void SendGetRoomByRoomCodeRequest(ChatRoomData chatroom)
    {
        Debug.Log("GetRoomByRoomCode...");


        var requestData = new RequestDataByRoomCodeOnly
        {
            roomCode = chatroom.roomCode


        };

        string requestJson = JsonUtility.ToJson(requestData);

        try
        {
            Debug.Log("Sending request to GetRoomByRoomCode...");

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.ChatRoom_JoinchatRoombyCode,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: true
            );


            if (response.IsSuccess)
            {

                CurrentRoom = JsonUtility.FromJson<JoinChatRoomResponse>(response.Text);

                Debug.Log($"SUCCESS! SendGetRoomByRoomCodeRequest '{response.Text}'");
                Debug.Log("SUCCESS! SendGetRoomByRoomCodeRequest" + CurrentRoom.ToString());


                if (CurrentRoom.data != null)
                {

              //      SetCurrentRoomData(CurrentRoom);
                    await CurrentRoom.data.room.LoadAllAssets();
                    // SendParticipantsRequest(CurrentRoom.data. id); new api resposne we get this in currentroom data.paqrticipants
                    chatPanel.SetChatRoom(CurrentRoom);
                    if (CurrentRoom.data.host.id.ToString() == DataManager.instance.userId)
                    {
                        // VoiceChatManager.instance.Start_voice_chatByHostOnly(CurrentRoom.data.room.id.ToString());
                    }
                    else
                    {
                        //  VoiceChatManager.instance.Start_voice_chatByHostOnly(CurrentRoom.data.room.id.ToString());
                        //add voicechat join code for other user
                    }

                    HomePanel.instance.OpenPanel(chatPanel.gameObject);
                    return;
                }
                else
                {
                    Debug.Log("No popular chat rooms available.");
                    return;
                }



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

    /*internal async void SendLeaveRoomRequest()
    {
        Debug.Log("LeaveRoom..." + CurrentRoom.data.room.id.ToString() +
        "...online data:::" + chatPanel.OnlineUserlist.Count);


        var requestData = new RequestDataByRoomIDOnly
        {
            roomId = CurrentRoom.data.room.id


        };

        string requestJson = JsonUtility.ToJson(requestData);
        string urlTemplate = GameConstants.GAME_URLS[(int)RequestType.ChatRoom_LeaveRoomByID];
        string apiUrl = GameConstants.FormatUrl(urlTemplate, ("roomId", CurrentRoom.data.room.id));
        // Use apiUrl for your API call
        try
        {
            Debug.Log("Sending request to LeaveRoom...+" + requestJson);

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.ChatRoom_LeaveRoomByID,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: true,
                 customUrl: apiUrl
            );

            Debug.Log($"GGOT LeaveRoom '{response}'");

            if (response.IsSuccess)
            {

                LeavRoom();


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
    }*/

   internal void EmitLeaveroom()
    {
        var data = new Dictionary<string, object> { { "roomId", CurrentRoom.data.room.id } };
        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.EmitEvents.leaveroom, payloadData: data
              , onAckResponse: (response) =>
         {
             if (response != null && response.success)
             {
                 LeavRoom();

                 Debug.Log($" onAckResponse userleftrealtime received: {response.success} items");
             }
             else
             {
                 Debug.LogWarning("Failed to get userleftrealtime ");
             }
         }
      );
    }
    internal async void SendHostRoomSettingsRequest()
    {
        Debug.Log("SendHostRoomSettingsRequest...");


        var requestData = new RequestDataByRoomIDOnly
        {
            roomId = CurrentRoom.data.room.id


        };

        string requestJson = JsonUtility.ToJson(requestData);
        string urlTemplate = GameConstants.GAME_URLS[(int)RequestType.ChatRoom_SettingsByID];
        string apiUrl = GameConstants.FormatUrl(urlTemplate, ("roomId", CurrentRoom.data.room.id));
        // Use apiUrl for your API call
        try
        {
            Debug.Log("Sending request to RoomSettings...+" + requestJson);

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.ChatRoom_SettingsByID,
                httpMethod: "PUT",
                requestData: requestJson,
                addAuthHeader: true,
                 customUrl: apiUrl
            );
            ChatRoomSettingResponse HostRoomsettingg = JsonUtility.FromJson<ChatRoomSettingResponse>(response.Text);
            Debug.Log($"GGOT RoomSettings '{response.Text}'");
            if (HostRoomsettings == null)
            {
               
               
                if (response.IsSuccess)
                {

                    UpdateHostSetting(HostRoomsettingg);

                }
                else
                {

                    Debug.LogError($"SERVER ERROR ({response.StatusCode}): {response.Text}");

                }
               
            }
            else
            {
                if (HostRoomsettings.success)
                {

                    chatRoomSetting_HostOnlyPanel.SetRoomSettingsData(HostRoomsettings.data);
                    HomePanel.instance.OpenPanel(chatRoomSetting_HostOnlyPanel.gameObject);
                    DataManager.instance.spriteMaganer.loaderManager.HideLoader();

                }
                else
                {
                    UpdateHostSetting(HostRoomsettingg);
                }
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
   async void UpdateHostSetting(ChatRoomSettingResponse HostRoomsetting)
    {
        HostRoomsettings = HostRoomsetting;
        await HostRoomsettings.data.LoadAllAssets();
        chatRoomSetting_HostOnlyPanel.SetRoomSettingsData(HostRoomsettings.data);
        HomePanel.instance.OpenPanel(chatRoomSetting_HostOnlyPanel.gameObject);
        DataManager.instance.spriteMaganer.loaderManager.HideLoader();
        Debug.Log("HostRoomsettings..." + HostRoomsettings.data.effectsEnabled);



    }
    internal async void SendParticipantsRequest(string roomId)
    {
        Debug.Log("Get Participantst..." + roomId.ToString());


        var requestData = new RequestDataByRoomIDOnly
        {
            roomId = roomId


        };

        string requestJson = JsonUtility.ToJson(requestData);
        string urlTemplate = GameConstants.GAME_URLS[(int)RequestType.ChatRoom_ParticipantsByID];
        string apiUrl = GameConstants.FormatUrl(urlTemplate, ("roomId", roomId));
        // Use apiUrl for your API call
        try
        {
            Debug.Log("Sending request to Participantst...+" + requestJson);

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.ChatRoom_ParticipantsByID,
                httpMethod: "GET",
                requestData: requestJson,
                addAuthHeader: true,
                 customUrl: apiUrl
            );

            Debug.Log($"GGOT Participantst '{response.Text}'");

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
            // This block always runs, whether the request succeeded or failed.
            // It's the perfect place to re-enable the UI.
            //createRoomButton.interactable = true;
        }
    }

    //============================================================================ChatRoommangment Room Sockt========================================================
    internal void Emituserapplyseat(string seatnumber)
    {
        var data = new Dictionary<string, object> { { "roomId", CurrentRoom.data.room.id },
            { "seatNumber", seatnumber} };
        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.EmitEvents.userapplyseat, payloadData: data
              , onAckResponse: (response) =>
         {
             Debug.Log("user apply seat response"+response);
             if (response != null && response.success)
             {

                 Debug.Log($" onAckResponse user_apply_seat received: {response.success} items");
             }
             else
             {
                 var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.ToString());
                 if (data.ContainsKey("success"))
                 {
                     bool Issucces = (bool)data.ContainsKey("success");
                     if (!Issucces)
                     {
                         FailedEmitResponse failedEmitResponse = JsonConvert.DeserializeObject<FailedEmitResponse>(response.ToString());
                         DataManager.instance.spriteMaganer.DisplayWarningPanel(failedEmitResponse.error);
                     }
                 }
                
                 
                 Debug.LogWarning("Failed to get user_apply_seat ");
             }
         }
      );
    }
    internal void Emituserleaveseat(string seatnumber)
    {
        var data = new Dictionary<string, object> { { "roomId", CurrentRoom.data.room.id },
            { "seatNumber", seatnumber} };
        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.EmitEvents.userleaveseat, payloadData: data
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
    public RectTransform bottomPanel,topPanel;

   
    private bool isHalf = false;

    public void TogglePanel()
    {
        if (isHalf)
        {
            // Full screen size
            bottomPanel.anchorMin = new Vector2(0.0f, 0.0f);
            bottomPanel.anchorMax = new Vector2(1.0f, 1.0f);
            bottomPanel.pivot = new Vector2(0.5f, 0.5f);

            bottomPanel.offsetMin = Vector2.zero;
            bottomPanel.offsetMax = Vector2.zero;
            topPanel.gameObject.SetActive(false);
            // Panel goes center automatically

        }
        else
        {
            // Half screen bottom
            bottomPanel.anchorMin = new Vector2(0.0f, 0.0f);   // bottom
            bottomPanel.anchorMax = new Vector2(1.0f, 0.5f);   // half height
            bottomPanel.pivot = new Vector2(0.5f, 0.0f);       // bottom center
            topPanel.gameObject.SetActive(true);
            bottomPanel.offsetMin = Vector2.zero;
            bottomPanel.offsetMax = Vector2.zero;

        }

        isHalf = !isHalf;
    }
}
