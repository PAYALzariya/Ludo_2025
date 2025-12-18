
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class FriendsManager : MonoBehaviour
{
    public static FriendsManager instance;

    [Header("All screens objects")]
    public GameObject[] screens;

    public MyFriendButton friendListPrefab, friendRequestListPrefab;
    public Transform fl_Parent, frl_parent;
    

    [Header("player search objects")]
    public TMP_InputField searchText;
    public TextMeshProUGUI friendname;
    public Button addFriendrequest;
    public GameObject searchObjects, requestSearchObject;

    [Header("All Panels")]
    public FriendListPanel friendlistPanel;
    public FriendRequestListPanel friendRequestlistPanel;
    public FriendMassagesListPanel friendMassagesListPanel;
    public FriendSearchPanel friendSearchPanel;
    public List<Toggle> toggleList = new List<Toggle>();
    private void Awake()
    {
        instance = this;
    }


    private void OnEnable()
    {
        OnToggleclick(toggleList[0]);
    for (int i = 0; i < toggleList.Count; i++)
    {
            if(toggleList[i].isOn)
            {
                 toggleList[i].GetComponentInChildren<TMP_Text>().color = Color.white;
            }
            else
            {
                toggleList[i].GetComponentInChildren<TMP_Text>().color = Color.black;
            }
    }
       // onClickFriendsBtn();
    }

    public void ShowScreen(int index)
    {
        for (int i = 0; i < screens.Length; i++)
            screens[i].SetActive(false);

        screens[index].SetActive(true);
    }

    public void OnToggleclick(Toggle toggle)
    {
        Debug.Log("onToggleClick"+toggle.isOn+"::name::"+toggle.name+"child count "+toggle.transform.GetComponentIndex() );
       
        if (toggle.isOn)
        {
            toggle.GetComponentInChildren<TMP_Text>().color = Color.white;
            if (toggle == toggleList[0])
            {
                onClickFriendsBtn();
                return;
            }
            else if (toggle == toggleList[1])
            {
                OnMassagesListButtonclick();
                return;
            }
            else if (toggle == toggleList[2])
            {
                OnRecentGameClick();
                return;

            }
          
        
        }
        else
        {
            toggle.GetComponentInChildren<TMP_Text>().color = Color.black;
        }    
    }

    public void OnClickAddSearchBtn()
    {
        ShowScreen(3);
        requestSearchObject.SetActive(false);
        searchObjects.SetActive(true);
        searchText.text = "";
    }

    public void onClickFriendsBtn()
    {
        Debug.Log("Friend btn click");
        FriendList();
        ShowScreen(0);
    }

    public void OnClickFrendRequestBTN(int index)
    {
        GetAllFriendsListRequest();
        screens[index].SetActive(true);
    }

    public void OnMassagesListButtonclick()
    {
        GetFriendMassageList();
        ShowScreen(1);
    }

    public void OnRecentGameClick()
    {
        ShowScreen(4);
    }

    public void RequestFriendList()
    {
      
        LudoSocketManager.Instance.EmitEvent_With_responseEventHanbleBothCallBack<FriendListResponse>(
    emitEvent: "friend_list_friends",
    responseEvent: "friend_list",
    onAckResponse: (response) =>
    {
        if (response != null && response.success)
        {
            Debug.Log($"Friend list received: {response.data} items");
            HandleFriendList(response);
        } 
        else
        {
            FailedEmitResponse failedApiResponse = JsonUtility.FromJson<FailedEmitResponse>(response.ToString());
            DataManager.instance.spriteMaganer.DisplayWarningPanel(failedApiResponse.error);
            Debug.LogWarning("Failed to get friends list");
        }
    },
    onPushResponse: (response) =>
    {
        if (response != null && response.success)
        {
            Debug.Log($"Friend list received: {response.data} items");
         //   HandleFriendList(response);
        } 
        else
        {
            Debug.LogWarning("Failed to get friends list");
        }
    }
);
     
       

    }
    void HandleFriendList(FriendListResponse response)
    {
        Debug.Log($"Received friend list with {response.data.Length} friends.");
        ShowFriendList(response);
    }
    /* public void OnClickRequestSearchbtn(Get_Search_Friend get_Search_Friend)
     {  //{"id":"04784898-8718-48ed-85d5-ad19d2d4054a","uniqueId":1001,"username":"priyankathummar",
        //"firstName":"priyanka","lastName":"thummar","gender":null,"birthday":null}'
         Debug.Log(get_Search_Friend.username + "  Frend request");
         GetFriend_request(get_Search_Friend.id);

     }*/


    // ---------------- SEARCH FRIEND ----------------
   

    // ---------------- FRIEND LIST ----------------
public    void FriendList()
    {
        Debug.Log("get friend_list_friends emit");
        LudoSocketManager.Instance.EmitEvent_With_responseEventHanbleBothCallBack<FriendListResponse>(
            emitEvent: "friend_list_friends",
            responseEvent: "friend_list",
            onPushResponse: (response) =>
            {
                    Debug.Log($"Friend list received: {response.ToString()} items");
                if (response != null && response.success)
                {
                    ShowFriendList(response);
                }
                else
                {
                    Debug.LogWarning("Failed to get friends list");
                }
            }
        );
    }

    private void ShowFriendList(FriendListResponse FLRdata)
    {
        foreach (Transform item in fl_Parent)
            Destroy(item.gameObject);

        foreach (var f in FLRdata.data)
        {
            MyFriendButton gfl = Instantiate(friendListPrefab, fl_Parent);
            gfl.friendData = f.friend;
            Debug.Log($"Friend: {f.friend.username} ({f.friend.uniqueId}) Added on {f.createdAt}");
        }
    }

    // ---------------- FRIEND REQUESTS ----------------
   public  void GetAllFriendsListRequest()
    {
        LudoSocketManager.Instance.EmitEvent_With_responseEventHanbleBothCallBack<FriendRequestResponse>(
            emitEvent: "friend_list_requests",
            responseEvent: "friend_requests_list",
            onPushResponse: (response) =>
            {
                if (response != null && response.success)
                {
                    Debug.Log($"Friend requests received: {response.data.Length} items");
                    ShowFriendRequestList(response);
                }
                else
                {
                    Debug.LogWarning("Failed to get friend requests");
                }
            }
        );
    }

    private void ShowFriendRequestList(FriendRequestResponse FRRdata)
    {
        foreach (Transform item in frl_parent)
            Destroy(item.gameObject);

        foreach (var req in FRRdata.data)
        {
            MyFriendButton gfr = Instantiate(friendRequestListPrefab, frl_parent);
            gfr.requestData = req;
            Debug.Log($"<color=red>Request from: </color> {req.sender.username} ({req.sender.id}) at {req.createdAt}");
        }
    }

    // ---------------- ERROR HANDLER ----------------
    private void ShowErrorPopup(string message)
    {
         DataManager.instance.spriteMaganer.DisplayWarningPanel(message);
    }

    internal async void GetFriend_request(string receiverId)
    {
        string requestJson = $"{{\"receiverId\":\"{receiverId}\"}}";

        try
        {
            Debug.Log("Sending request to Friend_request...");

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.Friend_request,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: true
            );

            Debug.Log($"send Friend_request '{response}'");

            if (response.IsSuccess)
            {
              
               // StartCoroutine(ErrorText("Request Sent successfully", 2));
                screens[2].SetActive(false);
                requestSearchObject.SetActive(false);
                searchObjects.SetActive(true);
            }
            else
            {
                DataManager.instance.spriteMaganer.DisplayWarningPanel(response.Text);
                Debug.LogError($"SERVER ERROR ({response.StatusCode}): {response.Text}");
                //StartCoroutine(ErrorText(response.Text, 2));
            }
        }
        catch (WebServiceException e)
        {
            Debug.LogError($"REQUEST FAILED ({e.StatusCode}): {e.ErrorMessage}");
              DataManager.instance.spriteMaganer.DisplayWarningPanel(e.ErrorMessage);
            //StartCoroutine(ErrorText(e.ErrorMessage, 2));
        }
    }

    void GetFriendMassageList()
    {
        // TODO: implement message list
    }
}

