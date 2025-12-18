using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.SocketIO;
using BestHTTP;
using Newtonsoft.Json;
using BestHTTP.JSON;
public class PrivateChatManager : MonoBehaviour
{
    [Header("Configuration")]
    public string serverUrl = "http://localhost:5000";

    [Header("UI References")]
    public Transform conversationsList;
    public GameObject conversationItemPrefab;
    public Transform messagesList;
    public GameObject messageItemPrefab;
    public InputField messageInput;
    public Button sendButton;

    // Events
    public event Action<PrivateMessage> OnMessageReceived;
    public event Action<PrivateMessage> OnMessageSent;
    public event Action<string> OnMessageSendError;
    public event Action<string> OnTypingStop; // friendId
    public event Action<List<ConversationItem>> OnConversationsUpdated;
    public event Action<List<PrivateMessage>> OnChatHistoryLoaded;

    // Private members
    private Dictionary<string, ConversationItem> conversations = new Dictionary<string, ConversationItem>();
    private Dictionary<string, List<PrivateMessage>> chatHistories = new Dictionary<string, List<PrivateMessage>>();
    private string currentFriendId;
    private string currentUserId;
    private string authToken;
    private bool isTyping = false;
    private float lastTypingTime;
    private const float TYPING_TIMEOUT = 2f;

    private void Start()
    {
     //   SetupSocketEventListeners();
     //   SetupUI();
    }

    private void Update()
    {
        // Handle typing timeout
        if (isTyping && Time.time - lastTypingTime > TYPING_TIMEOUT)
        {
            StopTyping();
        }
    }

    #region Initialization

    public void Initialize(string userId, string token)
    {
        currentUserId = userId;
        authToken = token;
        LoadConversations();
    }

    private void SetupSocketEventListeners()
    {
      /*  var socket = //TestLudoGameSocketManager.instance.socket;

        // Message events
        socket.On("private_message", OnPrivateMessageReceived);
        socket.On("private_message_sent", OnPrivateMessageSentConfirm);
        socket.On("private_message_send_error", OnPrivateMessageSendError);

        // Read receipt events
        socket.On("private_message_read_ack", OnMessageReadAck);

        // Typing events
        socket.On("private_typing_start", OnFriendTypingStart);
        socket.On("private_typing_stop", OnFriendTypingStop);*/
    }

    private void SetupUI()
    {
        if (sendButton != null)
            sendButton.onClick.AddListener(SendCurrentMessage);

        if (messageInput != null)
        {
            messageInput.onValueChanged.AddListener(OnMessageInputChanged);
            messageInput.onEndEdit.AddListener(OnMessageInputEndEdit);
        }
    }

    #endregion
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
    #region Socket Event Handlers

