using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

public class WinnerScreen : MonoBehaviour
{
    #region PUBLIC_VARIABLES
    [Header("Images")]
    public Image Banner;
    public Image LosePlayer;

    [Header("Text")]
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI Playermobile;
    public TextMeshProUGUI PlayerWon;
    public TextMeshProUGUI commision;

    [Space]
    [Space]
    // public GamePlayScreen gamePlayScreen;
    public Transform miniBoard;
    public Transform miniBoardParent;

    public GameObject ShareButtonPanelObj;
    public GameObject WinPanelObj;
    public RectTransform ContainerObj;

    #endregion

    #region PRIVATE_VARIABLES
    [Space]
    [SerializeField] private GameObject btnShare;
    [SerializeField] private GameObject referAndEarnTxtObj;
    [SerializeField] private string ShareText = string.Empty;

    private string sharingImagePath;
    private bool isWalletEnable;
    private float normalHeight = 650f, shortPopupHeight = 530f;
    #endregion

    #region UNITY_CALLBACKS

    // Use this for initialization
    void OnEnable()
    {
        StartCoroutine(bakctoHome());
    }

    void OnDisable()
    {
        StopCoroutine(bakctoHome());
        Reset();
        Ludo_UIManager.instance.gamePlayScreen.WinningParticle.SetActive(false);
    }

    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS
    public void ShareButtonTap()
    {
        //  Stephen ScreenShot sharing Process

#if UNITY_WEBGL //|| UNITY_EDITOR

        //JSON_Object jsonObj = new JSON_Object();
        //jsonObj.put("playerId", UIManager.Instance.assetOfGame.SavedLoginData.PlayerId);
        //jsonObj.put("clubId", ID);
        //jsonObj.put("type", "club");
        //jsonObj.put("code", clubCodeData);
        //jsonObj.put("url", Constants.PokerAPI.BaseUrl);
        //Debug.Log("code Event: " + jsonObj.toString());
        //ExternalCallClass.Instance.codeSharingMethod(jsonObj.toString());
#else
        //StartCoroutine(stopFalse());
        // StartCoroutine(TakeSSAndShare());

        string details = string.Empty;

        if (string.IsNullOrEmpty(ShareText))
            details = "I Won " + PlayerWon.text + " From Ludo Gaint, Use My Code " + Ludo_UIManager.instance.assetOfGame.SavedLoginData.refferal_code
                              + " For Joining Bonus. Download App Click :- " + LudoUtilityManager.Instance.GetAppStoreLink();
        else
            details = ShareText + "\n\nDownload App Click Now👇\n" + LudoUtilityManager.Instance.GetAppStoreLink();

        //NativeShare nativeShare = new NativeShare().SetText(details);

      /*  if (sharingImagePath != "") nativeShare.AddFile(sharingImagePath);
        nativeShare.Share();*/

        // new NativeShare().AddFile(sharingImagePath).SetText(details)
        //                .SetCallback((result, shareTarget) => Debug.LogError("Share result: " + result + ", selected app: " + shareTarget))
        //                .Share();
#endif
    }

    public void SetData(Winners WinData, string shareMessage)
    {
        Reset();
        PlayerName.text = WinData.playerName;
        Playermobile.text = WinData.mobile;
        PlayerWon.text = WinData.amount + "";
        commision.text = WinData.commission + "";
        ShareText = shareMessage;

        isWalletEnable = Ludo_UIManager.instance.assetOfGame.SavedLoginData.isWalletEnable;

        if (WinData.playerId.Equals(ServerSocketManager.instance.playerId))
        {
            // btnShare.SetActive(true && isWalletEnable);
            referAndEarnTxtObj.SetActive(true && isWalletEnable);
            Banner.Open();
            LosePlayer.Close();
            WinPanelObj.SetActive(true);
            ContainerObj.sizeDelta = new Vector2(ContainerObj.sizeDelta.x, normalHeight);
            Ludo_UIManager.instance.gamePlayScreen.WinningParticle.SetActive(false);
            Ludo_UIManager.instance.gamePlayScreen.WinningParticle.SetActive(true);
        }
        else
        {
            // Debug.LogError("Lost...");
            btnShare.SetActive(false);
            referAndEarnTxtObj.SetActive(false);
            Banner.Close();
            LosePlayer.Open();
            WinPanelObj.SetActive(false);
            ContainerObj.sizeDelta = new Vector2(ContainerObj.sizeDelta.x, shortPopupHeight);
        }
        this.Open();

        // Stephen Code Update
        miniBoard.SetParent(miniBoardParent);
        miniBoard.GetComponent<CanvasGroup>().alpha = 1;

        LoadGameBoard();
    }

