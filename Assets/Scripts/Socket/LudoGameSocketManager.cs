using BestHTTP.JSON;
using BestHTTP.SocketIO;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;


public class LudoGameSocketManager : MonoBehaviour
{

    // The main Socket.IO manager instance from BestHTTP
    private SocketManager manager;

    // Active socket used for communication
    private Socket socket;

    // Tracks if the socket is currently connected
    private static bool _socketConnected = false;

    // Singleton instance for easy access
    public static LudoGameSocketManager instance;

    // Socket server URL (set in Inspector)
 //   public string Socket_URL;

    // JWT or access token used for authentication
   // public string AccessToken, userID;

    // Public property for connection state
    public static bool IsSocketConnected
    {
        set { _socketConnected = value; }
        get { return _socketConnected; }
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ConnectWithAuthObject();
    }
    void OnDestroy()
    {
        if (manager != null)
        {
            Debug.Log(" Closing socket...");
            manager.Close();
        }
    }
    // Generic helper method to parse Socket.IO responses
    public T ParseSocketResponse<T>(object responseObj) where T : class
    {
        try
        {
            // If it's already a Dictionary, convert to JSON first
            if (responseObj is Dictionary<string, object> dict)
            {
                string json = Json.Encode(dict);
                Debug.Log(json);
                return JsonUtility.FromJson<T>(json);
            }
            // If it's a string, parse directly
            else if (responseObj is string jsonStr && !jsonStr.Contains("System.Collections.Generic.Dictionary"))
            {
                Debug.Log(jsonStr);
                return JsonUtility.FromJson<T>(jsonStr);
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse response as {typeof(T).Name}: {ex.Message}");
            return null;
        }
    }
    void ConnectWithAuthObject()
    {
        var options = new SocketOptions();

        // Send token as auth object
        options.AdditionalQueryParams = new PlatformSupport.Collections.ObjectModel.ObservableDictionary<string, string>
        {
            { "auth[token]",DataManager.instance.AccessToken} // Server reads "auth.token"
        };

        manager = new SocketManager(new Uri(GameConstants.Socket_URL), options);
        socket = manager.GetSocket();
        Debug.Log(manager.Uri);
        SetupFriendEventHandlers();
        manager.Open();
    }


    private void SetupFriendEventHandlers()
    {
        socket.On(SocketIOEventTypes.Connect, OnConnect);
/*
        // ===== FRIEND SEARCH EVENTS =====
        // Handle separate event emissions from backend (alternative to callbacks)
        socket.On("friend_search_result", (s, p, args) =>
        {
            Debug.Log("[EVENT] friend_search_result received");
            if (args != null && args.Length > 0 && args[0] != null)
            {
                Debug.Log($"[EVENT] Raw response type: {args[0].GetType().Name}");

                var response = ParseSocketResponse<BackendResponse>(args[0]);

                if (response != null)
                {
                    if (response.success && response.data != null && response.data.IsValid())
                    {
                        Debug.Log($"✅ [EVENT] User found: {response.data} ");
                        Debug.Log($"✅ [EVENT] User found: {response.data.username} (ID: {response.data.uniqueId})");
                        HandleUserFound(response.data);
                    }
                    else if (!response.success)
                    {
                        string errorMsg = response.error ?? "Unknown error";
                        Debug.LogError($"❌ [EVENT] Search failed: {errorMsg}");
                        HandleUserNotFound(errorMsg);
                    }
                    else
                    {
                        Debug.LogWarning("[EVENT] Response success but invalid user data");
                        HandleUserNotFound("Invalid user data received");
                    }
                }
                else
                {
                    Debug.LogError($"❌ [EVENT] Failed to parse response. Type: {args[0].GetType().Name}");
                    HandleUserNotFound("Failed to parse user data from event");
                }
            }
            else
            {
                Debug.LogWarning("❌ [EVENT] Empty friend_search_result data");
            }
        });

        socket.On("friend_search_error", (s, p, args) =>
        {
            Debug.Log("[EVENT] friend_search_error received");
            if (args != null && args.Length > 0 && args[0] != null)
            {
                Debug.Log($"[EVENT ERROR] Raw response type: {args[0].GetType().Name}");

                var response = ParseSocketResponse<BackendResponse>(args[0]);

                if (response != null && !string.IsNullOrEmpty(response.error))
                {
                    Debug.LogError($"❌ [EVENT] Search error: {response.error}");
                    HandleUserNotFound(response.error);
                }
                else
                {
                    // Fallback for legacy error format
                    string errorMsg = args[0].ToString();
                    if (errorMsg.Contains("System.Collections.Generic.Dictionary"))
                    {
                        errorMsg = "Server response parsing error";
                    }
                    Debug.LogError($"❌ [EVENT] Search error (fallback): {errorMsg}");
                    HandleUserNotFound(errorMsg);
                }
            }
            else
            {
                Debug.LogWarning("❌ [EVENT] Empty friend_search_error data");
                HandleUserNotFound("Unknown search error");
            }
        });
*/
        // ===== FRIEND REQUEST EVENTS =====
        socket.On("friend_request_received", (s, p, args) =>
        {
            Debug.Log("[EVENT] friend_request_received");
            if (args != null && args.Length > 0 && args[0] != null)
            {
                Debug.Log($"[EVENT] Raw response type: {args[0].GetType().Name}");

                var response = ParseSocketResponse<GenericBackendResponse>(args[0]);
                if (response != null && response.success && response.data != null)
                {
                    try
                    {
                        string requestJson = JsonUtility.ToJson(response.data);
                        var request = JsonUtility.FromJson<FriendRequestModel>(requestJson);

                        Debug.Log($"📩 New friend request from: {request.sender?.username}");
                        HandleFriendRequestReceived(request);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"❌ Failed to parse friend request data: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogError("❌ Failed to parse friend request from event");
                }
            }
        });

        socket.On("friend_request_accepted", (s, p, args) =>
        {
            Debug.Log("[EVENT] friend_request_accepted");
            if (args != null && args.Length > 0 && args[0] != null)
            {
                try
                {
                    string json = args[0].ToString();
                    var friendship = JsonUtility.FromJson<FriendshipModel>(json);
                    Debug.Log($"🎉 Friend request accepted! New friend: {friendship.friend?.username}");

                    // Handle friend request acceptance
                    HandleFriendRequestAccepted(friendship);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"❌ Failed to parse friendship data: {ex.Message}");
                }
            }
        });

        socket.On("friend_unfriended_by", (s, p, args) =>
        {
            Debug.Log("[EVENT] friend_unfriended_by");
            if (args != null && args.Length > 0 && args[0] != null)
            {
                try
                {
                    string json = args[0].ToString();
                    Debug.Log($"💔 You were unfriended: {json}");

                    // Handle being unfriended
                    HandleUnfriendedBy(json);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"❌ Failed to parse unfriend data: {ex.Message}");
                }
            }
        });

        // ===== ADDITIONAL BACKEND EVENT LISTENERS =====

        socket.On("friend_request_sent", (s, p, args) =>
        {
            Debug.Log("[EVENT] friend_request_sent");
            if (args != null && args.Length > 0 && args[0] != null)
            {
                var response = ParseSocketResponse<GenericBackendResponse>(args[0]);
                if (response != null && response.success)
                {
                    Debug.Log("✅ Friend request sent successfully");
                }
                else
                {
                    Debug.LogError($"❌ Friend request send failed: {response?.error}");
                }
            }
        });

        socket.On("friend_request_error", (s, p, args) =>
        {
            Debug.Log("[EVENT] friend_request_error");
            if (args != null && args.Length > 0 && args[0] != null)
            {
                var response = ParseSocketResponse<GenericBackendResponse>(args[0]);
                string errorMsg = response?.error ?? args[0].ToString();
                Debug.LogError($"❌ Friend request error: {errorMsg}");
            }
        });

        socket.On("friend_requests_list", (s, p, args) =>
        {
            Debug.Log("[EVENT] friend_requests_list");
            if (args != null && args.Length > 0 && args[0] != null)
            {
                var response = ParseSocketResponse<GenericBackendResponse>(args[0]);
                if (response != null && response.success)
                {
                    Debug.Log($"📋 Friend requests list received successfully");
                    // TODO: Parse and handle friend requests list from response.data
                }
                else
                {
                    Debug.LogError($"❌ Failed to get friend requests list: {response?.error}");
                }
            }
        });

        socket.On("friend_list", (s, p, args) =>
        {
            Debug.Log("[EVENT] friend_list");
            if (args != null && args.Length > 0 && args[0] != null)
            {
                var response = ParseSocketResponse<GenericBackendResponse>(args[0]);
                if (response != null && response.success)
                {
                    Debug.Log($"👥 Friends list received successfully");
                    // TODO: Parse and handle friends list from response.data
                }
                else
                {
                    Debug.LogError($"❌ Failed to get friends list: {response?.error}");
                }
            }
        });

        socket.On("friend_request_responded", (s, p, args) =>
        {
            Debug.Log("[EVENT] friend_request_responded");
            if (args != null && args.Length > 0 && args[0] != null)
            {
                var response = ParseSocketResponse<GenericBackendResponse>(args[0]);
                if (response != null && response.success)
                {
                    Debug.Log($"📝 Friend request response handled successfully");
                    // TODO: Handle friend request response from response.data
                }
                else
                {
                    Debug.LogError($"❌ Friend request response failed: {response?.error}");
                }
            }
        });

        socket.On("friend_request_deleted", (s, p, args) =>
        {
            Debug.Log("[EVENT] friend_request_deleted");
            if (args != null && args.Length > 0 && args[0] != null)
            {
                var response = ParseSocketResponse<GenericBackendResponse>(args[0]);
                if (response != null && response.success)
                {
                    Debug.Log($"🗑️ Friend request deleted successfully");
                    // TODO: Handle friend request deletion from response.data
                }
                else
                {
                    Debug.LogError($"❌ Friend request deletion failed: {response?.error}");
                }
            }
        });

        socket.On("friend_unfriended", (s, p, args) =>
        {
            Debug.Log("[EVENT] friend_unfriended");
            if (args != null && args.Length > 0 && args[0] != null)
            {
                var response = ParseSocketResponse<GenericBackendResponse>(args[0]);
                if (response != null && response.success)
                {
                    Debug.Log($"💔 Successfully unfriended user");
                    // TODO: Handle unfriend confirmation from response.data
                }
                else
                {
                    Debug.LogError($"❌ Unfriend operation failed: {response?.error}");
                }
            }
        });
              // Debug: Catch-all listener for ANY socket event
        socket.On(SocketIOEventTypes.Event, (s, p, args) =>
        {
            string eventName = p.EventName;
            string data = (args != null && args.Length > 0 && args[0] != null)
                ? args[0].ToString()
                : "no args";

            Debug.Log($"[SOCKET DEBUG] Event: {eventName} | Data: {data}");
        });
    }
     #region onMethodResponse
    // ===== FRIEND REQUEST EVENT HANDLERS =====
    private void HandleFriendRequestReceived(FriendRequestModel request)
    {
        Debug.Log($"📩 Processing incoming friend request from: {request.sender?.username}");
        // TODO: Show friend request notification to user
        // TODO: Update friend requests UI
        // TODO: Play notification sound, etc.
    }

