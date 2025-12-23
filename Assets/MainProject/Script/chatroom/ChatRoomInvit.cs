using NUnit.Framework;
using TMPro;

using UnityEngine;
using UnityEngine.UI;
public class SeatedUser
{
    public string userId;
    public string userName;
    public Sprite userProfilePic;
    public Sprite userFramePic;
}

public class ChatRoomInvit : MonoBehaviour
{
    public Image centerIcon;
    public UserProfilePicWithFrame userProfile;
    public Image sideIcon;
    public TMP_Text labelText;
    public Button button;
    public bool isSeated;
    public bool IsSeatLocked;
    public int seatNo;
    public SeatedUser seatedUser;

internal void SetSeatedUser(SeatedUser userDeatils)
    {
        seatedUser = userDeatils;
        isSeated = true;
        IsSeatLocked = false;      
        labelText.text = userDeatils.userName;
        button.interactable = false;
        userProfile.profilePic.sprite = userDeatils.userProfilePic;
        userProfile.frameImage.sprite = userDeatils.userFramePic;
        centerIcon.gameObject.SetActive(false);     
        userProfile.gameObject.SetActive(true);
        
    }
internal void ClearSeat()
    {
        isSeated = false;
        IsSeatLocked = false;
        seatedUser = null;
        button.interactable = true;
        CleruserProfile();
       centerIcon.gameObject.SetActive(true);
       userProfile.gameObject.SetActive(false);
       sideIcon.gameObject.SetActive(false);    
       centerIcon.sprite= DataManager.instance.spriteMaganer.unlocksprite;
       labelText.text = seatNo.ToString();

       
    }
    internal void SetSeatAsLocked()
    {
        Debug.Log("lockmic");
        IsSeatLocked = true;
        button.interactable = false;
        centerIcon.gameObject.SetActive(true);
        centerIcon.sprite =DataManager.instance.spriteMaganer.Locksprite;
        CleruserProfile();

    }
internal void SetSeatAsUnlocked()
    {
        IsSeatLocked = false;
        button.interactable = true;
        centerIcon.gameObject.SetActive(true);
        centerIcon.sprite =DataManager.instance.spriteMaganer.unlocksprite;
        CleruserProfile();
    }
    void CleruserProfile()
    {
        userProfile.profilePic = null;
        userProfile.frameImage = null;  
    }


}
