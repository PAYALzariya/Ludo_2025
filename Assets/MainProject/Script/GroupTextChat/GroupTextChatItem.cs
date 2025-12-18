using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroupTextChatItem : MonoBehaviour
{
    public Image userProfileImage;
    public TMP_Text userName;
    public TMP_Text message;
    public TMP_Text Level;
    public Image levelImage;


    public void SetOthersGroupTextChatItem(GrouptextchatData otherchatdata)
    {
        userName.text = otherchatdata.sender.username;
        message.text = otherchatdata.message.message;
        userProfileImage.sprite = otherchatdata.sender.profilePictureAsset.SpriteAssset;
        Level.text = "lv." + otherchatdata.sender.level.ToString();
        levelImage.sprite = otherchatdata.sender.levelData.levelSprite;
    }
    public void SetOwnChatmassage(GrouptxtmassageData massagedata)
    {
        userName.text = HomePanel.instance.chatManager.OwnProfileData.username;
        message.text = massagedata.message;
        userProfileImage.sprite = HomePanel.instance.chatManager.OwnProfileData.profilePictureAsset.SpriteAssset;
        Level.text = "lv." + HomePanel.instance.chatManager.OwnProfileData.level.ToString();
        levelImage.sprite = HomePanel.instance.chatManager.OwnProfileData.levelData.levelSprite;
    }
}
