using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BestHTTP.SocketIO;

public class PlayWithComputer : MonoTemplate
{
    #region Public_Variables

    #endregion

    #region Private_Variables
    [SerializeField] private Toggle twoPlayerToggle;
    [SerializeField] private Toggle fourPlayerToggle;
    [SerializeField] private Toggle yellowToggle;
    [SerializeField] private Toggle blueToggle;
    [SerializeField] private Toggle redToggle;
    [SerializeField] private Toggle greenToggle;
    private int playerCount = 2;
    public int colorIndex = 0;
    Button button;
    #endregion

    #region  Unity_Callback
    private void OnEnable()
    {
        yellowToggle.isOn = false;
        blueToggle.isOn = false;
        redToggle.isOn = true;
        greenToggle.isOn = false;
        twoPlayerToggle.isOn = true;
        fourPlayerToggle.isOn = false;
        SetColor(2);
        SetPlayerCOunt(2);
       // Game.Lobby.SetComputerSocketNamespace = "computer";
    }
    private void OnDisable()
    {
        colorIndex = -1;
    }
    #endregion

    #region Private_Methods
    private void GetplayGameResponse(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.JoinRoom + " cc response  :" + packet.ToString());

        JSONArray arr = new JSONArray(packet.ToString());
        string Source;
        Source = arr.getString(arr.length() - 1);
        var resp = Source;
        PokerEventResponse<GameStartDataResp> GetJoinRoomResponse = JsonUtility.FromJson<PokerEventResponse<GameStartDataResp>>(resp);
        Ludo_UIManager.instance.OpenLoader(false);
        if (GetJoinRoomResponse.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
        {
            // Debug.LogError("check..");
            Ludo_UIManager.instance.OpenLoader(false);
            Debug.Log("PJ");
            //pj    Ludo_UIManager.instance.gamePlayScreen.SetRoomDataAndPlay(GetJoinRoomResponse.result);

            // stephen
            // Ludo_UIManager.instance.miniBoardGamePlayScreen.SetRoomDataAndPlay(GetJoinRoomResponse.result);

            this.Close();
            Ludo_UIManager.instance.playOnline.Close();
        }
        else
        {
            Ludo_UIManager.instance.messagePanel.DisplayMessage(GetJoinRoomResponse.message);
        }
    }
    public void SetPlayerCOunt(int i)
    {
        playerCount = i;
        //GameStaticData.playerCount = i;
    }

    public void SetColor(int colorId)
    {
        colorIndex = colorId;
        //GameStaticData.vsComputerColorIndex = colorId;
    }
    #endregion

    #region Public_Methods
    public void CloseButtonTap()
    {
        this.Close();
        Ludo_UIManager.instance.homeScreen.Open();
    }
    public void StartGame()
    {
        if (!colorIndex.Equals(-1))
        {
            Ludo_UIManager.instance.OpenLoader(true);
            Ludo_UIManager.instance.socketManager.JoinComputerRoom(playerCount.ToString(), Cust_Utility.GetColorName(colorIndex), GetplayGameResponse);
        }
        else
        {
            Ludo_UIManager.instance.messagePanel.DisplayMessage("Please select color");
        }
    }
    public void OpenGameScene()
    {
        this.Close();
        Ludo_UIManager.instance.LocalGameScreen.Open();
        GameStaticData.gamesType = GamesType.WithComputer;
        GameStaticData.playerCount = playerCount;
    }
    #endregion

    #region Coroutine
    #endregion
}
