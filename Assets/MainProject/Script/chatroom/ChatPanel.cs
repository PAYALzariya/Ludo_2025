using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.UI;



public class ChatPanel : MonoBehaviour
{

    public List<Chatroompopup_inventoryType> inventorylist = new List<Chatroompopup_inventoryType>();
    public List<Profiles> sendinvotryprofilelist = new List<Profiles>();
    public ChatRoomProfile chatRoomprofile;

    public GameObject GiftPopupPanel;
    public TMP_ColorGradient selctedinventorycolor;
    public TMP_ColorGradient defaultinventorycolor;
    public Chatroompopup_inventoryItems prefab_nventoryItem;
    public List<GameObject> chatroomPanelList = new List<GameObject>();
    public ChatRoomSetting_HostOnly chatRoomSetting_HostOnlyPanel;
    public ChatRoomSetting_ParticipantOnly chatRoomSetting_ParticipantOnlyPanel;

    public GameObject bottomPanel;


    public ChatRoomInvit chatRoomInvitPrefab;
    public Transform chatRoomInvitParentTransform;
    public HostProfile hostProfile;
    public Dictionary<string, ChatRoomInvit> participantsSeatList = new Dictionary<string, ChatRoomInvit>();

    public Dictionary<string, CommonUserProfileItem> OnlineUserlist = new Dictionary<string, CommonUserProfileItem>();
    public ChatRoomInvit Selectdseatitem;
     public CommonUserProfileItem OnlineUserListItemPrefab;
    public Toggle micToggle;
    public GameObject ludoobject;

