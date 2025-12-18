using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Runtime.CompilerServices;

public class GamePlayMiniboardScreen : MonoTemplate
{
    #region Variables
    [Header("Components")]
    public Transform pawnsContainer;

    [Header("ScriptableObjects")]
    public LudoPlayer[] GamePlayers;
    public PartOfBoard[] BoardsColors;
    public PathManager pathManager;

    [Header("Boolean")]
    public bool HasJoin;

    [Header("Image/Sprites")]
    public List<Sprite> winnerPosition;

    [Header("Variables")]
    public int ownPlayerSeatIndex = 0;
    public GameStartDataResp currentRoomData;
    public PlayerInfoList AllLudoPlayerInfo;

    [Space]
    public bool isMyTurn = false;
    public bool isGameStarted = false;

    private string currentPlayerId;
    private int playerCount;
    #endregion

    #region Unity default methods
    private void OnEnable()
    {
        currentPlayerId = "";

        /*      
            Game.Lobby.LudoGameSocket.On(LudoConstants.LudoEvents.PlayerActionDetails, OnPlayerActionDetails);
            Game.Lobby.LudoGameSocket.On(LudoConstants.LudoEvents.PlayerInfoList, OnPlayerInfoListReceived);
            Game.Lobby.LudoGameSocket.On(LudoConstants.LudoEvents.GameStarted, OnGameStarted);
            Game.Lobby.LudoGameSocket.On(LudoConstants.LudoEvents.PlayerKill, OnPlayerKill);
            Game.Lobby.LudoGameSocket.On(LudoConstants.LudoEvents.skipMove, OnskipMove);
            Game.Lobby.LudoGameSocket.On(LudoConstants.LudoEvents.KeepCurrentPlayer, OnKeepCurrentPlayer);
            Game.Lobby.LudoGameSocket.On(LudoConstants.LudoEvents.PlayerLeft, OnPlayerLeft);
            Game.Lobby.LudoGameSocket.On(LudoConstants.LudoEvents.PlayerGameWin, OnPlayerGameWin);
            Game.Lobby.LudoGameSocket.On(LudoConstants.LudoEvents.GameFinished, OnGameFinished);
            Game.Lobby.LudoGameSocket.On(LudoConstants.LudoEvents.GameFinishedBots, OnGameFinishedBots);
            Game.Lobby.LudoGameSocket.On(LudoConstants.LudoEvents.ResetGame, OnResetGame);
        */
    }


    private void OnDisable()
    {
        /*  
            Game.Lobby.LudoGameSocket.Off(LudoConstants.LudoEvents.PlayerActionDetails, OnPlayerActionDetails);
            Game.Lobby.LudoGameSocket.Off(LudoConstants.LudoEvents.PlayerInfoList, OnPlayerInfoListReceived);
            Game.Lobby.LudoGameSocket.Off(LudoConstants.LudoEvents.GameStarted, OnGameStarted);
            Game.Lobby.LudoGameSocket.Off(LudoConstants.LudoEvents.PlayerKill, OnPlayerKill);
            Game.Lobby.LudoGameSocket.Off(LudoConstants.LudoEvents.skipMove, OnskipMove);
            Game.Lobby.LudoGameSocket.Off(LudoConstants.LudoEvents.KeepCurrentPlayer, OnKeepCurrentPlayer);
            Game.Lobby.LudoGameSocket.Off(LudoConstants.LudoEvents.PlayerLeft, OnPlayerLeft);
            Game.Lobby.LudoGameSocket.Off(LudoConstants.LudoEvents.PlayerGameWin, OnPlayerGameWin);
            Game.Lobby.LudoGameSocket.Off(LudoConstants.LudoEvents.GameFinished, OnGameFinished);
            Game.Lobby.LudoGameSocket.Off(LudoConstants.LudoEvents.GameFinishedBots, OnGameFinishedBots);
            Game.Lobby.LudoGameSocket.Off(LudoConstants.LudoEvents.ResetGame, OnResetGame);
        */
        currentPlayerId = "";
        ResetAllPawns();
    }
    #endregion

