using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultScreen : MonoTemplate
{
    #region Public_Variables
    public GameObject onlineGameResult;
    public GameObject offlineGameResult;
    public GameObject loseScreen;
    public GameObject container;

    public Transform[] players;
    //  public List<Transform> players;
    #endregion

    #region Private_Variables
    #endregion

    #region  Unity_Callback
    private void OnEnable()
    {

    }
    #endregion

    #region Private_Methods
    private void OpenOnlineGameResult()
    {
        onlineGameResult.SetActive(true);
        offlineGameResult.SetActive(false);
        this.Open();
    }
    private void OpenLocalGameResult()
    {
        onlineGameResult.SetActive(false);
        offlineGameResult.SetActive(true);
        this.Open();
    }
    private void SetPlayerCount()
    {
        //Debug.Log($"Player Count: {GameStaticData.playerCount}");



        for (int i = 0; i < GamePlayController.instance.gamePlayers.Length; i++)
        {
            if (!GamePlayController.instance.completePlayer.Contains(GamePlayController.instance.gamePlayers[i]))
            {
                GamePlayController.instance.completePlayer.Add(GamePlayController.instance.gamePlayers[i]);
            }
        }
        for (int i = 0; i < players.Length; i++)
        {
            Transform t = players[i];
            t.gameObject.SetActive(false);
            if (i < GameStaticData.playerCount)
            {
                t.gameObject.SetActive(true);
                t.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{GamePlayController.instance.completePlayer[i].GetPlayerName}";
            }
        }
    }
    void RemainingPlayer()
    {
    }
    private void SetComputerPlayers()
    {
        players[0].GetChild(1).GetComponent<TextMeshProUGUI>().text = $"You";
        players[1].GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Computer";
        players[2].gameObject.SetActive(false);
        players[3].gameObject.SetActive(false);
    }
    #endregion

    #region Public_Methods
    public void CloseWinningScreen()
    {
        this.Close();
    }
    public void OpenWinningScreen()
    {
        container.SetActive(true);
        loseScreen.SetActive(false);
        switch (GameStaticData.gamesType)
        {
            case GamesType.Tournament:
            case GamesType.QuickPlay:
                OpenOnlineGameResult();
                break;

            case GamesType.LocalMultiplayer:
            case GamesType.Private:
                SetPlayerCount();
                OpenLocalGameResult();
                break;

            case GamesType.WithComputer:
                SetComputerPlayers();
                OpenLocalGameResult();
                break;
        }
    }
    public void OpenLosingScreen()
    {
        container.SetActive(false);
        loseScreen.SetActive(true);
        this.Open();
    }
    public void BackToMenuScreen()
    {

        GameStaticData.commingBackFromGame = true;
        this.Close();
        GetGameScreen.Close();
        if (Ludo_UIManager.instance.gamePlayScreen)
            Ludo_UIManager.instance.gamePlayScreen.Close();
        GetHomeScreen.Open();
        //GameStaticData.commingBackFromGame = true;
        //this.Close();
        //GetGameScreen.Close();
        //GetHomeScreen.Open();
    }
    #endregion

    #region Coroutine
    #endregion
}
