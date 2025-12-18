using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class GamePlayScreen : MonoTemplate
{
    #region PUBLIC_VARIABLES

    [Header("Components")]
    public Transform pawnsContainer;

    [Header("ScriptableObjects")]
    public LudoPlayer[] GamePlayers;
    public PartOfBoard[] BoardsColors;
    public PathManager pathManager;
    public EmojiData emojiAnimationPrefab;

    [Header("Text")]
    public Text txtWaitingText;
    public TextMeshProUGUI txtGameType;
    public TextMeshProUGUI txtGameTypeGoti ,quickTimer;
    string BoardNumberRef;
    public TextMeshProUGUI txtwinner;
    public TextMeshProUGUI txtRunnerUp;
    public TMP_InputField txtCheating;

    [Header("Boolean")]
    public bool HasJoin;

    [Header("Image/Sprites")]
    public Animation gameStartAnim;
    public List<Sprite> winnerPosition;
    public List<EmojiData> emojiList;
    public Transform emojiParent;
    public Button CheatingButtonEnable;
    public Button ChatButton;
    public Button musicButton;
    public Button vibrateButton;
    public Button chatopButton;
    public Button logoutButton;
    public Button menuOptionButton;
    //  public Image imgEmoji;
    [Header("Variables")]
    public int ownPlayerSeatIndex = 0;
    public GameStartDataResp currentRoomData;
    public PlayerInfoList AllLudoPlayerInfo;
    public WinnerScreen WinnerFinalScreen;
    public botsGameFinishScreen WinnerBotsFinalScreen;
    public MessageScreen MessagePanel;
    public playerDetailsScreen playerDetailspanel;

    [SerializeField]
    public Sprite spriteChatOn;
    public Sprite spriteChatOff;
    [SerializeField]
    public Sprite spriteMusicOn;
    public Sprite spriteMusicOff;
    [SerializeField]
    public Sprite spriteVibrateOn;
    public Sprite spriteVibrateOff;

    [Header("Panel")]
    public GameObject panelNotYourTurmPopup;
    public GameObject settingTopPanel;

    // Stephen Game Complete UI update change
    public RectTransform ScreenShotRect;

    [Header("Particle")]
    public GameObject WinningParticle;
    #endregion

    #region PRIVATE_VARIABLES
    private string currentPlayerId;
    private int playerCount;
    [Space]
    public bool isMyTurn = false;
    public bool isGameStarted = false;
    #endregion

    #region UNITY_CALLBACKS
    // Use this for initialization
    void OnEnable()
    {
        MessagePanel.Close();
        playerDetailspanel.Close();
        txtGameType.transform.parent.gameObject.SetActive(false);
        txtGameTypeGoti.transform.parent.gameObject.SetActive(false);
        txtwinner.transform.parent.gameObject.SetActive(false);
        txtRunnerUp.transform.parent.gameObject.SetActive(false);
        txtWaitingText.transform.parent.gameObject.SetActive(false);
        WinnerFinalScreen.Close();
        WinnerBotsFinalScreen.Close();
        gameStartAnim.gameObject.SetActive(false);
        txtCheating.text = "";
        panelNotYourTurmPopup.SetActive(false);
        settingTopPanel.GetComponent<Animator>().Play("SP_Idle_anim");
        settingTopPanel.SetActive(false);
        menuOptionButton.gameObject.SetActive(true);

        if (PlayerPrefs.GetInt("ChatOption", 1) == 1)
        {
            ChatButton.Open();
            chatopButton.GetComponent<Image>().sprite = spriteChatOn;
        }
        else
        {
            ChatButton.Close();
            chatopButton.GetComponent<Image>().sprite = spriteChatOff;
        }

        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            musicButton.GetComponent<Image>().sprite = spriteMusicOn;
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = spriteMusicOff;
        }

        if (PlayerPrefs.GetInt("Vibration") == 1)
        {
            vibrateButton.GetComponent<Image>().sprite = spriteVibrateOn;
        }
        else
        {
            vibrateButton.GetComponent<Image>().sprite = spriteVibrateOff;
        }
        // txtCheating.gameObject.SetActive(Ludo_UIManager.instance.IscheatEnable);
        CheatingButtonEnable.interactable = Ludo_UIManager.instance.IscheatEnable;

        // stephen changes
        Ludo_UIManager.instance.soundManager.stopBgSound();

        CheatingButtonEnable.interactable = Ludo_UIManager.instance.IscheatEnable;
        currentPlayerId = "";

        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.PlayerActionDetails, OnPlayerActionDetails);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.PlayerInfoList, OnPlayerInfoListReceived);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.GameStartTimer, OnGameStartTimerReceived);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.RollDiceDetails, OnDiceRollDetails);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.RollDice, OnrollingDice);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.GameStarted, OnGameStarted);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.PlayerKill, OnPlayerKill);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.SkipMove, OnskipMove);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.KeepCurrentPlayer, OnKeepCurrentPlayer);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.TurnTimer, OnTurnTimer);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.PlayerLeft, OnPlayerLeft);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.PlayerGameWin, OnPlayerGameWin);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.GameFinished, OnGameFinished);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.GameFinishedBots, OnGameFinishedBots);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.EndOfJourney, OnEndOfJourney);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.ResetGame, OnResetGame);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.DisplayMessage, OndisplayMessage);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.SendEmoji, OnSendEmoji);

        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.QuickGameTimerTick, OnQuickGameTimerTick);
        foreach (EmojiData emoji in emojiList)
        {
            Destroy(emoji.gameObject);
        }

        // Stephen Update
        WinnerFinalScreen.miniBoard.GetComponent<CanvasGroup>().alpha = 0;
        WinnerFinalScreen.miniBoard.SetParent(this.transform);

    }
    void OnDisable()
    {
        MessagePanel.Close();
        playerDetailspanel.Close();
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.PlayerActionDetails, OnPlayerActionDetails);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.PlayerInfoList, OnPlayerInfoListReceived);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.GameStartTimer, OnGameStartTimerReceived);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.RollDiceDetails, OnDiceRollDetails);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.RollDiceDetails, OnrollingDice);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.GameStarted, OnGameStarted);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.PlayerKill, OnPlayerKill);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.SkipMove, OnskipMove);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.TurnTimer, OnTurnTimer);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.KeepCurrentPlayer, OnKeepCurrentPlayer);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.PlayerLeft, OnPlayerLeft);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.PlayerGameWin, OnPlayerGameWin);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.GameFinished, OnGameFinished);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.GameFinishedBots, OnGameFinishedBots);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.EndOfJourney, OnEndOfJourney);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.ResetGame, OnResetGame);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.DisplayMessage, OndisplayMessage);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.SendEmoji, OnSendEmoji);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.QuickGameTimerTick, OnQuickGameTimerTick);
        StopGameStartAnim();
        WinnerFinalScreen.Close();
        WinnerBotsFinalScreen.Close();
        gameStartAnim.gameObject.SetActive(false);
        txtGameType.transform.parent.gameObject.SetActive(false);
        txtGameTypeGoti.transform.parent.gameObject.SetActive(false);
        txtwinner.transform.parent.gameObject.SetActive(false);
        txtRunnerUp.transform.parent.gameObject.SetActive(false);
        WinnerFinalScreen.transform.parent.gameObject.SetActive(false);

        // stephen changes
        Ludo_UIManager.instance.soundManager.PlayBgSound();


        txtCheating.text = "";
        currentPlayerId = "";
        ResetAllPawns();
        foreach (EmojiData emoji in emojiList)
        {
            Destroy(emoji?.gameObject);
        }

     
    }

 
    #endregion

    #region DELEGATE_CALLBACKS

    public void PhotonRollDiceEvent(string playerId, string diceKey)
    {
        if (ServerSocketManager.instance.rootSocket.IsOpen)
        {
            LudoPlayer player = GetPlayerFromID(playerId);
            player.callingRollDice(diceKey);
        }
    }

    private void OnrollingDice(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.RollingDice + " Broadcast response  :" + packet.ToString());

        EndOfJourneyData OnKeepCurrentPlayer = JsonUtility.FromJson<EndOfJourneyData>(packet.GetPacketString());

        if (OnKeepCurrentPlayer.boardId != currentRoomData.boardId && OnKeepCurrentPlayer.boardId.Length != 0)
            return;

        LudoPlayer player = GetPlayerFromID(OnKeepCurrentPlayer.playerId);
        player.callingRollDice(OnKeepCurrentPlayer.diceKey);
    }
    private void OnskipMove(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.SkipMove + " Broadcast response  :" + packet.ToString());
		
        Ludo_UIManager.instance.miniBoardGamePlayScreen.OnskipMove(socket, packet, args);

        EndOfJourneyData OnskipMove = JsonUtility.FromJson<EndOfJourneyData>(packet.GetPacketString());

        if (OnskipMove.boardId != currentRoomData.boardId && OnskipMove.boardId.Length != 0)
            return;

        if (OnskipMove.playerId.Equals(ServerSocketManager.instance.playerId))
        {
            Ludo_UIManager.instance.soundManager.SkipStepSound();
        }
        LudoPlayer plr = GetPlayerFromID(OnskipMove.playerId);
        if (plr.PlayerId.Equals(ServerSocketManager.instance.playerId))
        {
            plr.checkIbuttonTap();
        }
    }
    private void OnKeepCurrentPlayer(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.KeepCurrentPlayer + " Broadcast response  :" + packet.ToString());
        Ludo_UIManager.instance.miniBoardGamePlayScreen.OnKeepCurrentPlayer(socket, packet, args);

        EndOfJourneyData OnKeepCurrentPlayer = JsonUtility.FromJson<EndOfJourneyData>(packet.GetPacketString());

        if (OnKeepCurrentPlayer.boardId != currentRoomData.boardId && OnKeepCurrentPlayer.boardId.Length != 0)
            return;

        if (OnKeepCurrentPlayer.playerId.Equals(ServerSocketManager.instance.playerId))
        {
            StartCoroutine(holdVibrateCall(OnKeepCurrentPlayer));
        }
    }

    private void OnSendEmoji(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.SendEmoji + " Broadcast response  :" + packet.ToString());
        SendEmoji OnsendEmojiData = JsonUtility.FromJson<SendEmoji>(packet.GetPacketString());
        LudoPlayer senderPlr = GetPlayerFromID(OnsendEmojiData.senderId);
        LudoPlayer receiverPlr = GetPlayerFromID(OnsendEmojiData.receiverId);
        //    playerDetailspanel.Close();
        Vector3 snderPos = senderPlr.playerProfilePicture.transform.position;
        Vector3 receiverPos = receiverPlr.playerProfilePicture.transform.position;
        StartCoroutine(DistributeEmoji(snderPos, receiverPos, OnsendEmojiData.emojiId));
    }

    private IEnumerator DistributeEmoji(Vector3 fromPosition, Vector3 toPosition, int id)
    {
        yield return new WaitForEndOfFrame();

        EmojiData emoji = Instantiate(emojiAnimationPrefab) as EmojiData;
        emojiList.Add(emoji);
        emoji.transform.SetParent(emojiParent, false);
        emoji.transform.SetAsLastSibling();
        emoji.Open();

        Vector3 fromPos = fromPosition;
        Vector3 fromAngle = fromPosition;

        Vector3 toPos = toPosition;
        Vector3 toAngle = toPosition;

        emoji.selectimg.sprite = Ludo_UIManager.instance.LudoEmoticansManager[id].emodefault;
        emoji.transform.position = fromPos;

        StartCoroutine(MoveDistributedEmoji(emoji, fromPos, toPos, fromAngle, toAngle, id));
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator MoveDistributedEmoji(EmojiData emoji, Vector3 fromPos, Vector3 toPos, Vector3 fromAngle, Vector3 toAngle, int id)
    {
        float a = 0f;
        float time = 1f;
        while (a < 1)
        {
            a += Time.deltaTime * (1 / time);
            emoji.transform.position = Vector3.Lerp(fromPos, toPos, a);
            yield return 0;
        }
        StartCoroutine(PlayEmojiAnim(emoji, id));

        yield return 0;
    }
    private IEnumerator PlayEmojiAnim(EmojiData emoji, int id)
    {
        float waitOnFrames = .025f / 1;

        for (int i = 0; i < 3; i++)
        {
            for (int j = i; j < Ludo_UIManager.instance.LudoEmoticansManager[id].emoSprites.Count - 1; j++)
            {
                if (emoji)
                    emoji.selectimg.sprite = Ludo_UIManager.instance.LudoEmoticansManager[id].emoSprites[j];

                yield return new WaitForSeconds(waitOnFrames);
            }
        }
        StartCoroutine(DestroyEmoji(emoji));
    }
    public IEnumerator DestroyEmoji(EmojiData emoji)
    {
        yield return new WaitForSeconds(3f);
        //emoji.Close();
        //emoji.selectimg.sprite = null;
        emojiList.Remove(emoji);
        //Destroy(emoji.gameObject);
    }
    private void OndisplayMessage(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.DisplayMessage + " Broadcast response  :" + packet.ToString());
        MessageDisplayData OnMessageDisplayData = JsonUtility.FromJson<MessageDisplayData>(packet.GetPacketString());
        LudoPlayer player = GetPlayerFromID(OnMessageDisplayData.playerId);
        player.showChatBubble(OnMessageDisplayData.playerId, OnMessageDisplayData.message);

    }
    private void OnResetGame(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.ResetGame + " Broadcast response  :" + packet.ToString());
        Ludo_UIManager.instance.miniBoardGamePlayScreen.OnResetGame(socket, packet, args);

        ResetData(true);
    }

    private void OnGameFinishedBots(Socket socket, Packet packet, params object[] args)
    {
        // Check if message popup is on
        if (Ludo_UIManager.instance.messagePanel.gameObject.activeInHierarchy)
        {
            // Close Message Pannel
            Ludo_UIManager.instance.messagePanel.Close();
        }

        Debug.Log(Ludo_Constants.LudoEvents.GameFinishedBots + " Broadcast response  :" + packet.ToString());

        Ludo_UIManager.instance.miniBoardGamePlayScreen.OnGameFinishedBots(socket, packet, args);

        PlayerGameWinBots OnGameFinishedBots = JsonUtility.FromJson<PlayerGameWinBots>(packet.GetPacketString());

        if (OnGameFinishedBots.boardId != currentRoomData.boardId && OnGameFinishedBots.boardId.Length != 0)
            return;

        WinnerBotsFinalScreen.SetData(OnGameFinishedBots);
    }

    private void OnEndOfJourney(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.EndOfJourney + " Broadcast response  :" + packet.ToString());


        EndOfJourneyData OnEndOfJourney = JsonUtility.FromJson<EndOfJourneyData>(packet.GetPacketString());

        if (OnEndOfJourney.boardId != currentRoomData.boardId && OnEndOfJourney.boardId.Length != 0)
            return;

        LudoPlayer player = GetPlayerFromID(OnEndOfJourney.playerId);
        player.AddPawnInHome();
    }

    private void OnGameFinished(Socket socket, Packet packet, params object[] args)
    {
        // Check if message popup is on
        if (Ludo_UIManager.instance.messagePanel.gameObject.activeInHierarchy)
        {
            // Close Message Pannel
            Ludo_UIManager.instance.messagePanel.Close();
        }
         Debug.Log(Ludo_Constants.LudoEvents.GameFinished + " Broadcast response  :" + packet.ToString());
    
        Ludo_UIManager.instance.miniBoardGamePlayScreen.OnGameFinished(socket, packet, args);

        PlayerGameWin OnGameFinished = JsonUtility.FromJson<PlayerGameWin>(packet.GetPacketString());

        if (OnGameFinished.boardId != currentRoomData.boardId && OnGameFinished.boardId.Length != 0)
            return;

        WinnerFinalScreen.SetData(OnGameFinished.winners, OnGameFinished.shareMessage);
        //WinnerBotsFinalScreen.SetData(OnGameFinished.winners);
    }
    private void OnQuickGameTimerTick(Socket socket, Packet packet, params object[] args)
    {
       // Debug.Log(Ludo_Constants.LudoEvents.QuickGameTimerTick + " Broadcast response  :" + packet.ToString());
        if (!gameObject.activeSelf)
            return;

        isGameStarted = false;
        QuickGameTimer quickGameTimer = JsonUtility.FromJson<QuickGameTimer>(packet.GetPacketString());

       

        if (quickGameTimer.boardId != currentRoomData.boardId && quickGameTimer.boardId.Length != 0)
            return;

        if (quickGameTimer.boardId.Equals(Ludo_Constants.Ludo.boardId))
        {
            string timeFormatted = $"{quickGameTimer.remainingSeconds / 60:00}:{quickGameTimer.remainingSeconds % 60:00}";

            quickTimer.text ="Time Left:"+ timeFormatted;// + " " + gameStartTimerDataResp.timer + " Seconds";
           
        }
       
    }
