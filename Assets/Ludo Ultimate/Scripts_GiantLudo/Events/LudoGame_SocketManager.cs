using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using BestHTTP.JSON;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;
using UnityEngine;

public class LudoGame_SocketManager : MonoBehaviour //MonoTemplate
{
    //---------------------------------------------------------------------------------------------------
    //-- Chat room ludo Events
    //---------------------------------------------------------------------------------------------------
    public void ChatRoomGameInit(SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("roomId", ServerSocketManager.instance.uniqueId);
        json.put("hostId", ServerSocketManager.instance.playerId);
        json.put("mode", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("durationMinutes",8);//DataManager.instance.AccessToken);
        json.put("maxPlayers", GameStaticData.playerCount.ToString());//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name request " + Ludo_Constants.ChatRoomLudo.ChatRoomGameInit + "   Parameters : " + json.toString());
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.ChatRoomLudo.ChatRoomGameInit , action, Json.Decode(json.toString()));
    }
    public void ChatRoomGameInvite(SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("roomId", ServerSocketManager.instance.uniqueId);
        json.put("hostId", ServerSocketManager.instance.playerId);
        json.put("PlayerIds", SystemInfo.deviceUniqueIdentifier.ToString());
      //  json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name request " + Ludo_Constants.ChatRoomLudo.ChatRoomGameInvite + "   Parameters : " + json.toString());
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.ChatRoomLudo.ChatRoomGameInvite , action, Json.Decode(json.toString()));
    }
    public void ChatRoomGameInviteResponse(SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("roomId", ServerSocketManager.instance.uniqueId);
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("accept", SystemInfo.deviceUniqueIdentifier.ToString());
      //  json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name request " + Ludo_Constants.ChatRoomLudo.ChatRoomGameInviteResponse + "   Parameters : " + json.toString());
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.ChatRoomLudo.ChatRoomGameInviteResponse , action, Json.Decode(json.toString()));
    }
    public void ChatRoomGameReady(SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("roomId", ServerSocketManager.instance.uniqueId);
        json.put("acceptedPlayers", ServerSocketManager.instance.playerId);
        json.put("maxPlayers", GameStaticData.playerCount.ToString());
        //  json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name request " + Ludo_Constants.ChatRoomLudo.ChatRoomGameReady + "   Parameters : " + json.toString());
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.ChatRoomLudo.ChatRoomGameReady, action, Json.Decode(json.toString()));
    }
    public void ChatRoomGameStart(SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("roomId", ServerSocketManager.instance.uniqueId);
        json.put("hostId", ServerSocketManager.instance.playerId);
        Debug.Log("$$$$ Emit name request " + Ludo_Constants.ChatRoomLudo.ChatRoomGameStart + "   Parameters : " + json.toString());
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.ChatRoomLudo.ChatRoomGameStart , action, Json.Decode(json.toString()));
    }
    //-----------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------
    public void updateUserSocketId(SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("userId", ServerSocketManager.instance.uniqueId);
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
 Debug.Log("$$$$ Emit name request " + Ludo_Constants.LudoEvents.UpdateUserSocketId + "   Parameters : " + json.toString());
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.UpdateUserSocketId, action, Json.Decode(json.toString()));
    }



  

    /*  public void updateProfile(Dictionary<string, object> data, SocketIOAckCallback action)
      {
          JSON_Object json = Utility.GetJsonObjects(data);
          Debug.Log($"updateProfile: {json.toString()}");
          GetRootSocket.Emit("updateProfile", action, Json.Decode(json.toString()));
      }*/

   
    public void onlineUserList(SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("productName", Application.productName);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
       // print(Ludo_Constants.LudoEvents.OnlineUserList + " - " + json.toString());
       Debug.Log("$$$$ Emit name request " + Ludo_Constants.LudoEvents.OnlineUserList + "  Parameters : " + json.toString());
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.OnlineUserList, action, Json.Decode(json.toString()));
    }

    public void checkRunningGame(SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("productName", Application.productName);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name request " + Ludo_Constants.LudoEvents.CheckRunningGame + "  Parameters : " + json.toString()); 
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.CheckRunningGame, action, Json.Decode(json.toString()));
    }

    public void GetWinnerPlayersList(SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("productName", Application.productName);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name request " + Ludo_Constants.LudoEvents.WinnerPlayers + "  Parameters : " + json.toString());
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.WinnerPlayers, action, Json.Decode(json.toString()));
    }

  /*  public void BlockPlayer(string blockPlayerId, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("currentPlayerId",  ServerSocketManager.instance.playerId);
        json.put("blockPlayerId", blockPlayerId);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
       //jsonObj.put("authToken", GetUnityCryptoAuthToken());
        print(LudoLudo_Constants.LudoEvents.BlockPlayer + " - " + json.toString());
        GetRootSocket.Emit(LudoLudo_Constants.LudoEvents.BlockPlayer, action, Json.Decode(json.toString()));
    }

    public void UnBlockPlayer(string blockedPlayerId, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("currentPlayerId",  ServerSocketManager.instance.playerId);
        json.put("blockedPlayerId", blockedPlayerId);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
       //jsonObj.put("authToken", GetUnityCryptoAuthToken());
        print(LudoLudo_Constants.LudoEvents.UnBlockPlayer + " - " + json.toString());
        GetRootSocket.Emit(LudoLudo_Constants.LudoEvents.UnBlockPlayer, action, Json.Decode(json.toString()));
    }

    public void GetBlockedPlayers(SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId",  ServerSocketManager.instance.playerId);
        json.put("productName", Application.productName);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());        
       //jsonObj.put("authToken", GetUnityCryptoAuthToken());
        print(LudoLudo_Constants.LudoEvents.GetBlockedPlayers + " - " + json.toString());
        GetRootSocket.Emit(LudoLudo_Constants.LudoEvents.GetBlockedPlayers, action, Json.Decode(json.toString()));
    }

    public void FindPlayerToBlock(string mobile, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId",  ServerSocketManager.instance.playerId);
        json.put("mobile", mobile);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
       //jsonObj.put("authToken", GetUnityCryptoAuthToken());
        print(LudoLudo_Constants.LudoEvents.FindPlayerToBlock + " - " + json.toString());
        GetRootSocket.Emit(LudoLudo_Constants.LudoEvents.FindPlayerToBlock, action, Json.Decode(json.toString()));
    }
*/
   /* public void GetSupportData(SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();        
        json.put("playerId", ServerSocketManager.instance.playerId);
        Debug.Log("$$$$ Emit name request " + Ludo_Constants.LudoEvents.GetSupportData + "    Parameters : " + json.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.GetSupportData, action, Json.Decode(json.toString()));
    }    
*/
    

    public void leaveRunningGame(string tId, string bId, string tourId, string Uid, string nspace, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("tableId", tId);
        json.put("boardId", bId);
        json.put("tournamentId", tourId);
        json.put("uniqueId", Uid);
        json.put("nameSpace", nspace);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("productName", Application.productName);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name request " + Ludo_Constants.LudoEvents.LeaveRunningGame + "    Parameters : " + json.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.LeaveRunningGame, action, Json.Decode(json.toString()));

    }

    public void JoinTableListing(string tabletype, string selectedGoti, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("tableType", tabletype);
        json.put("selectedGoti", selectedGoti);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("productName", Application.productName);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name request " + Ludo_Constants.LudoEvents.JoinTableListing + "    Parameters : " + json.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.JoinTableListing, action, Json.Decode(json.toString()));
    }

    public void LeaveTableListing(string tabletype, string selectedGoti, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("tableType", tabletype);
        json.put("selectedGoti", selectedGoti);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("productName", Application.productName);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name request   " + Ludo_Constants.LudoEvents.LeaveTableListing + "    Parameters : " + json.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.LeaveTableListing, action, Json.Decode(json.toString()));
    }

    public void getAllTable(string tabletype, string selectedGoti, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("tableType", tabletype);
        json.put("selectedGoti", selectedGoti);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("productName", Application.productName);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name request    " + Ludo_Constants.LudoEvents.GetAllTableParameters + "    Parameters : " + json.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.GetAllTableParameters, action, Json.Decode(json.toString()));

    }


    public void userSelectGoti(Dictionary<string, object> data, SocketIOAckCallback action)
    {
        JSON_Object json= Cust_Utility.GetJsonObjects(data);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.UserSelectGoti + "    Parameters : " + json.toString());
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.UserSelectGoti, action, Json.Decode(json.toString()));
    }
    public void JoinComputerRoom(string Players, string selectedGoti, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("totalPlayer", Players);
        json.put("userSelectGoti", selectedGoti);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("productName", Application.productName);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
          print(Ludo_Constants.LudoEvents.JoinRoom + " - " + json.toString());
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.JoinRoom + "    Parameters : " + json.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.JoinRoom, action, Json.Decode(json.toString()));
    }
    public void JoinprivateRoom(bool type, string passcodeStr, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("joinType", type);
        json.put("passcode", passcodeStr);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("productName", Application.productName);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.JoinRoom + "    Parameters : " + json.toString());
        //print(Game.Lobby.ServerSocketManager.instance.rootSocket.Namespace);
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.JoinRoom, action, Json.Decode(json.toString()));
    }

    public void JoinRoom(string tableId, string selectedGoti, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("tableId", tableId);
        json.put("uniqueId", ServerSocketManager.instance.uniqueId);
        json.put("userSelectGoti", selectedGoti);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("productName", Application.productName);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        json.put("maxPlayers", GameStaticData.playerCount);
        json.put("gotiCount", GameStaticData.numberGoties);
        json.put("gameType", Ludo_UIManager.instance.GetSelectedMode());
        if(Ludo_UIManager.instance.GetSelectedMode() == "quick")
        {
            json.put("durationMinutes", 8);
        }
        Debug.Log("$$$$ Emit name  requuest   " + Ludo_Constants.LudoEvents.JoinRoom + "    Parameters : " + json.toString());

        //print(Game.Lobby.ServerSocketManager.instance.rootSocket.Namespace);
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.JoinRoom, action, Json.Decode(json.toString()));
    }

    public void CheckGameStartTime(string tableId, string selectedGoti, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("tableId", tableId);
        json.put("uniqueId", ServerSocketManager.instance.uniqueId);
        json.put("userSelectGoti", selectedGoti);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("productName", Application.productName);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.CheckGameStartTime + "    Parameters : " + json.toString());
        //print(Game.Lobby.ServerSocketManager.instance.rootSocket.Namespace);
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.CheckGameStartTime, action, Json.Decode(json.toString()));

    }
    public void userSelectGotiRemove(string tableId, string uniqueId, SocketIOAckCallback action)
    {
        //JSON_Object json = Utility.GetJsonObjects(data);
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("tableId", tableId);
        json.put("uniqueId", ServerSocketManager.instance.uniqueId);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("productName", Application.productName);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.UserSelectGotiRemove + "    Parameters : " + json.toString());
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.UserSelectGotiRemove, action, Json.Decode(json.toString()));
    }

    public void enterRoom(Dictionary<string, object> data, SocketIOAckCallback action)
    {
        JSON_Object json = Cust_Utility.GetJsonObjects(data);
        json.put("osName", LudoUtilityManager.Instance.GetOSName());
        json.put("appVersion", LudoUtilityManager.Instance.GetApplicationVersion());
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.EnterRoom + "    Parameters : " + json.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.EnterRoom, action, Json.Decode(json.toString()));
    }

    public void exitRoom(Dictionary<string, object> data, SocketIOAckCallback action)
    {
        JSON_Object json = Cust_Utility.GetJsonObjects(data);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.ExitRoom + "    Parameters : " + json.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.ExitRoom, action, Json.Decode(json.toString()));
    }
    public void JoinGame(string bId, string tableId, string TournamentId, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("tableId", tableId);
        json.put("boardId", bId);
        json.put("TournamentId", TournamentId);
        json.put("uniqueId", ServerSocketManager.instance.uniqueId);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("productName", Application.productName);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
       
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.JoinGame + "    Parameters : " + json.toString());

        //Debug.Log("SocketStatus : " + Game.Lobby.socketManager.Socket.IsOpen);
        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.JoinGame, action, Json.Decode(json.toString()));        
        //Debug.Log("JoinGame Emmitted!!!" + Game.Lobby.ServerSocketManager.instance.rootSocket.Namespace);
        //Debug.Log("SocketStatus : " + Game.Lobby.socketManager.Socket.IsOpen);
    }

    public void RollDice(string boardId, string DiceValue, string diceKey, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("boardId", boardId);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("diceValue", DiceValue);
        json.put("diceKey", diceKey);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.RollDice + "    Parameters : " + json.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.RollDice, action, Json.Decode(json.toString()));
    }
    public void PlayerAction(string playerId, int pawnId, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("tokenId", pawnId);
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("boardId", Ludo_Constants.Ludo.boardId);
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request    " + Ludo_Constants.LudoEvents.PlayerAction + "    Parameters : " + json.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.PlayerAction, action, Json.Decode(json.toString()));
    }
    public void KillPlayer(string attackerPlayerId, int attackerTokenId, string killedPlayerId, int killedTokenId, SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("boardId", Ludo_Constants.Ludo.boardId);
        json.put("attackerPlayerId", attackerPlayerId);
        json.put("attackerTokenId", attackerTokenId);
        json.put("killedPlayerId",killedPlayerId);
        json.put("killedTokenId", killedTokenId);

        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("<color=#d05402>Emit name  request</color>" + Ludo_Constants.LudoEvents.KillPlayer + "    Parameters : " + json.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.KillPlayer, action, Json.Decode(json.toString()));
    }
    public void LeaveRoom(SocketIOAckCallback action)
    {
        JSON_Object json = new JSON_Object();
        json.put("playerId", ServerSocketManager.instance.playerId);
        json.put("boardId", Ludo_Constants.Ludo.boardId);
        json.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        json.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.LeaveRoom + "    Parameters : " + json.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.LeaveRoom, action, Json.Decode(json.toString()));
    }

    public void GetReconnect(SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", ServerSocketManager.instance.playerId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.ReconnectPlayer + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.ReconnectPlayer, action, Json.Decode(jsonObj.toString()));
    }
    public void GetReconnectGame(SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", ServerSocketManager.instance.playerId);
        jsonObj.put("boardId", Ludo_Constants.Ludo.boardId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.ReconnectGame + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.ReconnectGame, action, Json.Decode(jsonObj.toString()));
    }

    public void PlayAgain(string tableId, SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", ServerSocketManager.instance.playerId);
        jsonObj.put("boardId", Ludo_Constants.Ludo.boardId);
        jsonObj.put("tableId", tableId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.PlayAgain + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.PlayAgain, action, Json.Decode(jsonObj.toString()));
    }
    public void GetProfile(SocketIOAckCallback action)
    {        
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", ServerSocketManager.instance.playerId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.Playerprofile + "    Parameters : " + jsonObj.toString());

        if (ServerSocketManager.instance.playerId.Length <= 0)
        {
            Debug.Log("No Player Id Found.");            
        }
        else
        {
            ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.Playerprofile, action, Json.Decode(jsonObj.toString()));
        }        
    }
   
    public void GetShopDetails(SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", ServerSocketManager.instance.playerId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.GetShopDetails + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.GetShopDetails, action, Json.Decode(jsonObj.toString()));
    }
   
    public void PlayerProfileInfo(SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", ServerSocketManager.instance.playerId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.PlayerProfileInfo + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.PlayerProfileInfo, action, Json.Decode(jsonObj.toString()));
    }

    
    public void PlayerWithFriend(SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", ServerSocketManager.instance.playerId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.PlayerWithFriend + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.PlayerWithFriend, action, Json.Decode(jsonObj.toString()));
    }


    public void SubscribePrivateRoom(string bId, SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", ServerSocketManager.instance.playerId);
        jsonObj.put("boardId", bId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.SubscribePrivateRoom + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.SubscribePrivateRoom, action, Json.Decode(jsonObj.toString()));
    }

    public void StartGame(string bId, SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", ServerSocketManager.instance.playerId);
        jsonObj.put("boardId", bId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.StartGame + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.StartGame, action, Json.Decode(jsonObj.toString()));
    }
    public void LeavePrivateRoom(string bId, SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", ServerSocketManager.instance.playerId);
        jsonObj.put("boardId", bId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.LeavePrivateRoom + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.LeavePrivateRoom, action, Json.Decode(jsonObj.toString()));
    }

    public void ReconnectPrivateDashboard(string bId, SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", ServerSocketManager.instance.playerId);
        jsonObj.put("boardId", bId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.ReconnectPrivateDashboard + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.ReconnectPrivateDashboard, action, Json.Decode(jsonObj.toString()));
    }

    public void GetPlayerDefaultTurnCount(string pId, string bId, SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", pId);
        jsonObj.put("boardId", bId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.GetPlayerDefaultTurnCount + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.GetPlayerDefaultTurnCount, action, Json.Decode(jsonObj.toString()));
    }
  

    public void sendMessage(string bId, string Msg, SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId",  ServerSocketManager.instance.playerId);
        jsonObj.put("boardId", bId);
        jsonObj.put("message", Msg);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.SendMessage + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.SendMessage, action, Json.Decode(jsonObj.toString()));
    }
    public void messageList(string bId, SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId",  ServerSocketManager.instance.playerId);
        jsonObj.put("boardId", bId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.MessageList + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.MessageList, action, Json.Decode(jsonObj.toString()));
    }

    public void playerProfileInfo(string pId, string bId, SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId", pId);
        jsonObj.put("boardId", bId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.PlayerProfileInfo + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.PlayerProfileInfo, action, Json.Decode(jsonObj.toString()));
    }
    public void sendEmoji(string sId, string rId, int eId, string bId, SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("senderId", sId);
        jsonObj.put("receiverId", rId);
        jsonObj.put("emojiId", eId);
        jsonObj.put("boardId", bId);
        jsonObj.put("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        jsonObj.put("productName", Application.productName);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        Debug.Log("$$$$ Emit name  request   " + Ludo_Constants.LudoEvents.SendEmoji + "    Parameters : " + jsonObj.toString());

        ServerSocketManager.instance.rootSocket.Emit(Ludo_Constants.LudoEvents.SendEmoji, action, Json.Decode(jsonObj.toString()));
    }
   
  
    public void GetServerMaintenanceStatus(SocketIOAckCallback action)
    {
        JSON_Object jsonObj = new JSON_Object();
        jsonObj.put("playerId",  ServerSocketManager.instance.playerId);
        jsonObj.put("authToken", ServerSocketManager.instance.accessToken);//DataManager.instance.AccessToken);
        print(Ludo_Constants.LudoEvents.GetServerMaintenanceStatus + " - " + jsonObj.toString());
     //   Game.Lobby.socketManager.Socket.Emit(Ludo_Constants.LudoEvents.GetServerMaintenanceStatus, action, Json.Decode(jsonObj.toString()));
    }

    

   
}