    public void OnClickLudoGame()
    {

    }
    public void OnClickGift()
    {

        GiftPopupPanel.gameObject.SetActive(true);
    }
    public void OnClickOpenChat()
    {

    }
    internal void SetChatRoom(JoinChatRoomResponse chatroom)
    {
        bottomPanel.gameObject.SetActive(true);
        bottomPanel.transform.SetAsLastSibling();
        SetChatRoomHost(chatroom);
        SetChatRoomProfile(chatroom);
        if (HomePanel.instance.chatManager.CurrentRoom.data.room.maxParticipants != participantsSeatList.Count)
        {
            float tmplentgh = HomePanel.instance.chatManager.CurrentRoom.data.room.maxParticipants - participantsSeatList.Count;
            int looplength = Mathf.RoundToInt(tmplentgh);
            CloneChatRoomInvitList(looplength);
        }

    }
 internal   void ResetSeats()
    {
        foreach (var item in participantsSeatList)
        {
            item.Value.centerIcon.sprite = DataManager.instance.spriteMaganer.unlocksprite;
            item.Value.isSeated = false;
            item.Value.IsSeatLocked = false;
            item.Value.seatownerUserId = null;
            item.Value.centerIcon.gameObject.SetActive(true);
            item.Value.userProfile.gameObject.SetActive(false);
            item.Value.sideIcon.gameObject.SetActive(false);    
            item.Value.centerIcon.sprite= DataManager.instance.spriteMaganer.unlocksprite;
            item.Value.labelText.text = item.Value.seatNo.ToString();
        }
        Selectdseatitem = null;
    }
    internal void CloneChatRoomInvitList(int looplength)
    {

        Debug.Log("CloneChatRoomInvitList..." + HomePanel.instance.chatManager.CurrentRoom.data.room.maxParticipants);
        for (int i = 0; i < looplength; i++)
        {
            ChatRoomInvit item = Instantiate(chatRoomInvitPrefab);
            item.transform.SetParent(chatRoomInvitParentTransform);
            item.centerIcon.sprite = DataManager.instance.spriteMaganer.unlocksprite;
            item.transform.transform.position = chatRoomInvitParentTransform.position;
            item.transform.localScale = Vector3.one;
            item.userProfile.gameObject.SetActive(false);   
            item.isSeated = false;
            item.IsSeatLocked = false;
            item.gameObject.SetActive(true);
            item.seatNo = i + 1;
            Debug.Log("SEAT NO::" + item.seatNo);
            item.labelText.text = item.seatNo.ToString();
            item.button.onClick.AddListener(() => OnParticipantButtonClick(item));
            participantsSeatList.Add(item.labelText.text, item);


        }


    }
    
internal void OnParticipantButtonClick(ChatRoomInvit seatitem)
    {
        Debug.Log("OnParticipantButtonClick..." + hostProfile.userIshost);

        if (seatitem.isSeated)
        {

        }
        else
        {
            Selectdseatitem = seatitem;
            if (seatitem.IsSeatLocked)
            {
                if (hostProfile.userIshost)
                {

                    HomePanel.instance.OpenPanel(HomePanel.instance.chatManager.inviteManager.seatsettingOnlyForhostPanel.gameObject);
                }
                else
                {

                    HomePanel.instance.chatManager.inviteManager.OpenSendRequestToHostScreen(Selectdseatitem.seatNo.ToString());

                }
            }
            else
            {


                if (hostProfile.userIshost)
                {

                    HomePanel.instance.OpenPanel(HomePanel.instance.chatManager.inviteManager.seatsettingOnlyForhostPanel.gameObject);
                }
                else
                {

                    HomePanel.instance.chatManager.inviteManager.OpenSendRequestToHostScreen(Selectdseatitem.seatNo.ToString());

                }
            }

        }
    }
internal void UpdateSeat(string seatno, string userid)
    {
        Debug.Log("UpdateSeat"+ seatno);
      

       ChatParticipantData participantData;
        HomePanel.instance.chatManager.ChatParticipantDataList .TryGetValue(userid, out  participantData);
        ChatRoomInvit seatitem;
        participantsSeatList.TryGetValue(seatno, out seatitem);
        Debug.Log(" UpdateSeat ::ChatParticipantDataList::" + participantData);
        if (participantData != null)
        {
            
          Debug.Log("get seat:" + seatitem);
            seatitem.isSeated = true;
        seatitem.centerIcon.gameObject.SetActive(false);
        seatitem.userProfile.profilePic.sprite = participantData.profilePictureAsset.SpriteAssset;
        seatitem.userProfile.frameImage.sprite = participantData.frameData.frameSprite;
        seatitem.userProfile.gameObject.SetActive(true);
        seatitem.labelText.text = participantData.username;
        seatitem.seatownerUserId= userid;
        }
        else if(userid== HomePanel.instance.chatManager.OwnProfileData.id)
        {
           
            Debug.Log("get own seat:" + seatitem);
          /*  seatitem.isSeated = true;
            seatitem.centerIcon.gameObject.SetActive(false);
            seatitem.userProfile.profilePic.sprite = HomePanel.instance.chatManager.OwnProfileData.profilePictureAsset.SpriteAssset;
            seatitem.userProfile.frameImage.sprite = HomePanel.instance.chatManager.OwnProfileData.frameData.frameSprite;
            seatitem.userProfile.gameObject.SetActive(true);
            seatitem.labelText.text = HomePanel.instance.chatManager.OwnProfileData.username;
            seatitem.seatownerUserId = userid;*/
        }
       
        Debug.Log("profileitem.userNameText:" +  participantData.profilePictureAsset.SpriteAssset);
         Debug.Log("seatitem.PlayerNameText:" +  participantData.profilePicture);
        
//After  done wtih testing  this is DragCorrector way to join voice chat
        //HomePanel.instance.chatManager.voiceChatManager.JoinVoiceSession(HomePanel.instance.chatManager.CurrentRoom.data.room.id);
    }
    internal void ResetSeatDataWithUserid(string userid)
    {
        Debug.Log("UpdateUnSeat" + participantsSeatList.Count + "::OnlineUserlist::" + OnlineUserlist.Count);
    
        foreach (var seat in participantsSeatList)
        {
            ChatRoomInvit seatitem=seat.Value;

            if (seatitem.seatownerUserId== userid)
            {

                Resetseat(seatitem.seatNo.ToString());


            }   
        }
    }
   internal void Resetseat(string seatno)
    {
        foreach (var seat in participantsSeatList)
        {

            ChatRoomInvit seatitem = seat.Value;
            if (seatitem.seatNo.ToString()==seatno)
            {
                seatitem.isSeated = false;
                seatitem.IsSeatLocked = false;
                seatitem.seatownerUserId = null;
                seatitem.centerIcon.gameObject.SetActive(true);
                seatitem.userProfile.gameObject.SetActive(false);
                seatitem.labelText.text = seatitem.seatNo.ToString();
            }
        }
    }

