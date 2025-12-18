

using System;
using TMPro;

using UnityEngine;
using UnityEngine.UI;


public class GiftTextChatItem : MonoBehaviour
{
    public Image userProfileImage;
    public TMP_Text userName;
    public TMP_Text message;
    public TMP_Text Level, Quantity;
    public Image levelImage, giftImage;
   
    /// <summary>
    /// When I send a gift (also Own message)
    /// </summary>
    public void GiftSetChatMessage(GiftEntry response)
    {
        // if (ChatManager.instance.OwnProfileData.id == response.senderId)
        {
            userName.text = response.senderName;
            Debug.Log("<color=red>Call OWN ....</color>"+response);

            if (GiftPanelManager.Instance.GiftDataList.ContainsKey(response.giftId))
            {
                var giftData = GiftPanelManager.Instance.GiftDataList[response.giftId];
                giftImage.sprite = giftData.giftItem.giftePictureAsset.SpriteAssset;
                Quantity.text = giftData.giftItem.quantity;
              
            }
            if (ChatManager.instance.ChatParticipantDataList.TryGetValue(response.senderId, out var senderData))
            {
                userProfileImage.sprite = senderData.profilePictureAsset.SpriteAssset;
                Level.text = "lv." + senderData.level.ToString();
                levelImage.sprite = senderData.levelData.levelSprite;
            }
            else if (ChatManager.instance.OwnProfileData.id == response.senderId)
            {
                userProfileImage.sprite = ChatManager.instance.OwnProfileData.profilePictureAsset.SpriteAssset;
                Level.text = "lv." + ChatManager.instance.OwnProfileData.level.ToString();
                levelImage.sprite = ChatManager.instance.OwnProfileData.levelData.levelSprite;
            }else if(ChatManager.instance.CurrentRoom.data.host.id == response.senderId)
            {
                userProfileImage.sprite = ChatManager.instance.CurrentRoom.data.host.profilePictureAsset.SpriteAssset;
                Level.text = "lv." + ChatManager.instance.CurrentRoom.data.host.level.ToString();
                levelImage.sprite = ChatManager.instance.CurrentRoom.data.host.levelData.levelSprite;
            }

            // Get receiver username
            if (ChatManager.instance.ChatParticipantDataList.TryGetValue(response.receiverId, out var receiverData))
            {
                Debug.Log("<color=green>Response gift_sent_in_room </color>" + receiverData.username);

                message.text = "Send " + receiverData.username + " " + response.giftName;
                GiftPanelManager.Instance.allReciversName += receiverData.username + " ";
            }
            else if (ChatManager.instance.OwnProfileData.id == response.receiverId)
            {
                Debug.Log("<color=green>Response gift_sent_in_room </color>" + ChatManager.instance.OwnProfileData.username);
                message.text = "Send " + ChatManager.instance.OwnProfileData.username + " " + response.giftName;
                GiftPanelManager.Instance.allReciversName += receiverData.username + " ";
            }
           
        }
    }


   
}

[Serializable]
public class GiftReceivedResponse
{
    public string senderId;
    public string receiverId;
    public string giftId;
    public int quantity;
    public string roomId;
    public int id;
    public DateTime sentAt;
    public string senderName;
    public int senderLevel;
    public string giftName;
    public int giftCategoryId;
    public string giftCategoryName;
}



