using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO;
using System;
using PlatformSupport.Collections.ObjectModel;


namespace Game
{
    public class Lobby
    {
        public static EmptyDelegateEvent OnSocketReconnected;
        public static EmptyDelegateEvent SocketConnectionInitialization;

      /*  [Header("Socket")]
        public static SocketManager socketManager
        {
            get
            {
                return ServerSocketManager.instance.socketManager;
            }
        }*/
        //public static OnConnectionStateChanged onConnected;
        //public static OnConnectionStateChanged OnReconnected;
        public static string deviceUniqueID;
        public static string SOCKET_EVENT_CONNECT = "connect";
        public static string SOCKET_EVENT_RECONNECT = "reconnect";
        public static string SOCKET_EVENT_RECONNECTING = "reconnecting";
        public static string SOCKET_EVENT_RECONNECT_ATTEMPT = "reconnect_attempt";
        public static string SOCKET_EVENT_RECONNECT_FAILED = "reconnect_failed";
        public static string SOCKET_EVENT_DISCONNECT = "disconnect";
        public static string SOCKET_EVENT_ForceLogout = "forceLogOut";
        public static string SOCKET_EVENT_Maintenance = "maintenanceServer";
        public static string SOCKET_EVENT_MaintenanceOver = "maintenanceOver";
        public static string SOCKET_EVENT_MaintenanceForceLogout = "maintenanceForceLogout";
        public static string SOCKET_EVENT_AdminNotification = "adminNotification";
        public const string ticketUpdate = "ticketUpdate";

        private static bool _socketConnected = false;
        private static int _reconnectAttemptCount = 0;

      //  static Socket _GameSocket;
        public static int timerattampts = 0;
        static Socket _ComputerSocket;
       /* public static Socket ComputerGameSocket
        {
            get
            {
                  return ServerSocketManager.instance.rootSocket;
            }
            
        }
       
        public static Socket GameSocket
        {
            get
            {
               
                 return ServerSocketManager.instance.rootSocket;
            }
            set
            {

                GameSocket =  ServerSocketManager.instance.rootSocket;
            }
        }*/
/*
        public static string SetSocketNamespace
        {
            *//*  set
              {
                  SetSocketNamespace =  ServerSocketManager.instance.rootSocket.ToString();
              }
              get
              {
                  return ServerSocketManager.instance.rootSocket.ToString();
              }*//*

            set
            {
                LudoGameSocket =SocketManager.GetSocket("/" + value);
            }
            get
            {
                return LudoGameSocket.Namespace;
            }
        }
*/

        private static void OnConnect(Socket socket, Packet packet, object[] args)
        {
            Debug.Log($"<b><color=#158525>We are connected to ludo game:</color></b> {socket.Manager.Uri}");

            SocketConnected = true;
            ReconnectAttemptCount = 0;

            Ludo_UIManager.instance.OpenLoader(false);
            Ludo_UIManager.instance.OnConnected(true);
            SocketConnectionInitialization?.Invoke();


        }

        private static void OnReConnect(Socket Socket, Packet Packet, params object[] Args)
        {
            Debug.Log($"<b><color=#008dcf>Reconnected with:</color></b> {Socket.Manager.Uri}");
            //OnReconnected?.Invoke(true);
            Ludo_UIManager.instance.OnConnected(true);
            //Ludo_UIManager.instance.HideLoader();
            //Ludo_UIManager.instance.HidePopup();

            SocketConnected = true;
            ReconnectAttemptCount = 0;

            Ludo_UIManager.instance.OpenLoader(false);
            timerattampts = 0;
            Ludo_UIManager.instance.isUploadImage = false;

          

           

            

            if (Ludo_UIManager.instance.homeScreen.isActiveAndEnabled)
                Ludo_UIManager.instance.homeScreen.CheckFunctionOnEnable();
          

        }
        private static void OnReConnecting(Socket socket, Packet packet, params object[] args)
        {
            Debug.Log($"<b><color=#e2aa08>Reconnecting to:</color></b> {socket.Manager.Uri}");

            SocketConnected = false;

            //if (Ludo_UIManager.instance.updateProfileScreen.gameObject.activeInHierarchy || Ludo_UIManager.instance.loginScreen.gameObject.activeInHierarchy)
            //{
            //    if (timerattampts > 3)
            //    {
            //        if (!Ludo_UIManager.instance.isUploadImage)
            //        {
            //            Ludo_UIManager.instance.OpenLoader(true, "Reconnecting...");
            //        }
            //    }
            //}
            //else
            //{

            //    if (!Ludo_UIManager.instance.LocalGameScreen.gameObject.activeInHierarchy)
            //    {
            //        Ludo_UIManager.instance.OpenLoader(true, "Reconnecting...");
            //    }

            //}

        }