    private void OnPrivateMessageReceived(Socket socket, Packet packet, object[] args)
    {
        try
        {
            if (args != null && args.Length > 0 && args[0] != null)
            {
                var messageData = ParseSocketResponse<PrivateMessage>(args[0]);
                if (messageData != null)
                {
                    HandleReceivedMessage(messageData);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing received message: {ex.Message}");
        }
    }

    private void OnPrivateMessageSentConfirm(Socket socket, Packet packet, object[] args)
    {
        try
        {
            if (args != null && args.Length > 0 && args[0] != null)
            {
                Debug.Log("<color=red>on data response</color> " + args[0]);
                /*  var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.data.ToString());
              if (response.ContainsKey("data"))
              {
                  var messageData = JsonConvert.DeserializeObject<PrivateMessage>(response["data"].ToString());
                  HandleSentMessage(messageData);
              }*/
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing sent message confirmation: {ex.Message}");
        }
    }

    private void OnPrivateMessageSendError(Socket socket, Packet packet, object[] args)
    {
        try
        {
            Debug.Log("<color=red>on data response</color> " + args[0]);
           /* var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.data.ToString());
            if (response.ContainsKey("error"))
            {
                string error = response["error"].ToString();
                OnMessageSendError?.Invoke(error);
                Debug.LogError($"Message send error: {error}");
            }*/
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing message send error: {ex.Message}");
        }
    }

    private void OnMessageReadAck(Socket socket, Packet packet, object[] args)
    {
        try
        {
            Debug.Log("<color=red>on data response</color> " + args[0]);
            /*var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.data.ToString());
            int messageId = Convert.ToInt32(data["messageId"]);
            string readerId = data["readerId"].ToString();

            // Update message as read in UI
            UpdateMessageReadStatus(messageId, true);*/
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing read acknowledgment: {ex.Message}");
        }
    }

    private void OnFriendTypingStart(Socket socket, Packet packet, object[] args)
    {
        try
        {
            Debug.Log("<color=red>on data response</color> " + args[0]);
            /*var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.data.ToString());
            string friendId = data["from"].ToString();

            if (conversations.ContainsKey(friendId))
            {
                conversations[friendId].isTyping = true;
                OnTypingStart?.Invoke(friendId, conversations[friendId].friend.username);
                UpdateConversationUI(friendId);
            }*/
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing typing start: {ex.Message}");
        }
    }

    private void OnFriendTypingStop(Socket socket, Packet packet, object[] args)
    {
        try
        {
            Debug.Log("<color=red>on data response</color> " + args[0]);
            /*var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.data.ToString());
            string friendId = data["from"].ToString();

            if (conversations.ContainsKey(friendId))
            {
                conversations[friendId].isTyping = false;
                OnTypingStop?.Invoke(friendId);
                UpdateConversationUI(friendId);
            }*/
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing typing stop: {ex.Message}");
        }
    }

    #endregion

    #region Public API Methods

    public void SendMessage(string friendId, string messageType, string content, object metadata = null)
    {
        var messageData = new
        {
            receiverId = friendId,
            type = messageType,
            content = content,
            metadata = metadata ?? new { }
        };

       /* //TestLudoGameSocketManager.instance.socket.Emit("private_message_send", messageData, (response) =>
        {
            // Response handled by OnPrivateMessageSentConfirm or OnPrivateMessageSendError
        });*/
    }

    public void SendTextMessage(string friendId, string message)
    {
        if (string.IsNullOrEmpty(message.Trim())) return;
        SendMessage(friendId, "text", message.Trim());
    }

    public void MarkMessageAsRead(int messageId, string friendId)
    {
        var data = new
        {
            messageId = messageId,
            friendId = friendId
        };

        //TestLudoGameSocketManager.instance.socket.Emit("private_message_read", data);
    }

    public void OpenChatWithFriend(string friendId)
    {
        currentFriendId = friendId;
        LoadChatHistory(friendId);

        // Mark all messages from this friend as read
        if (conversations.ContainsKey(friendId))
        {
            conversations[friendId].unreadCount = 0;
            UpdateConversationUI(friendId);
        }
    }

    public void LoadConversations()
    {
        StartCoroutine(LoadConversationsCoroutine());
    }

    public void LoadChatHistory(string friendId, int limit = 50, DateTime? before = null)
    {
        StartCoroutine(LoadChatHistoryCoroutine(friendId, limit, before));
    }

    public void LoadMoreMessages(string friendId, int limit = 50)
    {
        if (!chatHistories.ContainsKey(friendId) || chatHistories[friendId].Count == 0)
            return;

        // Get the oldest message timestamp
        var oldestMessage = chatHistories[friendId][chatHistories[friendId].Count - 1];
        DateTime before = DateTime.Parse(oldestMessage.createdAt);

        StartCoroutine(LoadChatHistoryCoroutine(friendId, limit, before, true));
    }

    #endregion

    #region REST API Calls

    private System.Collections.IEnumerator LoadConversationsCoroutine()
    {
        string url = $"{serverUrl}/api/chat/private/conversations";

        using (HTTPRequest request = new HTTPRequest(new Uri(url)))
        {
            request.SetHeader("Authorization", $"Bearer {authToken}");
            request.SetHeader("Content-Type", "application/json");

            yield return StartCoroutine(request.Send());

            if (request.Response.IsSuccess)
            {
                try
                {
                    var messages = JsonConvert.DeserializeObject<List<PrivateMessage>>(request.Response.DataAsText);
                    ProcessConversations(messages);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing conversations: {ex.Message}");
                }
            }
            else
            {
                Debug.LogError($"Failed to load conversations: {request.Response.StatusCode} - {request.Response.Message}");
            }
        }
    }

    private System.Collections.IEnumerator LoadChatHistoryCoroutine(string friendId, int limit = 50, DateTime? before = null, bool append = false)
    {
        string url = $"{serverUrl}/api/chat/private/history?friendId={friendId}&limit={limit}";
        if (before.HasValue)
        {
            url += $"&before={before.Value:yyyy-MM-ddTHH:mm:ssZ}";
        }

        using (HTTPRequest request = new HTTPRequest(new Uri(url)))
        {
            request.SetHeader("Authorization", $"Bearer {authToken}");
            request.SetHeader("Content-Type", "application/json");

            yield return StartCoroutine(request.Send());

            if (request.Response.IsSuccess)
            {
                try
                {
                    var messages = JsonConvert.DeserializeObject<List<PrivateMessage>>(request.Response.DataAsText);
                    ProcessChatHistory(friendId, messages, append);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing chat history: {ex.Message}");
                }
            }
            else
            {
                Debug.LogError($"Failed to load chat history: {request.Response.StatusCode} - {request.Response.Message}");
            }
        }
    }

    #endregion

    #region Message Processing

    private void HandleReceivedMessage(PrivateMessage message)
    {
        string friendId = message.senderId == currentUserId ? message.receiverId : message.senderId;

        // Add to conversation
        UpdateConversation(friendId, message);

        // Add to chat history if this chat is currently open
        if (friendId == currentFriendId)
        {
            AddMessageToChatHistory(friendId, message);
            DisplayMessage(message);

            // Auto-mark as read if chat is open
            MarkMessageAsRead(message.id, friendId);
        }
        else
        {
            // Increment unread count
            if (conversations.ContainsKey(friendId))
            {
                conversations[friendId].unreadCount++;
                UpdateConversationUI(friendId);
            }
        }

        OnMessageReceived?.Invoke(message);
    }

    private void HandleSentMessage(PrivateMessage message)
    {
        string friendId = message.receiverId;

        // Add to conversation
        UpdateConversation(friendId, message);

        // Add to current chat if open
        if (friendId == currentFriendId)
        {
            AddMessageToChatHistory(friendId, message);
            DisplayMessage(message);
        }

        OnMessageSent?.Invoke(message);
    }

    private void ProcessConversations(List<PrivateMessage> messages)
    {
        conversations.Clear();

        foreach (var message in messages)
        {
            string friendId = message.senderId == currentUserId ? message.receiverId : message.senderId;
            PublicUser friend = message.senderId == currentUserId ? message.receiver : message.sender;

            var conversationItem = new ConversationItem
            {
                lastMessage = message,
                friend = friend,
                unreadCount = 0, // Will be calculated separately if needed
                isTyping = false
            };

            conversations[friendId] = conversationItem;
        }

        UpdateConversationsUI();
        OnConversationsUpdated?.Invoke(new List<ConversationItem>(conversations.Values));
    }

    private void ProcessChatHistory(string friendId, List<PrivateMessage> messages, bool append = false)
    {
        if (!append)
        {
            chatHistories[friendId] = new List<PrivateMessage>();
        }

        if (!chatHistories.ContainsKey(friendId))
        {
            chatHistories[friendId] = new List<PrivateMessage>();
        }

        if (append)
        {
            // Add older messages to the end
            chatHistories[friendId].AddRange(messages);
        }
        else
        {
            // Messages come in DESC order (newest first), reverse for display
            messages.Reverse();
            chatHistories[friendId] = messages;
        }

        if (friendId == currentFriendId)
        {
            DisplayChatHistory(messages, append);
        }

        OnChatHistoryLoaded?.Invoke(messages);
    }

    private void UpdateConversation(string friendId, PrivateMessage message)
    {
        if (!conversations.ContainsKey(friendId))
        {
            PublicUser friend = message.senderId == currentUserId ? message.receiver : message.sender;
            conversations[friendId] = new ConversationItem
            {
                friend = friend,
                unreadCount = 0,
                isTyping = false
            };
        }

        conversations[friendId].lastMessage = message;
        UpdateConversationUI(friendId);
    }

    private void AddMessageToChatHistory(string friendId, PrivateMessage message)
    {
        if (!chatHistories.ContainsKey(friendId))
        {
            chatHistories[friendId] = new List<PrivateMessage>();
        }

        // Insert at beginning (newest messages first)
        chatHistories[friendId].Insert(0, message);
    }

    #endregion

    #region UI Management

    private void UpdateConversationsUI()
    {
        // Clear existing conversation items
        foreach (Transform child in conversationsList)
        {
            Destroy(child.gameObject);
        }

        // Sort conversations by last message time
        var sortedConversations = new List<ConversationItem>(conversations.Values);
        sortedConversations.Sort((a, b) => DateTime.Parse(b.lastMessage.createdAt).CompareTo(DateTime.Parse(a.lastMessage.createdAt)));

        // Create UI items
        foreach (var conversation in sortedConversations)
        {
            CreateConversationItem(conversation);
        }
    }

    private void CreateConversationItem(ConversationItem conversation)
    {
        GameObject item = Instantiate(conversationItemPrefab, conversationsList);
        var conversationUI = item.GetComponent<ConversationItemUI>();

        if (conversationUI != null)
        {
            conversationUI.Setup(conversation);
            conversationUI.OnConversationClicked += (friendId) => OpenChatWithFriend(friendId);
        }
    }

    private void UpdateConversationUI(string friendId)
    {
        // Find and update the specific conversation UI item
        foreach (Transform child in conversationsList)
        {
            var conversationUI = child.GetComponent<ConversationItemUI>();
            if (conversationUI != null && conversationUI.GetFriendId() == friendId)
            {
                conversationUI.UpdateDisplay(conversations[friendId]);
                break;
            }
        }
    }

    private void DisplayChatHistory(List<PrivateMessage> messages, bool append = false)
    {
        if (!append)
        {
            // Clear existing messages
            foreach (Transform child in messagesList)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (var message in messages)
        {
            DisplayMessage(message);
        }
    }

    private void DisplayMessage(PrivateMessage message)
    {
        GameObject messageItem = Instantiate(messageItemPrefab, messagesList);
        var messageUI = messageItem.GetComponent<MessageItemUI>();

        if (messageUI != null)
        {
            bool isOwnMessage = message.senderId == currentUserId;
            messageUI.Setup(message, isOwnMessage);
        }
    }

    private void UpdateMessageReadStatus(int messageId, bool isRead)
    {
        // Find and update message read status in UI
        foreach (Transform child in messagesList)
        {
            var messageUI = child.GetComponent<MessageItemUI>();
            if (messageUI != null && messageUI.GetMessageId() == messageId)
            {
                messageUI.SetReadStatus(isRead);
                break;
            }
        }
    }

    #endregion

    #region Typing Indicators

    private void OnMessageInputChanged(string text)
    {
        if (string.IsNullOrEmpty(currentFriendId)) return;

        if (!string.IsNullOrEmpty(text) && !isTyping)
        {
            StartTyping();
        }

        lastTypingTime = Time.time;
    }

    private void OnMessageInputEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendCurrentMessage();
        }
    }

    private void StartTyping()
    {
        if (string.IsNullOrEmpty(currentFriendId)) return;

        isTyping = true;
        var data = new { friendId = currentFriendId };
        //TestLudoGameSocketManager.instance.socket.Emit("private_typing_start", data);
    }

    private void StopTyping()
    {
        if (!isTyping || string.IsNullOrEmpty(currentFriendId)) return;

        isTyping = false;
        var data = new { friendId = currentFriendId };
        //TestLudoGameSocketManager.instance.socket.Emit("private_typing_stop", data);
    }

    private void SendCurrentMessage()
    {
        if (messageInput != null && !string.IsNullOrEmpty(currentFriendId))
        {
            string message = messageInput.text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                SendTextMessage(currentFriendId, message);
                messageInput.text = "";
                StopTyping();
            }
        }
    }

    #endregion

    private void OnDestroy()
    {
        StopTyping();
    }
}
[System.Serializable]
public class PrivateMessage
{
    public int id;
    public string senderId;
    public string receiverId;
    public string type;
    public string content;
    public object metadata;
    public string createdAt;
    public bool deleted;
    public PublicUser sender;
    public PublicUser receiver;
}

[System.Serializable]
public class PublicUser
{
    public string id;
    public string username;
    public string profilePicture;
}

[System.Serializable]
public class ConversationItem
{
    public PrivateMessage lastMessage;
    public PublicUser friend;
    public int unreadCount;
    public bool isTyping;
}