    // common funcation
    public void OnClickPanelCloseButton(GameObject panel)
    {
        panel.gameObject.SetActive(false);

    }
    internal void SetChatRoomHost(JoinChatRoomResponse ChatRoom)
    {
        hostProfile.nameText.text = ChatRoom.data.host.username;
        hostProfile.image.texture = ChatRoom.data.host.profilePictureAsset.SpriteAssset.texture;
        Debug.Log("host profile teture::" + hostProfile.image.texture);
        hostProfile.frame.texture = ChatRoom.data.host.frameData.frameSprite.texture;
         Debug.Log("host frame teture::" + hostProfile.frame.texture.texelSize);
         Debug.Log("host frame mainTexture::" + hostProfile.frame.mainTexture.texelSize);
        hostProfile.userId = ChatRoom.data.host.id;

        if (ChatRoom.data.host.id == DataManager.instance.userId)
        {
            hostProfile.IshostInposition = true;
            hostProfile.userIshost = true;

        }
        else
        {
            hostProfile.IshostInposition = false;
            hostProfile.userIshost = false;

        }
    }
    internal void SetChatRoomProfile(JoinChatRoomResponse chatroom)
    {
        Debug.Log("chatroom.roomdata::"+chatroom.data.room.roomImageAsset.SpriteAssset);
        chatRoomprofile.nameText.text = chatroom.data.room.name;
        chatRoomprofile.iDText.text = "ID:" + chatroom.data.room.id.ToString();
        chatRoomprofile.image.texture = chatroom.data.room.roomImageAsset.SpriteAssset.texture;
    }
    public void OnShoudownButtonClick()
    {
   // HomePanel.instance.chatManager.SendLeaveRoomRequest();
      HomePanel.instance.chatManager.EmitLeaveroom();
    }
    public void OnRoomSettingButtonClick()
    {
        DataManager.instance.spriteMaganer.ShowLoaderWithProgress("Loading Room Settings");
        Debug.Log("OnRoomSettingButtonClick..." + HomePanel.instance.chatManager.HostRoomsettings + "::ishost::" +
        HomePanel.instance.chatManager.OwnProfileData.isHost);
        if (HomePanel.instance.chatManager.OwnProfileData.isHost)
        {
            HomePanel.instance.chatManager.SendHostRoomSettingsRequest();
            
        }
        else
        {
            OnSetParticipantSettings();
            
            HomePanel.instance.OpenPanel(chatRoomSetting_ParticipantOnlyPanel.gameObject);
            DataManager.instance.spriteMaganer.loaderManager.HideLoader();
        }




    }

       internal void OnSetParticipantSettings()
    {




     
        if (OnlineUserlist == null)
        {
            OnlineUserlist = new Dictionary<string, CommonUserProfileItem>();
        }
        Debug.Log("OnSetParticipantSettings"+HomePanel.instance.chatManager.ChatParticipantDataList.Count);
        foreach (var useritem in HomePanel.instance.chatManager.ChatParticipantDataList)
        {
            string userId = useritem.Key;
            ChatParticipantData participant = useritem.Value;
             
            
            if (OnlineUserlist.TryGetValue(userId, out CommonUserProfileItem existingItem))
            {
                Debug.Log("User found in OnlineUserlist: " + existingItem.userNameText.text);
                if (OnlineUserlist[userId].participantData != participant)
                {
                    OnlineUserlist[userId].participantData = participant;
                    OnlineUserlist[userId].SetUserProfile(); // update UI
                    Debug.Log("Updated user in OnlineUserlist: " + OnlineUserlist[userId].profileImage.sprite);
                }
            }
            else
            {
                if (!OnlineUserlist.ContainsKey(userId))
            {
               
                CommonUserProfileItem newitem = AddOnlineUserItem(
                      participant,
                      OnlineUserlist);
                Debug.Log("Adding new user to OnlineUserlist: " + newitem.userNameText.text);
            }
            }
        }
        foreach (var useritem in OnlineUserlist)
        {
            Debug.Log("OnSetParticipantSettings ..." + useritem.Value.userNameText.text);
            string userId = useritem.Key;

            if (HomePanel.instance.chatManager.ChatParticipantDataList.ContainsKey(userId))
            {

                HomePanel.instance.chatManager.chatPanel.DisplayOnlineUserItem(
                      chatRoomSetting_ParticipantOnlyPanel.onlineuserParent, useritem.Value);

            }
            else
            {
                Debug.Log("Removing user from OnlineUserlist: " + useritem.Value.userNameText.text);
                Destroy(useritem.Value.gameObject);
                OnlineUserlist.Remove(userId);
                break; // Exit the loop to avoid modifying the collection during iteration
            }
        }
        


        if (!OnlineUserlist.ContainsKey(HomePanel.instance.chatManager.OwnProfileData.id))
        {
            CommonUserProfileItem ownitem = AddOnlineUserItem(
                  HomePanel.instance.chatManager.OwnProfileData,
                  OnlineUserlist);
            HomePanel.instance.chatManager.chatPanel.DisplayOnlineUserItem(
                  chatRoomSetting_ParticipantOnlyPanel.onlineuserParent, ownitem);

        }
    }
    internal CommonUserProfileItem AddOnlineUserItem(ChatParticipantData userdata,
        Dictionary<string, CommonUserProfileItem> UIItemlist)
    {
        CommonUserProfileItem item = Instantiate(OnlineUserListItemPrefab);
        item.gameObject.SetActive(false);
        item.participantData = userdata;
        item.SetUserProfile();
        UIItemlist.Add(userdata.id, item);
       
        return item;
    }







