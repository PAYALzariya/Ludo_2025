

using BestHTTP.JSON;
using BestHTTP.SocketIO;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Timeline;
using static Crystal.SafeArea;


public class VoiceChatManager : MonoBehaviour
{
    [Header("Voice Chat Settings")]
    public AudioQuality defaultQuality = AudioQuality.Medium;
    // public bool autoJoinVoiceSession = true;
    public float speakingThreshold = 0.01f;
    public float speakingCooldown = 0.5f;

    [Header("UI References")]

    public Transform speakingIndicatorsParent;
    public SpeakingIndicator speakingIndicatorPrefab;

    // Events
    public event Action<string> OnVoiceChatStarted;
    public event Action<string> OnVoiceChatEnded;
    public event Action<string, string> OnUserStartedSpeaking;
    public event Action<string, string> OnUserStoppedSpeaking;
    public event Action<string, List<string>> OnVoiceParticipantsUpdated;

    // Private members
    private Dictionary<string, RTCPeerConnection> peerConnections = new Dictionary<string, RTCPeerConnection>();
    private Dictionary<string, AudioSource> remoteAudioSources = new Dictionary<string, AudioSource>();
    private Dictionary<string, GameObject> speakingIndicators = new Dictionary<string, GameObject>();
    private readonly ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

    public AudioSource microphoneSource;
    private MediaStream localStream;
    private AudioTrack audioTrack;
    internal string currentRoomId;
    public bool isInVoiceSession = false;
    public bool isMicrophoneActive = false;
    public bool isSpeaking = false;
    private float lastSpeakingTime;
    public Socket socket;
    // Audio quality settings
    public enum AudioQuality
    {
        Low,    // 32kbps, 16kHz
        Medium, // 64kbps, 24kHz  
        High    // 128kbps, 48kHz
    }
    /*private void Update()
    {
        WebRTC.Update();
        while (mainThreadActions.TryDequeue(out var action))
        {
            Debug.Log("mainThreadActions.TryDequeue::");
            action?.Invoke();
        }
    }*/
    private void OnEnable()
    {

        Invoke("Oninvokefunction", 1f);
    }

    void Oninvokefunction()
    {
        socket = LudoSocketManager.Instance.socket;
        if (socket.IsOpen)
        {
            Debug.Log("sockt::" + socket.IsOpen);
            //  UpdateEvent();
            SetupSocketEventListeners();

            SetupMicrophone();
        }
    }
    internal void OnQualityChanged(int qualityIndex)
    {

        AudioQuality quality = (AudioQuality)qualityIndex;
        Debug.Log("VoiceChatManager.AudioQuality::" + quality);
        SetAudioQuality(quality);


    }
    private void SetupMicrophone()
    {

        // microphoneSource = gameObject.AddComponent<AudioSource>();
        microphoneSource.loop = true;
        // microphoneSource.mute = true; // Don't play our own mic

        // Request microphone permission
        RequestMicrophonePermission();
    }


