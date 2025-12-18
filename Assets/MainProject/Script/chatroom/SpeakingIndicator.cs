using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeakingIndicator : MonoBehaviour
{
    [Header("UI References")]
    public Image speakingIcon;
    public TextMeshProUGUI usernameText;
   // public Animator pulseAnimator;
    
    [Header("Settings")]
    public Color speakingColor = Color.green;
    public Color normalColor = Color.gray;
    public AudioSource audioSource;
    private string userId;
    
    public void SetUser(string userId)
    {
        this.userId = userId;
        
        // You might want to fetch user display name here
        if (usernameText != null)
        {
            usernameText.text = GetUserDisplayName(userId);
        }
    }
    
    private void OnEnable()
    {
        if (speakingIcon != null)
        {
            speakingIcon.color = speakingColor;
        }
        
        
    }
    
    private void OnDisable()
    {
        if (speakingIcon != null)
        {
            speakingIcon.color = normalColor;
        }
        
        
    }
    
    private string GetUserDisplayName(string userId)
    {
        // Implement user display name lookup
        // This could query your user management system
        return $"User {userId}";
    }
}