        private static void OnReConnectAttempt(Socket socket, Packet packet, params object[] args)
        {
            Debug.Log($"<b><color=#e2aa08>Reconnect Attempt to:</color></b> {socket.Manager.Uri}");

            SocketConnected = false;
            ReconnectAttemptCount++;

            //timerattampts++;
            //if (Ludo_UIManager.instance.updateProfileScreen.gameObject.activeInHierarchy || Ludo_UIManager.instance.loginScreen.gameObject.activeInHierarchy)
            //{
            //    if (timerattampts > 3)
            //    {
            //        Ludo_UIManager.instance.OpenLoader(true, "Reconnecting...");
            //    }
            //}
            //else
            //{
            //    if (!Ludo_UIManager.instance.LocalGameScreen.gameObject.activeInHierarchy)
            //    {
            //        Ludo_UIManager.instance.OpenLoader(true, "Reconnecting...");
            //    }

            //}

        }
        private static void OnReConnectFailed(Socket socket, Packet packet, params object[] args)
        {
            Debug.Log($"<b><color=#d05402>Reconnection Failed:</color></b> {socket.Manager.Uri}");
            //OnReconnected?.Invoke(false);
            SocketConnected = false;
        }
        //static bool isDisconnect;
        private static void OnDisconnect(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
        {
            Debug.Log($"<b><color=#d05402>Disconnected with:</color></b> {socket.Manager.Uri}");
            //onConnected?.Invoke(false);
            Ludo_UIManager.instance.OnConnected(false);
            SocketConnected = false;

            //if (Ludo_UIManager.instance.updateProfileScreen.gameObject.activeInHierarchy || Ludo_UIManager.instance.loginScreen.gameObject.activeInHierarchy)
            //{
            //    if (timerattampts > 3)
            //    {
            //        Ludo_UIManager.instance.OpenLoader(true, "Reconnecting...");
            //    }
            //}
            //else
            //{
            //    if (!Ludo_UIManager.instance.LocalGameScreen.gameObject.activeInHierarchy)
            //    {
            //        Ludo_UIManager.instance.OpenLoader(true, "Reconnecting...");
            //    }

            //}
            //isDisconnect = true;
        }

        private static void OnForceLogout(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
        {
            Debug.Log("-- OnForceLogout --" + packet.ToString());
            JSONArray arr = new JSONArray(packet.ToString());

            JSON_Object ForceLogutPlayer = new JSON_Object(arr.getString((arr.length() - 1)));
            string ForceLogutPlayerId = ForceLogutPlayer.getString("playerId");

            if (ForceLogutPlayerId.Equals(ServerSocketManager.instance.playerId))
            {
                Ludo_UIManager.instance.ProfilePic = 0;
              //  PlayerPrefs.DeleteKey(Ludo_Constants.LudoConstants.PlayerNumber);
               // PlayerPrefs.DeleteKey(Ludo_Constants.LudoConstants.PlayerName);
                //PlayerPrefs.DeleteKey(LudoConstants.LudoConstants.PlayerEmail);
               // PlayerPrefs.SetInt(Ludo_Constants.LudoConstants.UserLogin, Ludo_Constants.LudoConstants.loggedOut);

                Ludo_UIManager.instance.Reset(false);
                //Ludo_UIManager.instance.tableManager.RemoveAllMiniTableData();

               
            }
        }

        private static void OnMaintenanceServer(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
        {
            Debug.Log("-- OnMaintenanceServer --" + packet.ToString());

            JSONArray jsonArray = new JSONArray(packet.ToString());
            var resp = jsonArray.getString(jsonArray.length() - 1);

            MaintenaceResp maintenaceResp = JsonUtility.FromJson<MaintenaceResp>(resp);
            Ludo_UIManager.instance.messagePanel.DisplayMessage(maintenaceResp.message);
        }

        private static void OnMaintenanceOver(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
        {
            Debug.Log("-- OnMaintenanceOver --" + packet.ToString());

            JSONArray jsonArray = new JSONArray(packet.ToString());
            var resp = jsonArray.getString(jsonArray.length() - 1);
            MaintenaceResp maintenaceResp = JsonUtility.FromJson<MaintenaceResp>(resp);

          
            Ludo_UIManager.instance.maintenancePanel.Close();
        }

        private static void OnMaintenanceForceLogout(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
        {
            Debug.Log("-- OnMaintenanceForceLogout --" + packet.ToString());

            JSONArray jsonArray = new JSONArray(packet.ToString());
            var resp = jsonArray.getString(jsonArray.length() - 1);

            MaintenaceResp maintenaceResp = JsonUtility.FromJson<MaintenaceResp>(resp);
            Ludo_UIManager.instance.maintenancePanel.OpenPanel(maintenaceResp.message);
        }

        private static void OnAdminNotification(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
        {
            Debug.Log("-- OnAdminNotification --" + packet.ToString());
        }

        public static void OnLoginScreenTap()
        {

        }

        public void OnLogOutDone()
        {
        }

        private static void OnTicketUpdateNotification(Socket socket, Packet packet, params object[] args)
        {
            Debug.Log("OnTicketUpdateNotification  : " + packet.ToString());
            // Activate Support Notification Icon
          /*  if (Ludo_UIManager.instance.homeScreen.SupportBtnNotificationIconRef.activeInHierarchy == false)
            {
                // Activate Game Object
                Ludo_UIManager.instance.homeScreen.SupportBtnNotificationIconRef.SetActive(true);
            }*/
        }

        public static void ReconnectPlayerCall()
        {
            Ludo_UIManager.instance.socketManager.GetReconnect((socket, packet, args) =>
            {
                Debug.Log("GetReconnect  : " + packet.ToString());
                //Ludo_UIManager.Instance.HideLoader();
                try
                {
                    JSONArray arr = new JSONArray(packet.ToString());
                    string Source;
                    Source = arr.getString(arr.length() - 1);
                    var resp1 = Source;
                    PokerEventResponse resp = JsonUtility.FromJson<PokerEventResponse>(resp1);

                    if (resp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
                    {
                        if (Ludo_UIManager.instance.gamePlayScreen.gameObject.activeInHierarchy)
                        {
                            GetReconnectGameCall();
                        }
                        if (Ludo_UIManager.instance.createRoom.gameObject.activeInHierarchy)
                        {
                            Ludo_UIManager.instance.createRoom.ReconnectPrivateCall();
                        }
                        if (Ludo_UIManager.instance.PlayerSearchPanel.gameObject.activeInHierarchy)
                        {
                            Ludo_UIManager.instance.PlayerSearchPanel.RejoinReconnect();
                        }

                        OnSocketReconnected?.Invoke();
                    }
                    else
                    {
                        if (resp.message == "forceLogout")
                        {

                        }
                        else if (resp.message == "No Running Game Found!" || resp.message == "")
                        {

                        }
                        else
                        {
                            ReconnectPlayerCall();
                        }
                    }

                }
                catch (System.Exception e)
                {
                    //	Ludo_UIManager.Instance.DisplayMessagePanel (LudoConstants.Messages.SomethingWentWrong);
                    //Ludo_UIManager.instance.DisplayMessagePanel("Something went wrong. Try again."));

                    Debug.LogError("exception  : " + e);
                }
            });
        }

        public static void GetReconnectGameCall()
        {
            Ludo_UIManager.instance.socketManager.GetReconnectGame(Ludo_UIManager.instance.gamePlayScreen.ReconnectJoinRoomresponse);
        }

        IEnumerator LogoutFunction(float timer)
        {
          //  Game.Lobby.socketManager.Close();
         //   Game.Lobby.socketManager.Open();
        //    Game.Lobby.ConnectToSocket();

            yield return new WaitForSeconds(timer);
        }

        public static bool SocketConnected
        {
            set
            {
                _socketConnected = value;
            }
            get
            {
                return _socketConnected;
            }
        }

        public static int ReconnectAttemptCount
        {
            set
            {
                _reconnectAttemptCount = value;

                if (Application.internetReachability == NetworkReachability.NotReachable)
                    Ludo_UIManager.instance.OpenLoader(true, "Check your internet connection");
                else if (_reconnectAttemptCount == 0)
                    Ludo_UIManager.instance.OpenLoader(false);
                else if ((_reconnectAttemptCount >= 2 && _reconnectAttemptCount < 5) || (_reconnectAttemptCount > 0 && _reconnectAttemptCount < 5 && Ludo_UIManager.instance.loader.isActiveAndEnabled))
                    Ludo_UIManager.instance.OpenLoader(true);
                else if (_reconnectAttemptCount >= 2 || (_reconnectAttemptCount > 0 && Ludo_UIManager.instance.loader.isActiveAndEnabled))
                    Ludo_UIManager.instance.OpenLoader(true, "Internet issue.\nTrying to reconnect with server"); //Ludo_UIManager.Instance.loaderPanel.ShowLoader("Internet issue.\nTrying to reconnect with server");
            }
            get
            {
                return _reconnectAttemptCount;
            }
        }

    }
}

public delegate void OnConnectionStateChanged(bool b);
public delegate void EmptyDelegateEvent();