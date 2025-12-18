using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using I2.Loc;
using DG.Tweening;

public class LudoHomeScreen : MonoBehaviour
{

    #region Public_Variables

    [Header("Pannels Ref")]
    public GameObject[] PannelsRef;

    [Header("Winner List")]
 //   public RectTransform ParentRectRef;
   // public TMP_Text WinnerListTextRef;
    Tween tweenRef;
    bool RequestNewWinnerListData = false;

    [Space]

   // public Button SupportButtonRef;
    bool SupportScreenStatus = false;
    //public GameObject SupportBtnNotificationIconRef;
    //public GameObject walletBonusRef;
    //public GameObject SharePanelRef;

    [Header("Block List")]
   // public Button BlockListButtonRef;
    bool BlockListScreenStatus = false;

    #endregion

    #region Private_Variables
 //   public TopPlayerInfo playerInfo;
    public TextMeshProUGUI twoPlayers;
    public TextMeshProUGUI threePlayers;
    public TextMeshProUGUI fourPlayers;
  /// <summary>
  ///  public TextMeshProUGUI allPlayers;
  /// </summary>
    //public TextMeshProUGUI txtChips;
    //public TextMeshProUGUI txtBonusWalletChips;
    private int _twoPlayersValue;
    private int _threePlayersValue;
    private int _fourPlayersValue;
    private int _TotalplayersValue;

   // [Header("Buttons")]
   /* [SerializeField] private Button btnProfile;
    [SerializeField] private Button btnWhatsApp;
    [SerializeField] private Button btnTelegram;
    [SerializeField] private Button btnCallCustomerCare;
    [SerializeField] private Button btnStore;
    [SerializeField] private Button btnStore_Google;
    [SerializeField] private Button btnNotice;
    [SerializeField] private Button btnPurchaseHistory;
    [SerializeField] private Button btnWithdrawHistory;
    [SerializeField] private Button btnLeaderboard;
    [SerializeField] private Button btnWallet;
    [SerializeField] private Button btnShare;
    [SerializeField] private Button btnTournament;
    [SerializeField] private string ShareText;*/

    // Stephen Aspect ration Update
    [Header("Panel")]
   // public RectTransform LogoImg;
  //  public RectTransform LogoAndButtonPanel;
    public RectTransform GameSelectionPanel;
  //  public RectTransform BottomButtonPanel;

    [Header("Particle")]
    public GameObject WelcomeParticle;

    #endregion

    #region  Unity_Callback

    void Awake()
    {
        Set_UI_AspectRatio();
        GameStaticData.isRegisteredPlayer = true;

        Debug.Log("GameStaticData.isRegisteredPlayer : " + GameStaticData.isRegisteredPlayer);

        // Check if the player is registered
      
    }

    private void Update()
    {
       /* if (!Ludo_UIManager.instance.assetOfGame.SavedLoginData.isWalletEnable)
        {
            Ludo_UIManager.instance.loginScreen.Close();
        }*/
    }

    public void ShowWelcomeParticle()
    {
        Invoke(nameof(ShowParticle), 1f);
    }

    private void ShowParticle()
    {
        if (!WelcomeParticle.activeInHierarchy && this.gameObject.activeSelf)
            WelcomeParticle.SetActive(true);
    }


    /* void Update()
     {
         if (Input.GetKeyDown(KeyCode.A))
         {

             Ludo_UIManager.instance.messagePanel.DisplayConfirmationMessage("You loose your coins once you leave the game", (b) =>
             {
                 if (b)
                 {

                 }
             });
         }
         if (Input.GetKeyDown(KeyCode.S))
         {
             Ludo_UIManager.instance.messagePanel.DisplayMessage("Are you sure you want Exit?");
         }
     }*/