    private void HandleFriendRequestAccepted(FriendshipModel friendship)
    {
        Debug.Log($"🎉 Processing accepted friend request. New friend: {friendship.friend?.username}");
        // TODO: Update friends list UI
        // TODO: Show success message
        // TODO: Refresh friends data
    }

    private void HandleUnfriendedBy(string unfriendData)
    {
        Debug.Log($"💔 Processing unfriend event: {unfriendData}");
        // TODO: Remove friend from UI
        // TODO: Show notification that you were unfriended
        // TODO: Refresh friends list
    }
    #endregion
    private void OnConnect(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("We are connected..........: {socket.Manager.Uri}");

        IsSocketConnected = true;     

    }
    //  == Emit Methods  ==
    // ======================
    // Final (with callbacks)
    // ======================
   /* public void SearchUser(string userId, Action<UserModel> onSuccess, Action<string> onError = null)
    {
        if (!IsSocketConnected)
        {
            Debug.LogWarning("Socket not connected. Cannot search user.");
            onError?.Invoke("Socket not connected");
            return;
        }

        var payload = new Dictionary<string, object> { { "userId", userId } };

        Debug.Log($"[EMIT] Searching for user: {userId}");

        socket.Emit(
            "friend_search_user",
            (Socket s, Packet p, object[] ackArgs) =>
            {
                if (ackArgs != null && ackArgs.Length > 0 && ackArgs[0] != null)
                {
                    var response = ParseSocketResponse<SearchResponse>(ackArgs[0]);

                    if (response != null && response.success && response.data != null && response.data.IsValid())
                    {
                        Debug.Log($"✅ [CALLBACK] User found: {response.data.username} (ID: {response.data.uniqueId})");
                        onSuccess?.Invoke(response.data);
                    }
                    else
                    {
                        string errorMsg = response?.error ?? "Unknown error";
                        Debug.LogError($"❌ [CALLBACK] Search failed: {errorMsg}");
                        onError?.Invoke(errorMsg);
                    }
                }
                else
                {
                    Debug.LogWarning("❌ Empty response from server");
                    onError?.Invoke("No response from server");
                }
            },
            payload
        );
    }*/
    // ===== ADDITIONAL EMIT METHODS =====
    public void SendFriendRequest(string receiverId, Action onSuccess, Action<string> onError = null)
    {
        if (!IsSocketConnected)
        {
            onError?.Invoke("Socket not connected");
            return;
        }

        var payload = new Dictionary<string, object> { { "receiverId", receiverId } };
        Debug.Log($"[EMIT] Sending friend request to: {receiverId}");

        socket.Emit(
            "friend_send_request",
            (Socket s, Packet p, object[] ackArgs) =>
            {
                var response = ParseSocketResponse<GenericBackendResponse>(ackArgs?[0]);
                if (response != null && response.success)
                    onSuccess?.Invoke();
                else
                    onError?.Invoke(response?.error ?? "Failed to send request");
            },
            payload
        );
    }

