using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BestHTTP.SocketIO;
using UnityEngine.UI;
//using I2.Loc;

public class TableData : MonoTemplate
{

    #region PUBLIC_VARIABLES

    [Header("Gamobjects")]
    public GameObject lockObj;

    //[Header ("Transforms")]


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    //[Header ("Images")]

    [Header("Buttons")]
    public Button JoinButton;

    [Header("Image and Sprites")]
    public Image panelImg;
    public Sprite greenPanelImg, YellowPanelImg;


    [Header("Text")]
    public TextMeshProUGUI winAmount;
    public TextMeshProUGUI entryFees;
    public TextMeshProUGUI onlinePlayers;

    [Header("ScriptableObjects")]
    public GetAllTableData data;
    //[Header ("Prefabs")]

    //[Header ("Enums")]


    //[Header ("Variables")]

    #endregion

    #region PRIVATE_VARIABLES
    public UserSelectGoti selectData;
    public List<int> selectedColors;

    private int playerSelectedColor = -1;
    //private bool eventCall = false;
    #endregion    

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS

    public void SetData(GetAllTableData data)
    {
        this.data = data;

        if (data.totalPrice > 0)
        {
            winAmount.text = data.totalPrice.ToString();
        }
        else
        {
            winAmount.text = "0";//"---";
        }
        if (data.entryFee > 0)
        {
            entryFees.text = $"Entry {data.entryFee}";
            //entryFees.GetComponent<LocalizationParamsManager>().SetParameterValue("Value", data.entryFee.ToString());
        }
        else
        {
            entryFees.text = "Free";//"---";
          //  entryFees.GetComponent<LocalizationParamsManager>().SetParameterValue("Value", "0");
        }
        if (data.onlineUser > 0)
        {
            onlinePlayers.text = $"Players : {data.onlineUser}";
            panelImg.sprite = YellowPanelImg;
           // onlinePlayers.GetComponent<LocalizationParamsManager>().SetParameterValue("Value", data.onlineUser.ToString());
        }
        else
        {
            onlinePlayers.text = "";
            panelImg.sprite = greenPanelImg;
        }
        JoinButton.interactable = true;
        this.Open();

        if (!Ludo_UIManager.instance.assetOfGame.SavedLoginData.isFirstDeposit && data.entryFee > 10)
        {
            lockObj.SetActive(false);
        }
        else
        {
            lockObj.SetActive(false);
        }
    }

    public void OnOpenGame()
    {
        if (lockObj.activeSelf)
        {
            //Ludo_UIManager.instance.storeScreen.Open();
            Ludo_UIManager.instance.playOnline.unlockTablePopup.Open();
        }
        else
        {
            JoinButton.interactable = false;
            CallEnterRoom();
            Invoke(nameof(enableJoinButton), 3f);
        }

        //oveshwork
        //Ludo_UIManager.instance.playOnline.DirectJoinGame(this.data._id);
    }

    public void UpdatePlayerOnlineCount(int onlineUser)
    {
        data.onlineUser = onlineUser;
        if (onlineUser > 0)
        {
            onlinePlayers.text = $"Players : {onlineUser}";
            panelImg.sprite = YellowPanelImg;
        }
        else
        {
            onlinePlayers.text = "";
            panelImg.sprite = greenPanelImg;
        }
    }
    #endregion

