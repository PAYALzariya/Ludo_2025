using System;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using UnityEngine;
using UnityEngine.UI;

public class Ludo_UIManager : MonoBehaviour
{

    #region Public_Variabels
    public static Ludo_UIManager instance;
    public SERVER server = SERVER.Production;
    //public GameObject demoScreen;
    public bool isUploadImage;
    [Header("Game Asset")]
    public AssetOfGame assetOfGame;

    [Space]
      public Loader loader;
    public Settings settings;
    public JoinRoom joinRoom;
    public GameRules gameRules;
    public PrivacyPolicy policy;
  
    public LudoHomeScreen homeScreen;
    public PlayOnline playOnline;
    public CreateRoom createRoom;
    public ShopScreen shopScreen;
    public TermsAndConditions tnc;
    public MessagePanel messagePanel;
  //  public SplashScreen splashScreen;
    public GameController LocalGameScreen;
     public PrizeDistribution distribution;
    public LudoGame_SocketManager socketManager;
    public LocalGamePlayPanel localGamePlay;
    public GamePlayScreen gamePlayScreen;
    public GamePlayMiniboardScreen miniBoardGamePlayScreen;
    public PlayWithComputer playWithComputer;
    public StartGameTimer WaitForGameScreen;
    public searchingPanel PlayerSearchPanel;
    public MaintenancePanel maintenancePanel;
    //private UniWebView uniWebViewobj;
    public LudoSoundManager soundManager;
    public Panel_LanguageSelection languageSelection;

 

    [Space]
    [Space]
    public List<LudoEmoticans> LudoEmoticansManager;

    public bool IscheatEnable = false;
    public bool isLogAllEnabled = false;
    public bool modeClassic = true;

    public GameObject downloadImageLoaderPrefab;
    [Space]
    public Transform canvas;
    public GameObject backgroundImage;
    [Header("Header")]
    public Reporter reporter;

    public PokerEventResponse responseRegisteration;
    public Toggle classicToggle;
    public Toggle quickToggle;
    

    private string selectedMode = "";
    #endregion

    #region Private_Variables

    public bool isLogOut = false;
    private int _profilePic;
    private string _uploadimageProfilePic;
    private string _username;

    #endregion

    #region Unity_Callback

