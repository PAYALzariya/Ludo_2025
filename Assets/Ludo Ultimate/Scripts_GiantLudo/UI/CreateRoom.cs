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
using Ludo_Constants;

public class CreateRoom : MonoTemplate
{
    #region Public_Variables
    public Image CreatorProfileImage;
    public TextMeshProUGUI RoomCode;
    public TextMeshProUGUI CreatorName;
    public List<privateJoiners> PrivatePlayers;
    public Button StartGameButton;
    public Button CancelButton;
    public Button BackButton;
    #endregion

    #region Private_Variables
    public string privateRoomBoardId;
    #endregion

    #region  Unity_Callback
    private void OnEnable()
    {
        CreatorProfileImage.Close();
        RoomCode.text = "";
        CreatorName.text = "";
        BackButton.Close();
        CancelButton.Close();
        StartGameButton.Close();
        StartGameButton.interactable = false;
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.PrivateRoomDetails, OnPrivateRoomDetails);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.LeavePrivateRoom, ResponseOfLeavePrivateRoom);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.GameStart, ResponseOfgameStart);
        CallEvent();
    }
    private void OnDisable()
    {

       ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.PrivateRoomDetails, OnPrivateRoomDetails);
       ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.LeavePrivateRoom, ResponseOfLeavePrivateRoom);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.GameStart, ResponseOfgameStart);
        CreatorProfileImage.Close();
    }
    #endregion
    #region Broadcast
    private void ResponseOfgameStart(Socket socket, Packet packet, params object[] args)
    {

        GetUIManager.OpenLoader(false);

        int arrLen = new JSONArray(packet.ToString()).length();
        Debug.Log(Ludo_Constants.LudoEvents.GameStart + " Broadcast response  :" + packet.ToString());

        GameStartDataResp response = JsonUtility.FromJson<GameStartDataResp>(packet.GetPacketString());
        Debug.Log("PJ");
      //pj  Ludo_UIManager.instance.gamePlayScreen.SetRoomDataAndPlay(response);
        this.Close();
        Ludo_UIManager.instance.playOnline.Close();

    }
    private void OnPrivateRoomDetails(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.PrivateRoomDetails + " Broadcast response  :" + packet.ToString());

        OnPrivateRoomDetailsData details = JsonUtility.FromJson<OnPrivateRoomDetailsData>(packet.GetPacketString());
        if (details.boardId != privateRoomBoardId && details.boardId.Length != 0)
            return;

        RoomCode.text = details.passcode;
        CreatorName.text = details.joiners[0].playerName;
        if (details.joiners[0].profilePic != null && !details.joiners[0].profilePic.Equals("default.png") && details.joiners[0].profilePic != "")
        {
            string getImageUrl =  details.joiners[0].profilePic;
            LudoUtilityManager.Instance.DownloadImage(getImageUrl, CreatorProfileImage, false, true);
            CreatorProfileImage.Open();
        }
        else
        {
            CreatorProfileImage.sprite = Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[details.joiners[0].avatar];
            CreatorProfileImage.Open();
        }

        //CreatorProfileImage.sprite = Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[details.joiners[0].avatar];
        foreach (privateJoiners plr in PrivatePlayers)
        {
            plr.ResetData();
        }
        if (details.joiners.Count > 0)
        {
            for (int i = 1; i < details.joiners.Count; i++)
            {
                if (!details.joiners[i].id.Equals(details.createrId))
                {
                    PrivatePlayers[i - 1].setData(details.joiners[i]);
                }
            }
        }

        if (details.createrId.Equals(ServerSocketManager.instance.playerId))
        {
            BackButton.Close();
            CancelButton.Open();
            StartGameButton.Open();
            StartGameButton.interactable = false;
        }
        else
        {
            BackButton.Open();
            CancelButton.Close();
            StartGameButton.Close();
            StartGameButton.interactable = false;
        }
        if (details.joiners.Count > 1)
        {
            StartGameButton.interactable = true;
        }
        else
        {
            StartGameButton.interactable = false;
        }

    }
    private void ResponseOfLeavePrivateRoom(Socket socket, Packet packet, params object[] args)
    {
        GetUIManager.OpenLoader(false);
        Debug.Log(Ludo_Constants.LudoEvents.LeavePrivateRoom + " Broadcast response  :" + packet.ToString());

        JSONArray arr = new JSONArray(packet.ToString());
        string message = arr.getJSONObject(1).getString("message");
        Ludo_UIManager.instance.messagePanel.DisplayMessage(message, MoveToHome);
    }
    #endregion

    #region Private_Methods
    private void LeavePrivateRoomDone()
    {
        Ludo_UIManager.instance.socketManager.LeavePrivateRoom(privateRoomBoardId, (socket, packet, args) =>
         {
             Debug.Log(Ludo_Constants.LudoEvents.LeavePrivateRoom + "  response  :" + packet.ToString());

             JSONArray arr = new JSONArray(packet.ToString());
             string Source;
             Source = arr.getString(arr.length() - 1);
             var resp = Source;
             PokerEventResponse<HomeDataItem> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<HomeDataItem>>(resp);

             if (HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
             {
                 Ludo_UIManager.instance.homeScreen.Open();
                 this.Close();
             }
         });

    }
    private void MoveToHome()
    {
        LeavePrivateRoomDone();
    }
    #endregion

    #region Public_Methods
    public void ReconnectPrivateCall()
    {
        Ludo_UIManager.instance.socketManager.ReconnectPrivateDashboard(privateRoomBoardId, (socket, packet, args) =>
        {
            Debug.Log(Ludo_Constants.LudoEvents.ReconnectPrivateDashboard + "  response  :" + packet.ToString());

            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventResponse<HomeDataItem> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<HomeDataItem>>(resp);

            if (!HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                Ludo_UIManager.instance.messagePanel.DisplayMessage(HomeDataResp.message, MoveToHome);
            }
        });
    }

    public void ShareButtonTap()
    {

#if UNITY_WEBGL //|| UNITY_EDITOR

        //JSON_Object jsonObj = new JSON_Object();
        //jsonObj.put("playerId", UIManager.Instance.assetOfGame.SavedLoginData.PlayerId);
        //jsonObj.put("clubId", ID);
        //jsonObj.put("type", "club");
        //jsonObj.put("code", clubCodeData);
        //jsonObj.put("url", LudoConstants.PokerAPI.BaseUrl);
        //Debug.Log("code Event: " + jsonObj.toString());
        //ExternalCallClass.Instance.codeSharingMethod(jsonObj.toString());
#else
        StartCoroutine(TakeSSAndShare());
#endif

    }
    public void EnableCreateRoomAndSetData(string BoardID)
    {
        privateRoomBoardId = BoardID;
        this.Open();
    }
    public void CallEvent()
    {

        Ludo_UIManager.instance.OpenLoader(true);
        Ludo_UIManager.instance.socketManager.SubscribePrivateRoom(privateRoomBoardId, (socket, packet, args) =>
         {
             //    Debug.Log("SubscribePrivateRoom  : " + packet.ToString());
             Ludo_UIManager.instance.OpenLoader(false);

         });
    }
    public void Cancel()
    {
        Ludo_UIManager.instance.messagePanel.DisplayConfirmationMessage("Are you sure you want Exit?", (b) =>
        {
            if (b)
            {
                LeavePrivateRoomDone();
            }
        });



    }

    public void StartGame()
    {
        Ludo_UIManager.instance.socketManager.StartGame(privateRoomBoardId, (socket, packet, args) =>
        {
            Debug.Log(Ludo_Constants.LudoEvents.StartGame + "  response  :" + packet.ToString());

            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventResponse<HomeDataItem> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<HomeDataItem>>(resp);

            if (!HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                Ludo_UIManager.instance.messagePanel.DisplayMessage(HomeDataResp.message);
            }

        });
        //this.Close();
        //GetGameScreen.Open();
        //GameStaticData.gamesType = GamesType.Private;
        //GameStaticData.playerCount = 4;

    }
    #endregion

    #region Coroutine
    private IEnumerator TakeSSAndShare()
    {
        yield return new WaitForEndOfFrame();
        //yield return new WaitForSeconds(0.2f);

        //Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //ss.Apply();

        /*	Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
          texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
          texture.LoadRawTextureData(texture.GetRawTextureData());
          texture.Apply();
          //sendTexture(texture, messageToSend);

          string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
          File.WriteAllBytes(filePath, texture.EncodeToPNG());*/

        string s1 = "Hey, I play Ludo with friends @ Gaint Ludo app. Come and join my Private Room! My Room code : ";

        string details = s1 + RoomCode.text + " " + Ludo_Constants.LudoConstants.BaseUrl;
     //   new NativeShare().AddFile("").SetSubject(Application.productName).SetText(details).Share();
    }
    #endregion
}