    public void ListFriendRequests(Action<FriendRequestModel[]> onSuccess, Action<string> onError = null)
    {
        if (!IsSocketConnected)
        {
            onError?.Invoke("Socket not connected");
            return;
        }

        Debug.Log("[EMIT] Listing friend requests");
/*
        socket.Emit(
            "friend_list_requests",
            (Socket s, Packet p, object[] ackArgs) =>
            {
                var response = ParseSocketResponse<FriendRequestResponse>(ackArgs?[0]);
                if (response != null && response.success)
                    onSuccess?.Invoke(new[] { response.data });
                else
                    onError?.Invoke(response?.error ?? "Failed to get friend requests");
            }
        );*/
    }

    public void RespondToFriendRequest(int requestId, string action, Action onSuccess, Action<string> onError = null)
    {
        if (!IsSocketConnected)
        {
            onError?.Invoke("Socket not connected");
            return;
        }

        var payload = new Dictionary<string, object>
    {
        { "requestId", requestId },
        { "action", action }
    };

        Debug.Log($"[EMIT] Responding to friend request {requestId} with action: {action}");

        socket.Emit(
            "friend_respond_request",
            (Socket s, Packet p, object[] ackArgs) =>
            {
                var response = ParseSocketResponse<GenericBackendResponse>(ackArgs?[0]);
                if (response != null && response.success)
                    onSuccess?.Invoke();
                else
                    onError?.Invoke(response?.error ?? "Failed to respond to friend request");
            },
            payload
        );
    }