    internal CommonUserProfileItem CloneOnlineUserItem(ChatParticipantData userdata)
    {
        CommonUserProfileItem item = Instantiate(OnlineUserListItemPrefab);
        item.gameObject.SetActive(false);
        item.participantData = userdata;
        item.SetUserProfile();
        return item;
    }
    internal void DisplayOnlineUserItem(Transform onlineuserParent,
        CommonUserProfileItem userItem)
    {

        userItem.transform.SetParent(onlineuserParent);
        userItem.transform.localScale = Vector3.one;
        userItem.transform.position = onlineuserParent.position;
        userItem.gameObject.SetActive(true);

    }



    internal void ClearOnlineAllUsers()
    {
        Debug.Log("ClearOnlineAllUsers..." + OnlineUserlist.Count);
        foreach (var user in OnlineUserlist)
        {
            Debug.Log("remove item..." + user.Value.userNameText.text);
            Destroy(user.Value.gameObject);

        }
        OnlineUserlist.Clear();
        Selectdseatitem = null;
    }

    internal Dictionary<string, CommonUserProfileItem> SyncWaitingListWithSendRequest(
Dictionary<string, CommonUserProfileItem> UIItemlist,
Dictionary<string, ChatParticipantData> ServerDataList)
    {
        Debug.Log("SyncWaitingListWithSendRequest...UIItemlist:" + UIItemlist.Count +
            "; ServerDataList:" + ServerDataList.Count);
        var newUiItemList = new Dictionary<string, CommonUserProfileItem>();



        foreach (var serverKvp in ServerDataList)
        {
            string userId = serverKvp.Key;
            ChatParticipantData participant = serverKvp.Value;

            Debug.Log("Processing ServerDataList userId: " + userId + " | participant username: " + participant.username);
            if (UIItemlist.TryGetValue(userId, out CommonUserProfileItem existingItem))
            {
                Debug.Log("User found in UIItemlist: " + existingItem.userNameText.text);
                if (UIItemlist[userId].participantData != participant)
                {

                    UIItemlist[userId].participantData = participant;
                    UIItemlist[userId].SetUserProfile(); // update UI
                    Debug.Log("Updated user in UIItemlist: " + UIItemlist[userId].profileImage.sprite);
                }




                Debug.Log("User already in UIItemlist: " + UIItemlist[userId].userNameText.text);

            }
            else
            {
                CommonUserProfileItem newitem = AddOnlineUserItem(
                      participant,
                      newUiItemList);
                Debug.Log("Adding new user to UIItemlist: " + newitem.userNameText.text);
            }
        }


        return newUiItemList;
    }
    //============================================================================================================SocktReposne Room managemnt ==========================
    internal void OnUserapplyseatsuccess(user_apply_seatSuccessResponse response1)
    {
        Debug.Log("OnUserapplyseatsuccess..." + response1.data.seatNumber+"::userid::"+response1.data.userId);
        if (!string.IsNullOrEmpty(response1.data.seatNumber))
        {
            string seatnumber = response1.data.seatNumber;
            if (participantsSeatList.ContainsKey(seatnumber))
            {
                HomePanel.instance.chatManager.chatPanel.UpdateSeat(seatnumber, HomePanel.instance.chatManager.OwnProfileData.id);
               
            }

        }
    }
    public void OnMicToggle()
    {
        Debug.Log("OnMicToggle..." + micToggle.isOn);
        if (micToggle.isOn)
        {
            HomePanel.instance.chatManager.voiceChatManager.isMicrophoneActive = true;
        }
        else
        {
            HomePanel.instance.chatManager.voiceChatManager.isMicrophoneActive = false;

        }
        HomePanel.instance.chatManager.voiceChatManager.ToggleMicrophone();
        
    }
   public  void OnSpeakerButtonClick()
    {
 

    }



}





