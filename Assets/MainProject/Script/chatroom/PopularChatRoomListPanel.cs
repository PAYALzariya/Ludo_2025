using BestHTTP.JSON;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;



public class PopularChatRoomListPanel : MonoBehaviour
{
    public PopularChatRoomItem clonePopularChatRoomItem;
    public Dictionary<string, PopularChatRoomItem> popularChatRoomItemsBYID = new Dictionary<string, PopularChatRoomItem>();
     public Dictionary<string, PopularChatRoomItem> popularChatRoomItemsBYNAME = new Dictionary<string, PopularChatRoomItem>();
    public PopularChatListResponse popularChatListResponse = null;
    public Transform chatRoomItemaParent;
    public Transform eventandAllRoomParnet;
    public Transform notFoundChatroomPanel;
    public FoundChatroomPanel foundChatroomPanel;
    public TMP_InputField searchRoomInput;
   // F2EBEB
    private void OnEnable()
    {
        DataManager.instance.spriteMaganer.loaderManager.HideLoader();
        CloseAllpanels();
        eventandAllRoomParnet.gameObject.SetActive(true);
    }
    
   void AddDatainpopularChatRoomItemsDictionary(string ID,string Name,PopularChatRoomItem ChatRoomItem)
    {
        popularChatRoomItemsBYID.Add(ID, ChatRoomItem);
        popularChatRoomItemsBYNAME.Add(Name.ToLower(), ChatRoomItem);
    }
    internal void AddChartRoomItem(ChatRoomData chatroom)
    {
        if (popularChatRoomItemsBYID == null)
        {
            popularChatRoomItemsBYID = new Dictionary<string, PopularChatRoomItem>();
            popularChatRoomItemsBYNAME = new Dictionary<string, PopularChatRoomItem>();
        }

    
        if (popularChatRoomItemsBYID.ContainsKey(chatroom.id))
        {
            //update it 
            UpdateRoomItem(popularChatRoomItemsBYID[chatroom.id], chatroom);

        }
        else
        {

            CloneChatRoomItem(chatroom,chatRoomItemaParent,true);
        }

    }
    void CloneChatRoomItem(ChatRoomData chatroom,Transform itemParent,bool addToDic)
    {
        PopularChatRoomItem cloneItem = Instantiate(clonePopularChatRoomItem);

        cloneItem.roomdata = chatroom;
        cloneItem.transform.SetParent(itemParent);
        cloneItem.transform.localScale = Vector3.one;
        cloneItem.transform.position = itemParent.position;
        cloneItem.roomNameText.text = cloneItem.roomdata .name;
        cloneItem.roomImage.sprite = cloneItem.roomdata .roomImageAsset.SpriteAssset;
        cloneItem.levelImage.sprite = cloneItem.roomdata .levelData.levelSprite;
        cloneItem.roomIdText.text = "ID: " + cloneItem.roomdata .id;
        cloneItem.OnllineuserCountText.text = cloneItem.roomdata.currentParticipants.ToString();
        
        cloneItem.button.onClick.AddListener(() => OnPopularChatRoomItemClick(cloneItem.roomdata));
         if(addToDic)
         {
            
        AddDatainpopularChatRoomItemsDictionary(cloneItem.roomdata.id, cloneItem.roomdata.name, cloneItem);
        }
        

        // print("Chat Room Image URL: " +   cloneItem.roomData .roomImageAsset.SpriteAssset);

    }
    
    void UpdateRoomItem(PopularChatRoomItem cloneItem, ChatRoomData chatroom)
    {
        cloneItem.roomNameText.text = chatroom.name;
        cloneItem.roomImage.sprite = chatroom.roomImageAsset.SpriteAssset;
        cloneItem.levelImage.sprite = chatroom.levelData.levelSprite;
        cloneItem.OnllineuserCountText.text = chatroom.currentParticipants.ToString();
    }
   internal void OnPopularChatRoomItemClick(ChatRoomData chatroom)
    {
        Debug.Log("OnPopularChatRoomItemClick " + chatroom.roomCode);
       CloseAllpanels();
        eventandAllRoomParnet.gameObject.SetActive(true);
        DataManager.instance.spriteMaganer.ShowLoaderWithProgress("Connecting to the chat room");
        //Debug.Log("OnPopularChatRoomItemClick " +   Clonechatroom.roomData.id);
        HomePanel.instance.chatManager.groupTextChatManager.ClearAllMessages();
        HomePanel.instance.chatManager.groupTextChatManager.
        AddCommonAnucementMessage("You have joined the room:" + chatroom.name);
        HomePanel.instance.chatManager.JoinRoomBySockt(chatroom.id);


    }
    public void OnCreatChatRoomButtonClick()
    {
         CloseAllpanels();
        eventandAllRoomParnet.gameObject.SetActive(true);
        if (DataManager.instance.IsRoomCreated)
        {

            HomePanel.instance.chatManager.JoinRoomBySockt(DataManager.instance.roomID);

        }
        else
        {
            HomePanel.instance.OpenPanel(HomePanel.instance.chatManager.createChatRoomPanel.gameObject);
        }
       
        
    }
    public void OnEditEndRoomSearchInputField()
    {
        
        if (!string.IsNullOrEmpty(searchRoomInput.text))
        {

            if (popularChatRoomItemsBYID.ContainsKey(searchRoomInput.text))
            {
                //DataManager.instance.spriteMaganer.DisplayWarningPanel("Find Data By ID:" + tMP_Input.text);
                string foundIdkey = searchRoomInput.text;
                UpdateUIForFindRoom(popularChatRoomItemsBYID[foundIdkey].roomdata);
                return;
            }
            else if (popularChatRoomItemsBYNAME.ContainsKey(searchRoomInput.text.ToLower()))
            {string foundNamekey = searchRoomInput.text;
                UpdateUIForFindRoom(popularChatRoomItemsBYID[foundNamekey].roomdata);
               // DataManager.instance.spriteMaganer.DisplayWarningPanel("Find Data By Name:" + tMP_Input.text);
                return;
            }
            else
            {   eventandAllRoomParnet.gameObject.SetActive(false);
                foundChatroomPanel.gameObject.SetActive(false);
                notFoundChatroomPanel.gameObject.SetActive(true);
                notFoundChatroomPanel.transform.SetAsLastSibling();
                //DataManager.instance.spriteMaganer.DisplayWarningPanel("No  Data Find From this Input :" + tMP_Input.text);
                return;
            }
        }
        else
        {
            foundChatroomPanel.gameObject.SetActive(false);
            notFoundChatroomPanel.gameObject.SetActive(false);
            eventandAllRoomParnet.gameObject.SetActive(true);
        }

    }
    void UpdateUIForFindRoom(ChatRoomData chatroomdata)
    {
        foundChatroomPanel.RemovefoundItem();
        eventandAllRoomParnet.gameObject.SetActive(false);
        foundChatroomPanel.gameObject.SetActive(true);
        CloneChatRoomItem(chatroomdata, foundChatroomPanel.foundItemParent, false);
        foundChatroomPanel.UpdateResultText("1");
        foundChatroomPanel.transform.SetAsLastSibling();
        notFoundChatroomPanel.gameObject.SetActive(false);


    }
    void CloseAllpanels()
    {
        searchRoomInput.text = string.Empty;
        eventandAllRoomParnet.gameObject.SetActive(false);
        foundChatroomPanel.gameObject.SetActive(false);
        notFoundChatroomPanel.gameObject.SetActive(false);
    }
   
}