  /*  public void ListFriends(Action<FriendListResponse> onSuccess, Action<string> onError = null, string search = null)
    {
        if (!IsSocketConnected)
        {
            onError?.Invoke("Socket not connected");
            return;
        }

        var payload = string.IsNullOrEmpty(search)
            ? new Dictionary<string, object>()
            : new Dictionary<string, object> { { "search", search } };

        Debug.Log($"[EMIT] Listing friends" + (string.IsNullOrEmpty(search) ? "" : $" with search: {search}"));

        socket.Emit(
            "friend_list_friends",
            (Socket s, Packet p, object[] ackArgs) =>
            {
                var response = ParseSocketResponse<FriendListResponse>(ackArgs?[0]);
                if (response != null && response.success && response.data != null)
                    onSuccess?.Invoke(response);
                else
                    onError?.Invoke(response?.error ?? "Failed to get friends list");
            },
            payload
        );
    }*/

    public void UnfriendUser(string friendId, Action onSuccess, Action<string> onError = null)
    {
        if (!IsSocketConnected)
        {
            onError?.Invoke("Socket not connected");
            return;
        }

        var payload = new Dictionary<string, object> { { "friendId", friendId } };
        Debug.Log($"[EMIT] Unfriending user: {friendId}");

        socket.Emit(
            "friend_unfriend",
            (Socket s, Packet p, object[] ackArgs) =>
            {
                var response = ParseSocketResponse<GenericBackendResponse>(ackArgs?[0]);
                if (response != null && response.success)
                    onSuccess?.Invoke();
                else
                    onError?.Invoke(response?.error ?? "Failed to unfriend user");
            },
            payload
        );
    }