    public void LoadGameBoard()
    {
        //   Capturing screenshot for sharing
        ShareButtonPanelObj.SetActive(false);
        string imageName = "share.png";

        if (File.Exists(Application.persistentDataPath + "/" + imageName))
            File.Delete(Application.persistentDataPath + "/" + imageName);

        sharingImagePath = Path.Combine(Application.persistentDataPath, imageName);
        ScreenCapture.CaptureScreenshot(imageName);

        Invoke(nameof(ActivateSharing), 0.25f);
    }

    private void ActivateSharing()
    {
        ShareButtonPanelObj.SetActive(true);
    }

    public void PlayAgain()
    {
        Ludo_UIManager.instance.socketManager.PlayAgain(Ludo_UIManager.instance.gamePlayScreen.currentRoomData.tableId, (socket, packet, args) =>
        {
            Debug.Log("PlayAgain  : " + packet.ToString());

            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventResponse<PlayAgainData> PlayAgainDataResp = JsonUtility.FromJson<PokerEventResponse<PlayAgainData>>(resp);

            if (PlayAgainDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                RejoinGame(PlayAgainDataResp.result.tableId, PlayAgainDataResp.result.userSelectGoti, PlayAgainDataResp.result.entryFee.ToString(), PlayAgainDataResp.result.priceAmount.ToString());
            }
            else
            {
                Ludo_UIManager.instance.messagePanel.DisplayMessage(PlayAgainDataResp.message, () =>
                {
                    Ludo_UIManager.instance.gamePlayScreen.LeaveRoomDone();
                });
            }
        });
    }
    #endregion

    #region PRIVATE_METHODS
    private void RejoinGame(string tableId, string userSelectGoti, string entryFee, string priceAmount)
    {
        this.Close();
        Ludo_UIManager.instance.gamePlayScreen.Close();
        Ludo_UIManager.instance.PlayerSearchPanel.SetDataAndOpen(tableId, userSelectGoti);
        Ludo_UIManager.instance.PlayerSearchPanel.SetBidAndPriceText(entryFee, priceAmount);
        //Ludo_UIManager.instance.socketManager.JoinRoom(tableId, userSelectGoti, GetplayGameResponse);
    }
    private void GetplayGameResponse(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("GetJoinRoomResponse  : " + packet.ToString());

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
    private void Reset()
    {
        PlayerName.text = "";
        Playermobile.text = "";
        PlayerWon.text = "";
        commision.text = "";
        LosePlayer.Close();
        Banner.Close();
    }
    #endregion

    #region COROUTINES
    public IEnumerator bakctoHome()
    {
        yield return new WaitForSeconds(5f);
        if (gameObject.activeInHierarchy)
        {
            Debug.Log("Leave Room And Get Back to Home");
            Ludo_UIManager.instance.gamePlayScreen.LeaveRoomDone();
        }
    }
    private IEnumerator TakeSSAndShare()
    {
        // Debug.LogError("Capturing Screen ..");
        yield return new WaitForEndOfFrame();
        // yield return new WaitForSeconds(1f);
        // string details = "I Won " + PlayerWon.text + " From Ludo Gaint, Use My Code " + Ludo_UIManager.instance.assetOfGame.SavedLoginData.refferal_code + " For Joining Bonus. Download App Click :- " + UtilityManager.Instance.GetAppStoreLink();
        // string sharingImagePath = UtilityManager.Instance.GetSharingImageFilePath();
        // NativeShare nativeShare = new NativeShare().SetText(details);

        // Debug.LogError("sharingImagePath.. " + sharingImagePath);
        // if (sharingImagePath != "")
        // {
        //     nativeShare.AddFile(sharingImagePath);
        // }

        // nativeShare.Share();
    }

    IEnumerator stopFalse()
    {
        Ludo_UIManager.instance.isUploadImage = true;
        yield return new WaitForSeconds(2f);
        Ludo_UIManager.instance.isUploadImage = false;
    }
    #endregion


    #region GETTER_SETTER


    #endregion



}
