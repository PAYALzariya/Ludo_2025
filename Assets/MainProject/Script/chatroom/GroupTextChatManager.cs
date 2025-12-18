using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class GroupTextChatManager : MonoBehaviour
{
    public ScrollRect ChatTextMassagesParent;
    public GroupTextChatItem prefab_groupTextChatItem;
    public GiftTextChatItem giftChatTextChatItem;
    public OnlyTextMsgItem prefab_onlyTextMsgItem;
    public NewUserEnter_MsgItem prefab_newUserEnter_MsgItem;
    public List<GameObject> AllTextMassageList = new List<GameObject>();
    public TMP_InputField massageInputField;
    void UpdateScrollviewcontentscrollbar()
    {
        ChatTextMassagesParent.verticalScrollbar.value = 1;

    }
    internal void AddNewTextChatMessage(GrouptextchatData txtData, bool IsownMsg)
    {
        GroupTextChatItem newTextChatItem = Instantiate(prefab_groupTextChatItem, ChatTextMassagesParent.content);
        newTextChatItem.transform.position = ChatTextMassagesParent.content. position;
        newTextChatItem.transform.localScale = Vector3.one;
        if (IsownMsg)
        {
            newTextChatItem.SetOwnChatmassage(txtData.message);

        }
        else
        {
            newTextChatItem.SetOthersGroupTextChatItem(txtData);


        }

        AllTextMassageList.Add(newTextChatItem.gameObject);
        UpdateScrollviewcontentscrollbar();
        
    }
    internal void AddOnlyTextMessage(string message)
    {
        OnlyTextMsgItem newTextChatItem = Instantiate(prefab_onlyTextMsgItem, ChatTextMassagesParent.content);
        newTextChatItem.transform.position = ChatTextMassagesParent.content.position;
        newTextChatItem.transform.localScale = Vector3.one;
        AllTextMassageList.Add(newTextChatItem.gameObject);
        newTextChatItem.DisplayMsg(message);
           UpdateScrollviewcontentscrollbar();
    }
    internal void AddNewGiftMessage(GiftEntry response,bool isShownMSG)
    {
        GiftTextChatItem newTextChatItem = Instantiate(giftChatTextChatItem, ChatTextMassagesParent.content);
        newTextChatItem.transform.position = ChatTextMassagesParent.content.position;
        newTextChatItem.transform.localScale = Vector3.one;
          
         UpdateScrollviewcontentscrollbar();
       
        newTextChatItem.GiftSetChatMessage(response);

        AllTextMassageList.Add(newTextChatItem.gameObject);
      
    }
    internal void AddNewUserEnterMessage(string userName)
    {
        NewUserEnter_MsgItem newTextChatItem = Instantiate(prefab_newUserEnter_MsgItem, ChatTextMassagesParent.content);
        newTextChatItem.transform.position = ChatTextMassagesParent.content.position;
        newTextChatItem.transform.localScale = Vector3.one;
        newTextChatItem.DisplayMsg(userName);
        AllTextMassageList.Add(newTextChatItem.gameObject);

    }
    internal void ClearAllMessages()
    {
        foreach (GameObject msg in AllTextMassageList)
        {
            Destroy(msg);
        }
        AllTextMassageList.Clear();
    }

    
    internal void OnNewUserEnterGroupMessage(string userName)
    {
        AddNewUserEnterMessage(userName);
    }
    internal void AddCommonAnucementMessage(string message)
    {
        AddOnlyTextMessage(message);
    }
    public void OnMassageInputFieldEndEdit()
    {
        if(!string.IsNullOrEmpty(massageInputField.text))
        {
            SendMassageEmit(massageInputField.text, HomePanel.instance.chatManager.
            CurrentRoom.data.room.id.ToString());
            massageInputField.text = string.Empty;
        }
    }
    //============================================================================SocktFunctions===========================================================================================
    internal void SendMassageEmit(string Msg,string roomid)
    {// roomId: '123', message: 'Hello everyone!',
        var data = new Dictionary<string, object> { { "roomId", roomid },{"message" , Msg } };
        LudoSocketManager.Instance.EmitWithAck<GrouptextchatResponse>(GameConstants.EmitEvents.SendGroupTextmessage,
         payloadData: data, onAckResponse: (response) =>
         {
             if (response != null && response.success)
             {
                
                 Debug.Log($" onAckResponse SendGroupTextmessage received: {response.data.message} items");
             }
             else
             {
                 Debug.LogWarning("Failed to get SendGroupTextmessage ");
             }
         }
      );


    }
}
