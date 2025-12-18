using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using BestHTTP.SocketIO;

public class ColorSelector : MonoTemplate
{
    #region PrivateVariables
    [SerializeField] private Button[] btnColorSelector;

    [SerializeField] private UserSelectGoti selectData;

    private List<int> selectedColors;
    private Action<int> colorCallback, onClickCallback;
    private string tableId;
    private int colorIndex;
    public EnterRoomData roomData;

    private string ENTRY_FEE = string.Empty;
    private string PRICE_POOL = string.Empty;
    #endregion

    #region Unity Callback
    void OnEnable()
    {
        colorIndex = -1;
        Debug.Log("Call enable color selcector");
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.GetUserSelectGoti, ResponseOfUserSelectGoti);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.UserSelectGotiRemove, UserSelectGotiRemove);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.SelectedToken, OnSelectedToken);
        foreach (Button token in btnColorSelector)
        {
            token.interactable = false;
        }

    }

    void OnDisable()
    {
          ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.GetUserSelectGoti, ResponseOfUserSelectGoti);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.UserSelectGotiRemove, UserSelectGotiRemove);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.SelectedToken, OnSelectedToken);
        colorIndex = -1;
    }
    #endregion

    #region Public Methods
    public void SetToken()
    {
        foreach (Button token in btnColorSelector)
        {
            token.interactable = false;
        }

        for (int i = 0; i < btnColorSelector.Length; i++)
        {
            for (int j = 0; j < roomData.colorId.Count; j++)
            {
                if (roomData.colorId[j] == i)
                {
                    //   Debug.Log(roomData.colorId[j] + "  &   " + i);
                    btnColorSelector[i].interactable = true;
                }
            }
        }
    }

    public void OpenColorSelector(string tableId, GotiSprites[] colors, int[] selectedColors, int playerColor, Action<int> onClickCallback, Action<int> callback = null)
    {
        this.tableId = tableId;

        for (int i = 0; i < btnColorSelector.Length; i++)
        {
            SetColorButtons(btnColorSelector[i], colors[i]);
        }

        ResetHard();
        SetPawsButtons(selectedColors);

        if (callback != null)
            colorCallback = callback;

        this.onClickCallback = onClickCallback;
        colorIndex = playerColor;
        this.Open();
        selectData.TableId = tableId;
    }

    public void SetColor(int i)
    {
        Button b = null;
        if (colorIndex >= 0)
            b = btnColorSelector[colorIndex];

        Button newB = btnColorSelector[i];
        DeSelectButton(b);
        SelectButton(newB);
        colorIndex = i;
        StartGame();
        //onClickCallback(i);
    }

    public void CloseWindow()
    {
        if (colorCallback != null)
            colorCallback(colorIndex);

        //CalluserSelectGotiRemove();
        CallExitRoom();

    }

    public void SetPawsButtons(int[] selectedColors)
    {
        foreach (int i in selectedColors)
        {
            SelectButton(btnColorSelector[i]);
        }
        for (int i = 0; i < btnColorSelector.Length; i++)
        {
            int index = Array.FindIndex(selectedColors, x => x == i);
            if (index == -1)
            {
                DeSelectButton(btnColorSelector[i]);
            }
        }
    }

    public void SettingEntryFeeAndPriceValue(string entryFee = "", string priceAmount = "")
    {
        // Stephen Update
        this.ENTRY_FEE = entryFee;
        this.PRICE_POOL = priceAmount;
    }

    public void StartGame()
    {
        if (!colorIndex.Equals(-1))
        {
            Ludo_UIManager.instance.PlayerSearchPanel.SetDataAndOpen(tableId, Cust_Utility.GetColorName(colorIndex));
            Ludo_UIManager.instance.PlayerSearchPanel.SetBidAndPriceText(ENTRY_FEE, PRICE_POOL);

            this.Close();
            // Ludo_UIManager.instance.playOnline.CloseButtonTap();

            //GetSocketManager.JoinRoom(tableId, Utility.GetColorName(colorIndex), GetplayGameResponse);
        }
        else
        {
            Ludo_UIManager.instance.messagePanel.DisplayMessage("Please select color");
        }
    }
    #endregion

    #region PrivateMethods
    private void UserSelectGotiRemove(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.UserSelectGotiRemove + " Broadcast response  :" + packet.ToString());

        if (!gameObject.activeSelf)
            return;

        EventResponse<UserSelectGotiRemove> response = JsonUtility.FromJson<EventResponse<UserSelectGotiRemove>>(packet.GetPacketString());
        int removedColorIndex = Cust_Utility.GetColorIndexFromName(response.result.removeGotiColor);
        DeSelectButton(btnColorSelector[removedColorIndex]);
    }

    private void OnSelectedToken(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.SelectedToken + " Broadcast response  :" + packet.ToString());

        if (!gameObject.activeSelf)
            return;

        SelectedToken response = JsonUtility.FromJson<SelectedToken>(packet.GetPacketString());
        roomData.colorId.Clear();
        roomData.colorName.Clear();

        for (int i = 0; i < response.colorId.Count; i++)
        {
            roomData.colorId.Add(response.colorId[i]);
            roomData.colorName.Add(response.colorName[i]);
        }

        SetToken();

    }

    private void GetplayGameResponse(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("GetJoinRoomResponse   response  : " + packet.ToString());

        JSONArray arr = new JSONArray(packet.ToString());
        string Source;
        Source = arr.getString(arr.length() - 1);
        var resp = Source;
        EventListResponse<GetAllTableData> HomeDataResp = JsonUtility.FromJson<EventListResponse<GetAllTableData>>(resp);

        if (HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
        {
            Ludo_UIManager.instance.WaitForGameScreen.Open();
            this.Close();
        }
    }
    private void CallExitRoom()
    {
        //Ludo_UIManager.instance.OpenLoader(true);
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("tableId", selectData.TableId);
        data.Add("playerId", ServerSocketManager.instance.playerId);
        data.Add("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        GetSocketManager.exitRoom(data, ResponseOfexitRoom);
        this.Close();
    }

    private void ResponseOfexitRoom(Socket socket, Packet packet, params object[] args)
    {
        Ludo_UIManager.instance.OpenLoader(false);
        Debug.Log(Ludo_Constants.LudoEvents.ExitRoom + "  response  :" + packet.ToString());

        EventResponse<EnterRoomData> response = JsonUtility.FromJson<EventResponse<EnterRoomData>>(packet.GetPacketString());
        if (response.status == EventResponse.STATUS_SUCCESS)
        {
            this.Close();
        }
        else
        {
            GetMessagePanel.DisplayMessage(response.message);
        }
    }
    private void ResponseOfUserSelectGoti(Socket socket, Packet packet, params object[] args)
    {
        GetUIManager.OpenLoader(false);

        int arrLen = new JSONArray(packet.ToString()).length();
        Debug.Log(Ludo_Constants.LudoEvents.GetUserSelectGoti + " Broadcast response  :" + packet.ToString());


        EventResponse<UserSelectGoti> response = JsonUtility.FromJson<EventResponse<UserSelectGoti>>(packet.GetPacketString());
        if (response.status == EventResponse.STATUS_SUCCESS)
        {
            selectData = response.result;
            PreparePlayerSelectedColors();
            SetPawsButtons(selectedColors.ToArray());
        }
        else
        {
            GetMessagePanel.DisplayMessage(response.message);
        }
    }
    private void CalluserSelectGotiRemove()
    {
        GetUIManager.OpenLoader(true);

        Dictionary<string, object> data = new Dictionary<string, object>();

        data.Add("tableId", selectData.TableId);
        data.Add("uniqueId", ServerSocketManager.instance.uniqueId);
        data.Add("playerId", ServerSocketManager.instance.playerId);
        data.Add("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        GetSocketManager.userSelectGotiRemove(selectData.TableId, AssetOfGame.uniqueId, ResponseOfuserSelectGotiRemove);
    }

    private void ResponseOfuserSelectGotiRemove(Socket socket, Packet packet, params object[] args)
    {
        GetUIManager.OpenLoader(false);
        Debug.Log(Ludo_Constants.LudoEvents.UserSelectGotiRemove + "  response  :" + packet.ToString());

        EventResponse<UserSelectGotiRemove> response = JsonUtility.FromJson<EventResponse<UserSelectGotiRemove>>(packet.GetPacketString());

        if (response.status == EventResponse.STATUS_FAIL)
            GetMessagePanel.DisplayMessage(response.message);
    }
    private void PreparePlayerSelectedColors()
    {
        if (selectedColors == null)
            selectedColors = new List<int>();

        selectedColors.Clear();
        string tableId = selectData.TableId;

        UserSelectGotiPlayers[] value = selectData.players;
        foreach (UserSelectGotiPlayers item in value)
        {
            int i = Cust_Utility.GetColorIndexFromName(item.selectedGoti);
            if (i != -1)
                selectedColors.Add(i);
        }
    }
    private void ResetHard()
    {
        ResetAllColors();
        colorIndex = 0;
    }
    private void ResetAllColors()
    {
        foreach (Button b in btnColorSelector)
        {
            DeSelectButton(b);
        }
    }

    private void SetColorButtons(Button b, GotiSprites sprites)
    {
        b.GetComponent<Image>().sprite = sprites.colorSprite;
        b.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprites.checkMark;
    }
    private void SelectButton(Button b)
    {
        b.transform.GetChild(0).gameObject.SetActive(true);
        b.interactable = false;
    }

    private void DeSelectButton(Button b)
    {
        if (b == null)
            return;

        b.transform.GetChild(0).gameObject.SetActive(false);
        b.interactable = true;
    }
    #endregion
}