    #region PRIVATE_METHODS
    private void enableJoinButton()
    {
        JoinButton.interactable = true;
    }
    private void CallEnterRoom()
    {
        Ludo_UIManager.instance.OpenLoader(true);
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("tableId", this.data._id);
        data.Add("playerId", ServerSocketManager.instance.playerId);
        data.Add("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        GetSocketManager.enterRoom(data, ResponseOfenterRoom);
    }



    private void ResponseOfenterRoom(Socket socket, Packet packet, params object[] args)
    {
        Ludo_UIManager.instance.OpenLoader(false);
        Debug.Log(Ludo_Constants.LudoEvents.EnterRoom + "  response  :" + packet.ToString());
        JSONArray arr = new JSONArray(packet.ToString());
        string Source = arr.getString(arr.length() - 1); // this already gives you the inner JSON array as string
        JSONArray jsonArr = new JSONArray(Source);
        string firstObj = jsonArr.getString(0); // extract the first object inside the array


        EventResponse<EnterRoomData> response = JsonUtility.FromJson<EventResponse<EnterRoomData>>(firstObj);

        if (response.status == EventResponse.STATUS_SUCCESS)
        {
            GetUIManager.assetOfGame.uniqueId = response.result.uniqueId;
            SetColorDataFromResponse(response.result);
            OpenColorSelector();
            GetPlayOnlineScreen.GetColorSelector.roomData = response.result;
            GetPlayOnlineScreen.GetColorSelector.SetToken();
        }
        else
        {
            if (response.message == "Insufficient Balance." && Ludo_UIManager.instance.assetOfGame.SavedLoginData.isWalletEnable == true)
            {
                Debug.Log("NoBalance ");
                //Ludo_UIManager.instance.playOnline.insufficientBalancePopup.Open();
           //     Ludo_UIManager.instance.storeScreen.Open();
            }
            else
                GetMessagePanel.DisplayMessage(response.message);
        }
    }



    private void CalluserSelectGoti(int selectedNumber)
    {
        GetUIManager.OpenLoader(true);

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("tableId", this.data._id);
        data.Add("userSelectGoti", Cust_Utility.GetColorName(selectedNumber));
        data.Add("playerId", ServerSocketManager.instance.playerId);
        data.Add("deviceId", SystemInfo.deviceUniqueIdentifier.ToString());
        GetSocketManager.userSelectGoti(data, ResponseOfUserSelectGoti);
    }
    private void ResponseOfUserSelectGoti(Socket socket, Packet packet, params object[] args)
    {
        GetUIManager.OpenLoader(false);

        int arrLen = new JSONArray(packet.ToString()).length();
        Debug.Log(Ludo_Constants.LudoEvents.UserSelectGoti + "  response  :" + packet.ToString());


        EventResponse<UserSelectGoti> response = JsonUtility.FromJson<EventResponse<UserSelectGoti>>(packet.GetPacketString());
        if (response.status == EventResponse.STATUS_SUCCESS)
        {
            selectData = response.result;
            PreparePlayerSelectedColors();

            if (!GetPlayOnlineScreen.GetColorSelector.gameObject.activeSelf)
                OpenColorSelector();
        }
        else
        {
            GetMessagePanel.DisplayMessage(response.message);
        }
    }

    private void SetColorDataFromResponse(EnterRoomData data)
    {
        if (selectedColors == null)
            selectedColors = new List<int>();

        selectedColors.Clear();

        foreach (string str in data.selectedColor)
        {
            selectedColors.Add(Cust_Utility.GetColorIndexFromName(str));
        }

    }
    private void SetNewColors()
    {
        PreparePlayerSelectedColors();
        Debug.Log($"Size of Array: {selectedColors.Count}");
        GetPlayOnlineScreen.GetColorSelector.SetPawsButtons(selectedColors.ToArray());
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

    private void SetPlayerColor(int i)
    {
        GetPlayOnlineScreen.SelectColor(i);
        CalluserSelectGoti(i);
    }

    private void OpenColorSelector()
    {
        GetPlayOnlineScreen.GetColorSelector.OpenColorSelector(data._id, GetPlayOnlineScreen.GetGotiColors,
                                                         selectedColors.ToArray(), GetRandomColorForPlayer(),
                                                         SetPlayerColor, null);
        // Stephen Update
        GetPlayOnlineScreen.GetColorSelector.SettingEntryFeeAndPriceValue(data.entryFee.ToString(), winAmount.text);

    }

    private int GetRandomColorForPlayer()
    {
        int num = 0;
        for (int i = 0; i < selectedColors.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (selectedColors[i] != j)
                {
                    num = j;
                    break;
                }
            }
        }
        selectedColors.Add(num);
        return num;
    }

    private void SetPlayerSelectedColorInArray(int value)
    {
        if (selectedColors == null)
            selectedColors = new List<int>();

        if (selectedColors.Count == 0)
        {
            selectedColors.Add(value);
        }
        else
        {
            int i = selectedColors.FindIndex(x => x == playerSelectedColor);
            Debug.Log($"{i}:{playerSelectedColor}");

            if (i < 0)
            {
                selectedColors.Add(value);
            }
            else
            {
                selectedColors[i] = value;
            }
        }
    }
    #endregion

    #region COROUTINES
    #endregion


    #region GETTER_SETTER
    public int SelectedColor
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

    public string TableId
    {
        get
        {
            return data._id;
        }
    }
    #endregion
}
