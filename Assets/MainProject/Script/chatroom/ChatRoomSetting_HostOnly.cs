

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatRoomSetting_HostOnly : MonoBehaviour
{
    public RawImage roomProfile;
    public TMP_Text roomIdText;
    public TMP_InputField roomNameInput;
    public TMP_InputField roomNoticeInput;
    public List<Profiles> onlineMembers = new List<Profiles>();
    public Profiles memberPrefab;
    public Transform onlineMembersParent;
    public string pickImagePath;
    public TMP_Text txtWheatMode;
    public TMP_Text txtMicMode;
    public TMP_Text txtRoomType;
   
    public RoomMusicPanel roommusicPanel;
    public List<GameObject>allpanels=new List<GameObject>();
    internal void RestData()
    {



    }
   
    public void OnMusicClickButton()
    {

        roommusicPanel.gameObject.SetActive(true);
        roommusicPanel.transform.SetAsLastSibling();

    }
    
    public void OnUpdateProfileImageButtonClick()
    {
     // HomePanel.instance.chatManager.OnChooseImageButtonClicked(roomProfile,pickImagePath);
      HomePanel.instance.chatManager.OnChooseImageButtonClicked(roomProfile);
    }
   
    internal void DisplayMember()
    {
        Debug.Log("Display Member " + HomePanel.instance.chatManager.ChatParticipantDataList.Count);
        foreach (var member in HomePanel.instance.chatManager.ChatParticipantDataList)
        {
            ChatParticipantData chatParticipantData = member.Value;
            Debug.Log("Member Name::"+chatParticipantData.username+ onlineMembers.Count);
            if (onlineMembers.Count == 0)
            {
                CloneOnlineMember((chatParticipantData));
               
            }
            else
            {


                if (onlineMembers.Exists(x => x.userId != chatParticipantData.id))
                {
                    CloneOnlineMember((chatParticipantData));
                }
                else
                {
                    Debug.Log("Member already exists::" + chatParticipantData.username);
                }
            }
             
           
        }
    }
    void CloneOnlineMember(ChatParticipantData chatParticipantData)
    {
        Profiles profile = Instantiate(memberPrefab);
        profile.transform.SetParent(onlineMembersParent);
        profile.transform.localScale = Vector3.one;
        profile.transform.position = onlineMembersParent.position;
        profile.nameText.text = chatParticipantData.username;
        profile.userId = chatParticipantData.id;
        profile.Frame.sprite = chatParticipantData.frameData.frameSprite;
        profile.image.sprite = chatParticipantData.profilePictureAsset.SpriteAssset;
        profile.gameObject.SetActive(true);
        onlineMembers.Add(profile);
    }
    internal void SetRoomSettingsData(ChatRoomData roomData)
    {
        Debug.Log("Room Data::" + roomData.name);
        roomIdText.text = "<b>Room Code:</b>" + roomData.roomCode.ToString();
        roomNameInput.text = roomData.name;
        roomNoticeInput.text = roomData.description;
        roomProfile.texture = roomData.roomImageAsset.SpriteAssset.texture;

        DisplayMember();
    }
    void CloseAllPanel()
    {
       foreach(var panel in allpanels)
        {
            panel.SetActive(false);
        }
    }
    void OpenPanel(int index)
    {
        allpanels[index].SetActive(true);
        allpanels[index].transform.SetAsLastSibling();
    }
    public void OnOptionClick(string optionname)
    {
        CloseAllPanel();
        switch (optionname.ToLower())
            {
            case "micmode":
                OpenPanel(0);
                Debug.Log("micmode Clicked");
                
                break;
            case "roomtype":
                OpenPanel(1);
                Debug.Log("roomtype Clicked");
                break;
            case "wheatmode":
                OpenPanel(2);
                Debug.Log("wheatmode Clicked");
                break;
            case "supermode":
                OpenPanel(3);
                Debug.Log("supper mode");
                break;
            case "publicscreen":
               // allpanels[4].SetActive(true);
                Debug.Log("publicscreen ");
                break;
            case "senstivescreen":
                OpenPanel(4);
                Debug.Log("senstivescreen ");
                break;
            case "effectswitch":
                OpenPanel(5);
                Debug.Log("effectswitch mode");
                break;


        }
    }
}