    void Awake()
    {
        //demoScreen.SetActive(false);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        SaveLoad.LoadGame();
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = false;
        Application.runInBackground = true;
        // Game.Lobby.ConnectToSocket();

        Reset(true);
        classicToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetMode("Classic"); });
        quickToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetMode("quick"); });

    }
    void SetMode(string mode)
    {
        selectedMode = mode;
        Debug.Log("Selected Mode: " + selectedMode);
    }

    public string GetSelectedMode()
    {
        if (classicToggle.isOn)
        {
            selectedMode = "Classic";
        }
        else
        {
            selectedMode = "quick";
        }
        Debug.Log("Selected Mode: " + selectedMode);
        return selectedMode;
    }

    #endregion

    #region Private_Methods

    public void Reset(bool isFirstTime)
    {
        foreach (Transform panels in canvas.transform)
        {
            if (panels.gameObject.activeInHierarchy)
            {
                panels.gameObject.SetActive(false);
            }
        }

      
            homeScreen.Open();

        canvas.gameObject.SetActive(true);
        backgroundImage.SetActive(true);
        isLogOut = false;
    }

    private void maxPlayerReachedResponse(Socket s, Packet packet, object[] obj)
    {

        Debug.Log("maxPlayerReachedResponse Broadcast: " + packet.ToString());
        //StartTimerData response = JsonUtility.FromJson<StartTimerData>(packet.GetPacketString());
        JSONArray arr = new JSONArray(packet.ToString());
        string message = arr.getJSONObject(1).getString("message");
        Ludo_UIManager.instance.messagePanel.DisplayMessage(message);
    }

    private void tourStartTimerResponse(Socket s, Packet packet, object[] obj)
    {
        Debug.Log("tourStartTimerResponse Broadcast: " + packet.ToString());
        StartTimerData response = JsonUtility.FromJson<StartTimerData>(packet.GetPacketString());
        if (gamePlayScreen.gameObject.activeInHierarchy)
        {
            gamePlayScreen.LeaveRoomDoneforTournament();
        }
       

    }

    private void ForceLogoutUser(Socket s, Packet packet, object[] obj)
    {
        Debug.Log($"Force Logout Broadcast: {packet}");

        assetOfGame.SavedLoginData = null;
      /*  PlayerPrefs.DeleteKey(Ludo_Constants.LudoConstants.PlayerName);
        PlayerPrefs.DeleteKey(Ludo_Constants.LudoConstants.PlayerNumber);
        PlayerPrefs.DeleteKey(Ludo_Constants.LudoConstants.PlayerEmail);*/
        Reset(false);

        messagePanel.DisplayMessage("You are block by admin. Please contact to admin.");
    }

    public void OnConnected(bool b)
    {
        if (b)
        {
            ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.TourStartTimer, tourStartTimerResponse);
            ServerSocketManager.instance.rootSocket.On("test", tourStartTimerResponse);
            ServerSocketManager.instance.rootSocket.On(Ludo_Constants.LudoEvents.MaxPlayerReached, maxPlayerReachedResponse);
            SubscribeEvents();
        }
        else
        {
            ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.TourStartTimer, tourStartTimerResponse);
            ServerSocketManager.instance.rootSocket.Off(Ludo_Constants.LudoEvents.MaxPlayerReached, maxPlayerReachedResponse);
            ServerSocketManager.instance.rootSocket.Off("test", tourStartTimerResponse);
            UnSubscribe();
        }
    }

    private void SubscribeEvents()
    {
        ServerSocketManager.instance.rootSocket.On("forceLogout", ForceLogoutUser);
    }

    private void UnSubscribe()
    {
        ServerSocketManager.instance.rootSocket.Off("forceLogout", ForceLogoutUser);
    }

    #endregion

    #region  Public_Methods

    public void OpenLoader(bool b, string message = "")
    {
        loader.gameObject.SetActive(b);
        loader.Message.text = message;
    }

    #endregion

    #region Getter_Setter

    public string Username
    {
        get
        {
            return _username;
        }
        set
        {
            _username = value;

            assetOfGame.SavedLoginData.name = _username;
          //  homeScreen.playerInfo.playerName.text = _username;
            
            //GameSelectionScreen.profilePic.sprite = assetOfGame.profileAvatarList.profileAvatarSprite[_profilePic];
        }
    }

    public Sprite uploadimageProfilePic
    {
        get
        {
            return Ludo_UIManager.instance.assetOfGame.spDefaultImage;
        }
        set
        {
            //_uploadimageProfilePic = value;

         //   homeScreen.playerInfo.profileImage.sprite = value;
            PlayerSearchPanel.UserImage.sprite = value;
            PlayerSearchPanel.luckyDrawPanelForThree.UserImage.sprite = value;
            PlayerSearchPanel.luckyDrawPanelForFour.UserImage.sprite = value;
            //UtilityManager.Instance.DownloadImage(_uploadimageProfilePic, homeScreen.playerInfo.profileImage, true);
            //UtilityManager.Instance.DownloadImage(_uploadimageProfilePic, profileScreen.ProfilePic, true);
            //UtilityManager.Instance.DownloadImage(_uploadimageProfilePic, tournamentList.playerInfo.profileImage, true);
            //UtilityManager.Instance.DownloadImage(_uploadimageProfilePic, PlayerSearchPanel.UserImage, true);
            //UtilityManager.Instance.DownloadImage(_uploadimageProfilePic, PlayerSearchPanel.luckyDrawPanelForThree.UserImage, true);
            //UtilityManager.Instance.DownloadImage(_uploadimageProfilePic, PlayerSearchPanel.luckyDrawPanelForFour.UserImage, true);

        }
    }

    public Sprite uploadBanner
    {
        get
        {
            return Ludo_UIManager.instance.assetOfGame.spDefaultBannerImage;
        }
        set
        {
          /*  leaderBoard.lastMonth.BannerImg.sprite = value;
            leaderBoard.currentMonth.BannerImg.sprite = value;
            *///   playerInfo.profileImage.sprite = value;
            //   profileScreen.ProfilePic.sprite = value;


        }
    }

    public int ProfilePic
    {
        get
        {
            return _profilePic;
        }
        set
        {
            _profilePic = value;

            assetOfGame.SavedLoginData.avatar = _profilePic;
           // homeScreen.playerInfo.profileImage.sprite = assetOfGame.profileAvatarList.profileAvatarSprite[_profilePic];
            PlayerSearchPanel.UserImage.sprite = assetOfGame.profileAvatarList.profileAvatarSprite[_profilePic];
            PlayerSearchPanel.luckyDrawPanelForThree.UserImage.sprite = assetOfGame.profileAvatarList.profileAvatarSprite[_profilePic];
            PlayerSearchPanel.luckyDrawPanelForFour.UserImage.sprite = assetOfGame.profileAvatarList.profileAvatarSprite[_profilePic];

            //GameSelectionScreen.profilePic.sprite = assetOfGame.profileAvatarList.profileAvatarSprite[_profilePic];
        }
    }

    #endregion

}

[Serializable]
public class LudoEmoticans
{
    public int emoticanId;
    public Sprite emodefault;
    public List<Sprite> emoSprites;
}