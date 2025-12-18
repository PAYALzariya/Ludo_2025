using BestHTTP.JSON;
using BestHTTP.SocketIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.WebRTC;
using UnityEngine;


public enum AudioQuality
{
    Low,
    Medium,
    High
}

[Serializable]
public class RemoteUserAudio
{
    public string userId;
    public AudioSource audioSource;
}

public class GroupVoiceChat_OnlyWebrtc : MonoBehaviour
{


    [Header("Voice Chat Settings")]
    public AudioQuality defaultQuality = AudioQuality.Medium;
    [Header("Local Audio")]
    public AudioSource microphoneSource;
    internal AudioStreamTrack localAudioTrack;

    [Header("Peer Connections")]
    private Dictionary<string, RTCPeerConnection> peerConnections = new Dictionary<string, RTCPeerConnection>();
    private Dictionary<string, AudioSource> remoteAudioSources = new Dictionary<string, AudioSource>();

    [Header("Socket")]
    public Socket socket; // Assign from your Socket.IO manager private AudioSource microphoneSource;
    private bool isInVoiceSession = false;
    public bool isMicrophoneActive = false;
    private bool isSpeaking = false;
    private float lastSpeakingTime;

    private   void OnEnable()
    {

        // Setup local audio
        InitLocalAudio();

        // Setup socket listeners

        Invoke("AddSockt", 1f);
      

    }
    void AddSockt()
    {
      socket = LudoSocketManager.Instance.socket;
        Debug.Log("socket: " + socket.IsOpen);


        SetupSocketListeners();
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
    private void OnConnectWithwebrtc(Socket socket, Packet packet, object[] args)
    {
        Debug.Log(">>>  WE ARE CONNECTED With webrtc! <<<" + args);


    }
    #region conncetWithCodeMethods
    public void InitLocalAudio()
    {
        if (localAudioTrack != null) return; // already initialized

        // Start microphone
        AudioClip micClip = Microphone.Start(null, true, 10, 48000);

        // Create an AudioSource so you can hear yourself (optional)
        AudioSource audioSource = microphoneSource;
        /* if (gameObject.GetComponent<AudioSource>())
         {
             audioSource = gameObject.GetComponent<AudioSource>();
         }
         else
         {


             audioSource = gameObject.AddComponent<AudioSource>();
         }*/
        audioSource.clip = micClip;
        audioSource.loop = true;
        audioSource.Play();

        GetMicPermisson();
        // Create the WebRTC audio track from this AudioSource
        // localAudioTrack = new AudioStreamTrack(audioSource);
    }
    IEnumerator GetMicPermisson()
    {
         if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
    {
        Debug.Log("Requesting microphone permission...");
        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
    }

    if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
    {
        Debug.LogError("Microphone permission denied!");
        yield break;
    }
    }
    internal void CreatePeerConnectionForUser(string roomId, string userId, bool shouldInitiate)
    {
        Debug.Log("CreatePeerConnectionForUser");
        if (peerConnections.ContainsKey(userId)) return;

        var config = new RTCConfiguration
        {
            iceServers = new RTCIceServer[]
            {
            new RTCIceServer { urls = new string[] { "stun:stun.l.google.com:19302" } }
            }
        };


        var pc = new RTCPeerConnection(ref config);
        peerConnections[userId] = pc;

        Debug.Log("peerConnections count::" + peerConnections.Count + "::localaudio::" + localAudioTrack);
        if (localAudioTrack != null)
            pc.AddTrack(localAudioTrack);





        // When we receive audio from this peer
        pc.OnTrack = ev =>
        {
            Debug.Log("pc.tran:" + ev.Track);
            if (ev.Track is AudioStreamTrack audioTrack)
            {
                Debug.Log("ev .Track::" + ev.Track);
                SetupRemoteAudio(userId, audioTrack);
            }
            else
            {
                Debug.LogWarning("Received non-audio track, skipping SetupRemoteAudio.");
            }
        };
        // When ICE candidates are found

        pc.OnIceCandidate = (RTCIceCandidate candidate) =>
        {
            Emit_SendIceCandidate(roomId, userId, candidate);
        };

        // If we are the one starting, create an Offer
        if (shouldInitiate)
        {
            StartCoroutine(Emit_CreateAndSendOffer(roomId, userId, pc));
        }
    }
    public void RemovePeer(string userId)
    {
        if (peerConnections.ContainsKey(userId))
        {
            peerConnections[userId].Close();
            peerConnections.Remove(userId);
        }

        if (remoteAudioSources.ContainsKey(userId))
        {
            Destroy(remoteAudioSources[userId].gameObject);
            remoteAudioSources.Remove(userId);
        }
    }
    internal IEnumerator StartMicrophone()
    {
        Debug.Log("StartMicrophone: devices=" + Microphone.devices.Length);

        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone devices found");
            yield break;
        }

        // Ensure AudioSource exists
        if (microphoneSource == null)
            microphoneSource = gameObject.AddComponent<AudioSource>();

        string micDevice = Microphone.devices[0];
        int sampleRate = GetSampleRateForQuality(defaultQuality);
        Debug.Log("StartMicrophone sampleRate: " + sampleRate);

        microphoneSource.clip = Microphone.Start(micDevice, true, 10, sampleRate);

        // Wait until mic starts recording
       while (Microphone.GetPosition(micDevice) <= 0)
        {
  Debug.Log("get  microphoneSource.position: " + microphoneSource.isPlaying);
            yield return null;
        }

        microphoneSource.loop = false;
        microphoneSource.Play();

        Debug.Log("StartMicrophone microphoneSource.isPlaying: " + microphoneSource.isPlaying);

        // Create WebRTC audio track
        localAudioTrack = new AudioStreamTrack(microphoneSource);
        Debug.Log("StartMicrophone localAudioTrack created");

        isMicrophoneActive = true;
    }
    public void MuteMicrophone()
    {
        if (localAudioTrack == null) return;

        foreach (var pc in peerConnections.Values)
        {
            var sender = pc.GetSenders().FirstOrDefault(s => s.Track == localAudioTrack);
            if (sender != null)
                pc.RemoveTrack(sender); // stop sending audio
        }
    }
    public void UnmuteMicrophone()
    {
        if (localAudioTrack == null) return;

        foreach (var pc in peerConnections.Values)
        {
            pc.AddTrack(localAudioTrack); // start sending audio again
        }
    }
    private void SetupRemoteAudio(string userId, AudioStreamTrack stream)
    {
        GameObject go = new GameObject($"RemoteAudio_{userId}");
        var source = go.AddComponent<AudioSource>();
        source.loop = true;
        source.Play();
        // Assign the audio track to the AudioSource
        source.SetTrack(stream);

        Debug.Log($"Remote audio setup complete for user {userId}");
    }



