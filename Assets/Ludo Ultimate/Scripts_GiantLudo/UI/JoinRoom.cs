using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.SocketIO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class JoinRoom : MonoBehaviour
{
    #region Public_Variables
    public TMP_InputField RoomCode;
    public Image Popup;
    public List<Button> numpadbuttons;
    public string inputstr = "";

    #endregion

    #region Private_Variables
    #endregion

    #region  Unity_Callback
    private void OnEnable()
    {
        Popup.Close();
        //LudoGame.Lobby.SetSocketNamespace = "private";
        inputstr = "";
        RoomCode.text = "";
        foreach (Button btn in numpadbuttons)
        {
            btn.onClick.AddListener(NumPressOperation);
        }
        CallEvent();
    }
    private void OnDisable()
    {
        Popup.Close();
        inputstr = "";
        RoomCode.text = "";
        foreach (Button btn in numpadbuttons)
        {
            btn.onClick.RemoveAllListeners();
        }
    }
    #endregion

    #region Private_Methods
    public void confirmationPopupOn()
    {
        Popup.Open();
    }
    public void CallEvent()
    {
        Ludo_UIManager.instance.OpenLoader(true);
        Ludo_UIManager.instance.socketManager.PlayerWithFriend((socket, packet, args) =>
        {
            //Debug.Log("PlayerWithFriend  : " + packet.ToString());
            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventResponse<PlayWithFriendResult> PlayWithFriendResultResp = JsonUtility.FromJson<PokerEventResponse<PlayWithFriendResult>>(resp);
            Ludo_UIManager.instance.OpenLoader(false);
            if (PlayWithFriendResultResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
           //     Game.Lobby.SetSocketNamespace = PlayWithFriendResultResp.result.nameSpace;
            }
            else
            {
                Ludo_UIManager.instance.messagePanel.DisplayMessage(PlayWithFriendResultResp.message);
            }

        });
    }
    void NumPressOperation()
    {
        GameObject currentobject = EventSystem.current.currentSelectedGameObject.gameObject;
        if (currentobject.transform.name.Equals("btnClear"))
        {
            inputstr = string.Empty;
            RoomCode.text = string.Empty;
        }
        else if (currentobject.transform.name.Equals("btnBack"))
        {
            if (!string.IsNullOrEmpty(inputstr))
            {
                inputstr = inputstr.Remove(inputstr.Length - 1);
                RoomCode.text = inputstr;
            }
        }
        else
        {
            if (inputstr.Length < 8)
            {
                int num = int.Parse(currentobject.transform.name);
                inputstr += num.ToString();
                RoomCode.text = inputstr;
            }
        }

    }
    private void JoinprivateRoomResponse(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("JoinprivateRoomResponse  : " + packet.ToString());

        JSONArray arr = new JSONArray(packet.ToString());
        string Source;
        Source = arr.getString(arr.length() - 1);
        var resp = Source;
        PokerEventResponse<JoinPrivateRoomResult> GetJoinRoomResponse = JsonUtility.FromJson<PokerEventResponse<JoinPrivateRoomResult>>(resp);
        Ludo_UIManager.instance.OpenLoader(false);
        if (GetJoinRoomResponse.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
        {
            //LudoConstants.Ludo.boardId = GetJoinRoomResponse.result.boardId;
            Ludo_UIManager.instance.OpenLoader(false);
            Ludo_UIManager.instance.createRoom.EnableCreateRoomAndSetData(GetJoinRoomResponse.result.boardId);
            this.Close();
        }
        else
        {
            Ludo_UIManager.instance.messagePanel.DisplayMessage(GetJoinRoomResponse.message);
        }
    }
    #endregion

    #region Public_Methods

    public void CloseButtonTap()
    {
        this.Close();
        Ludo_UIManager.instance.homeScreen.Open();
    }
    public void OpenJoinRoom()
    {
        if (!string.IsNullOrEmpty(inputstr))
        {
            Ludo_UIManager.instance.OpenLoader(true);
            Ludo_UIManager.instance.socketManager.JoinprivateRoom(true, inputstr, JoinprivateRoomResponse);
        }
        else
        {
            Ludo_UIManager.instance.messagePanel.DisplayMessage("Code is Empty!");
        }
    }
    public void OpenCreateRoom()
    {
        Ludo_UIManager.instance.OpenLoader(true);
        Ludo_UIManager.instance.socketManager.JoinprivateRoom(false, "", JoinprivateRoomResponse);
    }
    #endregion

    #region Coroutine
    #endregion
}