    private void OnEnable()
    {
        //if (Ludo_UIManager.instance.assetOfGame.SavedLoginData.isFirstDeposit)
        //{
        //    Debug.LogError("OnEnable..");
        //    CheckFunctionOnEnable();
        //    Ludo_UIManager.instance.soundManager.PlayBgSound();
        //}
        //else
        //{
        //    Ludo_UIManager.instance.storeScreen.Open();
        //    this.Close();
        //}        

        //Debug.LogError("OnEnable..");
/*
        if (!Ludo_UIManager.instance.assetOfGame.SavedLoginData.isWalletEnable)
        {
            SupportButtonRef.gameObject.SetActive(false);
            BlockListButtonRef.gameObject.SetActive(false);
            walletBonusRef.SetActive(false);
            SharePanelRef.SetActive(false);
        }
*/

       // CheckFunctionOnEnable();
//        Ludo_UIManager.instance.soundManager.PlayBgSound();
    }

    public void CheckFunctionOnEnable()
    {
        BackgroundEventManager.instance.CheckServerMaitenanceStatus();

        //PlayerPrefs.SetInt(Ludo_Constants.LudoConstants.UserLogin, Ludo_Constants.LudoConstants.loggedIn);
      //  playerInfo.SetPlayerData();
     //   CallupdateUserSocketId();
      //  GetHomeScreenShareText();

     /*   if (Ludo_UIManager.instance.assetOfGame.SavedLoginData.isWalletEnable)
        {
            CallAndResetWinnerScroll();
        }
*/
      //  checkRunningGame();
   //     RefreshTable();
        StopCoroutine("RefreshTableOnInterval");
        StartCoroutine("RefreshTableOnInterval");

        RefreshButtonPreventionFunction();
    }

    private void OnDisable()
    {
        StopCoroutine("RefreshTableOnInterval");
        Debug.Log("On Disable called...");

        // Kill And reset Tween
        //if (tweenRef != null)
        //{
        //    tweenRef.OnKill(() =>
        //    {
        //        // Set Position of the Text
        //        WinnerListTextRef.rectTransform.anchoredPosition = new Vector2(ParentRectRef.rect.width, WinnerListTextRef.rectTransform.anchoredPosition.y);

        //        // Check if invoke running
        //        if (IsInvoking(nameof(NextWinnerListCall)))
        //        {
        //            CancelInvoke(nameof(NextWinnerListCall));
        //        }
        //        tweenRef = null;
        //        RequestNewWinnerListData = false;
        //    });
        //    tweenRef.Kill(false);            
        //}        
    }

    private void CallupdateUserSocketId()
    {
        // Return if player is not registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            return;
        }

