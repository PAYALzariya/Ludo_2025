using BestHTTP.SocketIO;
using System;
using UnityEngine;
using static UnityEditor.Progress;

public class ServerSocketManager : MonoBehaviour
{
    public bool chatLudo = false;
    public string accessToken, playerId, uniqueId;
    #region Public_Variables
    public static ServerSocketManager instance;
    public static OnConnectionStateChanged onConnected;
    public static OnConnectionStateChanged OnReconnected;
    public bool isCheckDummy;
    public SocketManager socketManager;
    #endregion
    #region Private_Variables

    private const string SOCKET_EVENT_CONNECT = "connect";
    private const string SOCKET_EVENT_RECONNECT = "reconnect";
    private const string SOCKET_EVENT_RECONNECTING = "reconnecting";
    private const string SOCKET_EVENT_RECONNECT_ATTEMPT = "reconnect_attempt";
    private const string SOCKET_EVENT_RECONNECT_FAILED = "reconnect_failed";
    private const string SOCKET_EVENT_DISCONNECT = "disconnect";

    public Socket rootSocket;
    #endregion 
    private SocketManager Manager;
    #region  Unity_Callback

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //    this.transform.parent = null;
            //  DontDestroyOnLoad(this);
        }
        /* else
         {
             DestroyImmediate(this.gameObject);
         }*/
        Debug.Log("awake socktmanager");
        //   ConnectToSocketServer();
        // ConnectWithAuthObject();
    }
    private void Start()
    {
        if (!chatLudo)
        {
            ConnectWithAuthObject();
        }
        else
        {
            Manager = LudoSocketManager.Instance.Manager;
            rootSocket = Manager.GetSocket();
            Debug.Log(Manager.Uri);
            Manager.Open();
            accessToken = DataManager.instance.AccessToken;
            playerId = DataManager.instance.userId;
            uniqueId = DataManager.instance.uniqueId;
        }
        SetupAllListeners();
    }
    #endregion

    #region Private_Methods

    private void SetupAllListeners()
    {
        if (RootSocket == null) return;
        //   getAllTable();

    }
    void ConnectWithAuthObject()
    {
        Debug.Log("ConnectWithAuthObject");

        var options = new SocketOptions();
        //PlatformSupport.Collections.ObjectModel.ObservableDictionary
        // Send token as auth object
        if (isCheckDummy)
        {
            options.AdditionalQueryParams = new PlatformSupport.Collections.ObjectModel.ObservableDictionary<string, string>
            {

                { "auth[token]" ,accessToken}//,DataManager.instance.AccessToken} // Server reads "auth.token"
               // { "auth[token]" ,DataManager.instance.AccessToken} // Server reads "auth.token"
            };
        }
        else
        {
            options.AdditionalQueryParams = new PlatformSupport.Collections.ObjectModel.ObservableDictionary<string, string>
            {

              //  { "auth[token]" ,accessToken}//,DataManager.instance.AccessToken} // Server reads "auth.token"
                { "auth[token]" ,DataManager.instance.AccessToken} // Server reads "auth.token"
            };
            accessToken = DataManager.instance.AccessToken;
            playerId = DataManager.instance.userId;
            uniqueId = DataManager.instance.uniqueId;
        }

        Manager = new SocketManager(new Uri(GameConstants.Socket_URL), options);
        rootSocket = Manager.GetSocket();
        Debug.Log(Manager.Uri);
        Manager.Open();

        Debug.Log($"Connecting to: {new Uri(GameConstants.Socket_URL)}");
        RootSocket.On(SOCKET_EVENT_CONNECT, OnConnect);
        RootSocket.On(SOCKET_EVENT_RECONNECT, OnReconnect);
        RootSocket.On(SOCKET_EVENT_RECONNECT_ATTEMPT, OnReconnectAttempt);
        RootSocket.On(SOCKET_EVENT_RECONNECT_FAILED, OnReconnectFailed);
        RootSocket.On(SOCKET_EVENT_DISCONNECT, OnDisconnected);

    }

    public void SetupAllListenersDummy(DummyRootSocket ds)
    {
        /*   ds.On("StartTimer", OnStartTimer);
           ds.On("RollDiceDetails", OnRollDiceDetails);
           ds.On("playerActionDetails", OnPlayerActionDetails);
           ds.On("UpdateTableListingData", OnUpdateTableListingData);
           ds.On("GameFinished", OnGameFinished);*/
        // ... register all
    }

    private void OnDisconnected(Socket socket, Packet packet, object[] args)
    {
        Debug.Log($"<b><color=#d05402>Disconnected with:</color></b> {socket.Manager.Uri}");
        onConnected?.Invoke(false);
    }

    private void OnReconnectFailed(Socket socket, Packet packet, object[] args)
    {
        Debug.Log($"<b><color=#d05402>Reconnection Failed:</color></b> {socket.Manager.Uri}");
        OnReconnected?.Invoke(false);
    }

    private void OnReconnectAttempt(Socket socket, Packet packet, object[] args)
    {
        Debug.Log($"<b><color=#e2aa08>Reconnect Attempt to:</color></b> {socket.Manager.Uri}");
    }
    private void OnReconnect(Socket socket, Packet packet, object[] args)
    {
        Debug.Log($"<b><color=#008dcf>Reconnected with:</color></b> {socket.Manager.Uri}");
        OnReconnected?.Invoke(true);
    }

    private void OnConnect(Socket socket, Packet packet, object[] args)
    {
        Debug.Log($"<b><color=#158525>We are connected:</color></b> {socket.Manager.Uri}");
        Ludo_UIManager.instance.homeScreen.CheckFunctionOnEnable();
        //    Game.Lobby.ConnectToSocket();
        //    Game.Lobby.socketManager.Open();
        onConnected?.Invoke(true);
    }
    #endregion

    #region Public_Methods
    #endregion

    #region Coroutine
    #endregion

    #region Getter_Setter
    public Socket RootSocket => rootSocket;

    #endregion

}