    private void RequestMicrophonePermission()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#elif UNITY_IOS
        // iOS permissions are handled in Info.plist
#endif
    }

    #region Socket Event Setup
    void UpdateEvent()
    {
        OnVoiceChatStarted += OnVoiceChatStarted_EventCallBack;
        OnVoiceChatEnded += OnVoiceChatEnded_EventCallBack;
        OnUserStartedSpeaking += OnUserStartedSpeaking_EventCallBack;
        OnUserStoppedSpeaking += OnUserStoppedSpeaking_EventCallBack;
    }
    void OnVoiceChatStarted_EventCallBack(string roomid)
    {
        Debug.Log("OnVoiceChatStarted...");
    }
    void OnVoiceChatEnded_EventCallBack(string roomid)
    {
        Debug.Log("OnVoiceChatEnded...");
    }
    private void OnUserStartedSpeaking_EventCallBack(string roomId, string userId)
    {
        Debug.Log($"<color=green>User {userId} started speaking</color>");

    }

    private void OnUserStoppedSpeaking_EventCallBack(string roomId, string userId)
    {

        Debug.Log($"<color=green>User {userId} stopped speaking</color>");

    }
    private void SetupSocketEventListeners()
    {


        // Voice chat session events
        /* socket.On(GameConstants.ONEvents.voicechatstarted, OnVoiceChatStartedReceived);
          socket.On(GameConstants.ONEvents.voicechatended, OnVoiceChatEndedReceived);
        socket.On(GameConstants.ONEvents.voicesessionleft, OnVoiceSessionLeftReceived);
        */
        socket.On(GameConstants.ONEvents.voicesessionjoined, OnVoiceSessionJoinedReceived);
        socket.On(GameConstants.ONEvents.voiceparticipantjoined, OnVoiceParticipantJoinedReceived);
        socket.On(GameConstants.ONEvents.voiceparticipantleft, OnVoiceParticipantLeftReceived);

        // WebRTC signaling events
        socket.On(GameConstants.WebRTC_ONEvents.webrtcofferreceived, OnWebRTCOfferReceived);
        socket.On(GameConstants.WebRTC_ONEvents.webrtcanswerreceived, OnWebRTCAnswerReceived);
        socket.On(GameConstants.WebRTC_ONEvents.webrtcicecandidatereceived, OnWebRTCIceCandidateReceived);

        // Speaking state events
        socket.On(GameConstants.ONEvents.voiceuserspeakingstart, OnUserStartedSpeakingReceived);
        socket.On(GameConstants.ONEvents.voiceuserspeakingstop, OnUserStoppedSpeakingReceived);

        // Quality events
        socket.On(GameConstants.ONEvents.voicequalitychanged, OnVoiceQualityChangedReceived);
     //   socket.On(GameConstants.ONEvents.voicequalitychangedbyuser, OnVoiceQualityChangedByUserReceived);
    }
    /* private void OnVoiceChatStartedReceived(Socket socket, Packet packet, params object[] args)
     {
         Debug.Log("VoiceChatStarted..." + args);
         if (args.Length == 0) return;

         string jsonResponse = Json.Encode(args[0]);
         var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
         if (data.ContainsKey("success"))
         {
             object successObj;
             bool success = data.TryGetValue("success", out successObj) && Convert.ToBoolean(successObj);
             Debug.Log(success);
             if (data.ContainsKey("data") && success)
             {
                 var voiceData = JsonConvert.DeserializeObject<Dictionary<string, object>>(data["data"].ToString());
                 string roomId = voiceData["roomId"].ToString();
                 Debug.Log(roomId);
                 OnVoiceChatStarted?.Invoke(roomId);

                 // Auto-join if enabled
                 *//*if (autoJoinVoiceSession && roomId == currentRoomId)
                 {
                     autoJoinVoiceSession = false;
                     JoinVoiceSession(roomId);
                 }*//*
                 Debug.Log("<color=green>Response from OnVoiceChatStartedReceived </color>" + jsonResponse);
             }
             *//*var data = JsonConvert.DeserializeObject<AnswerData>(jsonResponse);
             HandleIncomingOffer(data.roomId, data.fromUserId, data.offer);*//*
         }

     }
     private void OnVoiceChatEndedReceived(Socket socket, Packet packet, params object[] args)
     {
         if (args.Length == 0) return;
         try
         {
             string jsonResponse = Json.Encode(args[0]);


             var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);

             if (data.ContainsKey("data"))
             {
                 var voiceData = JsonConvert.DeserializeObject<Dictionary<string, object>>(data["data"].ToString());
                 string roomId = voiceData["roomId"].ToString();

                 OnVoiceChatEnded?.Invoke(roomId);
                 CleanupVoiceSession();
             }

             Debug.Log("<color=green>Response from OnVoiceChatEndedReceived </color>" + jsonResponse);

         }
         catch (Exception ex)
         {
             Debug.LogError($"Failed to parse push OnVoiceChatEndedReceived for : {ex.Message}");
         }

     }

      private void OnVoiceSessionLeftReceived(Socket socket, Packet packet, params object[] args)
     {
         if (args.Length == 0) return;
         try
         {
             string jsonResponse = Json.Encode(args[0]);
            
    Debug.Log("<color=green>Response from OnVoiceSessionLeftReceived </color>" + jsonResponse);

        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse push OnVoiceSessionLeftReceived for : {ex.Message}");
        }

    }
*/
    private void OnVoiceSessionJoinedReceived(Socket socket, Packet packet, params object[] args)
    {
        if (args.Length == 0) return;
        try
        {
            string jsonResponse = Json.Encode(args[0]);
            var response = JsonConvert.DeserializeObject<joinvoicesessionRepsonse>(jsonResponse);

            string newParticipant = response.data.newParticipant;

            OnReceived_JoinVoiceSession(response);

            Debug.Log("<color=green>Response from OnVoiceSessionJoinedReceived </color>" + jsonResponse);

        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse push OnVoiceSessionJoinedReceived for : {ex.Message}");
        }

    }

    private void OnVoiceParticipantJoinedReceived(Socket socket, Packet packet, params object[] args)
    {
        if (args.Length == 0) return;
        try
        {
            string jsonResponse = Json.Encode(args[0]);

            var response = JsonConvert.DeserializeObject<joinvoicesessionRepsonse>(jsonResponse);

            string newParticipant = response.data.newParticipant;
            Debug.Log("<color=green>OnVoiceParticipantJoinedReceived::</color>" + jsonResponse);

            // Create peer connection for new participant (we don't initiate)
            foreach (var newParticipantt in response.data.participants)
            {
                if (newParticipantt.id != DataManager.instance.userId)
                {

                    if (!peerConnections.ContainsKey(newParticipantt.id))
                    {

                        Debug.Log(" not peerConnections OnVoiceParticipantJoinedReceived::" + newParticipant);
                        CreatePeerConnection(newParticipantt.id, false);
                    }
                }
            }




        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse push OnVoiceParticipantJoinedReceived for : {ex.Message}");
        }

    }
    private void OnVoiceParticipantLeftReceived(Socket socket, Packet packet, params object[] args)
    {
        if (args.Length == 0) return;
        try
        {
            string jsonResponse = Json.Encode(args[0]);
            Debug.Log("<color=green>Response from OnVoiceParticipantLeftReceived </color>" + jsonResponse);
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
            string leftParticipant = data["leftParticipant"].ToString();

            RemovePeerConnection(leftParticipant);



        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse push OnVoiceParticipantLeftReceived for : {ex.Message}");
        }

    }
    private void OnWebRTCOfferReceived(Socket socket, Packet packet, params object[] args)
    {
        if (args.Length == 0) return;
        try
        {
            string jsonResponse = Json.Encode(args[0]);
            Debug.Log("<color=green>Response from On_webrtcofferreceived </color>" + jsonResponse);
            CreateAndSendOfferReceivedResponse response = JsonConvert.DeserializeObject<CreateAndSendOfferReceivedResponse>(jsonResponse);


            HandleWebRTCOffer(response.data.fromUserId, response.data.offer);

            Debug.Log("Response from On_webrtcofferreceived offer::" + response.data.offer);

        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse push On_webrtcofferreceived for : {ex.Message}");
        }


    }
    private void OnWebRTCAnswerReceived(Socket socket, Packet packet, params object[] args)
    {
        if (args.Length == 0) return;
        try
        {
            string jsonResponse = Json.Encode(args[0]);
            Debug.Log("<color=green>Response from OnWebRTCAnswerReceived </color>" + jsonResponse);
            OnWebRTCAnswerReceivedResponse response = JsonConvert.DeserializeObject<OnWebRTCAnswerReceivedResponse>(jsonResponse);
            Debug.Log("<color=green>Response from OnWebRTCAnswerReceived response </color>" + response.data.answer.type);


            HandleWebRTCAnswer(response.data.fromUserId, response.data.answer);



        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse push OnWebRTCAnswerReceived for : {ex.Message}");
        }

    }
    private void OnWebRTCIceCandidateReceived(Socket socket, Packet packet, params object[] args)
    {
        if (args.Length == 0) return;

        try
        {
            string jsonResponse = Json.Encode(args[0]);

            WebRTCIceCandidateReceivedResponse response = JsonConvert.DeserializeObject<WebRTCIceCandidateReceivedResponse>(jsonResponse);



            if (response.data.candidate != null)
            {
                Debug.Log($"<color=green>ICE Candidate received from </color>" + jsonResponse);
                HandleWebRTCIceCandidate(response.data.fromUserId, response.data.candidate);


            }
        }
        catch (Exception ex)
        {
            Debug.Log("<color=red>Failed to parse OnWebRTCIceCandidateReceived:</color>" + ex.Message);
        }
    }
    private void OnUserStartedSpeakingReceived(Socket socket, Packet packet, params object[] args)
    {
        if (args.Length == 0) return;
        try
        {
            string jsonResponse = Json.Encode(args[0]);
            Debug.Log("<color=green>Response from OnUserStartedSpeakingReceived </color>" + jsonResponse);

            OnUserStartedSpeakingResponse response = JsonConvert.DeserializeObject<OnUserStartedSpeakingResponse>(jsonResponse);

            if (response != null && response.success)
            {
                string userId = response.data.userId;
                string roomId = response.data.roomId;
                ShowSpeakingIndicator(userId, true);
                OnUserStartedSpeaking?.Invoke(roomId, userId);
            }





        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse push OnUserStartedSpeakingReceived for : {ex.Message}");
        }

    }
    private void OnUserStoppedSpeakingReceived(Socket socket, Packet packet, params object[] args)
    {
        if (args.Length == 0) return;
        try
        {
            string jsonResponse = Json.Encode(args[0]);
            Debug.Log("<color=green>Response from OnUserStoppedSpeakingReceived </color>" + jsonResponse);


            OnUserStartedSpeakingResponse response = JsonConvert.DeserializeObject<OnUserStartedSpeakingResponse>(jsonResponse);

            if (response != null && response.success)
            {
                string userId = response.data.userId;
                string roomId = response.data.roomId;

                ShowSpeakingIndicator(userId, false);
                OnUserStoppedSpeaking?.Invoke(roomId, userId);

            }




        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse push OnUserStoppedSpeakingReceived for : {ex.Message}");
        }

    }
    private void OnVoiceQualityChangedReceived(Socket socket, Packet packet, params object[] args)
    {
        if (args.Length == 0) return;
        try
        {
            string jsonResponse = Json.Encode(args[0]);
            /*var data = JsonConvert.DeserializeObject<AnswerData>(jsonResponse);
            HandleIncomingOffer(data.roomId, data.fromUserId, data.offer);*/
            Debug.Log("<color=green>Response from OnVoiceQualityChangedReceived </color>" + jsonResponse);

        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse push OnVoiceQualityChangedReceived for : {ex.Message}");
        }

    }
    private void OnVoiceQualityChangedByUserReceived(Socket socket, Packet packet, params object[] args)
    {
        if (args.Length == 0) return;
        try
        {
            string jsonResponse = Json.Encode(args[0]);
            /*var data = JsonConvert.DeserializeObject<AnswerData>(jsonResponse);
            HandleIncomingOffer(data.roomId, data.fromUserId, data.offer);*/
            Debug.Log("<color=green>Response from OnVoiceQualityChangedByUserReceived </color>" + jsonResponse);

        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse push OnVoiceQualityChangedByUserReceived for : {ex.Message}");
        }

    }
    #endregion

    #region Public API Methods

    /* public void StartVoiceChat(string roomId)
     {
         Debug.Log("Starting voice chat...");
         currentRoomId = roomId;
 var data = new Dictionary<string, object> { { "roomId", currentRoomId } };
         LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.EmitEvents.startvoicechat, payloadData: data
               , onAckResponse: (response) =>
          {
              if (response != null && response.success)
              {

                  Debug.Log($" onAckResponse  received: {response.success} items");
              }
              else
              {
                  Debug.LogWarning("Failed to get response ");
              }
          }
       );

     }*/
    public void JoinVoiceSession(string roomId)
    {
        Debug.Log("Joining voice session...");
        currentRoomId = roomId;


        var data = new Dictionary<string, object> { { "roomId", currentRoomId } };
        LudoSocketManager.Instance.EmitWithAck<joinvoicesessionRepsonse>(GameConstants.EmitEvents.joinvoicesession,
         payloadData: data
              , onAckResponse: (response) =>
{
    Debug.Log(" Emit Joining voice session:" + response.data.participants.Length);
}


      );

    }

    void OnReceived_JoinVoiceSession(joinvoicesessionRepsonse response)
    {
        Debug.Log("<color=blue>OnReceived_JoinVoiceSession:</color>" + response.ToString());
        if (response != null && response.success)
        {


            // StartCoroutine(WebRTC.Update());
            isInVoiceSession = true;
            StartMicrophone();

            if (response.data.participants.Length > 0)
            {



                foreach (var participant in response.data.participants)
                {
                    Debug.Log("<color=blue>participant  in voice room</color>" + participant.id);

                    if (participant.id == HomePanel.instance.chatManager.OwnProfileData.id)
                    {

                        Debug.Log("<color=blue> Old participant enter in voice room</color>" + participant.id);



                    }
                    else
                    {
                        Debug.Log("<color=blue> New participant enter in voice room</color>" + participant.id);

                        CreatePeerConnection(participant.id, true); // We initiate
                    }

                }
            }
            Debug.Log($" onAckResponse  received: {response.success} items");
        }
        else
        {
            Debug.LogWarning("Failed to get response ");
        }
    }

    public void EndVoiceChat(string roomId)
    {
        var data = new Dictionary<string, object> { { "roomId", currentRoomId } };
        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.EmitEvents.leavevoicesession, payloadData: data
              , onAckResponse: (response) =>
         {
             if (response != null && response.success)
             {

                 Debug.Log($" onAckResponse  received: {response.success} items");
             }
             else
             {
                 Debug.LogWarning("Failed to get response ");
             }
         }
      );

    }



    public void LeaveVoiceSession()
    {
        if (!isInVoiceSession) return;

        var data = new Dictionary<string, object> { { "roomId", currentRoomId } };
        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.EmitEvents.leavevoicesession, payloadData: data
              , onAckResponse: (response) =>
         {
             if (response != null && response.success)
             {
                 CleanupVoiceSession();
                 Debug.Log($" onAckResponse  received: {response.success} items");
             }
             else
             {
                 Debug.LogWarning("Failed to get response ");
             }
         }
      );
    }

    public void SetAudioQuality(AudioQuality quality)
    {
        var qualityString = quality.ToString().ToLower();
        Debug.Log("SetAudioQuality currentRoomId:: " + currentRoomId);
        var data = new Dictionary<string, object> { { "roomId", currentRoomId }, { "quality", qualityString } };
        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.EmitEvents.voicequalitychange, payloadData: data
              , onAckResponse: (response) =>
         {
             if (response != null && response.success)
             {

                 Debug.Log($" onAckResponse voice-quality-change received: {response.success} items");
             }
             else
             {
                 Debug.LogWarning("Failed to get voice-quality-change ");
             }
         }
      );


    }

    internal void ToggleMicrophone()
    {
        isMicrophoneActive = !isMicrophoneActive;

        if (microphoneSource != null)
        {
            microphoneSource.mute = !isMicrophoneActive;
        }
        Debug.Log("Microphone is " + (isMicrophoneActive ? "unmuted" : "muted"));
        // Update local stream
        if (localStream != null)
        {
            foreach (var track in localStream.GetAudioTracks())
            {
                track.Enabled = isMicrophoneActive;
            }
        }
    }

    #endregion

    #region Socket Event Handlers

















    #endregion

    #region WebRTC Implementation

    private void CreatePeerConnection(string userId, bool shouldInitiate)
    {
        Debug.Log("CreatePeerConnection::");
        if (userId == HomePanel.instance.chatManager.OwnProfileData.id)
        {
            Debug.Log("<color=red> alredy Created PeerConnection</color>" + userId);
            return;
        }
        var configuration = new RTCConfiguration
        {
            iceServers = new RTCIceServer[]
     {
                new RTCIceServer { urls = new string[] { "stun:stun.l.google.com:19302" } },
                // new RTCIceServer { urls = new string[] { "stun:stun1.l.google.com:19302" } }
     }
        };

        var peerConnection = new RTCPeerConnection(ref configuration);
        peerConnections[userId] = peerConnection;
        Debug.Log("<color=red> total  peerConnections::</color> " + peerConnections.Count);

        if (localStream != null)
        {
            var audioTrack = localStream.GetAudioTracks().FirstOrDefault();
            foreach (var track in localStream.GetTracks())
            {
                Debug.Log("<color=orange> localStream track::</color> " + track.Kind.ToString());
                peerConnection.AddTrack(track, localStream);
                peerConnection.AddTransceiver(TrackKind.Audio);

            }
            if (audioTrack != null)
            {

            }
        }

        // Set up event handlers
        peerConnection.OnTrack += (RTCTrackEvent trackEvent) =>
        {
            Debug.Log("<color=orange>Track Event peerConnection.OnTrack::</color> " + userId);

            Debug.Log("<color=orange>Track Event peerConnection trackEvent.Track.Kind::</color> " + trackEvent.Track.Kind.ToString());
            if (trackEvent.Track.Kind == TrackKind.Audio)
            {
                //     mainThreadActions.Enqueue(() =>
                // {
                // if (trackEvent.Track is AudioStreamTrack audioTrack)
                {
                    Debug.Log("<color=orange>Track Event peerConnection. trackEvent.Track::</color> " + trackEvent.Track.Id);
                    SetupRemoteAudio(userId, trackEvent.Track as AudioStreamTrack);


                }
                // });
            }
        };

        peerConnection.OnIceCandidate += (RTCIceCandidate candidate) =>
        {
            Debug.Log("SendIceCandidate  SdpMLineIn dex:" + candidate.SdpMLineIndex);
            peerConnection.AddIceCandidate(candidate);
            // HandleWebRTCIceCandidate(userId, candidate);
            // SendIceCandidate(userId, candidate);
        };

        peerConnection.OnConnectionStateChange += (RTCPeerConnectionState state) =>
        {
            Debug.Log($"Peer connection with {userId}: {state}");

            if (state == RTCPeerConnectionState.Failed ||
                state == RTCPeerConnectionState.Disconnected)
            {
                RemovePeerConnection(userId);
            }
        };


        // If we should initiate, create and send offer
        if (shouldInitiate)
        {
            StartCoroutine(CreateAndSendOffer(userId, peerConnection));
        }
    }
    private System.Collections.IEnumerator NegotiationPeers(RTCPeerConnection localPeer, RTCPeerConnection remotePeer)
    {
        Debug.Log("NegotiationPeers ");
        var opCreateOffer = localPeer.CreateOffer();
        yield return opCreateOffer;

        if (opCreateOffer.IsError)
        {
            Debug.LogError($"Failed to create session description: {opCreateOffer.Error}");
            // OnCreateSessionDescriptionError(opCreateOffer.Error);
            yield break;
        }

        var offerDesc = opCreateOffer.Desc;
        yield return localPeer.SetLocalDescription(ref offerDesc);
        Debug.Log($"Offer from LocalPeer \n {offerDesc.sdp}");
        yield return remotePeer.SetRemoteDescription(ref offerDesc);

        var opCreateAnswer = remotePeer.CreateAnswer();
        yield return opCreateAnswer;

        if (opCreateAnswer.IsError)
        {
            Debug.LogError($"Failed to opCreateAnswer: {opCreateAnswer.Error}");
            //  OnCreateSessionDescriptionError(opCreateAnswer.Error);
            yield break;
        }

        var answerDesc = opCreateAnswer.Desc;
        yield return remotePeer.SetLocalDescription(ref answerDesc);
        Debug.Log($"Answer from RemotePeer \n {answerDesc.sdp}");
        yield return localPeer.SetRemoteDescription(ref answerDesc);
    }

    private System.Collections.IEnumerator CreateAndSendOffer(string userId, RTCPeerConnection peerConnection)
    {
        var offerOperation = peerConnection.CreateOffer();
        Debug.Log("CreateAndSendOffer:" + offerOperation.Desc.type.ToString());
        yield return offerOperation;

        if (offerOperation.IsError)
        {
            Debug.LogError($"Failed to create offer for {userId}: {offerOperation.Error.message}");
            yield break;
        }

        var offer = offerOperation.Desc;
        var setLocalDescOperation = peerConnection.SetLocalDescription(ref offer);
        yield return setLocalDescOperation;

        if (setLocalDescOperation.IsError)
        {
            Debug.LogError($"Failed to set local description for {userId}: {setLocalDescOperation.Error.message}");
            yield break;
        }


        // Send offer via socket
        string rate = GetSampleRateForQuality(defaultQuality).ToString();

        var oferrdata = new Dictionary<string, object>
        {
            { "type", offer.type },
              { "sdp", offer.sdp }
        };
        var data = new Dictionary<string, object>
    {
        { "roomId", currentRoomId },
        { "targetUserId", userId },
        { "offer", oferrdata },
        { "audioCodec", "opus" },
        { "sampleRate", rate }
    };

        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.WebRTC_EmitEvents.webrtcoffer,
        payloadData: data, onAckResponse: (response) =>
         {
             Debug.Log($" onAckResponse  received: {response} items");

         }
      );


    }

    private void HandleWebRTCOffer(string fromUserId, RTCSessionDescription offer)
    {
        if (!peerConnections.ContainsKey(fromUserId))
        {
            CreatePeerConnection(fromUserId, false);
        }

        var peerConnection = peerConnections[fromUserId];
        StartCoroutine(HandleOfferCoroutine(fromUserId, peerConnection, offer));
    }

    private System.Collections.IEnumerator HandleOfferCoroutine(string fromUserId, RTCPeerConnection peerConnection, RTCSessionDescription offer)
    {
        var setRemoteDescOperation = peerConnection.SetRemoteDescription(ref offer);
        yield return setRemoteDescOperation;

        if (setRemoteDescOperation.IsError)
        {
            Debug.LogError($"Failed to set remote description from {fromUserId}: {setRemoteDescOperation.Error.message}");
            yield break;
        }

        var answerOperation = peerConnection.CreateAnswer();
        yield return answerOperation;

        if (answerOperation.IsError)
        {
            Debug.LogError($"Failed to create answer for {fromUserId}: {answerOperation.Error.message}");
            yield break;
        }

        var answer = answerOperation.Desc;
        var setLocalDescOperation = peerConnection.SetLocalDescription(ref answer);
        yield return setLocalDescOperation;

        if (setLocalDescOperation.IsError)
        {
            Debug.LogError($"Failed to set local description for {fromUserId}: {setLocalDescOperation.Error.message}");
            yield break;
        }

        // Send answer via socket


        var answerdata = new Dictionary<string, object>
        {
            { "type", answer.type },
              { "sdp", answer.sdp }
        };
        var data = new Dictionary<string, object>
    {
        { "roomId", currentRoomId },
        { "targetUserId", fromUserId },
        { "answer", answerdata }
    };
        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.WebRTC_EmitEvents.webrtcanswer,
        payloadData: data, onAckResponse: (response) =>
         {
             if (response != null && response.success)
             {

                 Debug.Log($" onAckResponse  received: {response.success} items");
             }
             else
             {
                 Debug.LogWarning("Failed to get response ");
             }
         }
      );


    }

    private void HandleWebRTCAnswer(string userId, RTCSessionDescription answer)
    {
        if (!peerConnections.ContainsKey(userId)) return;

        var peerConnection = peerConnections[userId];
        StartCoroutine(SetRemoteDescription(peerConnection, answer));
    }

    private System.Collections.IEnumerator SetRemoteDescription(RTCPeerConnection peerConnection, RTCSessionDescription description)
    {
        var setRemoteDescOperation = peerConnection.SetRemoteDescription(ref description);
        yield return setRemoteDescOperation;

        if (setRemoteDescOperation.IsError)
        {
            Debug.LogError($"Failed to set remote description: {setRemoteDescOperation.Error.message}");
        }
    }

    private void HandleWebRTCIceCandidate(string userId, RTCIceCandidate candidate)
    {
        if (!peerConnections.ContainsKey(userId)) return;
        Debug.Log("HandleWebRTCIceCandidate::" + candidate.Candidate);
        var peerConnection = peerConnections[userId];
        peerConnection.AddIceCandidate(candidate);
        Debug.Log("HandleWebRTCIceCandidate peerConnection::" + peerConnection.GetReceivers().ToArray().Length);
        foreach (var receiver in peerConnection.GetReceivers())
        {
            Debug.Log("HandleWebRTCIceCandidate peerConnection receiver::" + receiver.Track.Id);
        }
        //  Debug.Log("HandleWebRTCIceCandidate peerConnection::" + peerConnection.GetReceivers().ToArray().Length);
    }

    private void SendIceCandidate(string targetUserId, RTCIceCandidate candidate)
    {
        if (candidate == null || string.IsNullOrEmpty(candidate.Candidate))
        {
            Debug.LogWarning("Invalid ICE candidate, skipping send.");
            return;
        }
        var candidateData = new Dictionary<string, object>
    {
        { "candidate", candidate.Candidate },
        { "sdpMid", candidate.SdpMid },
        { "sdpMLineIndex", candidate.SdpMLineIndex }
    };
        candidateData.TryGetValue("candidate", out var namecan);
        Debug.Log("SendIceCandidate SdpMLineIndex::" + candidate.SdpMLineIndex + "::SendIceCandidate  SdpMid::" + candidate.SdpMid);

        var data = new Dictionary<string, object>
    {
        { "roomId", currentRoomId },
        { "targetUserId", targetUserId },
        { "candidate", candidateData }
    };

        LudoSocketManager.Instance.EmitWithAck<SendIceCandidateReposne>(
            GameConstants.WebRTC_EmitEvents.webrtcicecandidate,
            payloadData: data,
            onAckResponse: (response) =>
            {
                Debug.Log($"ICE candidate sent. Ack: {response?.success}");
                //  HandleWebRTCIceCandidate();
            }
        );
    }

    internal void RemovePeerConnection(string userId)
    {
        if (peerConnections.ContainsKey(userId))
        {
            if (peerConnections[userId] != null)
            {
                lock (peerConnections)
                {
                    peerConnections[userId].Close();
                    peerConnections[userId].Dispose();
                    peerConnections.Remove(userId);
                    Debug.Log("RemovePeerConnection l");
                }
            }
        }
    }
    void RemoveRemoteAudio(string userId)
    {
        if (remoteAudioSources.ContainsKey(userId))
        {
            var remoteAudioSource = remoteAudioSources[userId];
            if (remoteAudioSource != null)
            {
                Destroy(remoteAudioSource.gameObject);
            }
            remoteAudioSources.Remove(userId);
        }
    }
    internal void RemoveSingleWebRtcConncetionAndData(string userId)
    {
        Debug.Log("RemovePeerConnection");
        RemovePeerConnection(userId);
        RemoveSpeakingIndicator(userId);
        RemoveRemoteAudio(userId);
    }

    #endregion

    #region Audio Management

    private void StartMicrophone()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone devices found");
            return;
        }

        string micDevice = Microphone.devices[0];
        int sampleRate = GetSampleRateForQuality(defaultQuality);

        // Play microphone locally
        microphoneSource.clip = Microphone.Start(micDevice, true, 1, sampleRate);
        microphoneSource.loop = true;
        while (!(Microphone.GetPosition(micDevice) > 0)) { } // Wait until mic starts
        microphoneSource.Play();

        // Create WebRTC stream
        localStream = new MediaStream();
        var audioTrack = new AudioStreamTrack(microphoneSource);
        localStream.AddTrack(audioTrack);

        Debug.Log("<color=orange>Microphone localStream Track:: </color>" + audioTrack.Id);

        isMicrophoneActive = true;

        // Start monitoring speaking
        StartCoroutine(MonitorSpeaking(audioTrack));
    }

    private void SetupRemoteAudio(string userId, AudioStreamTrack track)
    {
        if (remoteAudioSources.ContainsKey(userId))
        {
            var remoteAudioSource = remoteAudioSources[userId];
            remoteAudioSource.SetTrack(track);
            remoteAudioSource.Play();
            Debug.Log("went here or there 1");
            return;
        }

        Debug.Log("went here or there");
        // Create a GameObject for remote audio
        GameObject audioObject = new GameObject($"RemoteAudio_{userId}");
        audioObject.transform.SetParent(transform);

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = true;

        // Create an AudioClip to hold incoming audio (1 second buffer)
        int sampleRate = 48000; // max supported rate
        int channels = 1;
        int clipLength = sampleRate * channels;
        AudioClip clip = AudioClip.Create($"RemoteAudioClip_{userId}", bufferSize, channels, sampleRate, true, OnAudioRead);
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.SetTrack(track);
        audioSource.Play();

        float[] audioBuffer = new float[clipLength];
      
        // On receiving audio from WebRTC
        track.onReceived += (float[] data, int ch, int rate) =>
        {


            // var segment = buffer.GetData(); // returns ArraySegment<float>
            // var data = segment.Array;
            mainThreadActions.Enqueue(() =>
           {
               if (data == null && data.Length == 0) return;
               Debug.Log("Test:: On Received Track event invoked : " + data.Length);
               writePos = 0;
               lock (audioBuffer)
               {
                   for (int i = 0; i < data.Length; i++)
                   {
                       audioBuffer[writePos] = data[i];
                       writePos = (writePos + 1) % audioBuffer.Length;
                   }
               }
           });
        };

        remoteAudioSources[userId] = audioSource;
    }

    void OnAudioRead(float[] data)
    {
        if (audioBuffer == null || data == null)
            return;
        lock (audioBuffer)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = audioBuffer[readPos];
                readPos = (readPos + 1) % audioBuffer.Length;
            }
        }
    }

    private float[] audioBuffer;
    private int writePos = 0;
    private int readPos = 0;
    private bool hasAudio = false;

    private const int bufferSize = 48000 * 2; // 2 seconds buffer
    private int channels = 1;
    // private void OnAudioFilterRead(float[] data, int channels)
    // {
    //     if (!hasAudio) return;

    //     lock (audioBuffer)
    //     {
    //         for (int i = 0; i < data.Length; i++)
    //         {
    //             data[i] = audioBuffer[readPos];
    //             readPos = (readPos + 1) % bufferSize;
    //         }
    //     }
    // }



    private System.Collections.IEnumerator MonitorSpeaking(AudioStreamTrack audioTrack)
    {
        while (isInVoiceSession && isMicrophoneActive)
        {
            if (microphoneSource.clip != null && Microphone.IsRecording(Microphone.devices[0]))
            {
                float[] samples = new float[256];
                int micPosition = Microphone.GetPosition(Microphone.devices[0]);

                if (micPosition > samples.Length)
                {
                    microphoneSource.clip.GetData(samples, micPosition - samples.Length);

                    float sum = 0f;
                    foreach (float sample in samples)
                    {
                        sum += Mathf.Abs(sample);
                    }
                    float average = sum / samples.Length;

                    bool shouldSpeak = average > speakingThreshold;

                    if (shouldSpeak && !isSpeaking)
                    {
                        isSpeaking = true;
                        lastSpeakingTime = Time.time;
                        SendSpeakingState(true);
                    }
                    else if (!shouldSpeak && isSpeaking && Time.time - lastSpeakingTime > speakingCooldown)
                    {
                        isSpeaking = false;
                        SendSpeakingState(false);
                    }
                    else if (shouldSpeak && isSpeaking)
                    {
                        lastSpeakingTime = Time.time; // Reset cooldown
                    }
                }
            }

            yield return new WaitForSeconds(0.1f); // Check 10 times per second
        }
    }

    private void SendSpeakingState(bool speaking)
    {
        string eventName = speaking ? GameConstants.EmitEvents.voicespeakingstart : GameConstants.EmitEvents.voicespeakingstop;
        var data = new Dictionary<string, object> { { "roomId", currentRoomId } };
        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(eventName, payloadData: data
              , onAckResponse: (response) =>
         {
             if (response != null && response.success)
             {

                 Debug.Log($" onAckResponse SendSpeakingState received: {response.success} items");
             }
             else
             {
                 Debug.LogWarning("Failed to get response ");
             }
         }
      );

    }

    private int GetSampleRateForQuality(AudioQuality quality)
    {
        switch (quality)
        {
            case AudioQuality.Low: return 16000;
            case AudioQuality.Medium: return 24000;
            case AudioQuality.High: return 48000;
            default: return 24000;
        }
    }

    #endregion

    #region UI Management

    private void ShowSpeakingIndicator(string userId, bool speaking)
    {
        if (speaking)
        {
            if (!speakingIndicators.ContainsKey(userId))
            {
                var indicator = Instantiate(speakingIndicatorPrefab, speakingIndicatorsParent);
                var indicatorScript = indicator.GetComponent<SpeakingIndicator>();
                if (indicatorScript != null)
                {
                    indicatorScript.SetUser(userId);
                }
                speakingIndicators[userId] = indicator.gameObject;
            }
            speakingIndicators[userId].SetActive(true);
        }
        else
        {
            if (speakingIndicators.ContainsKey(userId))
            {
                speakingIndicators[userId].SetActive(false);
            }
        }
    }

    private void RemoveSpeakingIndicator(string userId)
    {
        if (speakingIndicators.ContainsKey(userId))
        {
            Debug.Log(" remove RemoveSpeakingIndicator");
            Destroy(speakingIndicators[userId]);
            speakingIndicators.Remove(userId);
        }
    }

    #endregion

    #region Utility Methods

    private void HandleSocketResponse(object response, string operation,
        System.Action<Dictionary<string, object>> success = null,
        System.Action<string> error = null)
    {
        try
        {
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.ToString());

            if (responseData.ContainsKey("success") && (bool)responseData["success"])
            {
                var data = responseData.ContainsKey("data") ?
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(responseData["data"].ToString()) :
                    new Dictionary<string, object>();
                success?.Invoke(data);
            }
            else
            {
                string errorMessage = responseData.ContainsKey("error") ?
                    responseData["error"].ToString() : "Unknown error";
                error?.Invoke(errorMessage);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing response for {operation}: {ex.Message}");
            error?.Invoke(ex.Message);
        }
    }

   
    internal void CleanupVoiceSession()
    {
        Debug.Log("CleanupVoiceSession " + peerConnections.Count);

        // Stop microphone
        if (Microphone.devices.Length > 0)
        {
            string micDevice = Microphone.devices[0];
            if (Microphone.IsRecording(micDevice))
            {
                Debug.Log("Microphone::" + micDevice);
                Microphone.End(micDevice);
            }
        }

        // Iterate over a copy of the keys to safely remove
        foreach (var userId in peerConnections.Keys.ToList())
        {
            Debug.Log("Cleaning peerConnections::" + userId);
            RemovePeerConnection(userId);
        }

        foreach (var userId in remoteAudioSources.Keys.ToList())
        {
            Debug.Log("Cleaning remoteAudioSources::" + userId);
            RemoveRemoteAudio(userId);
        }

        foreach (var userId in speakingIndicators.Keys.ToList())
        {
            Debug.Log("Cleaning speakingIndicators::" + userId);
            RemoveSpeakingIndicator(userId);
        }

        // Clear dictionaries
        peerConnections.Clear();
        remoteAudioSources.Clear();
        speakingIndicators.Clear();

        localStream = null;
        isInVoiceSession = false;
        isMicrophoneActive = false;
        isSpeaking = false;
        currentRoomId = "";

        Debug.Log("Voice session cleaned up");
    }

    #endregion

    private void OnDestroy()
    {
        CleanupVoiceSession();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && isInVoiceSession)
        {
            // Handle app pause - might want to leave voice session
            ToggleMicrophone(); // Mute microphone when app is paused
        }
    }
}