        //Ludo_UIManager.instance.OpenLoader(true);
        Ludo_UIManager.instance.socketManager.updateUserSocketId(ResponseOfupdateUserSocketId);
    }

    private void ResponseOfupdateUserSocketId(Socket socket, Packet packet, object[] args)
    {
        Ludo_UIManager.instance.OpenLoader(false);
        //Debug.Log($"updateUserSocketId Response: {packet}");
    }

    private void RefreshButtonPreventionFunction()
    {
        bool isWalletEnable = Ludo_UIManager.instance.assetOfGame.SavedLoginData.isWalletEnable;
        Debug.Log("Is Wallet Enable :" + isWalletEnable);
   //     btnProfile.interactable = isWalletEnable;
        //btnShare.gameObject.SetActive(isWalletEnable);
        //btnWhatsApp.gameObject.SetActive(isWalletEnable);
        //btnTelegram.gameObject.SetActive(isWalletEnable);
        //btnCallCustomerCare.gameObject.SetActive(isWalletEnable);
        //btnStore.gameObject.SetActive(isWalletEnable);
        //btnStore_Google.gameObject.SetActive(!isWalletEnable);
       /* btnNotice.gameObject.SetActive(isWalletEnable);
        btnPurchaseHistory.gameObject.SetActive(isWalletEnable);
        btnWithdrawHistory.gameObject.SetActive(isWalletEnable);
        btnLeaderboard.gameObject.SetActive(isWalletEnable);
        btnWallet.gameObject.SetActive(isWalletEnable);
        btnTournament.gameObject.SetActive(isWalletEnable);*/
    }

    #endregion

    #region Private_Methods

    private void Set_UI_AspectRatio()
    {
        // Setting UI according to the aspect ration of Device
        if (AspectRatioGetter.Instance.deviceStyle == DeviceStyle.Tall)
        {
          //  LogoImg.localScale = Vector3.one;
          //  LogoAndButtonPanel.sizeDelta = new Vector2(LogoAndButtonPanel.sizeDelta.x, 575f);
            GameSelectionPanel.sizeDelta = new Vector2(GameSelectionPanel.sizeDelta.x, 1050f);
        //    BottomButtonPanel.sizeDelta = new Vector2(BottomButtonPanel.sizeDelta.x, 200f);
        }
    }

    #endregion

    #region Public_Methods

    public void checkRunningGame()
    {
        // Return if player is not registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            return;
        }

        Ludo_UIManager.instance.socketManager.checkRunningGame((socket, packet, args) =>
        {
                   Debug.Log(Ludo_Constants.LudoEvents.CheckRunningGame + "  response  :" + packet.ToString());


            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventResponse<checkRunningGameData> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<checkRunningGameData>>(resp);

            if (HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                if (HomeDataResp.result.gamestart.GameStart)
                {
                    if (HomeDataResp.result.gamestart.nameSpace.Equals("private"))
                    {
                        Ludo_UIManager.instance.OpenLoader(true);

                        // Stephen Update Direct start already runnig game 
                        acceptCall(HomeDataResp.result.gamestart);
                    }
                    else
                    {
                        Ludo_UIManager.instance.OpenLoader(true);

                        // Stephen Update Direct start already runnig game 
                        acceptCall(HomeDataResp.result.gamestart);
                    }
                }
                else
                {
               //     Game.Lobby.SetSocketNamespace = HomeDataResp.result.gamestart.nameSpace;
                    Ludo_UIManager.instance.PlayerSearchPanel.SetDataAndOpen(HomeDataResp.result.gamestart.tableId,
                        HomeDataResp.result.gamestart.selectedGoti, HomeDataResp.result.gamestart.playerCount);

                    Ludo_UIManager.instance.PlayerSearchPanel.SetBidAndPriceText(HomeDataResp.result.gamestart.entryFee.ToString(), HomeDataResp.result.gamestart.priceAmount.ToString());

                    this.Close();
                }
            }
            else
            {
                if (Ludo_UIManager.instance.gamePlayScreen.gameObject.activeInHierarchy)
                {
                    Debug.Log("No Game Data Found, User has been thrown back to lobby.");
                    // check if user needs to go back to lobby
                    Ludo_UIManager.instance.gamePlayScreen.ThrowToLobby();
                    this.Open();
                    if (Ludo_UIManager.instance.loader.gameObject.activeInHierarchy)
                    {
                        Ludo_UIManager.instance.OpenLoader(false);
                    }

                    // Check if message popup is on
                    if (Ludo_UIManager.instance.messagePanel.gameObject.activeInHierarchy)
                    {
                        // Check if message popup contains message of leave game
                        if (Ludo_UIManager.instance.messagePanel.ReturnCurrentMessage().Contains("leave the game"))
                        {
                            // Close Message Pannel
                            Ludo_UIManager.instance.messagePanel.Close();
                        }
                    }
                }
            }
        });
    }

    public void GetWinnerPlayers()
    {
        Ludo_UIManager.instance.socketManager.GetWinnerPlayersList((socket, packet, args) =>
        {
               Debug.Log(Ludo_Constants.LudoEvents.WinnerPlayers + "  response  :" + packet.ToString());


            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventListResponse<WinnerPlayersData> Resp = JsonUtility.FromJson<PokerEventListResponse<WinnerPlayersData>>(resp);

            if (Resp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                if (Resp.result.Count > 0)
                {
                    string PlayerEntry = "";
                    string theWord = "";
                    // Store text in respons
                    for (int i = 0; i < Resp.result.Count; i++)
                    {
                        PlayerEntry = Resp.result[i].playerName + " won " + Resp.result[i].amount + " coins | ";
                        theWord += PlayerEntry;
                    }

                    // Get the size of the text for the given string.
                  //  Vector2 textSize = WinnerListTextRef.GetPreferredValues(theWord);

                    // Set the text
                 //   WinnerListTextRef.text = theWord;
                   // WinnerListTextRef.rectTransform.sizeDelta = new Vector2(textSize.x, WinnerListTextRef.rectTransform.sizeDelta.y);

                    //Debug.Log("screen Width" + ParentRectRef.rect.width);
                    //Debug.Log("Pos : " + WinnerListTextRef.rectTransform.anchoredPosition);

                    // Reset Winner tween list
                    ResetWinnerListTween();

                    // Check if invoke running
                    if (IsInvoking(nameof(NextWinnerListCall)))
                    {
                        CancelInvoke(nameof(NextWinnerListCall));
                    }

                    // Request New Data after 5 min
                    Invoke(nameof(NextWinnerListCall), (60 * 5));
                }
                else
                {
                    Debug.Log("No Winner Data List Found");

                    // Check if invoke running
                    if (IsInvoking(nameof(NextWinnerListCall)))
                    {
                        CancelInvoke(nameof(NextWinnerListCall));
                    }
                }
            }
            else
            {
                // Check if invoke running
                if (IsInvoking(nameof(NextWinnerListCall)))
                {
                    CancelInvoke(nameof(NextWinnerListCall));
                }
            }
        });
    }

    public void acceptCall(GameStartDataResp response)
    {
        Debug.Log("PJ");
        //pj     Ludo_UIManager.instance.gamePlayScreen.SetRoomDataAndPlay(response);
        this.Close();
        // Disable actice pannel
        for (int i = 0; i < PannelsRef.Length; i++)
        {
            if (PannelsRef[i].activeInHierarchy)
            {
                // Disable pannel
                PannelsRef[i].SetActive(false);
            }
        }
    }

    public void RejectCall(GameStartDataResp response)
    {
        Ludo_UIManager.instance.socketManager.leaveRunningGame(response.tableId, response.boardId, response.tournamentId, response.uniqueId, response.nameSpace, (socket, packet, args) =>
            {
                     Debug.Log(Ludo_Constants.LudoEvents.LeaveRunningGame + " Broadcast response  :" + packet.ToString());


                JSONArray arr = new JSONArray(packet.ToString());
                string Source;
                Source = arr.getString(arr.length() - 1);
                var resp = Source;
                PokerEventResponse<checkRunningGameData> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<checkRunningGameData>>(resp);

            });
    }
    public void RefreshTable()
    {
        callOnlinePlayerlist();
    }
    public void callOnlinePlayerlist()
    {
        Ludo_UIManager.instance.socketManager.onlineUserList((socket, packet, args) =>
        {
            Debug.Log("onlineUserList  : " + packet.ToString());

            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventResponse<HomeDataItem> HomeDataResp = JsonUtility.FromJson<PokerEventResponse<HomeDataItem>>(resp);

            if (HomeDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                twoPlayersValue = HomeDataResp.result.playerOnline2;
                threePlayersValue = HomeDataResp.result.playerOnline3;
                fourPlayersValue = HomeDataResp.result.playerOnline4;
                Totalplayers = HomeDataResp.result.totalPlayers;
            }
        });
    }
  /*  public void OpenWhatsapp()
    {
        AppsflyerInAppEventsManager.Instance.WhatsAppSupprt();
        Application.OpenURL(Ludo_UIManager.instance.assetOfGame.SavedLoginData.whatsappUrl);
    }
    public void OpenYoutube()
    {
        AppsflyerInAppEventsManager.Instance.OpenYoutubeChannel();
        Application.OpenURL(Ludo_UIManager.instance.assetOfGame.SavedLoginData.youtubeUrl);
    }*/

    public void FollowUs(int id)
    {
        if (id == 1)
        {
            Application.OpenURL(Ludo_UIManager.instance.assetOfGame.SavedLoginData.metaUrl);
        }
        else if (id == 2)
        {
            Application.OpenURL(Ludo_UIManager.instance.assetOfGame.SavedLoginData.youtubeUrl);
        }
        else if (id == 3)
        {
            Application.OpenURL(Ludo_UIManager.instance.assetOfGame.SavedLoginData.twitterUrl);
        }
        else if (id == 4)
        {
            Application.OpenURL(Ludo_UIManager.instance.assetOfGame.SavedLoginData.instaUrl);
        }
        else if (id == 5)
        {
            Application.OpenURL(Ludo_UIManager.instance.assetOfGame.SavedLoginData.pinterestUrl);
        }
    }

    public void OpenStoreScreen()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }

       /* if (Ludo_UIManager.instance.assetOfGame.SavedLoginData.isVerified)
        {
            Ludo_UIManager.instance.storeScreen.Open();
            this.Close();
        }
        else
        {
            Ludo_UIManager.instance.newRegisterSocialPanel.Open();
            Ludo_UIManager.instance.newRegisterSocialPanel.isDepositWithdraw = 1;
            this.Close();
        }*/
    }

    public void OpenInfoMessageForBonusWallet()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }

        // Display Info Message
        //Ludo_UIManager.instance.messagePanel.DisplayMessage($"<size=34>To Unlock The Bonus Wallet Please Recharge with min Amount</size>\n<size=23>Note: 10% of Bet Amount Will be Deducted From Bonus Wallet.</size>");
        Ludo_UIManager.instance.messagePanel.DisplayMessage($"<size=34>Note: 10% of Bet Amount Will be Deducted From Bonus Wallet.</size>");
    }

    public void OpenGoogleInAppStore()
    {
        Debug.Log("IAP Click : Do something special here");
        /*        if (Application.platform == RuntimePlatform.Android)
                {
                    Debug.Log("IAP : RuntimePlatform.Android");
                    Ludo_UIManager.instance.InAppScreen.Open();
                    this.Close();
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    Debug.Log("IAP : RuntimePlatform.IPhonePlayer");
                }*/

        Debug.Log("IAP : RuntimePlatform.Android");
    //    Ludo_UIManager.instance.InAppScreen.Open();
        this.Close();
    }

    public void OpenNotice()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }

      //  Ludo_UIManager.instance.notice.Open();
    }

    public void OpenPurchaseHistory()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }
      //  Ludo_UIManager.instance.purchaseHistory.Open();
    }
    public void OpenWithdrawalHistory()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }
      //  Ludo_UIManager.instance.withdrawalHistory.Open();
        //this.Close();
    }

    public void OpenLeaderBoard()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }

        //Ludo_UIManager.instance.messagePanel.DisplayMessage("Coming Soon ! ");
        //return;
      //  Ludo_UIManager.instance.leaderBoard.Open();
    }

    public void OpenSettings()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }

        Ludo_UIManager.instance.settings.SetData();
        this.Close();
    }

    public void OpenClainScreen()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }

        /*if (Ludo_UIManager.instance.assetOfGame.SavedLoginData.isVerified)
        {
            Ludo_UIManager.instance.withdrawScreen.Open();
        }
        else
        {
            Ludo_UIManager.instance.newRegisterSocialPanel.Open();
            Ludo_UIManager.instance.newRegisterSocialPanel.isDepositWithdraw = 2;
            this.Close();
        }*/
    }

    public void OpenPlayOnline(int num)
    {
        Debug.Log("OpenPlayOnline");
        // Check if the player is registered
      /*  if (GameStaticData.isRegisteredPlayer == false)
        {
            if (num == 2)
            {
GameStaticData.gamesType = GamesType.Player2;
            }
            else   if (num == 3)
            {
GameStaticData.gamesType = GamesType.Player3;
            }
            else  if (num == 4)
            {   
                GameStaticData.gamesType = GamesType.Player4;
                
            }
          
             PannelsRef[1].gameObject.SetActive(true);
           // OpenLogInPanel();
            return;
        }*/

        Ludo_UIManager.instance.playOnline.OpenWindow(num);
        //if (Ludo_UIManager.instance.assetOfGame.SavedLoginData.isFirstDeposit)
        //{
        //    Ludo_UIManager.instance.playOnline.OpenWindow(num);
        //}
        //else
        //{
        //    Ludo_UIManager.instance.storeScreen.Open();
        //}
        this.Close();
    }
    public void OpenPrivate()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }

        Ludo_UIManager.instance.messagePanel.DisplayMessage("Coming Soon !");
        return;

        // Ludo_UIManager.instance.joinRoom.Open();
        // this.Close();
    }
    public void OpenTournament()
    {
        Debug.Log("Tournament button click ");
        //Ludo_UIManager.instance.messagePanel.DisplayMessage("Coming Soon ! ");
        //return;
        //anderson
    //    Ludo_UIManager.instance.tournamentList.Open();
        this.Close();
    }
    public void LocalMultiPlayer()
    {
        // Check if the player is registered
       /* if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }*/

        //Ludo_UIManager.instance.messagePanel.DisplayMessage("Coming Soon ! ");
        //return;
        Ludo_UIManager.instance.localGamePlay.Open();
        this.Close();
    }
    public void OpenWithComputer()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }

        Ludo_UIManager.instance.playWithComputer.Open();
        this.Close();
    }

    #region Winner List Methods    

    public void StartScrollingWinnerList()
    {/*
        // Store temp index        
        float Time = 1f;
        float ScrollingStepUnit = 100f;
        float FinalXPosition = (WinnerListTextRef.rectTransform.rect.width) * -1f;
        //Debug.Log("Final X pos : " + FinalXPosition);
        float EstimatedTime = 0f;

        // Calculate Time
        EstimatedTime = (WinnerListTextRef.rectTransform.rect.width * Time) / ScrollingStepUnit;

        // Kill And reset Tween
        if (tweenRef != null)
        {
            tweenRef.OnKill(() =>
            {
                // Set Position of the Text
                WinnerListTextRef.rectTransform.anchoredPosition = new Vector2(ParentRectRef.rect.width, WinnerListTextRef.rectTransform.anchoredPosition.y);
                tweenRef = null;
            });
            tweenRef.Kill(false);
        }

        // Check if not active in scene
        if (WinnerListTextRef.gameObject.activeInHierarchy == false)
        {
            WinnerListTextRef.gameObject.SetActive(true);
        }

        // Start Scrolling List        
        tweenRef = WinnerListTextRef.rectTransform.DOAnchorPos(new Vector2(FinalXPosition, WinnerListTextRef.rectTransform.anchoredPosition.y), EstimatedTime)
        .SetEase(Ease.Linear)
        .SetDelay(0.5f)
        .OnComplete(() =>
        {
            // Reset Winner Scroll Postions            
            if (RequestNewWinnerListData == false)
            {
                RequestNewWinnerListData = true;
                GetWinnerPlayers();
            }
            else
            {
                // Reset Tween
                ResetWinnerListTween();
            }
        });*/
    }

    public void NextWinnerListCall()
    {
        // Set false
        RequestNewWinnerListData = false;
        CallAndResetWinnerScroll();
    }

    public void CallAndResetWinnerScroll()
    {
        // Request For New data check
        if (RequestNewWinnerListData == false)
        {
            RequestNewWinnerListData = true;
            GetWinnerPlayers();
        }
    }

    void ResetWinnerListTween()
    {
        // Kill And reset Tween
        if (tweenRef != null)
        {
            tweenRef.OnKill(() =>
            {
                // Set Position of the Text
               // WinnerListTextRef.rectTransform.anchoredPosition = new Vector2(ParentRectRef.rect.width, WinnerListTextRef.rectTransform.anchoredPosition.y);
                tweenRef = null;

                // Start Scrolling Text
                StartScrollingWinnerList();
            });
            tweenRef.Kill(false);
        }
        else
        {
            // Start Scrolling Text
            StartScrollingWinnerList();
        }
    }

    #endregion

    #region Is Player Registered Methods

    [Space]
    public bool isWelcomePanelOpen = false;
    public void OpenLogInPanel()
    {
        
        if (!isWelcomePanelOpen)
        {
           
    //  Ludo_UIManager.instance.welcomePopup.SetVerifyTextAndStarButtontAnimation();
            isWelcomePanelOpen = true;
        }
    }

    #endregion

    #endregion

    #region Coroutine

    private IEnumerator RefreshTableOnInterval()
    {
        while (true)
        {
            if (gameObject.activeSelf)
            {
                yield return new WaitForSeconds(Ludo_Constants.Ludo.RefreshHomeOnlineInterval);
                RefreshTable();
            }
            //if (TournamentDetailsScreen.gameObject.activeSelf)
            //{
            //    StopCoroutine("RefreshTableOnInterval");
            //}
        }
    }

    #endregion

    #region GETTER_SETTER

    /* public string Chips
     {
         set
         {
             txtChips.text = value;
         }
     }*/

    public double Chips
    {
        set
        {
            // txtChips.text = value.ToString();
            Debug.Log("chips " + Math.Round(value, 2).ToString());
          //  txtChips.text = Math.Round(value, 2).ToString();
        }
    }

    public int twoPlayersValue
    {
        get
        {
            return _twoPlayersValue;
        }
        set
        {
            //print("TotalTablePotAmount:" + value);
            _twoPlayersValue = value;
            twoPlayers.gameObject.SetActive(value > 0);
            twoPlayers.text = "Players: " + value.ToString();
          //  twoPlayers.GetComponent<LocalizationParamsManager>().SetParameterValue("Value", value.ToString());
        }
    }

    public int threePlayersValue
    {
        get
        {
            return _threePlayersValue;
        }
        set
        {
            _threePlayersValue = value;
            threePlayers.gameObject.SetActive(value > 0);
            threePlayers.text = "Players: " + value.ToString();
          //  threePlayers.GetComponent<LocalizationParamsManager>().SetParameterValue("Value", value.ToString());
        }
    }

    public int fourPlayersValue
    {
        get
        {
            return _fourPlayersValue;
        }
        set
        {
            _fourPlayersValue = value;
            fourPlayers.gameObject.SetActive(value > 0);
            fourPlayers.text = "Players: " + value.ToString();
           // fourPlayers.GetComponent<LocalizationParamsManager>().SetParameterValue("Value", value.ToString());
        }
    }

    public int Totalplayers
    {
        get
        {
            return _TotalplayersValue;
        }
        set
        {
            //print("TotalTablePotAmount:" + value);
            _TotalplayersValue = value;
            //allPlayers.gameObject.SetActive(value > 0);
            //allPlayers.text = "Players Online : " + value.ToString();
        }
    }

    #endregion

    #region Share Referral Code


    #region Copy To ClipBoard

   // public TMP_Text ReferralCodeTextRef;

    /// <summary>
    /// Puts the string into the Clipboard.
    /// </summary>
    public void CopyToClipboard(string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }

    public void CopyReferalCode()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }
