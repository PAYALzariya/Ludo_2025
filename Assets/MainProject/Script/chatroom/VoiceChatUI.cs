using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class VoiceChatUI : MonoBehaviour
{
    [Header("UI References")]
    public Button startVoiceChatButton;
    public Button endVoiceChatButton;
    public Button joinVoiceButton;
    public Button leaveVoiceButton;
    public Button micToggleButton;
    public TMP_Dropdown qualityDropdown;
    public TMP_Text statusText;
    public GameObject voiceChatPanel;
    
    [Header("Mic Button States")]
    public Sprite micOnSprite;
    public Sprite micOffSprite;
    
    public VoiceChatManager voiceChatManager;
    private string currentRoomId;
    private bool isHost = false;
    private bool isMicOn = true;

    private void OnEnable()
    {

        SetupEventListeners();
        SetupUI();
        SetRoomInfo(HomePanel.instance.chatManager.CurrentRoom.data.room.id,HomePanel.instance.chatManager.OwnProfileData.isHost);
    }
    
    private void SetupEventListeners()
    {
        // Button events
        startVoiceChatButton.onClick.AddListener(StartVoiceChat);
        endVoiceChatButton.onClick.AddListener(EndVoiceChat);
        joinVoiceButton.onClick.AddListener(JoinVoiceSession);
        leaveVoiceButton.onClick.AddListener(LeaveVoiceSession);
        micToggleButton.onClick.AddListener(ToggleMicrophone);
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        
        // Voice chat manager events
        if (voiceChatManager != null)
        {
            voiceChatManager.OnVoiceChatStarted += OnVoiceChatStarted;
            voiceChatManager.OnVoiceChatEnded += OnVoiceChatEnded;
            voiceChatManager.OnUserStartedSpeaking += OnUserStartedSpeaking;
            voiceChatManager.OnUserStoppedSpeaking += OnUserStoppedSpeaking;
        }
    }
    
    private void SetupUI()
    {
        // Setup quality dropdown
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string> { "Low", "Medium", "High" });
        qualityDropdown.value = 1; // Medium by default
        
        UpdateUIState();
    }
    
    public void SetRoomInfo(string roomId, bool isHost)
    {
        Debug.Log("roomId:" + roomId + "::isHost:" + isHost);
        this.currentRoomId = roomId;
        this.isHost = isHost;
        UpdateUIState();
    }
    
    private void UpdateUIState()
    {
        // Show/hide buttons based on host status and voice chat state
        startVoiceChatButton.gameObject.SetActive(isHost);
        endVoiceChatButton.gameObject.SetActive(isHost);
        
        // Update mic button sprite
        if (micToggleButton.GetComponent<Image>() != null)
        {
            micToggleButton.GetComponent<Image>().sprite = isMicOn ? micOnSprite : micOffSprite;
        }
    }
    
    #region Button Handlers
    
    private void StartVoiceChat()
    {
        if (voiceChatManager != null && !string.IsNullOrEmpty(currentRoomId))
        {
            //voiceChatManager.StartVoiceChat(currentRoomId);
            statusText.text = "Starting voice chat...";
        }
    }
    
    private void EndVoiceChat()
    {
        if (voiceChatManager != null && !string.IsNullOrEmpty(currentRoomId))
        {
            //voiceChatManager.EndVoiceChat(currentRoomId);
            statusText.text = "Ending voice chat...";
        }
    }
    
    private void JoinVoiceSession()
    {
        if (voiceChatManager != null && !string.IsNullOrEmpty(currentRoomId))
        {
          //  voiceChatManager.JoinVoiceSession(currentRoomId);
            statusText.text = "Joining voice session...";
        }
    }
    
    private void LeaveVoiceSession()
    {
        if (voiceChatManager != null)
        {
            voiceChatManager.LeaveVoiceSession();
            statusText.text = "Leaving voice session...";
        }
    }
    
    private void ToggleMicrophone()
    {
        if (voiceChatManager != null)
        {
            voiceChatManager.ToggleMicrophone();
            isMicOn = !isMicOn;
            UpdateUIState();
            statusText.text = isMicOn ? "Microphone ON" : "Microphone OFF";
        }
    }
    
    private void OnQualityChanged(int qualityIndex)
    {
        if (voiceChatManager != null)
        {
            VoiceChatManager.AudioQuality quality = (VoiceChatManager.AudioQuality)qualityIndex;
            Debug.Log("VoiceChatManager.AudioQuality::" + quality);
            voiceChatManager.SetAudioQuality(quality);
            statusText.text = $"Audio quality: {quality}";
        }
    }
    
    #endregion
    
    #region Voice Chat Events
    
    private void OnVoiceChatStarted(string roomId)
    {
        if (roomId == currentRoomId)
        {
            statusText.text = "Voice chat started!";
            voiceChatPanel.SetActive(true);
            joinVoiceButton.gameObject.SetActive(true);
        }
    }
    
    private void OnVoiceChatEnded(string roomId)
    {
        if (roomId == currentRoomId)
        {
            statusText.text = "Voice chat ended";
            voiceChatPanel.SetActive(false);
        }
    }
    
    private void OnUserStartedSpeaking(string roomId, string userId)
    {
        if (roomId == currentRoomId)
        {
            Debug.Log($"User {userId} started speaking");
        }
    }
    
    private void OnUserStoppedSpeaking(string roomId, string userId)
    {
        if (roomId == currentRoomId)
        {
            Debug.Log($"User {userId} stopped speaking");
        }
    }
    
    #endregion
}