// ---------------- FRIEND LIST ----------------
[System.Serializable]
public class FriendUser
{
    public string id;
    public int uniqueId;
    public string username;
    public string firstName;
    public string lastName;
    public string gender;
    public string birthday;
}

[System.Serializable]
public class FriendData
{
    public int id;
    public string userId;
    public string friendId;
    public bool deleted;
    public string createdAt;
    public FriendUser friend;
}

[System.Serializable]
public class FriendListResponse
{
    public bool success;
    public long timestamp;
    public FriendData[] data;
}

// ---------------- FRIEND REQUESTS ----------------
[System.Serializable]
public class FriendRequestUser
{
    public string id;
    public int uniqueId;
    public string username;
    public string firstName;
    public string lastName;
    public string gender;
    public string profilePicture;
    public string email;
    public string role;
    public int level;
    public string country;
}

[System.Serializable]
public class FriendRequestData
{
    public int id;
    public string senderId;
    public string receiverId;
    public string status;
    public bool deleted;
    public string createdAt;
    public string respondedAt;
    public FriendRequestUser sender;
}

[System.Serializable]
public class FriendRequestResponse
{
    public bool success;
    public long timestamp;
    public FriendRequestData[] data;
}
//---------------------------------------

// ===== Models =====
[System.Serializable]
public class UserModel
{
    public string id;
    public string username;
    public string uniqueId;
    public string avatar;


