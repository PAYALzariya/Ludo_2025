using UnityEngine;
using Unity.WebRTC;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using BestHTTP.SocketIO;
using System.Collections;

public class GroupVoiceChatManager : MonoBehaviour
{

    public GameObject remoteAudioPrefab;
    private AudioSource microphoneSource;
    private MediaStream localStream;
    private Dictionary<string, RTCPeerConnection> peerConnections = new Dictionary<string, RTCPeerConnection>();
    private Dictionary<string, AudioSource> remoteAudioSources = new Dictionary<string, AudioSource>();
    private readonly ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();
    private Socket socket;
   

    private void OnEnable()
    {
       
        if(gameObject.GetComponent<AudioSource>() == null)
        {

            microphoneSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
                       microphoneSource = gameObject.GetComponent<AudioSource>();
        }
            microphoneSource.loop = true;
        microphoneSource.mute = true;

        HandleSocketConnected(LudoSocketManager.Instance.socket);
    }

    private void HandleSocketConnected(Socket connectedSocket)
    {
        socket = connectedSocket;
        SetupSocketListeners();
    }

    private void Update()
    {
        while (mainThreadActions.TryDequeue(out var action))
            action?.Invoke();
    }

    #region Local Audio
    public void StartLocalAudio()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("No microphone found. Joining as listener only.");
            return;
        }
        else
        {
            Debug.Log("Microphone found: " + Microphone.devices[0]);
        }

        localStream = new MediaStream();
        var micClip = Microphone.Start(null, true, 10, 48000);
        microphoneSource.clip = micClip;
        microphoneSource.Play();

        var audioTrack = new AudioStreamTrack(microphoneSource);
        localStream.AddTrack(audioTrack);
    }
    #endregion

    #region Socket.IO Listeners
    private void SetupSocketListeners()
    {
        
       /* var data = new Dictionary<string, object> { { "roomId",HomePanel.instance.chatManager.CurrentRoom.data.room.id } };
        LudoSocketManager.Instance.EmitEvent_With_responseEventHanbleBothCallBack<start_voice_chatResponse>(emitEvent: "start-voice-chat", responseEvent: "voice_chat_started", payloadData: data,
            onPushResponse: (response) =>
            {
                if (response != null && response.success)
                {

                    Debug.Log($"Handle onPushResponse  started-voice-chat received: {response.data.roomId} items");
                    //voiceChatManager. currentRoomId = response.data.roomId.ToString();
                    StartLocalAudio();
                    CreatePeerConnection(DataManager.instance.userId, false);
                }
                else
                {
                    Debug.LogWarning("Failed to get start-voice-chat ");
                }
            }
);*/
    
    socket.On("voice_chat_ended", (s, p, args) =>
        {
            Debug.Log("[Socket] voice_chat_ended received");
            mainThreadActions.Enqueue(() => CleanupAll());
        });

        socket.On("voice_session_joined", (s, p, args) =>
        {
            string newUserId = args[0].ToString();
            Debug.Log($"[Socket] voice_session_joined: {newUserId}");
            mainThreadActions.Enqueue(() => CreatePeerConnection(newUserId, true));
        });

        socket.On("voice_session_left", (s, p, args) =>
        {
            string userId = args[0].ToString();
            Debug.Log($"[Socket] voice_session_left: {userId}");
            mainThreadActions.Enqueue(() => RemovePeerConnection(userId));
        });

        socket.On("voice-participant-joined", (s, p, args) =>
        {
            string userId = args[0].ToString();
            Debug.Log($"[Socket] voice-participant-joined: {userId}");
            mainThreadActions.Enqueue(() => CreatePeerConnection(userId, true));
        });

        socket.On("voice-participant-left", (s, p, args) =>
        {
            string userId = args[0].ToString();
            Debug.Log($"[Socket] voice-participant-left: {userId}");
            mainThreadActions.Enqueue(() => RemovePeerConnection(userId));
        });

        // WebRTC signaling
        socket.On("webrtc-offer-received", (s, p, args) =>
        {
            var data = args[0] as Dictionary<string, object>;
            string fromUserId = data["fromUserId"].ToString();
            string sdp = data["offer"].ToString();
            mainThreadActions.Enqueue(() => StartCoroutine(HandleOffer(fromUserId, sdp)));
        });

        socket.On("webrtc-answer-received", (s, p, args) =>
        {
            var data = args[0] as Dictionary<string, object>;
            string fromUserId = data["fromUserId"].ToString();
            string sdp = data["answer"].ToString();
            mainThreadActions.Enqueue(() => StartCoroutine(HandleAnswer(fromUserId, sdp)));
        });

        socket.On("webrtc-ice-candidate-received", (s, p, args) =>
        {
            var data = args[0] as Dictionary<string, object>;
            string fromUserId = data["fromUserId"].ToString();
            var candidate = data["candidate"] as RTCIceCandidateInit;
            mainThreadActions.Enqueue(() =>
            {
                if (peerConnections.TryGetValue(fromUserId, out var pc))
                    pc.AddIceCandidate(new RTCIceCandidate(candidate));
            });
        });
    }
    #endregion

    #region WebRTC Methods
    public void CreatePeerConnection(string remoteUserId, bool isInitiator)
    {
        if (peerConnections.ContainsKey(remoteUserId)) return;

        var config = new RTCConfiguration
        {
            iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } } }
        };

        var pc = new RTCPeerConnection(ref config);

        pc.OnIceCandidate = candidate =>
        {
            socket.Emit("webrtc-ice-candidate", new Dictionary<string, object>
            {
                { "target", remoteUserId },
                { "candidate", candidate }
            });
        };

        pc.OnTrack = e => mainThreadActions.Enqueue(() => SetupRemoteAudio(remoteUserId, e.Track));
        pc.OnNegotiationNeeded = () =>
        {
            if (isInitiator) StartCoroutine(CreateAndSendOffer(pc, remoteUserId));
        };

        if (localStream != null)
        {
            foreach (var track in localStream.GetTracks())
                pc.AddTrack(track, localStream);
        }
        Debug.Log($"Created PeerConnection with {remoteUserId}, Initiator: {isInitiator}, Count:{peerConnections.Count}");
       

        peerConnections.Add(remoteUserId, pc); // add at the end
        
    }

    private IEnumerator CreateAndSendOffer(RTCPeerConnection pc, string remoteUserId)
    {
        var op = pc.CreateOffer();
        yield return op;
        if (!op.IsError)
        {
            var offer = op.Desc;
            yield return pc.SetLocalDescription(ref offer);
            socket.Emit("webrtc-offer", new Dictionary<string, object> { { "target", remoteUserId }, { "offer", offer } });
        }
    }

    private IEnumerator HandleOffer(string fromUserId, string sdp)
    {
        if (!peerConnections.ContainsKey(fromUserId))
            CreatePeerConnection(fromUserId, false);

        var pc = peerConnections[fromUserId];
        var desc = new RTCSessionDescription { type = RTCSdpType.Offer, sdp = sdp };
        yield return pc.SetRemoteDescription(ref desc);

        var answerOp = pc.CreateAnswer();
        yield return answerOp;
        if (!answerOp.IsError)
        {
            var answer = answerOp.Desc;
            yield return pc.SetLocalDescription(ref answer);
            socket.Emit("webrtc-answer", new Dictionary<string, object> { { "target", fromUserId }, { "answer", answer } });
        }
    }

    private IEnumerator HandleAnswer(string fromUserId, string sdp)
    {
        if (!peerConnections.TryGetValue(fromUserId, out var pc)) yield break;
        var desc = new RTCSessionDescription { type = RTCSdpType.Answer, sdp = sdp };
        yield return pc.SetRemoteDescription(ref desc);
    }
    #endregion

    #region Remote Audio
    private void SetupRemoteAudio(string userId, MediaStreamTrack track)
    {
        if (track.Kind != TrackKind.Audio) return;

        if (!remoteAudioSources.TryGetValue(userId, out var audio))
        {
            var go = Instantiate(remoteAudioPrefab, transform);
            audio = go.GetComponent<AudioSource>();
            remoteAudioSources[userId] = audio;
        }

        audio.SetTrack(track as AudioStreamTrack);
        audio.loop = true;
        audio.Play();
    }
    #endregion

    #region Cleanup
    public void RemovePeerConnection(string userId)
    {
        if (peerConnections.TryGetValue(userId, out var pc))
        {
            pc.Close();
            peerConnections.Remove(userId);
        }

        if (remoteAudioSources.TryGetValue(userId, out var audio))
        {
            Destroy(audio.gameObject);
            remoteAudioSources.Remove(userId);
        }
    }

    public void CleanupAll()
    {
        foreach (var pc in peerConnections.Values)
            pc.Close();
        peerConnections.Clear();

        foreach (var audio in remoteAudioSources.Values)
            Destroy(audio.gameObject);
        remoteAudioSources.Clear();

        if (localStream != null)
        {
            foreach (var track in localStream.GetTracks())
                track.Stop();
            localStream.Dispose();
            localStream = null;
        }
    }
    #endregion

}
