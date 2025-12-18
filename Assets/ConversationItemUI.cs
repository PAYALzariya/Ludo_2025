using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Image friendAvatar;
    public TextMeshProUGUI friendName;
    public TextMeshProUGUI lastMessage;
    public TextMeshProUGUI timestamp;
    public GameObject unreadBadge;
    public TextMeshProUGUI unreadCount;
    public GameObject typingIndicator;
    public Button conversationButton;

    public event Action<string> OnConversationClicked;

    private string friendId;
    private ConversationItem conversationData;

    private void Start()
    {
        if (conversationButton != null)
            conversationButton.onClick.AddListener(() => OnConversationClicked?.Invoke(friendId));
    }

    public void Setup(ConversationItem conversation)
    {
        conversationData = conversation;
        friendId = conversation.friend.id;
        UpdateDisplay(conversation);
    }

    public void UpdateDisplay(ConversationItem conversation)
    {
        conversationData = conversation;

        // Friend info
        if (friendName != null)
            friendName.text = conversation.friend.username;

        // Load avatar
        if (friendAvatar != null && !string.IsNullOrEmpty(conversation.friend.profilePicture))
        {
            StartCoroutine(LoadAvatarImage(conversation.friend.profilePicture));
        }

        // Last message
        if (lastMessage != null)
        {
            string messageText = conversation.lastMessage.content;
            if (conversation.lastMessage.type == "image")
                messageText = "📷 Image";
            else if (conversation.lastMessage.type == "voice")
                messageText = "🎤 Voice message";

            lastMessage.text = messageText;
        }

        // Timestamp
        if (timestamp != null)
        {
            DateTime messageTime = DateTime.Parse(conversation.lastMessage.createdAt);
            timestamp.text = FormatTimestamp(messageTime);
        }

        // Unread badge
        if (unreadBadge != null)
        {
            unreadBadge.SetActive(conversation.unreadCount > 0);
            if (unreadCount != null && conversation.unreadCount > 0)
            {
                unreadCount.text = conversation.unreadCount > 99 ? "99+" : conversation.unreadCount.ToString();
            }
        }

        // Typing indicator
        if (typingIndicator != null)
            typingIndicator.SetActive(conversation.isTyping);
    }

    public string GetFriendId()
    {
        return friendId;
    }

    private System.Collections.IEnumerator LoadAvatarImage(string url)
    {
        using (var request = new BestHTTP.HTTPRequest(new System.Uri(url)))
        {
            yield return StartCoroutine(request.Send());

            if (request.Response.IsSuccess && friendAvatar != null)
            {
                Texture2D texture = request.Response.DataAsTexture2D;
                if (texture != null)
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                    friendAvatar.sprite = sprite;
                }
            }
        }
    }

    private string FormatTimestamp(DateTime messageTime)
    {
        TimeSpan timeDiff = DateTime.Now - messageTime;

        if (timeDiff.TotalMinutes < 1)
            return "now";
        else if (timeDiff.TotalHours < 1)
            return $"{(int)timeDiff.TotalMinutes}m";
        else if (timeDiff.TotalDays < 1)
            return $"{(int)timeDiff.TotalHours}h";
        else if (timeDiff.TotalDays < 7)
            return $"{(int)timeDiff.TotalDays}d";
        else
            return messageTime.ToString("MMM dd");
    }
}
/*
*/