    public string coverPicture;


    // Helper method to check if user data is valid
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(username);
    }
    public LoadedSpriteAsset Avatar_ImageAsset;
    public LoadedSpriteAsset ProfilePicture_ImageAsset;
    public LoadedSpriteAsset CoverPicture_ImageAsset;
    public async UniTask LoadAllAssets()
    {
        Avatar_ImageAsset._sourceUrl = avatar;
        Avatar_ImageAsset.SpriteAssset = await DataManager.instance.LoadSprite(Avatar_ImageAsset);

        /* ProfilePicture_ImageAsset._sourceUrl = profilePicture;
         ProfilePicture_ImageAsset.SpriteAssset = await DataManager.instance.LoadSprite(ProfilePicture_ImageAsset);*/

        CoverPicture_ImageAsset._sourceUrl = coverPicture;
        CoverPicture_ImageAsset.SpriteAssset = await DataManager.instance.LoadSprite(CoverPicture_ImageAsset);

    }
}

[System.Serializable]
public class FriendRequestModel
{
    public int id;
    public string senderId;
    public string receiverId;
    public string status;
    public bool deleted;
    public DateTime createdAt;
    public object respondedAt;
    public UserModel sender;

}

[System.Serializable]
public class FriendshipModel
{
    public int id;
    public string userId;
    public string friendId;
    public UserModel friend;
    public string createdAt;
}

[System.Serializable]
public class ErrorResponse
{
    public string error;
}

[System.Serializable]
public class FriendRequestListWrapper
{
    public FriendRequestModel[] items;
}

[System.Serializable]
public class FriendshipListWrapper
{
    public FriendshipModel[] items;
}
[System.Serializable]
public class SearchUserPayload
{
    public string userId;
}

// Backend response format for user search
[System.Serializable]
public class BackendResponse
{
    public bool success;

    public string error;

    public UserModel data;  // Directly typed as UserModel for user search
    public long timestamp;  // Unix timestamp from server

}

// Generic backend response format (for events that return different data types)
[System.Serializable]
public class GenericBackendResponse
{
    public bool success;
    public string error;
    public object data;  // Will be parsed based on context
    public long timestamp;
}

[System.Serializable]
public class Sender
{
    public string id;
    public int uniqueId;
    public string username;
    public string firstName;
    public string lastName;
    public object gender;
    public object profilePicture;
    public object coverPicture;
    public string email;
    public string role;
}

