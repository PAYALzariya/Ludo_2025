using System.Collections;
using System.Collections.Generic;
//using System.Net.Sockets;
using BestHTTP.SocketIO;
using UnityEngine;
using TMPro;

public class StartGameTimer : MonoTemplate
{

    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    //[Header ("Transforms")]


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    //[Header ("Images")]


    [Header("Text")]
    public TextMeshProUGUI Timer;

    //[Header ("Prefabs")]

    //[Header ("Enums")]


    //[Header ("Variables")]

    #endregion

    #region PRIVATE_VARIABLES

    #endregion

    #region UNITY_CALLBACKS
    // Use this for initialization
    void OnEnable()
    {
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.StartTimer, ResponseOfstartTimer);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.BackToLobby, ResponseOfbackToLobby);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.GameStart, ResponseOfgameStart);

    }
    void OnDisable()
    {
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.StartTimer, ResponseOfstartTimer);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.BackToLobby, ResponseOfbackToLobby);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.GameStart, ResponseOfgameStart);
        Timer.text = "";

    }

    #endregion

    #region DELEGATE_CALLBACKS

    private void ResponseOfstartTimer(Socket socket, Packet packet, params object[] args)
    {
        GetUIManager.OpenLoader(false);
        Debug.Log(Ludo_Constants.LudoEvents.StartTimer + " Broadcast response  :" + packet.ToString());

        JSONArray arr = new JSONArray(packet.ToString());
        StartTimerData response = JsonUtility.FromJson<StartTimerData>(packet.GetPacketString());
        Timer.text = "Game will start in " + response.timer + " Seconds";

    }

    private void ResponseOfbackToLobby(Socket socket, Packet packet, params object[] args)
    {
        GetUIManager.OpenLoader(false);
        Debug.Log(Ludo_Constants.LudoEvents.BackToLobby + " Broadcast response  :" + packet.ToString());


        JSONArray arr = new JSONArray(packet.ToString());
        string message = arr.getJSONObject(1).getString("message");
        GetMessagePanel.DisplayMessage(message, MoveToHome);
    }
    private void ResponseOfgameStart(Socket socket, Packet packet, params object[] args)
    {
        GetUIManager.OpenLoader(false);

        int arrLen = new JSONArray(packet.ToString()).length();
        Debug.Log(Ludo_Constants.LudoEvents.GameStart + " Broadcast response  :" + packet.ToString());

        GameStartDataResp response = JsonUtility.FromJson<GameStartDataResp>(packet.GetPacketString());

        Ludo_UIManager.instance.gamePlayScreen.SetRoomDataAndPlay(response);
        this.Close();
        Ludo_UIManager.instance.playOnline.Close();

    }
    #endregion

    #region PUBLIC_METHODS



    #endregion

    #region PRIVATE_METHODS
    private void MoveToHome()
    {
        Ludo_UIManager.instance.homeScreen.Open();
        Ludo_UIManager.instance.gamePlayScreen.Close();
        this.Close();
    }
    #endregion

    #region COROUTINES
    #endregion


    #region GETTER_SETTER
    #endregion
}
