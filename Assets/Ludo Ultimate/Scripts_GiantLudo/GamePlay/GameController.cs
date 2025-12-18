using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoTemplate
{

    #region PublicVariables
    public static OnDebugModeStatusChanged OnDebugModeStart;

    public static GameController instance;
    public GamePlayController gamePlayController;
    public GameResultScreen resultScreen;
    public List<GameObject> winnerPlace;
    public List<Sprite> winnerPosition;
    public PartOfBoard[] boardParts;
    public Sprite defaultDice;
    public Animation gameStartAnim;

    #endregion

    #region PrivateVariables
    [SerializeField] private PlayerData[] idealPlayers;
    [SerializeField] private Sprite[] diceValueSprite;
    [SerializeField] private Sprite diceDefaultImage;

    [Space]
    [SerializeField] private PathManager pathManager;
    [SerializeField] private Transform pawnsNewParent;

    [Header("Avatars for VS Computer LudoGame")]
    [SerializeField] private AvatarAndGenderInfo avatarGenterInfo;

    private List<PlayerData> inGamePlayers;
    private MonoBehaviour controller;

    private int debugCounter = 0;
    #endregion

    #region UnityCallBacks
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (inGamePlayers == null)
        {
            inGamePlayers = new List<PlayerData>();
        }
    }
    private void OnEnable()
    {
        resultScreen.CloseWinningScreen();
        ResetWinner();
        CloseAllPawns();
        SetLocalData();
        debugCounter = 0;

        // stephen changes
        Ludo_UIManager.instance.soundManager.stopBgSound();
    }
    void OnDisable()
    {
        resultScreen.CloseWinningScreen();
        ResetGameData();
        Ludo_UIManager.instance.soundManager.InGameStartedStop();

        // stephen changes
        Ludo_UIManager.instance.soundManager.PlayBgSound();
    }
    #endregion


    #region Public_Methods
    public Sprite GetDiceValue(int steps)
    {
        return diceValueSprite[steps - 1];
    }
    public void OnTurnOnDebugMode()
    {
        debugCounter++;
        if (debugCounter >= 5)
        {
            OnDebugModeStart?.Invoke(true);
            debugCounter = 0;
        }
    }
    #endregion

    #region PrivateMethods
    private void ResetGameData()
    {
        OnDebugModeStart?.Invoke(false);
    }

    void ResetWinner()
    {
        foreach (GameObject gm in winnerPlace)
        {
            gm.SetActive(false);
        }
    }
    private void CloseAllPawns()
    {
        foreach (PartOfBoard item in boardParts)
        {
            item.OpenPawns(false, 0, null);
        }
    }

    private void SetLocalData()
    {
        switch (GameStaticData.gamesType)
        {
            case GamesType.LocalMultiplayer:
                SetGameDataForLocalMultiplayer();
                break;

            case GamesType.WithComputer:
                SetGameDataForLocalWithComputer();
                break;
        }
    }


    /* Index with coresponding position on board.
        |-1-2-|
        |-0-3-|
    */
    private void SetGameDataForLocalMultiplayer()
    {
        ResetIdaalPlayers();
        if (GameStaticData.playerCount == 2)
        {
            SetPlayersForTwoPlayer();
        }
        else
        {
            SetPlayerForMorePlayers();
        }
        gamePlayController.enabled = true;
        gamePlayController.SetupGame();
        // controller = gameObject.AddComponent<GamePlayController>();
    }

    private void SetPlayersForTwoPlayer()
    {
        PlayerData pd = idealPlayers[0];
        pd.Open();
        pd.PlayerName = GameStaticData.playersName[0];
        PartOfBoard pob = Array.Find(boardParts, x => x.ColorIndex == 0);
        pob.SetPartAtPosition(0);
        pob.SetNewParent(pawnsNewParent);
        pd.SetPawns = pob.GetPawns;
        pd.parthighlight = pob;
        pob.OpenPawns(true, 0, pd);
        inGamePlayers.Add(pd);

        pd = idealPlayers[2];
        pd.Open();
        pd.PlayerName = GameStaticData.playersName[1];
        pob = Array.Find(boardParts, x => x.ColorIndex == 1);
        pob.SetPartAtPosition(2);
        pob.SetNewParent(pawnsNewParent);
        pd.SetPawns = pob.GetPawns;
        pd.parthighlight = pob;
        pob.WinnerPosition.rectTransform.anchoredPosition = new Vector3(-95.8f, 117f, 0f);
        pob.OpenPawns(true, 0, pd);
        inGamePlayers.Add(pd);

        Array.Find(boardParts, x => x.ColorIndex == 2).SetPartAtPosition(1);
        Array.Find(boardParts, x => x.ColorIndex == 3).SetPartAtPosition(3);
    }

    private void SetPlayerForMorePlayers()
    {
        for (int i = 0; i < GameStaticData.playersName.Count; i++)
        {
            string playerName = GameStaticData.playersName[i];
            PlayerData pd = idealPlayers[i];
            pd.Open();
            pd.PlayerName = playerName;
            inGamePlayers.Add(pd);

            PartOfBoard pob = Array.Find(boardParts, x => x.ColorIndex == i);
            pob.SetPartAtPosition(i);
            pob.OpenPawns(true, 0, pd);
            pob.SetNewParent(pawnsNewParent);
            pd.SetPawns = pob.GetPawns;
            pd.parthighlight = pob;
        }
    }

    private void SetGameDataForLocalWithComputer()
    {

    }
    private void ResetIdaalPlayers()
    {
        foreach (PlayerData item in idealPlayers)
        {
            item.Close();
        }
        inGamePlayers.Clear();
    }
    #endregion

    #region GetterSetter
    public PlayerData[] GetPlayers => idealPlayers;
    public PlayerData[] GetGamePlayers => inGamePlayers.ToArray();
    public PathManager GetPathManager => pathManager;
    public Sprite DiceDefaultImage => diceDefaultImage;
    #endregion
}

public delegate void OnDebugModeStatusChanged(bool b);