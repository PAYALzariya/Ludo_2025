using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayController : MonoTemplate
{

    #region  PublicVariables
    public static GamePlayController instance;
    #endregion

    #region  PrivateVariables
    [SerializeField] private float turnCompleteTime = 30f;
    public PlayerData[] gamePlayers;
    public List<PlayerData> completePlayer;
    private int currentPlayherIndex = 0;
    private int currentDiceResult = 0;
    private int counterOf6 = 0;
    private bool gameComplete = false;
    #endregion

    #region  UnityCallback
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //SetupGame();
    }

    void OnEnable()
    {
        if (completePlayer == null)
        {
            completePlayer = new List<PlayerData>();
        }
        else
        {
            completePlayer.Clear();
        }

        TurnTimer.onTimerRunsOut += OnTimerRunsOut;
        DiceController.OnResult += OnDiceResult;
        PawnControl.OnPawnPathCompletes += OnPawnCompletsThePath;
        PlayerData.onPlayerGameComplete += OnPlayerCompleteGame;

    }

    void OnDisable()
    {
        TurnTimer.onTimerRunsOut -= OnTimerRunsOut;
        DiceController.OnResult -= OnDiceResult;
        PawnControl.OnPawnPathCompletes -= OnPawnCompletsThePath;
        PlayerData.onPlayerGameComplete -= OnPlayerCompleteGame;
        Ludo_UIManager.instance.soundManager.InGameStartedStop();
    }
    #endregion

    #region  PublicMethods
    public void SetupGame()
    {
        currentPlayherIndex = 0;
        gamePlayers = GetGameScreen.GetGamePlayers;
        gameComplete = false;
        foreach (PlayerData plr in gamePlayers)
        {
            plr.HighlightPlayer(false);
        }
        GameController.instance.gameStartAnim.gameObject.SetActive(true);
        GameController.instance.gameStartAnim.Play();

        StartCoroutine("StopGameStartAnim");
        //  StartGame();
    }

    public void TranspherTurnToNextPlayer()
    {
        counterOf6 = 0;

        CurrentPlayer.DisableAllPawns();
        currentPlayherIndex++;

        if (completePlayer != null)
        {
            if (completePlayer.Contains(CurrentPlayer))
            {
                TranspherTurnToNextPlayer();
            }
        }
        CurrentPlayer.die.GetComponent<Image>().sprite = Ludo_UIManager.instance.LocalGameScreen.defaultDice;

        StartTimerForCurrentPlayer();
    }

    #endregion

    #region  privateMethods
    private void OnTimerRunsOut()
    {
        TranspherTurnToNextPlayer();
    }
    private void OnDiceResult(int result)
    {
        //  CurrentPlayer.HighlightPawns(result, false);
        //Debug.Log("current player : " + currentPlayherIndex);
        if (result == 6)
        {
            counterOf6++;
        }
        else
        {
            counterOf6 = 0;
        }

        currentDiceResult = result;
        //CurrentPlayer.HighlightPawns(result, false);
        if (counterOf6 == 3)
        {
            CurrentPlayer.DisableAllPawns();
            CurrentPlayer.StopTimer();
            TranspherTurnToNextPlayer();
        }
        else
        {
            CurrentPlayer.parthighlight.highlightPartobj.Play("boardLineAnimation");
            CurrentPlayer.HighlightPawns(result, true);
        }
    }
    private void OnPawnCompletsThePath(PathElement pe, PawnControl pawn, bool isComplete)
    {
        Debug.Log("OnPawnCompletsThePath() ");
        if (gameComplete)
        {
            foreach (PlayerData item in gamePlayers)
            {
                item.DisableAllPawns();
                item.DisableDice();
            }
            return;
        }

        bool killed = false;
        if (pe.type != ElementType.immortalLand)
        {

            Debug.Log("-----------Killing-----------() ");
            Debug.LogError($"Value of Killing: {killed}");
            killed = pe.CheckForKill(pawn);
        }
        else
        {
            if (pe.stared)
            {
                Ludo_UIManager.instance.soundManager.InstarsafeAreaOnce();
            }
            /*else
            {

                Ludo_UIManager.instance.soundManager.FirstStepSound();
            } */
        }
        if (IsCurrentPlayerTurn || killed || isComplete)
        {
            Debug.Log("-----------Killing-----------() ");
            StartTimerForCurrentPlayer();
        }
        else
        {
            TranspherTurnToNextPlayer();
        }
    }

    private void OnPlayerCompleteGame(PlayerData data)
    {
        if (completePlayer == null)
        {
            completePlayer = new List<PlayerData>();
        }

        completePlayer.Add(data);
        data.parthighlight.WinnerPosition.sprite = GetGameScreen.winnerPosition[completePlayer.Count - 1];
        data.parthighlight.WinnerPosition.Open();
        if (completePlayer.Count >= gamePlayers.Length - 1)
        {
            gameComplete = true;
            Ludo_UIManager.instance.soundManager.WinningSoundOnce();
            GetGameScreen.resultScreen.OpenWinningScreen();
            foreach (PlayerData item in gamePlayers)
            {
                item.DisableAllPawns();
                item.DisableDice();
            }
        }
    }

    private void StartGame()
    {
        StartTimerForCurrentPlayer();
    }

    private void StartTimerForCurrentPlayer()
    {

        if (completePlayer != null)
        {
            if (completePlayer.Contains(CurrentPlayer))
            {
                TranspherTurnToNextPlayer();
            }
        }
        DisablDiceOfAllPlayers();
        Ludo_UIManager.instance.soundManager.OpponentTurnSoundOnce();

        CurrentPlayer.StartTimer(turnCompleteTime, false);
    }

    private void DisablDiceOfAllPlayers()
    {
        foreach (PlayerData item in gamePlayers)
        {
            item.DisableDice();
        }
    }
    #endregion

    #region Coroutines
    IEnumerator StopGameStartAnim()
    {
        yield return new WaitForSeconds(3.5f);
        GameController.instance.gameStartAnim.Stop();
        GameController.instance.gameStartAnim.gameObject.SetActive(false);
        StartGame();
    }
    #endregion
    #region  GetterSetter
    public PlayerData CurrentPlayer
    {
        get
        {
            if (currentPlayherIndex >= gamePlayers.Length)
            {
                currentPlayherIndex = 0;
            }

            return gamePlayers[currentPlayherIndex];
        }
    }

    private bool IsCurrentPlayerTurn
    {
        get
        {
            return currentDiceResult == 6;
        }
    }
    #endregion
}