    // ===== CALLBACK RESPONSE HANDLERS =====
/*
    private void HandleFriendRequestResponse(object[] ackArgs)
    {
        if (ackArgs != null && ackArgs.Length > 0 && ackArgs[0] != null)
        {
            var response = ParseSocketResponse<searc>(ackArgs[0]);

            if (response != null)
            {
                if (response.success)
                {
                    Debug.Log("✅ Friend request sent successfully");
                }
                else
                {
                    Debug.LogError($"❌ Failed to send friend request: {response.error}");
                }
            }
            else
            {
                Debug.LogError("❌ Failed to parse friend request response");
            }
        }
    }
*/
    private void HandleFriendRequestsListResponse(object[] ackArgs)
    {
        if (ackArgs != null && ackArgs.Length > 0 && ackArgs[0] != null)
        {
            var response = ParseSocketResponse<GenericBackendResponse>(ackArgs[0]);
            if (response != null)
            {
                if (response.success)
                {
                    Debug.Log($"✅ Friend requests list received successfully");
                    // TODO: Parse and display friend requests from response.data
                }
                else
                {
                    Debug.LogError($"❌ Failed to get friend requests: {response.error}");
                }
            }
            else
            {
                Debug.LogError("❌ Failed to parse friend requests list response");
            }
        }
    }

    private void HandleFriendRequestResponseAction(object[] ackArgs)
    {
        if (ackArgs != null && ackArgs.Length > 0 && ackArgs[0] != null)
        {
            var response = ParseSocketResponse<GenericBackendResponse>(ackArgs[0]);
            if (response != null)
            {
                if (response.success)
                {
                    Debug.Log("✅ Friend request response sent successfully");
                }
                else
                {
                    Debug.LogError($"❌ Failed to respond to friend request: {response.error}");
                }
            }
            else
            {
                Debug.LogError("❌ Failed to parse friend request response");
            }
        }
    }

    private void HandleFriendsListResponse(object[] ackArgs)
    {
        if (ackArgs != null && ackArgs.Length > 0 && ackArgs[0] != null)
        {
            var response = ParseSocketResponse<GenericBackendResponse>(ackArgs[0]);
            if (response != null)
            {
                if (response.success)
                {
                    Debug.Log($"✅ Friends list received successfully");
                    // TODO: Parse and display friends list from response.data
                }
                else
                {
                    Debug.LogError($"❌ Failed to get friends list: {response.error}");
                }
            }
            else
            {
                Debug.LogError("❌ Failed to parse friends list response");
            }
        }
    }

    private void HandleUnfriendResponse(object[] ackArgs)
    {
        if (ackArgs != null && ackArgs.Length > 0 && ackArgs[0] != null)
        {
            var response = ParseSocketResponse<GenericBackendResponse>(ackArgs[0]);
            if (response != null)
            {
                if (response.success)
                {
                    Debug.Log("✅ User unfriended successfully");
                }
                else
                {
                    Debug.LogError($"❌ Failed to unfriend user: {response.error}");
                }
            }
            else
            {
                Debug.LogError("❌ Failed to parse unfriend response");
            }
        }
    }

}
/*

// ===== Models =====
[System.Serializable]
public class UserModel
{
    public string id;
    public string username;
    public string uniqueId;
    public string avatar;
    public string firstName;
    public string lastName;
    public string gender;
    public string profilePicture;
    public string coverPicture;
    public string email;
    public string role;

    // Helper method to check if user data is valid
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(username);
    }
}

[System.Serializable]
public class FriendRequestModel
{
    public int id;
    public string senderId;
    public UserModel sender;
    public string receiverId;
    public UserModel receiver;
    public string createdAt;
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
public class SearchResponse
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

// Response format for friend request events
[System.Serializable]
public class FriendRequestResponse
{
    public bool success;
    public string error;
    public FriendRequestModel data;
    public long timestamp;
}

// Response format for friend list events
[System.Serializable]
public class FriendListResponse
{
    public bool success;
    public string error;
    public FriendshipModel[] data;
    public long timestamp;
}*/