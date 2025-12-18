using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI timestampText;
    public Image messageBackground;
    public GameObject readIndicator;
    public Image senderAvatar;

    [Header("Message Styles")]
    public Color ownMessageColor = Color.blue;
    public Color friendMessageColor = Color.gray;

    private int messageId;
    private bool isOwnMessage;

    public void Setup(PrivateMessage message, bool isOwn)
    {
        messageId = message.id;
        isOwnMessage = isOwn;

        // Message content
        if (messageText != null)
        {
            if (message.type == "text")
                messageText.text = message.content;
            else if (message.type == "image")
                messageText.text = "📷 Image";
            else if (message.type == "voice")
                messageText.text = "🎤 Voice message";
        }

        // Timestamp
        if (timestampText != null)
        {
            DateTime messageTime = DateTime.Parse(message.createdAt);
            timestampText.text = messageTime.ToString("HH:mm");
        }

        // Message styling
        if (messageBackground != null)
        {
            messageBackground.color = isOwn ? ownMessageColor : friendMessageColor;
        }

        // Alignment
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = isOwn ? new Vector2(0.3f, 0) : new Vector2(0, 0);
            rectTransform.anchorMax = isOwn ? new Vector2(1, 1) : new Vector2(0.7f, 1);
        }

        // Read indicator (only for own messages)
        if (readIndicator != null)
        {
            readIndicator.SetActive(isOwn);
        }

        // Avatar (only for friend messages)
        if (senderAvatar != null)
        {
            senderAvatar.gameObject.SetActive(!isOwn);
            if (!isOwn && message.sender != null && !string.IsNullOrEmpty(message.sender.profilePicture))
            {
                StartCoroutine(LoadAvatarImage(message.sender.profilePicture));
            }
        }
    }

    public int GetMessageId()
    {
        return messageId;
    }

    public void SetReadStatus(bool isRead)
    {
        if (readIndicator != null && isOwnMessage)
        {
            // Change read indicator appearance
            var image = readIndicator.GetComponent<Image>();
            if (image != null)
            {
                image.color = isRead ? Color.blue : Color.gray;
            }
        }
    }

    private System.Collections.IEnumerator LoadAvatarImage(string url)
    {
        using (var request = new BestHTTP.HTTPRequest(new System.Uri(url)))
        {
            yield return StartCoroutine(request.Send());

            if (request.Response.IsSuccess && senderAvatar != null)
            {
                Texture2D texture = request.Response.DataAsTexture2D;
                if (texture != null)
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                    senderAvatar.sprite = sprite;
                }
            }
        }
    }
}
