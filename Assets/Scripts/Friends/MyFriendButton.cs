using UnityEngine;
using TMPro;

public class MyFriendButton : MonoBehaviour
{
    public TextMeshProUGUI f_name;

    // The full FriendData object from the server
    public FriendUser friendData;
    public UserProfilePicWithFrame picWithFrame;
    public CountryInfo countryInfo;
    public FriendRequestData requestData;

    void Start()
    {
        //  if (friendData != null && friendData. != null)
        {
            f_name.text = friendData.username;
        }
    }

    public void OnClickDeleteFriend()
    {
        if (friendData != null)
        {
            //  Debug.Log($"Delete friend {friendData.friend?.username}");
            // FriendsManager.instance.GetFriend_unfriend(friendData.friend.id);
        }
    }

    public void SetProfile(Country countrydata )
    {
        countryInfo.countrynameTxt.text = countrydata.countryname;
        countryInfo.flagimg.sprite = countrydata.flagSprite;
        

    }
}

// Optional extension/helper
public static class FriendDataExtensions
{
    public static int friendLevel(this FriendData fd)
    {
        // Example: if you later add 'level' in FriendUser, return it safely
        return (fd.friend != null) ? fd.friend.uniqueId % 100 : 0;
    }
}