    #region Socket broadcast methods
    public void OnPlayerActionDetails(Socket socket, Packet packet, params object[] args)
    {
       // Debug.Log($"OnPlayerActionDetails Broadcast: {packet}");
        PlayerActionDetails details = JsonUtility.FromJson<PlayerActionDetails>(packet.GetPacketString());

        if (details.boardId != currentRoomData.boardId && details.boardId.Length != 0)
            return;

        LudoPlayer player = GetPlayerFromID(details.playerId);
        player.MovePawnFromPlayerAction_MiniBoard(details);

        // Update all players’ score text from "scores" list
        UpdateAllPlayersScore(details);
    }
    private void UpdateAllPlayersScore(PlayerActionDetails details)
    {
        if (details.scores == null || details.scores.Count == 0)
            return;

        foreach (PlayerScore scoreData in details.scores)
        {
            // Find player by scoreData.playerId
            LudoPlayer p = GetPlayerFromID(scoreData.playerId);
            if (p != null && p.scoreTxt != null)
            {
                Debug.Log(scoreData.score);
                p.scoreTxt.text = scoreData.score.ToString();
            }
        }
    }

    public void OnPlayerInfoListReceived(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
    {
       // Debug.Log("OnPlayerInfoListReceived Mini : " + packet.ToString());

        // Debug.LogError("Mini Board = " + gameObject.activeSelf);
        if (!gameObject.activeSelf)
            return;

        PlayerInfoList playerInfoResp = JsonUtility.FromJson<PlayerInfoList>(packet.GetPacketString());

        // Debug.LogError("Mini playerInfoResp = " + playerInfoResp.boardId + " === " + currentRoomData.boardId);
        if (playerInfoResp.boardId != currentRoomData.boardId && playerInfoResp.boardId.Length != 0)
            return;

        try
        {
            // Debug.LogError("Mini currentRoomData.boardId = " + this.currentRoomData.boardId);
             AllLudoPlayerInfo = playerInfoResp;
            GeneratePlayers(playerInfoResp);
            foreach (LudoPlayer plr in GamePlayers)
            {
                plr.die.GetComponent<Button>().interactable = false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("OnPlayerInfoReceived -> Exception  : " + e);
        }
    }

    public void OnGameStarted(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
    {
       // Debug.Log("GameStarted  : " + packet.ToString());

        if (!gameObject.activeSelf)
            return;

        JSON_Object gameStartedObj = new JSON_Object(packet.GetPacketString());

        if (gameStartedObj.has("boardId") && gameStartedObj.getString("boardId") != currentRoomData.boardId)
            return;

    }

    public void OnPlayerKill(Socket socket, Packet packet, params object[] args)
    {
       // Debug.Log($"OnPlayerKill Broadcast: {packet.ToString()}");

        PlayerKillDetails OnPlayerKilldetails = JsonUtility.FromJson<PlayerKillDetails>(packet.GetPacketString());

        if (OnPlayerKilldetails.boardId != currentRoomData.boardId && OnPlayerKilldetails.boardId.Length != 0)
            return;

        LudoPlayer player = GetPlayerFromID(OnPlayerKilldetails.killedPlayer.playerId);
        player.KillpawnBytokenId_MiniBoard(OnPlayerKilldetails.killedPlayer);
    }

    public void OnskipMove(Socket socket, Packet packet, params object[] args)
    {
       // Debug.Log($"OnskipMove Broadcast: {packet.ToString()}");

        EndOfJourneyData OnskipMove = JsonUtility.FromJson<EndOfJourneyData>(packet.GetPacketString());

        if (OnskipMove.boardId != currentRoomData.boardId && OnskipMove.boardId.Length != 0)
            return;

        LudoPlayer plr = GetPlayerFromID(OnskipMove.playerId);
        if (plr.PlayerId.Equals(ServerSocketManager.instance.playerId))
        {
            plr.checkIbuttonTap();
        }
    }

    public void OnKeepCurrentPlayer(Socket socket, Packet packet, params object[] args)
    {
      //  Debug.Log($"OnKeepCurrentPlayer Broadcast: {packet.ToString()}");

        EndOfJourneyData OnKeepCurrentPlayer = JsonUtility.FromJson<EndOfJourneyData>(packet.GetPacketString());

        if (OnKeepCurrentPlayer.boardId != currentRoomData.boardId && OnKeepCurrentPlayer.boardId.Length != 0)
            return;

        if (OnKeepCurrentPlayer.playerId.Equals(ServerSocketManager.instance.playerId))
        {
            // StartCoroutine(holdVibrateCall(OnKeepCurrentPlayer));
        }
    }

    public void OnPlayerLeft(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
    {
     //   Debug.Log("OnPlayerLeft  : " + packet.ToString());
        if (!gameObject.activeSelf)
            return;

        JSONArray arr = new JSONArray(packet.ToString());

        string Source;
        Source = arr.getString(arr.length() - 1);
        var resp = Source;
        JSON_Object playerObj = new JSON_Object(resp.ToString());

        if (playerObj.has("boardId") && playerObj.getString("boardId") != currentRoomData.boardId)
            return;

        RemovePlayer(playerObj.getString("playerId"));
    }

    public void OnPlayerGameWin(Socket socket, Packet packet, params object[] args)
    {
      //  Debug.Log($"OnPlayerGameWin Broadcast: {packet.ToString()}");
        PlayerGameWin OnPlayerGameWin = JsonUtility.FromJson<PlayerGameWin>(packet.GetPacketString());

        if (OnPlayerGameWin.boardId != currentRoomData.boardId && OnPlayerGameWin.boardId.Length != 0)
            return;

        LudoPlayer player = GetPlayerFromID(OnPlayerGameWin.winners.playerId);
        Sprite WinSet = winnerPosition[OnPlayerGameWin.winners.rank - 1];
        player.WinnerObjDisplay(WinSet, OnPlayerGameWin.winners.playerId);
    }

    public void OnGameFinished(Socket socket, Packet packet, params object[] args)
    {
    //    Debug.Log($"OnGameFinished Broadcast: {packet.ToString()}");

        PlayerGameWin OnGameFinished = JsonUtility.FromJson<PlayerGameWin>(packet.GetPacketString());

        if (OnGameFinished.boardId != currentRoomData.boardId && OnGameFinished.boardId.Length != 0)
            return;
    }

    public void OnGameFinishedBots(Socket socket, Packet packet, params object[] args)
    {
     //   Debug.Log($"OnGameFinishedBots Broadcast: {packet.ToString()}");
        PlayerGameWinBots OnGameFinishedBots = JsonUtility.FromJson<PlayerGameWinBots>(packet.GetPacketString());

        if (OnGameFinishedBots.boardId != currentRoomData.boardId && OnGameFinishedBots.boardId.Length != 0)
            return;
    }

    public void OnResetGame(Socket socket, Packet packet, params object[] args)
    {
     //   Debug.Log($"OnResetGame Broadcast: {packet.ToString()}");
        ResetData(true);
    }

    #endregion

    #region Custom socket broadcast related methods
    // Create inline method of this
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LudoPlayer GetPlayerFromID(string id)
    {
        //StartCoroutine( DiceAnimation(2, SetDiceValue));
        return Array.Find(GamePlayers, x => x.PlayerId == id);
    }

    public void SetRoomDataAndPlay(GameStartDataResp currentRoomData)
    {
        // Debug.LogError("MINI currentRoomData = " + currentRoomData);
        this.currentRoomData = currentRoomData;

        if (currentRoomData.nameSpace.Equals("computer"))
        {
            foreach (LudoPlayer plr in GamePlayers)
            {
                plr.turnTimer.Close();
                plr.IButton.Close();
                plr.Profile.Close();
            }
        }
        else
        {
            foreach (LudoPlayer plr in GamePlayers)
            {
                plr.IButton.Open();
                plr.Profile.Open();
            }
        }

        ResetData(false);
    }

    // called from main game
    public void GameJoinRoomresponse(Socket socket, Packet packet, params object[] args)
    {
        // Debug.LogError("JoinRoomresponse Mini Game.. : " + packet.ToString());
        currentPlayerId = "";
        if (packet != null)
        {
            /*JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;*/

            JSONArray arr = new JSONArray(packet.ToString());
            string Source = arr.getString(arr.length() - 1); // this already gives you the inner JSON array as string
            JSONArray jsonArr = new JSONArray(Source);
            string firstObj = jsonArr.getString(0); // extract the first object inside the array

            PokerEventResponse<HomeDataItem> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<HomeDataItem>>(firstObj);

            // stephen Update
            // Debug.LogError("Joined Room..");
            isGameStarted = true;

            if (!HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                Ludo_UIManager.instance.messagePanel.DisplayConfirmationMessage(HomeDataResp.message, (b) =>
                {
                    if (b)
                    {
                        LeaveRoomDone();
                    }
                });
            }
        }
    }


    public void LeaveRoomDone()
    {
        Ludo_UIManager.instance.socketManager.LeaveRoom((socket, packet, args) =>
        {
            Debug.Log(Ludo_Constants.LudoEvents.LeaveRoom + " respnose  : " + packet.ToString());

            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventResponse<HomeDataItem> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<HomeDataItem>>(resp);

            if (HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                GameStaticData.commingBackFromGame = true;
                PlayerReset();
                // this.Close();
                // GetGameScreen.Close();
                // if (Ludo_UIManager.instance.gamePlayScreen)
                // {
                //     Ludo_UIManager.instance.gamePlayScreen.Close();
                // }
                // GetHomeScreen.Open();
            }
        });

    }
    public void LeaveRoomDoneforTournament()
    {
        Ludo_UIManager.instance.socketManager.LeaveRoom((socket, packet, args) =>
        {
            Debug.Log(Ludo_Constants.LudoEvents.LeaveRoom + " respnose  : " + packet.ToString());
            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventResponse<HomeDataItem> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<HomeDataItem>>(resp);

            if (HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                GameStaticData.commingBackFromGame = true;
                PlayerReset();
                this.Close();
                GetGameScreen.Close();
                if (Ludo_UIManager.instance.gamePlayScreen)
                {
                    Ludo_UIManager.instance.gamePlayScreen.Close();
                }
            }
        });

    }

    public void GeneratePlayers(PlayerInfoList playerData)
    {
        // Debug.LogError("Generating players Mini.." + playerData.playerInfo);

        ResetAllPawns();
        SetPlayerCount();
        bool isOwnPlayerSeatedInList = false;
        foreach (PlayerInfoItem plr in playerData.playerInfo)
        {
            if (plr.id == ServerSocketManager.instance.playerId)
            {
                HasJoin = true;
                isOwnPlayerSeatedInList = true;
                ResetSeatIndexForOwnPlayer(plr.seatIndex);
                break;
            }
        }
        if (!HasJoin && isOwnPlayerSeatedInList)
        {
            ResetData(!HasJoin);
        }
        AllLudoPlayerInfo = GetSortedList(playerData);

        int i = 0;
        foreach (PlayerInfoItem plr in playerData.playerInfo)
        {
            int newSeatIndex = i;

            LudoPlayer player = GamePlayers[newSeatIndex];
            PartOfBoard pob = Array.Find(BoardsColors, x => x.ColorIndex == plr.colorIndex);

            player.PlayerName = plr.username;
            player.Playerphone = plr.mobile;
            pob.SetPartAtPosition(newSeatIndex);

            player.playerInfo = plr;
            player.PlayerId = plr.id;
            player.TimerBlock.PlayerId = plr.id;

            if (!player.gameObject.activeSelf)
            {
                player.Open();
                pob.SetNewParent(pawnsContainer);

                if (plr.profilePic != null && !plr.profilePic.Equals("default.png") && plr.profilePic != "")
                {
                  //  string getImageUrl = Ludo_Constants.LudoConstants.GetBaseUrl + plr.profilePic;
                     string getImageUrl =  plr.profilePic;
                    LudoUtilityManager.Instance.DownloadImage(getImageUrl, player.playerProfilePicture, true, true);
                }
                else
                {
                    player.playerProfilePicture.sprite = Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[plr.avatar];
                }
                // player.playerProfilePicture.Open();
                player.parthighlight = pob;
                player.SetPawns = pob.pawns;

                if (plr.tokens != null && plr.tokens.Count > 0)
                {
                    player.HighlightPlayer(false);
                    player.OpenPawns(true);
                    player.SetPawnsData_MiniBoard(plr.tokens.ToArray());
                }
                else
                {
                    player.Close();
                }
                player.StopTimer();
            }
            else
            {
                if (plr.tokens != null && plr.tokens.Count > 0)
                {
                    player.HighlightPlayer(false);
                    player.OpenPawns(true);
                    player.SetPawnsData_MiniBoard(plr.tokens.ToArray());
                }
                else
                {
                    player.Close();
                }
                player.StopTimer();
            }
            i++;
        }
    }

    private void RemovePlayer(string playerId)
    {
        int index = GetPlayerIndexByPlayerId(playerId);

        if (index != -1)
        {
            if (playerId.Equals(ServerSocketManager.instance.playerId))
            {
                HasJoinedRoom = false;
            }
            GamePlayers[index].parthighlight.highlightPartobj.Play("Idle");

            GamePlayers[index].OpenPawns(false);

            Sprite WinSet = winnerPosition[3];
            if (!playerId.Equals(ServerSocketManager.instance.playerId))
            {
                GamePlayers[index].PlayerLeftObjDisplay(WinSet, playerId);
            }
            StartCoroutine(PlayerLeftObjHide(0f, GamePlayers[index]));
        }
    }

    public int GetPlayerIndexByPlayerId(string playerId)
    {
        for (int i = 0; i < GamePlayers.Length; i++)
        {
            if (GamePlayers[i].PlayerId != null && GamePlayers[i].PlayerId.Equals(playerId))
            {
                return i;
            }
        }
        return -1;
    }
    #endregion

    #region COROUTINES
    public IEnumerator PlayerLeftObjHide(float timer, LudoPlayer plr)
    {
        yield return new WaitForSeconds(timer);

        plr.Close();
        plr.playerInfo = null;

        if (!plr.PlayerId.Equals(ServerSocketManager.instance.playerId))
        {
            if (plr.parthighlight.Position == 2)
            {
                plr.parthighlight.PlayerLeftImage.transform.localRotation = new Quaternion(0, 0, 0.7071068f, 0.7071068f);
            }
            else if (plr.parthighlight.Position == 1)
            {
                plr.parthighlight.PlayerLeftImage.transform.localRotation = new Quaternion(0, 0, 0, 0);

            }
            else if (plr.parthighlight.Position == 3)
            {
                plr.parthighlight.PlayerLeftImage.transform.localRotation = new Quaternion(0, 0, 1, 0);

            }
            plr.parthighlight.PlayerLeftImage.Open();
        }
        else
        {
            plr.parthighlight.PlayerLeftImage.Close();

        }
        if (plr.PlayerId.Equals(ServerSocketManager.instance.playerId))
        {
            LeaveRoomDone();
        }
        plr.PlayerId = "";
    }

    #endregion


    #region Custom Reset methods

    private void PlayerReset()
    {
        foreach (LudoPlayer plr in GamePlayers)
        {
            plr.turnTimer.fillAmount = 0;
        }
        ResetSizeAllPawns();
        ResetAllPawns();
    }

    private void ResetSeatIndexForOwnPlayer(int ownPlayerSeatIndex)
    {
        if (GamePlayers[0].PlayerId != ServerSocketManager.instance.playerId)
        {
            //DestroyInstantiatedObjects();
        }

        int count = ownPlayerSeatIndex;
        for (int i = 0; i < GamePlayers.Length - ownPlayerSeatIndex; i++)
        {
            int newSeatIndex = count;
            count++;

        }
        count = 0;
        for (int i = GamePlayers.Length - ownPlayerSeatIndex; i < GamePlayers.Length; i++)
        {
            int newSeatIndex = count;
            count++;
        }
    }

    void ResetData(bool isPlayerOnly)
    {
        foreach (LudoPlayer plr in GamePlayers)
        {
            if (!isPlayerOnly)
            {
                plr.PlayerName = "";
                plr.Playerphone = "";
                plr.PlayerId = "";
                plr.Close();
            }
            plr.WinnerRankPosition.Close();
            plr.PlayerLeftImg.Close();
            plr.HighlightPlayer(false);
        }
        foreach (PartOfBoard pr in BoardsColors)
        {
            pr.ResetBoard();
        }
    }

    private void ResetAllPawns()
    {
        foreach (LudoPlayer player in GamePlayers)
        {
            player.OpenPawns(false);
        }
    }
    private void ResetSizeAllPawns()
    {
        foreach (LudoPlayer player in GamePlayers)
        {
            foreach (PawnControl pPawns in player.pawns)
            {
                pPawns.ResetPawn();
            }
        }

    }

    private void SetPlayerCount()
    {
        int counter = 0;
        for (int i = 0; i < AllLudoPlayerInfo.playerInfo.Count; i++)
        {
            PlayerInfoItem item = AllLudoPlayerInfo.playerInfo[i];
            string id = item.id;
            if (id != null && id.Length > 0)
            {
                counter++;
                if (id == ServerSocketManager.instance.playerId)
                {
                    ownPlayerSeatIndex = item.seatIndex;
                }
            }
        }
        playerCount = counter;
    }

    private PlayerInfoList GetSortedList(PlayerInfoList list)
    {
        List<PlayerInfoItem> l = list.playerInfo;
        List<PlayerInfoItem> newList = l.Skip(ownPlayerSeatIndex).ToList();
        foreach (PlayerInfoItem item in newList)
        {
            l.Remove(item);
        }
        l.InsertRange(0, newList);
        list.playerInfo = l;

        PlayerInfoList pl = list;
        pl.playerInfo = l;
        return pl;
    }
    #endregion


    #region GETTER_SETTER
    public bool HasJoinedRoom
    {
        get
        {
            return HasJoin;
        }
        set
        {
            HasJoin = value;
        }
    }

    #endregion
}