#if UNITY_EDITOR
    // private void OnGUI()
    // {
    //     if (GUI.Button(new Rect(10, 10, 250, 150), "Poke!"))
    //     {
    //         Debug.LogError("Photo Khich meri....");
    //         NativeShareManager.Instance.TakeScreenshotOfElement(ScreenShotRect, LudoLudo_Constants.LudoLudo_Constants._screenShotPath);
    //     }
    // }
#endif


    private void OnPlayerGameWin(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.PlayerGameWin + " Broadcast response  :" + packet.ToString());
        Ludo_UIManager.instance.miniBoardGamePlayScreen.OnPlayerGameWin(socket, packet, args);

        PlayerGameWin OnPlayerGameWin = JsonUtility.FromJson<PlayerGameWin>(packet.GetPacketString());

        if (OnPlayerGameWin.boardId != currentRoomData.boardId && OnPlayerGameWin.boardId.Length != 0)
            return;

        LudoPlayer player = GetPlayerFromID(OnPlayerGameWin.winners.playerId);
        Sprite WinSet = winnerPosition[OnPlayerGameWin.winners.rank - 1];
        player.WinnerObjDisplay(WinSet, OnPlayerGameWin.winners.playerId);

    }
    private void OnPlayerKill(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.PlayerKill + " Broadcast response  :" + packet.ToString());
        //     Debug.Log($"OnPlayerKill Broadcast response  : {packet.ToString()}");
        Ludo_UIManager.instance.miniBoardGamePlayScreen.OnPlayerKill(socket, packet, args);

        PlayerKillDetails OnPlayerKilldetails = JsonUtility.FromJson<PlayerKillDetails>(packet.GetPacketString());

        if (OnPlayerKilldetails.boardId != currentRoomData.boardId && OnPlayerKilldetails.boardId.Length != 0)
            return;

        LudoPlayer player = GetPlayerFromID(OnPlayerKilldetails.killedPlayer.playerId);
        player.KillpawnBytokenId(OnPlayerKilldetails.killedPlayer);
    }

    private void OnPlayerActionDetails(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.PlayerActionDetails + " Broadcast response  :"+ packet.ToString());
        Ludo_UIManager.instance.miniBoardGamePlayScreen.OnPlayerActionDetails(socket, packet, args);


        PlayerActionDetails details = JsonUtility.FromJson<PlayerActionDetails>(packet.GetPacketString());

        UpdateAllPlayersScore(details);
        //if (details.boardId.Length == 0 || details.boardId != currentRoomData.boardId)
        if (details.boardId != currentRoomData.boardId && details.boardId.Length != 0)
            return;

        LudoPlayer player = GetPlayerFromID(details.playerId);
        player.MovePawnFromPlayerAction(details);
        // Update all players’ score text from "scores" list
    }
    private void UpdateAllPlayersScore(PlayerActionDetails details)
    {
        if (details.scores == null || details.scores.Count == 0)
        {
            Debug.Log("NULLLLL");
            return;
        }

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

    private void OnDiceRollDetails(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.RollDiceDetails + " Broadcast response  :" + packet.ToString());

        RollDiceDetails diceDetails = JsonUtility.FromJson<RollDiceDetails>(packet.GetPacketString());

        //if (diceDetails.boardId != currentRoomData.boardId && diceDetails.boardId.Length != 0)
        if (diceDetails.boardId.Length == 0 || diceDetails.boardId != currentRoomData.boardId)
            return;

        LudoPlayer player = GetPlayerFromID(diceDetails.playerId);
        player.die.diceValue = diceDetails.diceValue;
        player.SetDiceData(diceDetails);
        player.parthighlight.highlightPartobj.Play("Idle");
        player.StopRollingDice(diceDetails);
    }

    public void JoinGameresponse(Socket socket, Packet packet, params object[] args)
    {
        JoinGameResponseRecieved = true;
        Ludo_UIManager.instance.OpenLoader(false);
        if (RecallJoinRoomRef != null)
        {
            StopCoroutine(RecallJoinRoomRef);
        }
        Debug.Log(Ludo_Constants.LudoEvents.JoinGame + " respnose  : " + packet.ToString());
        currentPlayerId = "";
        if (packet != null)
        {
            /*  JSONArray arr = new JSONArray(packet.ToString());
              string Source;
              Source = arr.getString(arr.length() - 1);
              var resp = Source;*/

            JSONArray arr = new JSONArray(packet.ToString());
            string Source = arr.getString(arr.length() - 1); // this already gives you the inner JSON array as string
            JSONArray jsonArr = new JSONArray(Source);
            string firstObj = jsonArr.getString(0); // extract the first object inside the array

            PokerEventResponse<HomeDataItem> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<HomeDataItem>>(firstObj);

            // stephen Update
            Debug.Log("Joined Room..");
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

        Ludo_UIManager.instance.miniBoardGamePlayScreen.GameJoinRoomresponse(socket, packet, args);
    }

    public void ReconnectJoinRoomresponse(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.ReconnectGame + " respnose  : " + packet.ToString());
        currentPlayerId = "";
        if (packet != null)
        {
            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventResponse<HomeDataItem> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<HomeDataItem>>(resp);

            if (!HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                Ludo_UIManager.instance.messagePanel.DisplayMessage(HomeDataResp.message, LeaveRoomDone);

            }
        }
    }
    void OnPlayerLeft(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.PlayerLeft + " Broadcast response  :" + packet.ToString());
        Ludo_UIManager.instance.miniBoardGamePlayScreen.OnPlayerLeft(socket, packet, args);

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
    private void OnTurnTimer(Socket socket, Packet packet, params object[] args)
    {
        Debug.LogWarning(Ludo_Constants.LudoEvents.TurnTimer + " Broadcast response  :" + packet.ToString());


        // Check if game is started or not
        if (isGameStarted == false)
        {
            txtWaitingText.transform.parent.gameObject.SetActive(false);
            Invoke(nameof(StopGameStartAnim), 0.25f);
        }

        TurnTimerData timerData = JsonUtility.FromJson<TurnTimerData>(packet.GetPacketString());

        if (timerData.boardId != currentRoomData.boardId && timerData.boardId.Length != 0)
            return;


        LudoPlayer pplr = GetPlayerFromID(timerData.playerId);
        if (pplr == null)
            return;

        string playerId = timerData.playerId;

        //if (playerId.Equals(Ludo_UIManager.instance.assetOfGame.SavedLoginData.playerId))
        //{
        //    isMyTurn = true;
        //}
        //else
        //{
        //    isMyTurn = false;
        //}

        if (currentPlayerId != playerId)
        {
            LudoPlayer plr = GetPlayerFromID(currentPlayerId);
            //plr.StopTimer();
            if (plr != null)
            {
                plr.ResetPlayerForTurn(false, false);
                plr.parthighlight.HighlightBoard(false);
            }
        }
        else
        {
            LudoPlayer plr = GetPlayerFromID(currentPlayerId);
            plr.parthighlight.HighlightBoard(true);
        }
        LudoPlayer player = GetPlayerFromID(playerId);
        //player.StopTimer();
        if (currentRoomData.nameSpace.Equals("computer"))
        {
            if (timerData.timer == 0)
            {
                player.ResetPlayerForTurn(false, !timerData.diceRolled);
            }
            else
            {
                player.ResetPlayerForTurn(false, !timerData.diceRolled);
            }
        }
        else
        {
            if (timerData.timer == 0)
            {
                player.ResetPlayerForTurn(false, false);
            }
            else
            {
                player.ResetPlayerForTurn(true, !timerData.diceRolled);
            }
        }

        player.SetTimerValue((timerData.timer), timerData.maxTimer);
        currentPlayerId = playerId;

        for (int i = 0; i < GamePlayers.Length; i++)
        {
            if (GamePlayers[i].PlayerId != timerData.playerId)
            {
                if (GamePlayers[i].gameObject.activeSelf)
                {
                    foreach (PawnControl pc in GamePlayers[i].pawns)
                    {
                        pc.highlightAnim.SetActive(false);
                    }
                }
            }
        }

        if (timerData.diceRolled && playerId == ServerSocketManager.instance.playerId)
        {
            TurnTimerPlayerData = timerData.turnPlayerData;
            LudoPlayer ludoPlayer = GetPlayerFromID(playerId);

            int index = 0;
            foreach (PawnControl pawn in ludoPlayer.pawns)
            {
                pawn.currentDistance = timerData.turnPlayerData.tokens[index].distance;
                index++;
            }
        }
    }

    private void OnPlayerInfoListReceived(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
    {
        //Debug.Log("OnPlayerInfoListReceived  : " + packet.ToString());
        Debug.Log(Ludo_Constants.LudoEvents.PlayerInfoList + " Broadcast response  :" + packet.ToString());
        Ludo_UIManager.instance.miniBoardGamePlayScreen.OnPlayerInfoListReceived(socket, packet, args);


        // Debug.LogError("Normal Board = " + gameObject.activeSelf);
        if (!gameObject.activeSelf)
            return;

        PlayerInfoList playerInfoResp = JsonUtility.FromJson<PlayerInfoList>(packet.GetPacketString());

        // Debug.LogError(" playerInfoResp = " + playerInfoResp.boardId + " === " + currentRoomData.boardId);
        if (playerInfoResp.boardId != currentRoomData.boardId && playerInfoResp.boardId.Length != 0)
            return;

        try
        {
            // Debug.LogError("currentRoomData.boardId = " + this.currentRoomData.boardId);
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

    private void OnGameStartTimerReceived(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.GameStartTimer + " Broadcast response  :" + packet.ToString());
        if (!gameObject.activeSelf)
            return;

        isGameStarted = false;
        gameStartTimerData gameStartTimerDataResp = JsonUtility.FromJson<gameStartTimerData>(packet.GetPacketString());

        // Check if we got timer
        if (gameStartTimerDataResp.timer == 3)
        {
            // Sub string at for showing only 10 characters
            string s = gameStartTimerDataResp.boardNumber;
            if (s.Length > 10)
                s = s.Substring(0, 10);
            BoardNumberRef = $"[{s}]";
        }

        if (gameStartTimerDataResp.boardId != currentRoomData.boardId && gameStartTimerDataResp.boardId.Length != 0)
            return;

        if (gameStartTimerDataResp.boardId.Equals(Ludo_Constants.Ludo.boardId))
        {
            txtWaitingText.text = gameStartTimerDataResp.message;// + " " + gameStartTimerDataResp.timer + " Seconds";
            txtWaitingText.transform.parent.gameObject.SetActive(true);
            if (gameStartTimerDataResp.timer.Equals(2))
            {
                //Ludo_UIManager.instance.soundManager.InGameStartedOnce();
                gameStartAnim.gameObject.SetActive(true);
                gameStartAnim.Play();

                // stephen update

                StartCoroutine(StartGameCall(1f));
                txtWaitingText.transform.parent.gameObject.SetActive(false);
            }
            if (gameStartTimerDataResp.timer < 2)
            {
                txtWaitingText.transform.parent.gameObject.SetActive(false);
            }
        }
        if (gameStartTimerDataResp.timer.Equals(0))
        {
            txtWaitingText.transform.parent.gameObject.SetActive(false);
            Invoke(nameof(StopGameStartAnim), 1.5f);
        }

        // Close because Unwanted invoking..
        // CancelInvoke(nameof(StopGameStartAnim));
        // Invoke(nameof(StopGameStartAnim), gameStartTimerDataResp.timer + 1.5f);
    }
    private void OnGameStarted(Socket socket, BestHTTP.SocketIO.Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.GameStarted + " Broadcast response  :" + packet.ToString());
     //   Debug.Log(Constants.LudoEvents.GameStarted +"on GameStarted   response  : " + packet.ToString());
        Ludo_UIManager.instance.miniBoardGamePlayScreen.OnGameStarted(socket, packet, args);

        if (!gameObject.activeSelf)
            return;

        JSON_Object gameStartedObj = new JSON_Object(packet.GetPacketString());

        if (gameStartedObj.has("boardId") && gameStartedObj.getString("boardId") != currentRoomData.boardId)
            return;

        txtWaitingText.text = "";
        txtWaitingText.transform.parent.gameObject.SetActive(false);
        StopGameStartAnim();
        Ludo_UIManager.instance.WaitForGameScreen.Close();
    }
    #endregion

    #region PUBLIC_METHODS
    public void ChatButtonTap()
    {
        MessagePanel.SetAndOpen();
    }

    public void cheatingTextEnable()
    {
        txtCheating.gameObject.SetActive(!txtCheating.gameObject.activeSelf);
        if (!txtCheating.gameObject.activeSelf)
        {
            txtCheating.text = "";
        }
    }

    public void txtCheatingEndEdit()
    {
        if (!gameObject.activeSelf)
        {
            txtCheating.text = "";
        }

        if (txtCheating.text != "")
        {
            int cheatNum = int.Parse(txtCheating.text);
            if (cheatNum > 6 || cheatNum.Equals(0))
            {
                txtCheating.text = "";
            }
        }
        return;
    }

    public void LeaveRoom()
    {
        Debug.Log("Check.. " + isGameStarted);
        Debug.Log("Check.. " + currentRoomData.nameSpace);

        if (isGameStarted)
        {
            string messageDisplay = "";
            if (this.currentRoomData.nameSpace.Equals("normal"))
            {
                messageDisplay = "You loose your coins once you leave the game";
            }
            else
            {
                messageDisplay = "Are you sure you want to leave the game";
            }

            GetMessagePanel.DisplayConfirmationMessage(messageDisplay, (b) =>
            {
                //Debug.Log
                if (b)
                {
                    LeaveRoomDone();
                }
            });

            //Issue that's why call again --Tournament Popus Reduce Temperarry 
            GetMessagePanel.DisplayConfirmationMessage(messageDisplay, (b) =>
            {
                //Debug.Log
                if (b)
                {
                    LeaveRoomDone();
                }
            });
        }
        else
        {
            // if game is in waiting mode
            GetMessagePanel.DisplayMessage("Can not leave room before starting game!!");
        }
    }

    public void SetRoomDataAndPlay(GameStartDataResp currentRoomDataRun)
    {
        Debug.Log($" not emit just get sub data SetRoomDataAndPlay..{JsonUtility.ToJson(currentRoomDataRun)}");
        Ludo_UIManager.instance.miniBoardGamePlayScreen.SetRoomDataAndPlay(currentRoomDataRun);

        this.currentRoomData = new GameStartDataResp();
        this.currentRoomData = currentRoomDataRun;
        if (currentRoomDataRun.nameSpace.Equals("computer"))
        {
            foreach (LudoPlayer plr in GamePlayers)
            {
                plr.turnTimer.Close();
                plr.IButton.Close();
                plr.Profile.Close();
            }
            ChatButton.Close();
        }
        else if (currentRoomDataRun.nameSpace.Equals("tournament"))
        {
            foreach (LudoPlayer plr in GamePlayers)
            {
                //plr.IButton.Open();
                plr.Profile.Open();
            }
            ChatButton.Open();
        }
        else
        {
            foreach (LudoPlayer plr in GamePlayers)
            {
                plr.IButton.Open();
                plr.Profile.Open();
            }
            ChatButton.Open();
        }

        Ludo_Constants.Ludo.boardId = currentRoomDataRun.boardId;
    //    Game.Lobby.SetSocketNamespace = currentRoomDataRun.nameSpace;
        ResetData(false);
        this.Open();

        Debug.Log("Reached Before Join Game   " + this.currentRoomData.boardId+"   " + this.currentRoomData.tableId);
        Ludo_UIManager.instance.socketManager.JoinGame(this.currentRoomData.boardId, this.currentRoomData.tableId, this.currentRoomData.tournamentId, JoinGameresponse);
      
        // Recall Check running game        
        RecallJoinRoomRef = StartCoroutine(RecallCheckRunningGame(this.currentRoomData, 3f));
    }

    private bool JoinGameResponseRecieved = false;
    private Coroutine RecallJoinRoomRef = null;
    IEnumerator RecallCheckRunningGame(GameStartDataResp currentRoomDataRunRef, float repeatRate)
    {
        while (JoinGameResponseRecieved == false)
        {
            yield return new WaitForSeconds(repeatRate);
            if (ServerSocketManager.instance.rootSocket.IsOpen)
            {
                Debug.Log("RecallJoinGame : " + JoinGameResponseRecieved);
                Ludo_UIManager.instance.socketManager.JoinGame(currentRoomDataRunRef.boardId, currentRoomDataRunRef.tableId, currentRoomDataRunRef.tournamentId, JoinGameresponse);
            }
        }
    }

    public void ThrowToLobby()
    {
        if (RecallJoinRoomRef != null)
        {
            StopCoroutine(RecallJoinRoomRef);
        }

     //   Debug.Log("Cancel Invoke");
        JoinGameResponseRecieved = false;
        this.Close();
    }

    public void OnMenuOptionButtonTap()
    {
     //   Debug.Log("OnMenuOptionButtonTap");
        settingTopPanel.SetActive(true);
        menuOptionButton.gameObject.SetActive(false);
        settingTopPanel.GetComponent<Animator>().Play("SP_In_anim");
    }
    public void OnSettingPanelTap()
    {
        //Debug.Log("OnSettingPanelTap");
        settingTopPanel.GetComponent<Animator>().Play("SP_Out_anim");
        StartCoroutine(setSettinOff());
    }
    IEnumerator setSettinOff()
    {
        yield return new WaitForSeconds(0.5f);
        settingTopPanel.SetActive(false);
        menuOptionButton.gameObject.SetActive(true);
    }
    public void MusicButtonTap()
    {

        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            PlayerPrefs.SetInt("Sound", 0);
            musicButton.GetComponent<Image>().sprite = spriteMusicOff;
        }
        else
        {
            PlayerPrefs.SetInt("Sound", 1);
            musicButton.GetComponent<Image>().sprite = spriteMusicOn;
        }
        Debug.Log("bgMusic = >" + PlayerPrefs.GetInt("bgMusic"));
    }

    public void VibrateButtonTap()
    {

        if (PlayerPrefs.GetInt("Vibration") == 1)
        {
            PlayerPrefs.SetInt("Vibration", 0);
            vibrateButton.GetComponent<Image>().sprite = spriteVibrateOff;
        }
        else
        {
            PlayerPrefs.SetInt("Vibration", 1);
            vibrateButton.GetComponent<Image>().sprite = spriteVibrateOn;
        }
        Debug.Log("bgMusic = >" + PlayerPrefs.GetInt("bgMusic"));
    }
    public void ChatOptionButtonTap()
    {

        if (PlayerPrefs.GetInt("ChatOption", 1) == 1)
        {
            PlayerPrefs.SetInt("ChatOption", 0);
            chatopButton.GetComponent<Image>().sprite = spriteChatOff;
            ChatButton.Close();
        }
        else
        {
            PlayerPrefs.SetInt("ChatOption", 1);
            chatopButton.GetComponent<Image>().sprite = spriteChatOn;
            ChatButton.Open();
        }
        Debug.Log("bgMusic = >" + PlayerPrefs.GetInt("bgMusic"));
    }
    #endregion

    #region PRIVATE_METHODS
    void PlayWelcomeSound()
    {
        Ludo_UIManager.instance.soundManager.WelcomeToLudoGiantOnce();
    }

    void StopGameStartAnim()
    {
        // Debug.LogError("StopGameStartAnim...");
        isGameStarted = true;
        StopCoroutine(nameof(StartGameCall));
        gameStartAnim.Stop();
        gameStartAnim.gameObject.SetActive(false);
        //Ludo_UIManager.instance.soundManager.InGameStartedStop();
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
    private void RemovePlayer(string playerId)
    {
        int index = GetPlayerIndexByPlayerId(playerId);

        if (index != -1)
        {
            //			Ludo_UIManager.Instance.historyPanel.AddPlayerLeftLog(GamePlayers[index].playerInfo.username);
            //			PokerPlayer plr = GetPlayerById (playerId);
            //			string Message = plr.txtUsername.text + " is " + LudoLudo_Constants.Poker.PokerPlayerStatusLeft;
            //			StartCoroutine (RemovePLayerInfo (Message));		

            if (playerId.Equals(ServerSocketManager.instance.playerId))
            {
                HasJoinedRoom = false;
            }

            //GamePlayers[index].parthighlight.HighlightBoard(false);
            GamePlayers[index].parthighlight.highlightPartobj.Play("Idle");


            GamePlayers[index].OpenPawns(false);
            //GamePlayers[index].parthighlight.OpenPawns(false);

            Sprite WinSet = winnerPosition[3];
            if (!playerId.Equals(ServerSocketManager.instance.playerId))
            {
                GamePlayers[index].PlayerLeftObjDisplay(WinSet, playerId);
            }

            StartCoroutine(PlayerLeftObjHide(0f, GamePlayers[index]));


        }
    }
    public void LeaveRoomDone()
    {
        Ludo_UIManager.instance.socketManager.LeaveRoom((socket, packet, args) =>
        {
            Debug.Log(Ludo_Constants.LudoEvents.LeaveRoom + " respnose  : " + packet.ToString());
            /*  JSONArray arr = new JSONArray(packet.ToString());
              string Source;
              Source = arr.getString(arr.length() - 1);
              var resp = Source;*/
            JSONArray arr = new JSONArray(packet.ToString());
            string Source = arr.getString(arr.length() - 1); // this already gives you the inner JSON array as string
            JSONArray jsonArr = new JSONArray(Source);
            string firstObj = jsonArr.getString(0); // extract the first object inside the array


            PokerEventResponse<HomeDataItem> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<HomeDataItem>>(firstObj);

            if (HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                GameStaticData.commingBackFromGame = true;
                StopGameStartAnim();
                Ludo_UIManager.instance.soundManager.InGameStartedStop();
                PlayerReset();
                this.Close();
                GetGameScreen.Close();
                if (Ludo_UIManager.instance.gamePlayScreen)
                {
                    Ludo_UIManager.instance.gamePlayScreen.Close();
                }
                GetHomeScreen.Open();
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
                StopGameStartAnim();
                Ludo_UIManager.instance.soundManager.InGameStartedStop();
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

    public int GetSeatIndexForPlayer(int index)
    {
        int newSeatIndex = ownPlayerSeatIndex + (GamePlayers.Length - index);
        if (newSeatIndex > GamePlayers.Length - 1)
        {
            newSeatIndex = newSeatIndex - GamePlayers.Length;
        }

        //int newSeatIndex = (ownPlayerSeatIndex + index) % playerCount;
        return newSeatIndex;
    }

    public void GeneratePlayers(PlayerInfoList SpadePlayerData)
    {
        // Debug.LogError("Generating players.." + SpadePlayerData.playerInfo);
        ResetAllPawns();
        SetPlayerCount();
        bool isOwnPlayerSeatedInList = false;
        foreach (PlayerInfoItem plr in SpadePlayerData.playerInfo)
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
        AllLudoPlayerInfo = GetSortedList(SpadePlayerData);

        int i = 0;
        foreach (PlayerInfoItem plr in SpadePlayerData.playerInfo)
        {
            //int newSeatIndex = GetSeatIndexForPlayer(plr.seatIndex);
            int newSeatIndex = i;

            LudoPlayer player = GamePlayers[newSeatIndex];
            //PartOfBoard pob = BoardsColors[plr.colorIndex];
            PartOfBoard pob = Array.Find(BoardsColors, x => x.ColorIndex == plr.colorIndex);
            //Debug.Log($"POB NAme => {pob.name} | colorIndex NAme => {plr.colorIndex} | New Seat Index => {newSeatIndex}");

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

                /* if (plr.profilePic.Equals(null) || plr.profilePic.Equals(""))
                 {
                     print("avatar => " + plr.avatar);
                     player.playerProfilePicture.sprite = Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[plr.avatar];
                 }
                 else
                 {
                     string getImageUrl = LudoLudo_Constants.LudoLudo_Constants.GetBaseUrl + "/" + plr.profilePic;
                     UtilityManager.Instance.DownloadImage(getImageUrl, player.playerProfilePicture, true);
                 }*/
                if (plr.profilePic != null && !plr.profilePic.Equals("default.png") && plr.profilePic != "")
                {
                    string getImageUrl =plr.profilePic;
                    LudoUtilityManager.Instance.DownloadImage(getImageUrl, player.playerProfilePicture, true, true);
                }
                //if (plr.profilePic == null || plr.profilePic.Equals(""))
                else
                {
                    player.playerProfilePicture.sprite = Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[plr.avatar];
                }
                player.playerProfilePicture.Open();
                player.parthighlight = pob;
                player.SetPawns = pob.pawns;

                if (plr.tokens != null && plr.tokens.Count > 0)
                {
                    player.HighlightPlayer(false);
                    player.OpenPawns(true);
                    //    player.path = null;
                    player.SetPawnsData(plr.tokens.ToArray());

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
                    //    player.path = null;
                    player.SetPawnsData(plr.tokens.ToArray());
                }
                else
                {
                    player.Close();
                }
                player.StopTimer();
            }

            i++;
            //    player.parthighlight.ResetBoard();
        }

        if (SpadePlayerData.gamePlayercounts != null && SpadePlayerData.gamePlayercounts.Length > 0)
        {
            txtGameType.text = SpadePlayerData.gamePlayercounts;
            txtGameType.transform.parent.gameObject.SetActive(true);
        }
        if (SpadePlayerData.gamePlaySelectedGoti != null && SpadePlayerData.gamePlaySelectedGoti.Length > 0)
        {
            txtGameTypeGoti.text = SpadePlayerData.gamePlaySelectedGoti + " " + BoardNumberRef;
            txtGameTypeGoti.transform.parent.gameObject.SetActive(true);
        }
        if (SpadePlayerData.winnerPrizeData != null && SpadePlayerData.winnerPrizeData.Count > 0)
        {
            if (SpadePlayerData.winnerPrizeData.Count.Equals(2))
            {
                //  Double x = SpadePlayerData.winnerPrizeData[0].amount;
                //  Double.TryParse(txtwinner.text, out x);
                //  txtwinner.text = x.ToString("0.00");
                txtwinner.text = LudoUtilityManager.Instance.DecimalFormat(SpadePlayerData.winnerPrizeData[0].amount);//.ToString();
                txtwinner.transform.parent.gameObject.SetActive(true);
                //  txtRunnerUp.text = x.ToString("0.00");
                txtRunnerUp.text = LudoUtilityManager.Instance.DecimalFormat(SpadePlayerData.winnerPrizeData[1].amount);//.ToString();
                txtRunnerUp.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                txtwinner.transform.parent.gameObject.SetActive(true);

                // Double x= SpadePlayerData.winnerPrizeData[0].amount;
                // Double.TryParse(txtwinner.text, out x);
                // txtwinner.text = x.ToString("0.00");
                txtwinner.text = LudoUtilityManager.Instance.DecimalFormat(SpadePlayerData.winnerPrizeData[0].amount);//.ToString();

            }
        }
    }

    public void ShowNotYourTurnPopup()
    {
        panelNotYourTurmPopup.SetActive(false);
        panelNotYourTurmPopup.SetActive(true);

        CancelInvoke("CloseNotYourTurnPopup");
        Invoke("CloseNotYourTurnPopup", 1f);
    }

    private void CloseNotYourTurnPopup()
    {
        panelNotYourTurmPopup.SetActive(false);
    }

    public LudoPlayer GetPlayerFromID(string id)
    {
        //StartCoroutine( DiceAnimation(2, SetDiceValue));
        return Array.Find(GamePlayers, x => x.PlayerId == id);
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

    #region COROUTINES
    public IEnumerator PlayerLeftObjHide(float timer, LudoPlayer plr)
    {
        yield return new WaitForSeconds(timer);

        //bool isOpen;

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
    public IEnumerator StartGameCall(float timer)
    {
        print("Sound => " + PlayerPrefs.GetInt("Sound"));
        Ludo_UIManager.instance.soundManager.InGameStartedOnce();
        yield return new WaitForSeconds(Ludo_UIManager.instance.soundManager.gameStart.clip.length + 0.1f);
    }
    private IEnumerator holdVibrateCall(EndOfJourneyData OnKeepCurrentPlayer)
    {
        yield return new WaitForSeconds(0.8f);
        LudoPlayer player = GetPlayerFromID(OnKeepCurrentPlayer.playerId);
        player.VibratePlayer();
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

    [SerializeField] private TurnPlayerData _turnTimerPlayerData;
    public TurnPlayerData TurnTimerPlayerData
    {
        set
        {
            _turnTimerPlayerData = value;
        }
        get
        {
            return _turnTimerPlayerData;
        }
    }
    #endregion
}