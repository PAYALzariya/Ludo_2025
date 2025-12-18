using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LudoPlayer : MonoTemplate
{

    #region PublicVariables
    public TextMeshProUGUI scoreTxt;
    public Animation starAnimation;
    //public static OnPLayerComplete onPlayerGameComplete;
    public PathElement[] path;
    public PathElement[] path_miniBoard;
    public PartOfBoard parthighlight;
    public Animation ProfileHighlightAnim;
    public Image WinnerRankPosition;
    public Image PlayerLeftImg;
    [Header("String")]
    public string PlayerId;
    public PlayerInfoItem playerInfo;
    public myTimer TimerBlock;
    public skipStepScreen skipStepPopup;
    public Button IButton;
    public Button Profile;
    public CanvasGroup chatCanvasGroup;
    public GameObject chatBubble;
    #endregion

    #region PrivateVariable
    [SerializeField] public Text txtChat;
    [SerializeField] public TextMeshProUGUI txtPlayerName;
    [SerializeField] public TextMeshProUGUI txtPlayerphnenumber;
    [SerializeField] public Image playerProfilePicture;
    [SerializeField] public Sprite defaultUserSprite;
    [SerializeField] public Image turnTimer;
    //[SerializeField] public TurnTimer turnTimer;
    [SerializeField] public GameObject arrow;

    [Space]
    [SerializeField] public PawnControl[] pawns;
    public Transform[] pawnsInitPos;
    [SerializeField] public DiceController die;

    [Space]
    [SerializeField] public PathType gamePathType = PathType.bottomLeft;

    //private int maxSteps = 6;
    private int steps = 0;
    //private float timeToComplete = 0;
    //private int pawnCompleted = 0;

    private const float timeForOneStep = 0.15f;
    //private const float timeForOneStep = 0.15f;
    public float maxTimer;
    public float timer;
    public Color whiteHighlightColor;
    public Color redHighlightColor;

    private int previousDataTokenId = 0;
    private int previousDataCurrentDistance = 0;
    private int previousDataStepsToMove = 0;

    private PlayerProfileInfo playerProfileInfo = null;
    #endregion

    #region unityCallback
    void OnEnable()
    {
        //chatCanvasGroup.alpha = 0;
        die.GetComponent<Image>().sprite = GetGameScreen.DiceDefaultImage;
        arrow.SetActive(false);
        skipStepPopup.closeButtonTap();
        turnTimer.fillAmount = 0;
        turnTimer.Close();
        //pawnCompleted = 0;
        WinnerRankPosition.Close();
        PlayerLeftImg.Close();
        PathManager.onPathCompleteSetup += OnPathSetUpComplete;
        PathManager.onPathCompleteSetup += OnPathSetUpComplete_MiniBoard;
        playerProfileInfo = null;
    }

    void OnDisable()
    {
        //chatCanvasGroup.alpha = 0;
        StopCoroutine(DisplayChatMessageCoroutine());
        turnTimer.fillAmount = 0;
        turnTimer.Close();
        PathManager.onPathCompleteSetup -= OnPathSetUpComplete;
        PathManager.onPathCompleteSetup -= OnPathSetUpComplete_MiniBoard;

        CancelInvoke("CloseChatBubble");
        CloseChatBubble();
    }
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            turnTimer.fillAmount = 1f - (timer / maxTimer);
            if (timer < maxTimer / 3)
            {
                turnTimer.color = redHighlightColor;
            }
            else
                turnTimer.color = whiteHighlightColor;
        }

    }
    #endregion

    #region PublicMethods
    public void openProfileButtonTap()
    {
        if (this.playerProfileInfo != null)
        {
            Ludo_UIManager.instance.gamePlayScreen.playerDetailspanel.OpenPanel(this.playerInfo.id, this.playerProfileInfo, playerProfilePicture.sprite);
            return;
        }

        //Ludo_UIManager.instance.gamePlayScreen.playerDetailspanel.setDataAndOpen(this.playerInfo.id, this.playerInfo.avatar, this.playerInfo.profilePic);//anderson
        // Ludo_UIManager.instance.OpenLoader(true);
        Ludo_UIManager.instance.socketManager.playerProfileInfo(this.playerInfo.id, Ludo_UIManager.instance.gamePlayScreen.currentRoomData.boardId, (socket, packet, args) =>
        {
            Debug.Log(Ludo_Constants.LudoEvents.PlayerProfileInfo + " respnose  : " + packet.ToString());
            // Stephen Disable Loader on 09/06/2022, client's feedback
            // Ludo_UIManager.instance.OpenLoader(false);
            PokerEventResponse<PlayerProfileInfo> playerProfileInfo = JsonUtility.FromJson<PokerEventResponse<PlayerProfileInfo>>(LudoUtilityManager.Instance.GetPacketString(packet));

            if (playerProfileInfo.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                this.playerProfileInfo = playerProfileInfo.result;
                Ludo_UIManager.instance.gamePlayScreen.playerDetailspanel.OpenPanel(this.playerInfo.id, playerProfileInfo.result, playerProfilePicture.sprite);
            }
            else
                Ludo_UIManager.instance.messagePanel.DisplayMessage(playerProfileInfo.message);
        });
    }
    public void showChatBubble(string playerId, string message)
    {
        if (playerId.Equals(this.PlayerId))
        {
            if (message != null)
            {
                //string message = chatMessageReceivedResponse.message;
                /*if (message.Length > 75)
                {
                    message = message.Substring(0, 75);// + "...";
                }*/

                txtChat.text = message;
                //chatBubble.GetComponent<VerticalLayoutGroup>().enabled = false;
                //Canvas.ForceUpdateCanvases();
                //chatBubble.GetComponent<VerticalLayoutGroup>().enabled = true;

                if (gameObject.activeSelf)
                {
                    CancelInvoke("CloseChatBubble");
                    CloseChatBubble();
                    //StopCoroutine(DisplayChatMessageCoroutine());
                }
                if (gameObject.activeSelf)
                {
                    Ludo_UIManager.instance.soundManager.BubbleClickSound();
                    chatCanvasGroup.gameObject.SetActive(true);
                   // Invoke("CloseChatBubble", Ludo_Constants.LudoConstants.CHAT_BUBBLE_DISPLAY_TIMER);
                    //StartCoroutine(DisplayChatMessageCoroutine());
                }
            }
        }
    }

    private void CloseChatBubble()
    {
        chatCanvasGroup.gameObject.SetActive(false);
    }

    public void VibratePlayer()
    {
        try
        {
            Ludo_UIManager.instance.soundManager.Vibrate();
        }
        catch
        {
            print("vibrate error");
        }
    }
    public void checkIbuttonTap()
    {
        foreach (LudoPlayer plr in Ludo_UIManager.instance.gamePlayScreen.GamePlayers)
        {
            if (plr.gameObject.activeInHierarchy)
            {
                plr.skipStepPopup.closeButtonTap();
            }
        }

        skipStepPopup.SetData(PlayerId, Ludo_UIManager.instance.gamePlayScreen.currentRoomData.boardId);
    }
    public void OpenPawns(bool flag)
    {
        // Debug.Log($"Opening the pawns: <color=red>{flag}</color>");
        for (int i = 0; i < pawns.Length; i++)
        {
            PawnControl item = pawns[i];
            item.SetData(pawnsInitPos[i], flag);
            // Debug.LogError($"{this.name} = LUDO = {pawnsInitPos[i].position} <== LocalPos ==> {pawnsInitPos[i].GetComponent<RectTransform>().anchoredPosition3D}");
        }
    }

    public void WinnerObjDisplay(Sprite WinnerObj, string playerId)
    {
        //parthighlight.WinnerPosition.sprite = WinnerObj;
        //parthighlight.WinnerPosition.Open();
        int index = Ludo_UIManager.instance.gamePlayScreen.GetPlayerIndexByPlayerId(playerId);

        if (index != -1)
        {
            if (Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].parthighlight.Position == 0)
            {
                Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].WinnerRankPosition.rectTransform.anchoredPosition = new Vector2(7, 429);
            }
            else if (Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].parthighlight.Position == 1)
            {
                Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].WinnerRankPosition.rectTransform.anchoredPosition = new Vector2(7, -420);
            }
            else if (Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].parthighlight.Position == 2)
            {
                Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].WinnerRankPosition.rectTransform.anchoredPosition = new Vector2(2, -420);
            }
            else if (Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].parthighlight.Position == 3)
            {
                Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].WinnerRankPosition.rectTransform.anchoredPosition = new Vector2(2, 429);
            }
        }
        WinnerRankPosition.sprite = WinnerObj;
        WinnerRankPosition.Open();

    }
    public void PlayerLeftObjDisplay(Sprite WinSet, string playerId)
    {
        int index = Ludo_UIManager.instance.gamePlayScreen.GetPlayerIndexByPlayerId(playerId);

        if (index != -1)
        {
            if (Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].parthighlight.Position == 2)
            {
                Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].parthighlight.PlayerLeftImage.transform.localRotation = new Quaternion(0, 0, 0.7071068f, 0.7071068f);
            }
            else if (Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].parthighlight.Position == 1)
            {
                Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].parthighlight.PlayerLeftImage.transform.localRotation = new Quaternion(0, 0, 0, 0);

            }
            else if (Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].parthighlight.Position == 3)
            {
                Ludo_UIManager.instance.gamePlayScreen.GamePlayers[index].parthighlight.PlayerLeftImage.transform.localRotation = new Quaternion(0, 0, 1, 0);

            }
        }
        PlayerLeftImg.Open();
    }

    public void KillpawnBytokenId(KilledPlayer KilledPlayerData)
    {
        PawnControl pawn = GetPawnFromId(KilledPlayerData.tokenId);
        pawn.transform.localScale = Vector3.one;
        pawn.RemovePawnFromPathElement();
        //Debug.Log("Pawn pathIndex => " + KilledPlayerData.currentDistance);
        reversePath(KilledPlayerData.currentDistance).DebugDisplay();
        pawn.Kill(reversePath(KilledPlayerData.currentDistance));
    }

    public void KillpawnBytokenId_MiniBoard(KilledPlayer KilledPlayerData)
    {
        PawnControl pawn = GetPawnFromId(KilledPlayerData.tokenId);
        pawn.transform.localScale = Vector3.one;
        pawn.RemovePawnFromPathElement();
        // Debug.LogError("Pawn pathIndex => " + KilledPlayerData.currentDistance);
        reversePath_MiniBoard(KilledPlayerData.currentDistance).DebugDisplay();
        pawn.Kill(reversePath_MiniBoard(KilledPlayerData.currentDistance));
    }

    public void MovePawnFromPlayerAction(PlayerActionDetails details)
    {
        Debug.Log("MovePawnFromPlayerAction .. details");
        if (IsPreviousAndCurrentTokenTurnDataMatched(details.tokenId, details.currentDistance, details.stepsToMove) == true)
            return;

        previousDataTokenId = details.tokenId;
        previousDataCurrentDistance = details.currentDistance;
        previousDataStepsToMove = details.stepsToMove;

        DisableAllPawns();

        if (path == null || path.Length <= 0)
            OnPathSetUpComplete();

        if (path_miniBoard == null || path_miniBoard.Length <= 0)
            OnPathSetUpComplete_MiniBoard();


        PawnControl pawn = GetPawnFromId(details.tokenId);
        steps = details.stepsToMove;
        PathElement[] movePath = GetPathElements(details.currentDistance);

        if (details.currentDistance == 0)
        {
            SendPawnToFirstStep(pawn, path[0], timeForOneStep);
        }
        else
        {
            SendPawnOnPath(movePath, pawn, timeForOneStep);
        }

        DisableDice(false);
        StopTimer();
        //StartCoroutine(stopHighlightTokens());        
    }

    public void MovePawnFromPlayerAction_MiniBoard(PlayerActionDetails details)
    {
        // Debug.LogError("MovePawnFromPlayerAction_MiniBoard .. ");
        if (IsPreviousAndCurrentTokenTurnDataMatched(details.tokenId, details.currentDistance, details.stepsToMove) == true)
            return;

        previousDataTokenId = details.tokenId;
        previousDataCurrentDistance = details.currentDistance;
        previousDataStepsToMove = details.stepsToMove;

        DisableAllPawns();

        if (path_miniBoard == null || path_miniBoard.Length <= 0)
            OnPathSetUpComplete_MiniBoard();


        PawnControl pawn = GetPawnFromId(details.tokenId);
        steps = details.stepsToMove;
        PathElement[] movePath = GetPathElements_miniBoard(details.currentDistance);

        if (details.currentDistance == 0)
        {
            SendPawnToFirstStep_MiniBoard(pawn, path_miniBoard[0], timeForOneStep);
        }
        else
        {
            SendPawnOnPath_miniBoard(movePath, pawn, timeForOneStep);
        }

        // DisableDice(false);
        // StopTimer();
        //StartCoroutine(stopHighlightTokens());        
    }


    public void MovePawnFromPlayerAction(int tokenId, int currentDistance, int stepsToMove)
    {
        Debug.LogError("MovePawnFromPlayerAction .. StepsTomove");
        if (IsPreviousAndCurrentTokenTurnDataMatched(tokenId, currentDistance, stepsToMove) == true)
            return;

        previousDataTokenId = tokenId;
        previousDataCurrentDistance = currentDistance;
        previousDataStepsToMove = stepsToMove;

        DisableAllPawns();

        if (path == null || path.Length <= 0)
            OnPathSetUpComplete();

        PawnControl pawn = GetPawnFromId(tokenId);
        steps = stepsToMove;
        PathElement[] movePath = GetPathElements(currentDistance);

        if (currentDistance == 0)
        {
            SendPawnToFirstStep(pawn, path[0], timeForOneStep);
        }
        else
        {
            SendPawnOnPath(movePath, pawn, timeForOneStep);
        }

        DisableDice(false);
        StopTimer();
    }

    IEnumerator stopHighlightTokens()
    {
        yield return new WaitForSeconds(0.8f);
        HighlightPlayer(false);
    }
    public void ResetPlayerForTurn(bool Timer, bool diceStatus)
    {
     //   Debug.LogError("<<<<<<<<<<ResetPlayerForTurn>>>>>>>>>>>>>>>>>>");
        DisableDice(diceStatus);
        turnTimer.gameObject.SetActive(Timer);
        //SetAllPawnsOnTop();
    }
    public void SetTimerValue(float currentTIme, float totalTime)
    {
        this.timer = currentTIme;
        this.maxTimer = totalTime;
        // Debug.LogError($"timer .. {this.timer}");

        if (currentTIme <= totalTime / 6 && ServerSocketManager.instance.playerId.Equals(playerInfo.id))
        {
            if (Ludo_UIManager.instance.gamePlayScreen.AllLudoPlayerInfo.winnerPrizeData != null &&
                Ludo_UIManager.instance.gamePlayScreen.AllLudoPlayerInfo.winnerPrizeData.Count > 0
                && Ludo_UIManager.instance.gamePlayScreen.AllLudoPlayerInfo.winnerPrizeData[0].amount > 0)
            {
                Ludo_UIManager.instance.soundManager.AttentionSoundOnce();
            }
        }
        //turnTimer.Open();
        //turnTimer.StartTimerFromPoint(currentTIme, totalTime);
    }

    public void CLearPath()
    {
        foreach (PathElement item in path)
        {
            if (item.pawns != null)
            {
                item.pawns.Clear();
            }
        }
    }
    public void SetPlayerData()
    {
        path = GetGameScreen.GetPathManager.GetPathForPawns(gamePathType);
    }

    public void AddPawnInHome()
    {
        Ludo_UIManager.instance.soundManager.InHomeSoundOnce();
        StartStarAnimation();
        StopStartAnimationAfterComplete();
    }

    public void DisableDice(bool b = false)
    {
        if (PlayerId == ServerSocketManager.instance.playerId)
        {
            die.GetComponent<Button>().interactable = b;
            if (die.btnDiceRollPrevention)
                die.btnDiceRollPrevention.gameObject.SetActive(!b);
        }

        arrow.SetActive(b);
      //  Debug.Log("bbbbbbbbbbbbbbbbbbbbbbb : " + b);

        HighlightPlayer(b);
        if (b)
        {
            die.SetDiceValueToDefault();
            DisableAllPawns();
        }
        //richa main  parthighlight.HighlightBoard(b);
    }
    public void HighlightPlayer(bool open)
    {
        if (open)
        {
            ProfileHighlightAnim.Play();
            ProfileHighlightAnim.gameObject.SetActive(true);
        }
        else
        {
            ProfileHighlightAnim.Stop();
            ProfileHighlightAnim.gameObject.SetActive(false);
        }
    }

    public void SetDiceData(RollDiceDetails diceDetails)
    {
        steps = diceDetails.diceValue;
        die.diceValue = diceDetails.diceValue;
        die.SetDiceResult(steps);
        /* if (diceDetails.playerId.Equals(Ludo_UIManager.instance.assetOfGame.SavedLoginData.playerId))
         {
             die.SetDiceResult(steps);
         }
         else
         {

             if (diceDetails.isRoll)
             {
                 Debug.Log("diceDetails.isRool => " + diceDetails.isRoll);
                 die.StopRollingAndSetValue(steps, diceDetails.playerId);
             }
             else
             {
                 Debug.Log("diceDetails.isRool => " + diceDetails.isRoll);
                 die.SetDiceResult(steps);
             }
         }*/
        PawnData[] data = diceDetails.readyToMove;
        foreach (PawnData pd in data)
        {
            PawnControl pc = Array.Find(pawns, x => x.Index == pd.id);
            pc.HighlightPawn(true, steps);
        }
        parthighlight.HighlightBoard(true);
        parthighlight.highlightPartobj.Play("boardLineAnimation");
        //StartCoroutine(resetHighlightPart(data, 0.8f));
    }
    IEnumerator resetHighlightPart(PawnData[] d, float t)
    {
        yield return new WaitForSeconds(t);
        PawnData[] data = d;
        foreach (PawnData pd in data)
        {
            PawnControl pc = Array.Find(pawns, x => x.Index == pd.id);
            pc.HighlightPawn(true, steps);
        }
        parthighlight.HighlightBoard(true);
        parthighlight.highlightPartobj.Play("boardLineAnimation");
    }
    public void DisableAllPawns()
    {
        foreach (PawnControl item in pawns)
        {
            item.DeactivatePawn();
            item.HighlightPawn(false, 0);
        }
    }
    public void StopTimer()
    {
        turnTimer.fillAmount = 0;
        turnTimer.Close();
        arrow.SetActive(false);
        HighlightPlayer(false);
        //parthighlight.HighlightBoard(false);
        parthighlight.highlightPartobj.Play("Idle");
    }
    public PathElement[] reversePath(int currentPos)
    {
        PathElement[] pathel = path.SubsequenceReverse(currentPos);
        return pathel;
    }

    public PathElement[] reversePath_MiniBoard(int currentPos)
    {
        PathElement[] pathel = path_miniBoard.SubsequenceReverse(currentPos);
        return pathel;
    }

    public PathElement[] GetPathElements(int currentPos)
    {
        PathElement[] elements = path.Subsequence(currentPos, steps);
        return elements;
    }

    public PathElement[] GetPathElements_miniBoard(int currentPos)
    {
        PathElement[] elements = path_miniBoard.Subsequence(currentPos, steps);
        return elements;
    }

    public void RollDice()
    {
        if (PlayerId != ServerSocketManager.instance.playerId)
            return;

        CallRollDice();
        DisableDice(false);
    }

    public void RollDicePreventionMessage()
    {
        if (PlayerId != ServerSocketManager.instance.playerId)
            return;

        bool myTurn = false;
        foreach (PawnControl pawn in pawns)
        {
            if (pawn.IsHighlighted())
            {
                myTurn = true;
                break;
            }
        }

        if (myTurn)
        {

        }
        else
        {
            Ludo_UIManager.instance.gamePlayScreen.ShowNotYourTurnPopup();
            Debug.Log("It's not your turn!");
        }
    }

    public void SetPawnsData(TokensItem[] items)
    {
        if (path == null || path.Length <= 0)
            OnPathSetUpComplete();

        for (int i = 0; i < pawns.Length; i++)
        {
            PawnControl pawn = pawns[i];
            TokensItem TI = items[i];
            int id = items[i].id;
            pawn.SetIndex(id);

            pawn.myPlayerID = PlayerId;
            Button b = pawn.GetComponent<Button>();

            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => OnPawnClick(id));
            if (TI.distance > 0)
            {
                pawn.RemovePawnFromPathElement();
                pawn.transform.localScale = Vector3.one;
                PathElement PawnGetPath = path[TI.distance - 1];
                pawn.transform.position = PawnGetPath.transform.position;
                pawn.AddPawnToPathElement(PawnGetPath, false);
                pawn.parent = Ludo_UIManager.instance.gamePlayScreen.pawnsContainer;
            }
            else
            {
                pawn.transform.SetParent(this.parthighlight.pawnsParent);
                pawn.ResetPawn(this.parthighlight.oldPOsitions[i]);
                pawn.parent = Ludo_UIManager.instance.gamePlayScreen.pawnsContainer;
                pawn.transform.SetParent(Ludo_UIManager.instance.gamePlayScreen.pawnsContainer);
            }
        }
    }

    public void SetPawnsData_MiniBoard(TokensItem[] items)
    {
        if (path_miniBoard == null || path_miniBoard.Length <= 0)
            OnPathSetUpComplete_MiniBoard();

        for (int i = 0; i < pawns.Length; i++)
        {
            PawnControl pawn = pawns[i];
            TokensItem TI = items[i];
            int id = items[i].id;
            pawn.SetIndex(id);
            Button b = pawn.GetComponent<Button>();

            // b.onClick.RemoveAllListeners();
            // b.onClick.AddListener(() => OnPawnClick(id));
            if (TI.distance > 0)
            {
                pawn.RemovePawnFromPathElement();
                pawn.transform.localScale = Vector3.one;
                PathElement PawnGetPath = path_miniBoard[TI.distance - 1];
                pawn.transform.position = PawnGetPath.transform.position;
                pawn.AddPawnToPathElement(PawnGetPath, false);
                pawn.parent = Ludo_UIManager.instance.miniBoardGamePlayScreen.pawnsContainer;
            }

            else
            {
                pawn.transform.SetParent(this.parthighlight.pawnsParent);
                pawn.ResetPawn(this.parthighlight.oldPOsitions[i]);
                pawn.parent = Ludo_UIManager.instance.miniBoardGamePlayScreen.pawnsContainer;
                pawn.transform.SetParent(Ludo_UIManager.instance.miniBoardGamePlayScreen.pawnsContainer);
            }
        }
    }

    public void PawnToHome(int i)
    {
        PawnControl pawn = GetPawnFromId(this.playerInfo.tokens[i].id);
        pawn.transform.localScale = Vector3.one;
        pawn.RemovePawnFromPathElement();
        reversePath(this.playerInfo.tokens[i].distance).DebugDisplay();
        pawn.Kill(reversePath(this.playerInfo.tokens[i].currentPos));
    }
    #endregion

    #region PrivateMethods
    private bool IsPreviousAndCurrentTokenTurnDataMatched(int tokenId, int currentDistance, int StepsToMove)
    {
        if (previousDataTokenId == tokenId && previousDataCurrentDistance == currentDistance && previousDataStepsToMove == StepsToMove)
            return true;
        else
            return false;
    }

    private void ClearPreviousTokenData()
    {
        previousDataTokenId = 0;
        previousDataCurrentDistance = 0;
        previousDataStepsToMove = 0;
    }

    private void SendPawnToFirstStep(PawnControl pawn, PathElement pathElement, float time)
    {
        // Debug.LogError("SendPawnToFirstStep..");
        StartCoroutine(Cust_Utility.MoveFromPositionToPositionXY(true, time, pawn.transform, pathElement.transform, () =>
         {
             pawn.AddPawnToPathElement(pathElement, true);
             if (PlayerId.Equals(ServerSocketManager.instance.playerId))
             {
                 VibratePlayer();
             }
             Ludo_UIManager.instance.soundManager.TokenMoveSoundOnce();
         }));
    }

    private void SendPawnToFirstStep_MiniBoard(PawnControl pawn, PathElement pathElement, float time)
    {
        StartCoroutine(Cust_Utility.MoveFromPositionToPositionXY(true, time, pawn.transform, pathElement.transform, () =>
        {
            pawn.AddPawnToPathElement(pathElement, true);
        }));
    }

    private void SendPawnOnPath(PathElement[] path, PawnControl pawn, float time)
    {
        if (pawn.lastPathElement != null)
        {
            pawn.RemovePawnFromPathElement();
        }
        PathElement lastPE = path[path.Length - 1];
        StartCoroutine(Cust_Utility.MoveToPath(pawn.transform, path, time, () =>
        {
            if (PlayerId.Equals(ServerSocketManager.instance.playerId))
            {
                VibratePlayer();
            }
            Ludo_UIManager.instance.soundManager.TokenMoveSoundOnce();
        }, () =>
        {
            pawn.AddPawnToPathElement(lastPE, true);
        }));
    }

    private void SendPawnOnPath_miniBoard(PathElement[] path, PawnControl pawn, float time)
    {
        if (pawn.lastPathElement != null)
        {
            pawn.RemovePawnFromPathElement();
        }
        PathElement lastPE = path[path.Length - 1];
        StartCoroutine(Cust_Utility.MoveToPath(pawn.transform, path, time, () =>
        {
            if (PlayerId.Equals(ServerSocketManager.instance.playerId))
            {
                VibratePlayer();
            }
            Ludo_UIManager.instance.soundManager.TokenMoveSoundOnce();
        }, () =>
        {
            pawn.AddPawnToPathElement(lastPE, true);
        }));
    }

    private PawnControl GetPawnFromId(int id)
    {
        return Array.Find(pawns, x => x.Index == id);
    }

    private void OnPawnClick(int id)
    {
        if (playerInfo.id != ServerSocketManager.instance.playerId)
        {
            Ludo_UIManager.instance.gamePlayScreen.ShowNotYourTurnPopup();
            return;
        }

        CallPlayerAction(id);
    }
    private void SetAllPawnsOnTop()
    {
        foreach (PawnControl item in pawns)
        {
            item.transform.SetAsLastSibling();
        }
    }
    public void callingRollDice(string diceKey)
    {
        die.StartRollingCoroutine(diceKey);
    }

    public void StopRollingDice(RollDiceDetails diceDetails)
    {
        StartCoroutine(StopRollingDiceIenum(diceDetails));
        //die.StopRollingCoroutine();
    }

    public bool IsTokenAvailable(int tokenId)
    {
        foreach (TokensItem token in playerInfo.tokens)
        {
            if (token.id == tokenId)
                return true;
        }

        return false;
    }

    IEnumerator StopRollingDiceIenum(RollDiceDetails diceDetails)
    {
        yield return new WaitForSeconds(0.4f);
        die.StopRollingCoroutine();
        SetDiceData(diceDetails);
    }

    DateTime dateTimeLastRollDiceRequest = DateTime.Now.AddHours(-1);
    private void CallRollDice()
    {
        if ((DateTime.Now - dateTimeLastRollDiceRequest).TotalSeconds < 1)
            return;
        else
            dateTimeLastRollDiceRequest = DateTime.Now;

        string diceKey = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        //die.StartRollingCoroutine();
        die.StartRollingCoroutine(diceKey); //anderson
        GetSocketManager.RollDice(BoardId, Ludo_UIManager.instance.gamePlayScreen.txtCheating.text, diceKey, ResponseOfRollDice);

    }

    private void ResponseOfRollDice(Socket socket, Packet packet, object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.RollDice + " respnose  : " + packet.ToString());
    }

    //DateTime dateTimeLastPawnStepRequest = DateTime.Now.AddHours(-1);
    private void CallPlayerAction(int id)
    {
        //  if ((DateTime.Now - dateTimeLastPawnStepRequest).TotalSeconds < 1)
        //    return;
        //  else
        //    dateTimeLastPawnStepRequest = DateTime.Now;

        TurnPlayerData turnTimerPlayerData = Ludo_UIManager.instance.gamePlayScreen.TurnTimerPlayerData;
        int index = 0;
        for (int i = 0; i < turnTimerPlayerData.tokens.Count; i++)
        {
            if (turnTimerPlayerData.tokens[i].id == id)
            {
                index = i;
                break;
            }
        }
        GetSocketManager.PlayerAction(ServerSocketManager.instance.playerId, id, ResponseOfPlayerAction);

        //try
        //{
        //    if (LudoGame.Lobby.socketManager.Socket.IsOpen)
        //        LudoPhotonChatManager.instance.SendPhotonMessagePawnStep(id, turnTimerPlayerData.tokens[index].distance, turnTimerPlayerData.diceValue);
        //}
        //catch(Exception e)
        //{
        //    Debug.Log("Exception in CallPlayerAction " + e.Source);
        //}
    }

    private void ResponseOfPlayerAction(Socket socket, Packet packet, object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.PlayerAction + " respnose  : " + packet.ToString());
    }

    private void OnPathSetUpComplete()
    {
        path = Ludo_UIManager.instance.gamePlayScreen.pathManager.GetPathForPawns(gamePathType);
    }

    private void OnPathSetUpComplete_MiniBoard()
    {
        path_miniBoard = Ludo_UIManager.instance.miniBoardGamePlayScreen.pathManager.GetPathForPawns_miniboard(gamePathType);
    }

    private void StartStarAnimation()
    {
        starAnimation.gameObject.SetActive(true);
        starAnimation.Play();
    }

    private void StopStartAnimationAfterComplete()
    {
        StartCoroutine(StopingStarAnimation());
    }
    #endregion


    #region Coroutine
    private IEnumerator DisplayChatMessageCoroutine()
    {
        //yield return new WaitForSeconds(Ludo_Constants.LudoConstants.CHAT_BUBBLE_DISPLAY_TIMER);
        yield return new WaitForSeconds(2);
        chatCanvasGroup.gameObject.SetActive(false);

        ////StartCoroutine(ForceUpdateCanvasCoroutine());
        //float i = 0;

        //float fromAlpha = chatCanvasGroup.alpha;
        //float toAlpha = 1;

        //while (i < 1)
        //{
        //    i += Time.deltaTime * LudoConstants.LudoConstants.CHAT_BUBBLE_DISPLAY_TIMER;
        //    chatCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, i);

        //    yield return 0;
        //}

        //fromAlpha = 1;
        //toAlpha = 0;

        //yield return new WaitForSeconds(LudoConstants.LudoConstants.CHAT_BUBBLE_DISPLAY_TIMER);

        //i = 0;
        //while (i < 1)
        //{
        //    i += Time.deltaTime * LudoConstants.LudoConstants.CHAT_BUBBLE_DISPLAY_TIMER;

        //    chatCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, i);
        //    yield return 0;
        //}
    }
    private IEnumerator StopingStarAnimation()
    {
        yield return new WaitForSeconds(1.2f);
        starAnimation.gameObject.SetActive(false);
    }
    #endregion

    #region GetterSetter
    //public TurnTimer TurnTimer => turnTimer;
    public PathElement FirstStepPath => path[0];
    public int PathLength => path.Length;

    public PathType PathType => gamePathType;
    public PawnControl[] SetPawns
    {
        set
        {
            if (value.Length <= 0)
            {
                return;
            }

            pawns = value;

            foreach (PawnControl item in pawns)
            {
            }
        }
    }
    public string PlayerName
    {
        set
        {
            txtPlayerName.text = value;
        }
    }

    public string Playerphone
    {
        set
        {
            txtPlayerphnenumber.text = value;
        }
    }
    public string BoardId
    {
        get
        {
            return Ludo_Constants.Ludo.boardId;
        }
    }
    #endregion
}
//public delegate void OnPLayerComplete(ludoPlayer data);