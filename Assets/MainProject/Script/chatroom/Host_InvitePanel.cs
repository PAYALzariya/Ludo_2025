using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Host_InvitePanel : MonoBehaviour
{
    public TMP_InputField searchbar;
    public Transform SearchITemParent;
    public Dictionary<string, CommonUserProfileItem> OnlineuserToInivitList = new Dictionary<string, CommonUserProfileItem>();
   // public List<CommonUserProfileItem> searchList = new List<CommonUserProfileItem>();
    void OnEnable()
    {
       
        
        AddSearchList();
    }
    public void OnSearchFind()
    {
        if (!string.IsNullOrEmpty(searchbar.text))
        { Debug.Log("searchbar::" + searchbar.text+"::itemclone::");
            foreach (var useritem in OnlineuserToInivitList)
            {
                if (useritem.Key == searchbar.text)
                {
                    Debug.Log("find:::" + searchbar.text);
                    
                    useritem.Value.gameObject.SetActive(true);
                }
                else
                {
                    useritem.Value.gameObject.SetActive(false);
                }
                Debug.Log("item::" + useritem.Key);
            }

        }
        else
        {
            foreach (var useritem in OnlineuserToInivitList)
            {
                useritem.Value.gameObject.SetActive(true);
            }
        }
    }
    void CloneAndDisplaySearchItem(ChatParticipantData searchitem)
    { 
        Debug.Log("CloneAndDisplaySearchItem::" + searchbar.text);
        CommonUserProfileItem item =HomePanel.instance.chatManager.chatPanel.CloneOnlineUserItem(searchitem);
        item.transform.SetParent(SearchITemParent);
        item.transform.position = SearchITemParent.position;
        item.transform.localScale = Vector3.one;
        item.button.onClick.AddListener(() => onAddButtonClick(item));
        item.button.gameObject.SetActive(true);
        item.buttontext.text = "Add";
        item.gameObject.SetActive(true);
        item.SetUserProfile();
        OnlineuserToInivitList.Add(item.participantData.id, item);
    }
    void onAddButtonClick(CommonUserProfileItem item)
    {
        HomePanel.instance.chatManager.inviteManager.InviteUSer(HomePanel.instance.chatManager.CurrentRoom.data.room.id
        , HomePanel.instance.chatManager.chatPanel.Selectdseatitem.seatNo.ToString(), item.participantData.id);
    }
    internal void AddSearchList()
    {
        foreach (var useritem in OnlineuserToInivitList)
        {
            Destroy(useritem.Value.gameObject);
        }
        OnlineuserToInivitList.Clear();
        Debug.Log("AddSearchList " + OnlineuserToInivitList.Count);
       
        if (OnlineuserToInivitList == null)
        {

            OnlineuserToInivitList = new Dictionary<string, CommonUserProfileItem>();
        }
        else
        {
           
          
        }
        Debug.Log("ChatParticipantDataList " + HomePanel.instance.chatManager.ChatParticipantDataList.Count);

        foreach (var useritem in HomePanel.instance.chatManager.ChatParticipantDataList)
        {
            string userId = useritem.Key;
            if (!OnlineuserToInivitList.ContainsKey(userId))
            {
                CloneAndDisplaySearchItem(useritem.Value);
            }
            
           
            Debug.Log("userId:" + useritem.Key);

        }
    }
    
}
