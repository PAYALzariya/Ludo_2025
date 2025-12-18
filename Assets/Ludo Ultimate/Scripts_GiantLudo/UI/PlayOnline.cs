using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.SocketIO;
//using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayOnline : MonoBehaviour
{
    #region Public_Variables
    public GotiSprites[] colorsOfPlayers;

    [Space]
    [Header("Panels")]
    public PlayOnlineInsufficientBalancePopup insufficientBalancePopup;
    public PlayOnlineUnlockTablePopup unlockTablePopup;
    public Loader loader;
    #endregion

    #region Private_Variables
    [Header("Images")]
    [SerializeField] private ColorSelector selector;
    [SerializeField] private Image playerColor;

    [Space]
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI txtGoti1OnlinePlayer;
    [SerializeField] private TextMeshProUGUI txtGoti2OnlinePlayer;
    [SerializeField] private TextMeshProUGUI txtGoti3OnlinePlayer;
    [SerializeField] private TextMeshProUGUI txtGoti4OnlinePlayer;

    [Space]
    [Header("Toggles")]
    [SerializeField] private Toggle goti1;
    [SerializeField] private Toggle goti2;
    [SerializeField] private Toggle goti3;
    [SerializeField] private Toggle goti4;

    [Header("Transfrom")]
    public Transform Parent;
    [Header("Prefabs")]

    [SerializeField] private List<TableData> TableDataObjectList;
    [SerializeField] private TableData GetAllTableDataPrefab;

    private int playerSelectedColor;
    private int[] playersColors = new int[0];

    [SerializeField] private List<TableDataItem> tableHistoryList = new List<TableDataItem>();
    #endregion    

    #region  SocketEvent_Callback
    public void CallTableListEvent()
    {
        ClearObjects();
        AddTableFromHistory();
        //loader.Open();
        Ludo_UIManager.instance.socketManager.getAllTable(GameStaticData.playerCount.ToString(), GameStaticData.numberGoties.ToString(), GetAllTableResponse);
     
      //  socketManager.getAllTable(GameStaticData.playerCount.ToString(),
      //   GameStaticData.numberGoties.ToString(), GetAllTableResponse);

             /*Ludo_UIManager.instance.socketManager.JoinTableListing(GameStaticData.playerCount.ToString(), GameStaticData.numberGoties.ToString(), (socket, packet, args) => {
                Debug.Log("JoinTableListing response: " + packet.ToString());
             });*/
    }
    #endregion
    #region  Unity_Callback
    private void Awake()
    {
        goti1.onValueChanged.RemoveAllListeners();
        goti2.onValueChanged.RemoveAllListeners();
        goti3.onValueChanged.RemoveAllListeners();
        goti4.onValueChanged.RemoveAllListeners();

        goti1.onValueChanged.AddListener((b) =>
        {
            OnNumbersOfGotisSelected(b, 1);
        });
        goti2.onValueChanged.AddListener((b) =>
        {
            OnNumbersOfGotisSelected(b, 2);
        });
        goti3.onValueChanged.AddListener((b) =>
        {
            OnNumbersOfGotisSelected(b, 3);
        });
        goti4.onValueChanged.AddListener((b) =>
        {
            OnNumbersOfGotisSelected(b, 4);
        });

        goti1.isOn = true;
        goti2.isOn = false;
        goti3.isOn = false;
        goti4.isOn = false;
    }

    private void OnEnable()
    {
        selector.Close();
        playerColor.sprite = colorsOfPlayers[0].colorSprite;
        // Debug.Log("Ludo_Constants.LudoEvents::" + Game.Lobby.socketManager.Socket);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.UpdateTableListingData, OnUpdateTableListingData);
     //   ServerSocketManager.instance.RootSocket.On(Ludo_Constants.LudoEvents.UpdateTableListingData, OnUpdateTableListingData);
     //  Game.Lobby.OnSocketReconnected += OnReconnect;

        //goti1.onValueChanged.RemoveAllListeners();
        //goti2.onValueChanged.RemoveAllListeners();
        //goti3.onValueChanged.RemoveAllListeners();
        //goti4.onValueChanged.RemoveAllListeners();

        //goti1.onValueChanged.AddListener((b) =>
        //{
        //    OnNumbersOfGotisSelected(b, 1);
        //});
        //goti2.onValueChanged.AddListener((b) =>
        //{
        //    OnNumbersOfGotisSelected(b, 2);
        //});
        //goti3.onValueChanged.AddListener((b) =>
        //{
        //    OnNumbersOfGotisSelected(b, 3);
        //});
        //goti4.onValueChanged.AddListener((b) =>
        //{
        //    OnNumbersOfGotisSelected(b, 4);
        //});
        GameStaticData.numberGoties = 1;
        if (!Ludo_UIManager.instance.chatLudo)
        {
            CallTableListEvent();
        }
        //goti1.isOn = true;
        //goti2.isOn = false;
        //goti3.isOn = false;
        //goti4.isOn = false;
        ////PlayersColor = 2;
        insufficientBalancePopup.Close();
    }
    private void OnDisable()
    {
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.UpdateTableListingData, OnUpdateTableListingData);
      //  Game.Lobby.OnSocketReconnected -= OnReconnect;

        goti1.isOn = true;
        goti2.isOn = false;
        goti3.isOn = false;
        goti4.isOn = false;

        ClearObjects();

        //try
        //{
        //    Ludo_UIManager.instance.socketManager.LeaveTableListing(GameStaticData.playerCount.ToString(), GameStaticData.numberGoties.ToString(), (socket, packet, args) =>
        //    {
        //        Debug.Log("LeaveTableListing response: " + packet.ToString());
        //    });
        //}
        //catch(Exception e)
        //{
        //    Debug.Log("Error on LeaveTableListing fucntion: " + e);
        //}
    }
    #endregion

    #region Public_Methods
    public void DirectJoinGame(string tId)
    {
        Ludo_UIManager.instance.socketManager.JoinRoom(tId, "", GetplayGameResponse);
    }
    private void GetplayGameResponse(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.JoinRoom + " respnose  : " + packet.ToString());

        JSONArray arr = new JSONArray(packet.ToString());
        string Source;
        Source = arr.getString(arr.length() - 1);
        var resp = Source;
        EventListResponse<GetAllTableData> HomeDataResp = JsonUtility.FromJson<EventListResponse<GetAllTableData>>(resp);

        if (HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
        {
            Ludo_UIManager.instance.WaitForGameScreen.Open();
            Ludo_UIManager.instance.WaitForGameScreen.Timer.text = "Game will join Soon";
            this.Close();
        }
    }
    public void OnNumbersOfGotisSelected(bool b, int index)
    {
        if (b)
        {
            GameStaticData.numberGoties = index;
            if (!Ludo_UIManager.instance.chatLudo)
            {
                CallTableListEvent();
            }
        }
        //Debug.Log("GameStaticData.numberGoties = > " + GameStaticData.numberGoties);
    }

    public void OpenWindow(int i)
    {
        GameStaticData.gamesType = GamesType.QuickPlay;
        GameStaticData.playerCount = i;
        this.Open();
    }
    public void OpenGame()
    {
        this.Close();
        Ludo_UIManager.instance.LocalGameScreen.Open();
    }
    public void CloseButtonTap()
    {
        Ludo_UIManager.instance.homeScreen.Open();
        this.Close();
    }

    public void SelectColor(int colorIndex)
    {
        playerColor.sprite = colorsOfPlayers[colorIndex].colorSprite;
        PlayersColor = colorIndex;
    }
    #endregion

    #region Private_Methods
    private void OnReconnect()
    {
        if (!Ludo_UIManager.instance.chatLudo)
            CallTableListEvent();
    }

    private void ClearObjects()
    {

        foreach (Transform Data in Parent)
        {
            Destroy(Data.gameObject);
        }
        DestroyAllTables();
    }
    private void DestroyAllTables()
    {
        if (TableDataObjectList == null)
            return;

        if (TableDataObjectList != null)
        {
            foreach (TableData go in TableDataObjectList)
            {
                Destroy(go.gameObject);
            }
            TableDataObjectList = new List<TableData>();
        }

    }
    private void RemoveOtherTournaments(List<GetAllTableData> roomsList)
    {
        if (TableDataObjectList != null)
        {
            if (roomsList == null || roomsList.Count == 0)
            {
                foreach (TableData tro in TableDataObjectList.ToArray())
                {
                    TableDataObjectList.Remove(tro);
                    Destroy(tro.gameObject);
                }
            }
            else
            {
                List<string> roomIdsList = roomsList.Select(o => o._id).ToList();

                foreach (TableData tro in TableDataObjectList.ToArray())
                {
                    for (int i = 0; i < roomsList.Count; i++)
                    {
                        if (roomsList == null || !roomIdsList.Contains(tro.data._id))
                        {
                            TableDataObjectList.Remove(tro);
                            Destroy(tro.gameObject);
                        }
                    }
                }
            }
        }
    }

    private TableData GetTournamentObjIfAlreadyCreated(string tableID)
    {
        if (TableDataObjectList != null)
        {
            for (int i = 0; i < TableDataObjectList.Count; i++)
            {
                if (tableID.Equals(TableDataObjectList[i].data._id))
                {
                    return TableDataObjectList[i];
                }
            }
        }
        return null;
    }
    public string temptabledata;
    public PokerEventResponse<TableDataItem> HomeDataResp;
    private void GetAllTableResponse(Socket socket, Packet packet, params object[] args)
    {
      //  Debug.Log(Ludo_Constants.LudoEvents.GetAllTableParameters + " respnose  : " + packet.ToString());

        /*JSONArray arr = new JSONArray(packet.ToString());
        string Source;
        Source = arr.getString(arr.length() - 1);
        var resp = Source;
        loader.Close();
       HomeDataResp = JsonUtility.FromJson<PokerEventResponse<TableDataItem>>(resp);*/

        JSONArray arr = new JSONArray(packet.ToString());
        string Source = arr.getString(arr.length() - 1); // this already gives you the inner JSON array as string
        JSONArray jsonArr = new JSONArray(Source);
        string firstObj = jsonArr.getString(0); // extract the first object inside the array

        HomeDataResp = JsonUtility.FromJson<PokerEventResponse<TableDataItem>>(firstObj);


        Debug.Log(arr.length()  + "        Source:  " + Source);
        //     Debug.Log(arr.getString(arr.length() - 2) +"    resp :" +resp);
        // PokerEventResponse<TableDataItem> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<TableDataItem>>(resp);
        //   List<PokerEventResponse<TableDataItem>> parsedList = JsonHelper.FromJsonArray<PokerEventResponse<TableDataItem>>(resp);
        //   HomeDataResp = parsedList[0];
        loader.Close();
        if (HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
        {
         //   Debug.Log("1");
            UpdateTableEventHistory(HomeDataResp.result);
        //    if (GameStaticData.playerCount != HomeDataResp.result.tableType || GameStaticData.numberGoties != HomeDataResp.result.selectedGoti)
          //      return;

            UpdatePlayerCountGotiWise(HomeDataResp.result.goti1OnlinePlayer, HomeDataResp.result.goti2OnlinePlayer,
                HomeDataResp.result.goti3OnlinePlayer, HomeDataResp.result.goti4OnlinePlayer);

            for (int i = 0; i < HomeDataResp.result.tableData.Count; i++)
            {
             //   Debug.Log("2");
                TableData obj = GetTournamentObjIfAlreadyCreated(HomeDataResp.result.tableData[i]._id);

                if (obj != null)
                {
                //    Debug.Log("3");
                    obj.SetData(HomeDataResp.result.tableData[i]);
                }
                else
                {
                 //   Debug.Log("4");
                    TableData TableDataDetails = Instantiate(GetAllTableDataPrefab) as TableData;
                    TableDataDetails.SetData(HomeDataResp.result.tableData[i]);
                    TableDataDetails.transform.SetParent(Parent, false);
                    TableDataObjectList.Add(TableDataDetails);
                }
            }
          //  Game.Lobby.SetSocketNamespace = HomeDataResp.result.nameSpace;
            RemoveOtherTournaments(HomeDataResp.result.tableData);
        }
        else
        {
           // Debug.Log("5");
            TableDataObjectList.Clear();
            foreach (Transform Data in Parent)
            {
            //    Debug.Log("6");
                Destroy(Data.gameObject);
            }
        }
    }

    private void SetPlayerSelectedColorInArray(int value)
    {
        if (playersColors == null)
            playersColors = new int[0];

        if (playersColors.Length == 0)
        {
            Array.Resize(ref playersColors, 1);
            playersColors[0] = value;
        }
        else
        {
            int i = Array.FindIndex(playersColors, x => x == playerSelectedColor);

            if (i < 0)
            {
                int len = playersColors.Length;
                Array.Resize(ref playersColors, len + 1);

                playersColors[len] = value;
            }
            else
            {
                playersColors[i] = value;
            }
        }
    }

    private void UpdateTableEventHistory(TableDataItem tableEventResponse)
    {
        bool tableExisted = false;

        foreach (TableDataItem tableHistory in tableHistoryList)
        {
           // Debug.Log("11");
            if (tableHistory.tableType == tableEventResponse.tableType && tableHistory.selectedGoti == tableEventResponse.selectedGoti)
            {
             //   Debug.Log("22");
                tableExisted = true;
                tableHistory.tableData = tableEventResponse.tableData;
                break;
            }
        }

        if (tableExisted == false)
            tableHistoryList.Add(tableEventResponse);
    }

    private void AddTableFromHistory()
    {
        TableDataItem existedTableData = null;

        foreach (TableDataItem tableHistory in tableHistoryList)
        {
            if (tableHistory.tableType == GameStaticData.playerCount && tableHistory.selectedGoti == GameStaticData.numberGoties)
            {
                existedTableData = tableHistory;
                break;
            }
        }

        if (existedTableData == null)
        {
            loader.Open();
        }
        else
        {
            foreach (GetAllTableData tableData in existedTableData.tableData)
            {
                TableData TableDataDetails = Instantiate(GetAllTableDataPrefab);
                TableDataDetails.SetData(tableData);
                TableDataDetails.transform.SetParent(Parent, false);
                TableDataObjectList.Add(TableDataDetails);
            }
        }
    }

    private void OnUpdateTableListingData(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.UpdateTableListingData + " Broadcast response  :" + packet.ToString());

        UpdateTableListingData updateTableListingData = JsonUtility.FromJson<UpdateTableListingData>(packet.GetPacketString());

        foreach (TableData tableDataObj in TableDataObjectList)
        {
            if (tableDataObj.TableId == updateTableListingData.tableId)
            {
                Debug.Log("match table 1");
                tableDataObj.UpdatePlayerOnlineCount(updateTableListingData.onlinePlayer);
                UpdatePlayerCountGotiWise(updateTableListingData.goti1OnlinePlayer, updateTableListingData.goti2OnlinePlayer,
                    updateTableListingData.goti3OnlinePlayer, updateTableListingData.goti4OnlinePlayer);
                break;
            }
        }
    }

    private void UpdatePlayerCountGotiWise(int goti1Count, int goti2Count, int goti3Count, int goti4Count)
    {
     //  Debug.Log("2222");
        txtGoti1OnlinePlayer.text = "Players\n" + goti1Count;
        txtGoti2OnlinePlayer.text = "Players\n" + goti2Count;
        txtGoti3OnlinePlayer.text = "Players\n" + goti3Count;
        txtGoti4OnlinePlayer.text = "Players\n" + goti4Count;

        //txtGoti1OnlinePlayer.GetComponent<LocalizationParamsManager>().SetParameterValue("Value", goti1Count.ToString());
       // txtGoti2OnlinePlayer.GetComponent<LocalizationParamsManager>().SetParameterValue("Value", goti2Count.ToString());
        //txtGoti3OnlinePlayer.GetComponent<LocalizationParamsManager>().SetParameterValue("Value", goti3Count.ToString());
        //txtGoti4OnlinePlayer.GetComponent<LocalizationParamsManager>().SetParameterValue("Value", goti4Count.ToString());
    }
    #endregion

    #region Coroutine
    #endregion

    #region  GetterSetter
    public int PlayersColor
    {
        get
        {
            return playerSelectedColor;
        }
        set
        {
            SetPlayerSelectedColorInArray(value);
            playerSelectedColor = value;
        }
    }

    public GotiSprites[] GetGotiColors => colorsOfPlayers;
    public ColorSelector GetColorSelector => selector;
    #endregion
}

[System.Serializable]
public struct GotiSprites
{
    public Sprite colorSprite;
    public Sprite checkMark;
}