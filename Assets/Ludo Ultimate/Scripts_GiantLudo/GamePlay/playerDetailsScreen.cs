using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerDetailsScreen : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    [Header("Transforms")]
    public Transform emoParent;

    [Header("ScriptableObjects")]
    public EmojiAnimation emojisDisplayObj;

    //[Header ("DropDowns")]


    [Header("Images")]
    public Image playerProfilePicture;

    [Header("Text")]
    public TextMeshProUGUI username;
    public Text txtEarning;
    public Text txtwinRate;
    public Text txtprivate;
    public Text txtwoPlayers;
    public Text txthreePlayers;
    public Text txtFourPlayers;

    //[Header ("Prefabs")]

    //[Header ("Enums")]
    public string id;
    public int EmojiId;
    public int avatar;
    public string profilePic;
    public List<EmojiAnimation> emotionList;
    //[Header ("Variables")]

    #endregion

    #region PRIVATE_VARIABLES
    [SerializeField] private ScrollRect scrollRectEmoji;
    #endregion

    #region UNITY_CALLBACKS
    // Use this for initialization

    private void Awake()
    {
        Reset();
        SetEmoticans();
    }

    void OnEnable()
    {        
        EmojiId = 0;
        scrollRectEmoji.verticalNormalizedPosition = 1;
    }
    void OnDisable()
    {
        //Reset();
    }

    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS
    public void CloseButtonTap()
    {
        this.Close();
    }
    public void setDataAndOpen(string playerId,int avtar,string profile)
    {
        resetData();

        Ludo_UIManager.instance.socketManager.playerProfileInfo(playerId, Ludo_UIManager.instance.gamePlayScreen.currentRoomData.boardId,
       (socket, packet, args) =>
       {
           Debug.Log(Ludo_Constants.LudoEvents.PlayerProfileInfo + " respnose  : " + packet.ToString());
           Ludo_UIManager.instance.OpenLoader(false);
           JSONArray arr = new JSONArray(packet.ToString());
           string Source;
           Source = arr.getString(arr.length() - 1);
           var resp = Source;

           PokerEventResponse<PlayerProfileInfo> playerProfileInfo = JsonUtility.FromJson<PokerEventResponse<PlayerProfileInfo>>(resp);

           if (playerProfileInfo.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
           {
               this.id = playerId;
               this.profilePic = profile;
               this.avatar = avtar;
               if (profilePic != null && !profilePic.Equals("default.png") && profilePic != "")
               {
                   string ImageUrl = profilePic;
                   LudoUtilityManager.Instance.DownloadImage(ImageUrl, playerProfilePicture, true, true);
               }
               else
               {
                   playerProfilePicture.sprite = Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[avatar];
               }
               username.text = playerProfileInfo.result.username.ToString();
               txtEarning.text= playerProfileInfo.result.totalEarn.ToString();
               txtwinRate.text = playerProfileInfo.result.winRate.ToString();
               txtprivate.text = playerProfileInfo.result.privateWin.ToString();
               txtwoPlayers.text = playerProfileInfo.result.twoPlayerWin.ToString();
               txthreePlayers.text = playerProfileInfo.result.threePlayerWin.ToString();
               txtFourPlayers.text = playerProfileInfo.result.fourPlayerWin.ToString();
               this.id= playerId;

           }
           else
           {
               Ludo_UIManager.instance.messagePanel.DisplayMessage(playerProfileInfo.message);
           }
       });

        this.Open();
    }

    public void OpenPanel(string playerId, PlayerProfileInfo playerInformation, Sprite playerSprite)
    {
        this.id = playerId;
        playerProfilePicture.sprite = playerSprite;
        username.text = playerInformation.username.ToString();
        txtEarning.text = playerInformation.totalEarn.ToString();
        txtwinRate.text = playerInformation.winRate.ToString();
        txtprivate.text = playerInformation.privateWin.ToString();
        txtwoPlayers.text = playerInformation.twoPlayerWin.ToString();
        txthreePlayers.text = playerInformation.threePlayerWin.ToString();
        txtFourPlayers.text = playerInformation.fourPlayerWin.ToString();
        this.Open();
    }

    public void SendButtonTap()
    {
        Ludo_UIManager.instance.socketManager.sendEmoji(ServerSocketManager.instance.playerId, this.id,this.EmojiId, Ludo_UIManager.instance.gamePlayScreen.currentRoomData.boardId,
        (socket, packet, args) =>
        {
            Debug.Log(Ludo_Constants.LudoEvents.SendEmoji + " respnose  : " + packet.ToString());
            Ludo_UIManager.instance.OpenLoader(false);
            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
        
            PokerEventResponse<PlayerProfileInfo> playerProfileInfo = JsonUtility.FromJson<PokerEventResponse<PlayerProfileInfo>>(resp);
        
            if (playerProfileInfo.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                this.Close();
            }
            else
            {
                Ludo_UIManager.instance.messagePanel.DisplayMessage(playerProfileInfo.message);
            }
        });

    }
    #endregion

    #region PRIVATE_METHODS

    void SetEmoticans()
    {        
        for (int i = 0; i < Ludo_UIManager.instance.LudoEmoticansManager.Count; i++)
        {
            EmojiAnimation EmoObj = Instantiate(emojisDisplayObj) as EmojiAnimation;
            //EmoObj.setDatAndOpen(Ludo_UIManager.instance.LudoEmoticansManager[i].emoSprites,
            //    Ludo_UIManager.instance.LudoEmoticansManager[i].emoticanId, 0.25f);
            EmoObj.setDatAndOpen(Ludo_UIManager.instance.LudoEmoticansManager[i].emoSprites,
                Ludo_UIManager.instance.LudoEmoticansManager[i].emoticanId, 1f);
            EmoObj.transform.SetParent(emoParent, false);
            emotionList.Add(EmoObj);
        }
        DeselectEmoji();
        emotionList[0].selectimg.Open();
        
    }
    private void Reset()
    {
        foreach (Transform tr in emoParent)
        {
            Destroy(tr.gameObject);
        }
        emotionList.Clear();
    }
    public void DeselectEmoji()
    {
        for (int i = 0; i < emotionList.Count; i++)
        {
            emotionList[i].selectimg.Close();
        }
    }
    private void resetData()
    {
        txtEarning.text = "";
        txtwinRate.text = "";
        txtprivate.text = "";
        txtwoPlayers.text = "";
        txthreePlayers.text = "";
        txtFourPlayers.text = "";
    }
    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER


    #endregion



}