/*
        if (ReferralCodeTextRef.text != "")
        {
            // Copy Text to clipboard
            CopyToClipboard(ReferralCodeTextRef.text);
        }
        else
        {
            Debug.Log("Refrral Code Empty");
        }*/
    }

    #endregion

    #endregion

    #region Support Screen

    public void OpenSupportScreen()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }

        // Check if support screen is closed
        if (SupportScreenStatus == false)
        {
            // Set support status true
            SupportScreenStatus = true;

            // Set Button Interactable false
     //       SupportButtonRef.interactable = false;

            
        }
    }

  

    #endregion

    #region Block List

    public void OpenBlockList()
    {
        // Check if the player is registered
        if (GameStaticData.isRegisteredPlayer == false)
        {
            OpenLogInPanel();
            return;
        }

        // Check if support screen is closed
        if (BlockListScreenStatus == false)
        {
            // Set support status true
            BlockListScreenStatus = true;

            // Set Button Interactable false
         //   BlockListButtonRef.interactable = false;

            // Get Block List Data
            GetBlockListData();
        }
    }

    public void GetBlockListData()
    {
        // Close Home Screen and open support screen
        Ludo_UIManager.instance.homeScreen.Close();
       // Ludo_UIManager.instance.UserBlockScreen.Open();

        // Set support status true
        BlockListScreenStatus = false;

        // Set Button Interactable false
     //   BlockListButtonRef.interactable = true;
    }

    #endregion

}

[System.Serializable]
public class ShareAndEarnText
{
    public string resultText = string.Empty;
}