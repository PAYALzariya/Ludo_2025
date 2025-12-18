using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FriendRequest : MonoBehaviour
{
    public TextMeshProUGUI r_name, r_Id;
    public FriendRequestData requestUser; // comes from server

    void Start()
    {
        r_name.text = requestUser.sender.username;
        r_Id.text = requestUser.sender.id.ToString();
      //  r_email.text = requestUser.email;
    }

    public void OnClickAccept()
    {
        Debug.Log($"Accepting friend request from {requestUser.id}");
        // TODO: call FriendsManager.instance.AcceptFriendRequest(requestUser.id);
        var payload = new Dictionary<string, object>
        {
            { "requestId", requestUser.id },
            { "action", "accept" }
        };
        LudoSocketManager.Instance.EmitEvent_With_responseEventHanbleBothCallBack<FriendRequestAcceptResponse>(
             emitEvent: "friend_respond_request",
             responseEvent: "friend_request_responded",
             payloadData: payload,
             onPushResponse: (response) =>
             {
                 if (response != null && response.success)
                 {
                     Debug.Log($"Friend list received: {response.data} items");
                     //      OnSuccessFunction(response);
                    // FriendsManager.instance.FriendList();
                     //FriendsManager.instance.GetAllFriendsListRequest();
                     //   ShowFriendList(response);
                 }
                 else
                 {
                     Debug.LogWarning("Failed to get friends list");
                 }
             }
         );
    }

    public void OnClickDecline()
    {
        Debug.Log($"Declining friend request from {requestUser.sender.username}");
        var payload = new Dictionary<string, object>
        {
            { "requestId", requestUser.id },
            { "action", "reject" }
        };
        LudoSocketManager.Instance.EmitEvent_With_responseEventHanbleBothCallBack<FriendRequestAcceptResponse>(
             emitEvent: "friend_respond_request",
             responseEvent: "friend_request_responded",
             payloadData: payload,
             onPushResponse: (response) =>
             {
                 if (response != null && response.success)
                 {
                     Debug.Log($"Friend list received: {response.data} items");
                     //      OnSuccessFunction(response);
                     FriendsManager.instance.FriendList();
                     FriendsManager.instance.GetAllFriendsListRequest();
                     //   ShowFriendList(response);
                 }
                 else
                 {
                     Debug.LogWarning("Failed to get friends list");
                 }
             }
         );
    }
}
[System.Serializable]
public class FriendRequestAcceptResponse
{
    public bool success;
    public long timestamp;
    public FriendRequestAcceptData data;
}

[System.Serializable]
public class FriendRequestAcceptData
{
    public FriendRequestDetail request;
    public FriendshipDetail friendship;
    public FriendProfile friend;
}

// ------------------- Request Info -------------------
[System.Serializable]
public class FriendRequestDetail
{
    public int id;
    public string senderId;
    public string receiverId;
    public string status;        // "accepted", "pending", etc.
    public bool deleted;
    public string createdAt;
    public string respondedAt;
}

// ------------------- Friendship Info -------------------
[System.Serializable]
public class FriendshipDetail
{
    public int id;
    public string userId;
    public string friendId;
    public bool deleted;
    public string createdAt;
    public FriendProfile friend;   // nested friend object
}

// ------------------- Friend Profile -------------------
[System.Serializable]
public class FriendProfile
{
    public string id;
    public string username;
    public string email;
    public string phone;
    public string passwordHash;
    public string firstName;
    public string lastName;
    public string gender;
    public string bio;
    public string birthday;
    public bool birthdayEditable;
    public string profilePicture;
    public string coverPicture;
    public bool isActive;
    public string role;
    public string provider;
    public string providerId;
    public int uniqueId;
    public string customId;
    public int level;
    public string country;
    public bool deleted;
    public string createdAt;
    public string updatedAt;
}