    public void HandleIncomingOffer(string roomId, string fromUserId, RTCSessionDescription offer)
    {
        if (!peerConnections.ContainsKey(fromUserId))
            CreatePeerConnectionForUser(roomId, fromUserId, false);

        var pc = peerConnections[fromUserId];
        StartCoroutine(Emit_HandleOfferCoroutine(roomId, fromUserId, pc, offer));
    }


    // Add ICE candidate from remote
    public void AddRemoteIceCandidate(string userId, RTCIceCandidate candidate)
    {
        if (peerConnections.ContainsKey(userId))
        {
            peerConnections[userId].AddIceCandidate(candidate);
        }
    }
    private void notUsed_SetupRemoteAudio(string userId, MediaStreamTrack track)
    {
        if (!(track is AudioStreamTrack audioTrack))
        {
            Debug.LogWarning("Track is not an AudioStreamTrack, skipping");
            return;
        }

        if (!remoteAudioSources.ContainsKey(userId))
        {
            var source = gameObject.AddComponent<AudioSource>();
            remoteAudioSources[userId] = source;
            source.loop = true;
            source.Play();
        }

        remoteAudioSources[userId].SetTrack(audioTrack);
    }
    #endregion
    #region EmiteMethod
    private IEnumerator Emit_CreateAndSendOffer(string roomid, string userId, RTCPeerConnection peerConnection)
    {
        Debug.Log("Emit_CreateAndSendOffer");
        var offerOperation = peerConnection.CreateOffer();
        yield return offerOperation;
Debug.Log("rutn Emit_CreateAndSendOffer");
        if (offerOperation.IsError)
        {
            Debug.LogError($"Failed to create offer for {userId}: {offerOperation.Error.message}");
            yield break;
        }
Debug.Log("offerOperation Emit_CreateAndSendOffer");
        var offer = offerOperation.Desc;
        var setLocalDescOperation = peerConnection.SetLocalDescription(ref offer);
        yield return setLocalDescOperation;

        if (setLocalDescOperation.IsError)
        {
            Debug.LogError($"Failed to set local description for {userId}: {setLocalDescOperation.Error.message}");
            yield break;
        }

        // Send offer via socket
        var offerData = new
        {
            roomId = roomid,
            targetUserId = userId,
            offer = offer,
            audioCodec = "opus",
            sampleRate = GetSampleRateForQuality(defaultQuality)
        };

Debug.Log("rutn webrtcoffer");
        LudoSocketManager.Instance.EmitWithAck<GrouptextchatResponse>(
            GameConstants.WebRTC_EmitEvents.webrtcoffer, payloadData: offerData, onAckResponse: (response) =>
         {
             if (response != null && response.success)
             {

                 Debug.Log($" onAckResponse webrtcoffer received: {response.data.message} items");
             }
             else
             {
                 Debug.LogWarning("Failed to get webrtcoffer ");
             }
         }
      );
    }
    private void Emit_SendIceCandidate(string roomid, string userId, RTCIceCandidate candidate)
    {
        Debug.Log("Emit_SendIceCandidate");
        var candidateData = new
        {
            roomId = roomid,
            targetUserId = userId,
            candidate = candidate
        };

        LudoSocketManager.Instance.EmitWithAck<GrouptextchatResponse>
        (GameConstants.WebRTC_EmitEvents.webrtcicecandidate, payloadData: candidateData, onAckResponse: (response) =>
          {
              if (response != null && response.success)
              {

                  Debug.Log($" onAckResponse webrtcicecandidate received: {response.data.message} items");
              }
              else
              {
                  Debug.LogWarning("Failed to get webrtcicecandidate ");
              }
          }
       );
    }
    private IEnumerator Emit_HandleOfferCoroutine(string roomId, string userId, RTCPeerConnection pc, RTCSessionDescription offer)
    {
        var setRemoteOp = pc.SetRemoteDescription(ref offer);
        yield return setRemoteOp;

        if (setRemoteOp.IsError)
        {
            Debug.LogError($"SetRemoteDescription failed: {setRemoteOp.Error.message}");
            yield break;
        }

        var answerOp = pc.CreateAnswer();
        yield return answerOp;

        if (answerOp.IsError)
        {
            Debug.LogError($"CreateAnswer failed: {answerOp.Error.message}");
            yield break;
        }

        var answer = answerOp.Desc;
        var setLocalOp = pc.SetLocalDescription(ref answer);
        yield return setLocalOp;

        if (setLocalOp.IsError)
        {
            Debug.LogError($"SetLocalDescription for answer failed: {setLocalOp.Error.message}");
            yield break;
        }

        // Send Answer via socket
        var answerData = new
        {
            roomId = roomId,
            targetUserId = userId,
            answer = answer
        };


        LudoSocketManager.Instance.EmitWithAck<GrouptextchatResponse>(
            GameConstants.WebRTC_EmitEvents.webrtcanswer, payloadData: answerData, onAckResponse: (response) =>
         {
             if (response != null && response.success)
             {

                 Debug.Log($" onAckResponse webrtcoffer received: {response.data.message} items");
             }
             else
             {
                 Debug.LogWarning("Failed to get webrtcoffer ");
             }
         }
      );
    }
    #endregion
    #region Public method Access Handle
    internal IEnumerator OnJoinedUser_addVoice(string roomid, string userid, bool isHost)
    {
        Debug.Log("OnJoinedUser_addVoice :: " + userid + " host? " + isHost);

        if (isHost)
        {
            // ✅ Host starts microphone (only once!)
            if (!isMicrophoneActive)
            {
                StartCoroutine(StartMicrophone());
                yield return StartCoroutine(StartMicrophone());
            }

            // Host does not need to create peer connection for himself
            // Just wait until others join
        }
        else
        {
            // ✅ Audience joins:
            // They don’t record mic, they just prepare for remote audio from host
            CreatePeerConnectionForUser(roomid, userid, false);
        }

        /* Debug.Log("OnJoinedUser_addVoice");
         if (!isMicrophoneActive)
         {

         yield return StartCoroutine(StartMicrophone());
         }
         // MuteMicrophone();
         CreatePeerConnectionForUser(roomid, userid, inits);*/
    }
    #endregion
    #region Socket Events
    internal void SetupSocketListeners()
    {

        On_webrtcofferreceived();
        On_webrtcanswerreceived();
        On_webrtcicecandidatereceived();
    }
    void On_webrtcofferreceived()
    {
        socket.On(GameConstants.WebRTC_ONEvents.webrtcofferreceived, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                /*var data = JsonConvert.DeserializeObject<AnswerData>(jsonResponse);
                HandleIncomingOffer(data.roomId, data.fromUserId, data.offer);*/
                Debug.Log("<color=green>Response from webrtcofferreceived </color>" + jsonResponse);

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push webrtcofferreceived for : {ex.Message}");
            }
        });
    }
    void On_webrtcanswerreceived()
    {
        socket.On(GameConstants.WebRTC_ONEvents.webrtcanswerreceived, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                /*  var data = JsonConvert.DeserializeObject<AnswerData>(packet.RawData);
             HandleIncomingAnswer(data.fromUserId, data.answer);*/
                Debug.Log("<color=green>Response from webrtcanswerreceived </color>" + jsonResponse);

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push webrtcanswerreceived for : {ex.Message}");
            }
        });
    }
    void On_webrtcicecandidatereceived()
    {
        socket.On(GameConstants.WebRTC_ONEvents.webrtcicecandidatereceived, (s, p, args) =>
        {
            if (args.Length == 0) return;
            try
            {
                string jsonResponse = Json.Encode(args[0]);
                /* var data = JsonConvert.DeserializeObject<IceCandidateData>(jsonResponse);
                  AddRemoteIceCandidate(data.fromUserId, data.candidate);*/
                Debug.Log("<color=green>Response from webrtcicecandidatereceived </color>" + jsonResponse);

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse push webrtcicecandidatereceived for : {ex.Message}");
            }
        });
    }

    #endregion
    private void Update()
{
    if (microphoneSource != null && microphoneSource.clip != null)
    {
        float[] samples = new float[128];
        int micPos = Microphone.GetPosition(null) - samples.Length;
        if (micPos < 0) return;

        microphoneSource.clip.GetData(samples, micPos);
        float sum = 0;
        foreach (var s in samples) sum += Mathf.Abs(s);

        float amplitude = sum / samples.Length;
        // For example, show amplitude in UI
        //Debug.Log("Mic amplitude: " + amplitude);
    }
}
}
