using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class searchingPanel : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    [Header("ScriptableObjects")]
    public LuckyDrawPanel luckyDrawPanel;
    public LuckyDrawThreePanel luckyDrawPanelForThree;
    public LuckyDrawFourPanel luckyDrawPanelForFour;

    [Header("Buttons")]
    public Button BackButton;
    public Button musicButton;
    [Header("Images")]
    public Image timerBlock;
    public Image UserImage;

    [SerializeField]
    public Sprite spriteMusicOn;
    public Sprite spriteMusicOff;

    [Header("Text")]
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI myUserNameTxt;

    public Transform twoReel;
    public Transform threeReel1;
    public Transform threeReel2;
    public SlotReel threeReelright;
    public Transform fourReel1;
    public Transform fourReel2;
    public Transform fourReel3;

    [Header("Variables")]
    public bool isJoined = false;
    public bool backtolobbyTimerFinished = false;
    public string tableId = "";
    public string selectedGoti = "";
    public float timer;
    public float maxTimer = 5f;
    public List<Joinersusers> JoinersPLayers;

    [Space]
    [SerializeField] private GameObject EntryFeeAndPriceTextObj;
    [SerializeField] private TextMeshProUGUI Txt_EntryFees;
    [SerializeField] private TextMeshProUGUI Txt_PricePool;

    #endregion

    #region PRIVATE_VARIABLES

    #endregion

    #region UNITY_CALLBACKS

    // Use this for initialization
    void OnEnable()
    {
        //timer = 5;
        isJoined = false;

         ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.StartTimer, ResponseOfstartTimer);
         ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.BackToLobby, ResponseOfbackToLobby);
        ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.GameStart, ResponseOfgameStart);

        Timer.text = "00:00";
        //   SetReelPos();

        if (PlayerPrefs.GetInt("bgMusic") == 1)
        {
            musicButton.GetComponent<Image>().sprite = spriteMusicOn;
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = spriteMusicOff;
        }
    }

    void OnDisable()
    {
        isJoined = false;
        backtolobbyTimerFinished = false;
          ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.StartTimer, ResponseOfstartTimer);
          ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.BackToLobby, ResponseOfbackToLobby);
        ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.GameStart, ResponseOfgameStart);
        Timer.text = "00:00";


        //   SetReelPos();

        // Stephen Update
        Txt_EntryFees.text = string.Empty;
        Txt_PricePool.text = string.Empty;
        EntryFeeAndPriceTextObj.SetActive(false);
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            timerBlock.fillAmount = 1f - (timer / maxTimer);
            BackButton.interactable = true;
        }
        else
        {
            BackButton.interactable = false;
        }
    }

    #endregion

    #region DELEGATE_CALLBACKS

    private void ResponseOfstartTimer(Socket socket, Packet packet, params object[] args)
    {
        timer = 0;
        Ludo_UIManager.instance.OpenLoader(false);
        Debug.Log(Ludo_Constants.LudoEvents.StartTimer + " Broadcast response  :" + packet.ToString());

        JSONArray arr = new JSONArray(packet.ToString());
        StartTimerData response = JsonUtility.FromJson<StartTimerData>(packet.GetPacketString());
        Timer.text = string.Format("{1:D2}:{2:D2}", TimeSpan.FromSeconds(response.timer).Hours, TimeSpan.FromSeconds(response.timer).Minutes, TimeSpan.FromSeconds(response.timer).Seconds);
        //Timer.text = "Game will start in " + response.timer + " Seconds";
        JoinersPLayers = response.joiners;
        checkforJoiners();

        //   BackButton.interactable = false;
        //   timerBlock.fillAmount = 1;

    }

    private void ResponseOfbackToLobby(Socket socket, Packet packet, params object[] args)
    {
        Ludo_UIManager.instance.OpenLoader(false);
        Debug.Log(Ludo_Constants.LudoEvents.BackToLobby + " Broadcast response  :" + packet.ToString());

        JSONArray arr = new JSONArray(packet.ToString());
        string message = arr.getJSONObject(1).getString("message");
        //if (isJoined)
        //{
        Ludo_UIManager.instance.messagePanel.DisplayMessage(message, MoveToHome);
        //}
    }

    private void ResponseOfgameStart(Socket socket, Packet packet, params object[] args)
    {
        Ludo_UIManager.instance.OpenLoader(false);

        int arrLen = new JSONArray(packet.ToString()).length();
        Debug.Log(Ludo_Constants.LudoEvents.GameStart + " Broadcast response  :" + packet.ToString());

        GameStartDataResp response = JsonUtility.FromJson<GameStartDataResp>(packet.GetPacketString());

        if (!response.players.Contains(ServerSocketManager.instance.playerId))
        {
            this.Close();
            Ludo_UIManager.instance.playOnline.Close();
            Ludo_UIManager.instance.homeScreen.Open();
            return;
        }
        Debug.Log("PJ" + response);
          Ludo_UIManager.instance.gamePlayScreen.SetRoomDataAndPlay(response);        

        this.Close();
        Ludo_UIManager.instance.playOnline.Close();
        Ludo_UIManager.instance.homeScreen.Close();
    }

    private void GetplayGameResponse(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.JoinRoom + " tt response  :" + packet.ToString());

        isJoined = true;
        /*  JSONArray arr = new JSONArray(packet.ToString());
          string Source;
          Source = arr.getString(arr.length() - 1);
          var resp = Source;*/
        JSONArray arr = new JSONArray(packet.ToString());
        string Source = arr.getString(arr.length() - 1); // this already gives you the inner JSON array as string
        JSONArray jsonArr = new JSONArray(Source);
        string firstObj = jsonArr.getString(0); // extract the first object inside the array


     //   Debug.Log("Source ... " + Source);
        EventResponse<SearchingJoinGameResponse> HomeDataResp = JsonUtility.FromJson<EventResponse<SearchingJoinGameResponse>>(firstObj);
       // Debug.Log("@@@@@ 1   HomeDataResp.status " + HomeDataResp.result.status);
       if (HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
        {
       // Debug.Log("@@@@@ 1");
          /*  if (HomeDataResp.result.runningGameData.boardId.Length > 0)
            {
                Debug.Log("@@@@@ 2  SetRoomDataAndPlay.." + JsonUtility.ToJson(HomeDataResp.result.runningGameData));

           //     Ludo_UIManager.instance.gamePlayScreen.SetRoomDataAndPlay(HomeDataResp.result.runningGameData);                
                this.Close();
                Ludo_UIManager.instance.playOnline.Close();
            }    */        
        }
    }

    #endregion

    #region PUBLIC_METHODS

    public void RejoinReconnect()
    {
        if (!isJoined && backtolobbyTimerFinished)
        {
            Ludo_UIManager.instance.messagePanel.Close();
            StopCoroutine(joiRoomRequest());
            StartCoroutine(joiRoomRequest(1f));
            //Ludo_UIManager.instance.socketManager.JoinRoom(tableId, selectedGoti, GetplayGameResponse);
        }
    }

    public void MusicButtonTap()
    {

        if (PlayerPrefs.GetInt("bgMusic") == 1)
        {
            PlayerPrefs.SetInt("bgMusic", 0);
            Ludo_UIManager.instance.soundManager.stopBgSound();
            musicButton.GetComponent<Image>().sprite = spriteMusicOff;
        }
        else
        {
            PlayerPrefs.SetInt("bgMusic", 1);
            Ludo_UIManager.instance.soundManager.PlayBgSound();
            musicButton.GetComponent<Image>().sprite = spriteMusicOn;
        }
        Debug.Log("bgMusic = >" + PlayerPrefs.GetInt("bgMusic"));
    }

    public void SetDataAndOpen(string tId, string selectedGoties, int playerCount)
    {
        GameStaticData.playerCount = playerCount;
        SetDataAndOpen(tId, selectedGoties, false);
    }

    public void SetBidAndPriceText(string entryFee, string priceAmount)
    {
        // Stephen Update
        Txt_EntryFees.text = entryFee;
        Txt_PricePool.text = priceAmount;
        EntryFeeAndPriceTextObj.SetActive(true);
    }

    public void SetDataAndOpen(string tId, string selectedGoties, bool callJoinRoomEvent = true)
    {
        myUserNameTxt.text = Ludo_UIManager.instance.assetOfGame.SavedLoginData.username;
        this.tableId = tId;
        this.selectedGoti = selectedGoties;

        if (callJoinRoomEvent)
        {
            timer = 5;
            timerBlock.fillAmount = 0;
        }
        else
        {
            timer = 0;
            timerBlock.fillAmount = 1;
        }

        luckyDrawPanel.Close();
        luckyDrawPanelForThree.Close();
        luckyDrawPanelForFour.Close();
        this.Open();
        isJoined = false;
        if (GameStaticData.playerCount.ToString().Equals("2"))
        {
            luckyDrawPanel.Open();
            StartCoroutine(GameSlotStart());
        }
        else if (GameStaticData.playerCount.ToString().Equals("3"))
        {
            luckyDrawPanelForThree.Open();
            StartCoroutine(GameSlotStartThreePlayers());
        }
        else if (GameStaticData.playerCount.ToString().Equals("4"))
        {
            luckyDrawPanelForFour.Open();
            StartCoroutine(GameSlotStartFourPlayers());
        }

        // Ludo_UIManager.instance.socketManager.CheckGameStartTime(tableId, selectedGoti, CheckGameStartTimeCall);
        if (callJoinRoomEvent)
            StartCoroutine(joiRoomRequest());
        else
            StartCoroutine(joiRoomRequest(0));
    }

    public void MoveToHome()
    {
        if (Ludo_UIManager.instance.homeScreen.isActiveAndEnabled)
            Ludo_UIManager.instance.homeScreen.CheckFunctionOnEnable();
        else
            Ludo_UIManager.instance.homeScreen.Open();

        Ludo_UIManager.instance.gamePlayScreen.Close();
        this.Close();
    }

    #endregion

    #region PRIVATE_METHODS

    void SetReelPos()
    {
        twoReel.localPosition = new Vector3(0, 0, 0);

        fourReel1.localPosition = new Vector3(0, 0, 0);
        fourReel2.localPosition = new Vector3(0, -400, 0);
        fourReel3.localPosition = new Vector3(0, -607, 0);

        threeReelright.reels[0].transform.localPosition = new Vector3(0f, -400f, 0f);
        threeReelright.reels[1].transform.localPosition = new Vector3(0f, 1400f, 0f);
        threeReelright.reels[2].transform.localPosition = new Vector3(0f, 3200f, 0f);
        threeReelright.reels[3].transform.localPosition = new Vector3(0f, 5000f, 0f);
    }

    void CheckGameStartTimeCall(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("CheckGameStartTimeCall  response   : " + packet.ToString());

        JSONArray arr = new JSONArray(packet.ToString());
        string Source;
        Source = arr.getString(arr.length() - 1);
        var resp = Source;
        EventListResponse<GetAllTableData> HomeDataResp = JsonUtility.FromJson<EventListResponse<GetAllTableData>>(resp);

        if (HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
        {
            StopCoroutine(joiRoomRequest());
            timer = 0;

            isJoined = true;
            Ludo_UIManager.instance.socketManager.JoinRoom(tableId, selectedGoti, GetplayGameResponse);
        }
        else
        {

        }

    }

    void checkforJoiners()
    {
        if (GameStaticData.playerCount.ToString().Equals("2"))
        {
            if (JoinersPLayers.Count > 0)//&& JoinersPLayers.Count.Equals(2))
            {
                luckyDrawPanel.setOpponantData(JoinersPLayers);
            }
        }
        else if (GameStaticData.playerCount.ToString().Equals("3"))
        {
            if (JoinersPLayers.Count > 0)
            {
                luckyDrawPanelForThree.setOpponantData(JoinersPLayers);
            }
        }
        else if (GameStaticData.playerCount.ToString().Equals("4"))
        {
            luckyDrawPanelForFour.setOpponantData(JoinersPLayers);
        }
    }

    #endregion

    #region COROUTINES

    IEnumerator joiRoomRequest(float waitingTime = 5f)
    {
        yield return new WaitForSeconds(waitingTime);
        backtolobbyTimerFinished = true;
        if (!isJoined)
        {
            Ludo_UIManager.instance.socketManager.JoinRoom(tableId, selectedGoti, GetplayGameResponse);
        }
    }

    IEnumerator GameSlotStart()
    {
        //luckyDrawPanel.Close();
        yield return new WaitForSeconds(0.2f);
        luckyDrawPanel.OnSpinButtonTap();
    }

    IEnumerator GameSlotStartThreePlayers()
    {
        yield return new WaitForSeconds(0.2f);
        luckyDrawPanelForThree.OnSpinButtonTap();
    }

    IEnumerator GameSlotStartFourPlayers()
    {
        yield return new WaitForSeconds(0.2f);
        luckyDrawPanelForFour.OnSpinButtonTap();
    }

    #endregion    

}
