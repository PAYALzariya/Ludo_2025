using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;
using System.IO;
//using I2.Loc;
public class lastMonthPanel : MonoBehaviour
{
    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    [Header("Transforms")]
    public Transform dataParent;
    public WinnerData rDataObj;


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    [Header("Images")]
    public Image BannerImg;
    public Image LoaderImg;
    public Text txtBanner;
    [Header("Text")]
    public TextMeshProUGUI winnerOfMonth;

    //[Header ("Prefabs")]

    //[Header ("Enums")]


    //[Header ("Variables")]
    private string bannerUrl = "";
    private bool bannerDownloaded = false;
    #endregion

    #region PRIVATE_VARIABLES
    private string previousEventResponse = "";
    #endregion

    #region UNITY_CALLBACKS

    private void Awake()
    {
        Reset();
        // LoaderImg.Open();
    }

    void OnEnable()
    {
        txtBanner.Close();
        //LoaderImg.Open();

        // Commented for static image loading
        callEvent();
        /* DownloadBannerImageIfFailed(); */
    }

    void OnDisable()
    {
        // LoaderImg.Close();
        //Reset();
    }

    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS

    public void OnTranslationReady(string Translation, string Error)
    {
        winnerOfMonth.text = Translation;
    }
    private void callEvent()
    {        
        Ludo_UIManager.instance.OpenLoader(true);
    /*    Ludo_UIManager.instance.socketManager.LeaderBoard("refer", "last", (socket, packet, args) =>
        {
            Ludo_UIManager.instance.OpenLoader(false);
            Debug.Log("LeaderBoard  : " + packet.ToString());

            if (previousEventResponse == packet.ToString())
                return;
            Reset();
            previousEventResponse = packet.ToString();
            
            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventResponse<LeaderboardResultData> GetLeaderboardResultDataResp = JsonUtility.FromJson<PokerEventResponse<LeaderboardResultData>>(resp);

            if (GetLeaderboardResultDataResp.status.Equals(LudoConstants.LudoAPI.KeyStatusSuccess))
            {
                //winnerOfMonth.text = "Winner Of Month " + GetLeaderboardResultDataResp.result.winnerOfMonth;
                string str = "Winner Of Month " + GetLeaderboardResultDataResp.result.winnerOfMonth;

                if (LocalizationManager.CurrentLanguageCode == "en")
                {
                    winnerOfMonth.text = str;
                }
                else
                {
                    GoogleTranslation.Translate(str, "en", LocalizationManager.CurrentLanguageCode, OnTranslationReady);
                }

                Reset();
                for (int i = 0; i < GetLeaderboardResultDataResp.result.list.Count; i++)
                {
                    WinnerData rDataObjPrefabData = Instantiate(rDataObj) as WinnerData;
                    rDataObjPrefabData.SetData(GetLeaderboardResultDataResp.result.list[i], i);
                    rDataObjPrefabData.transform.SetParent(dataParent, false);
                }

                *//*          //  Commented for static banner, we are not loading banner image from server now on..!! 
                    if (GetLeaderboardResultDataResp.result.banner != null && GetLeaderboardResultDataResp.result.banner != "")
                    {
                        string ImageUrl = LudoConstants.LudoConstants.GetBaseUrl + GetLeaderboardResultDataResp.result.banner;
                        bannerUrl = ImageUrl;
                        txtBanner.Close();
                        string tableName = GetImageName(ImageUrl);

                        if (File.Exists(Application.persistentDataPath + "/" + tableName) && Directory.Exists(Path.GetDirectoryName(Application.persistentDataPath + "/" + tableName)))
                        {
                            if (BannerImg.sprite != null && BannerImg.sprite.name.Equals(tableName))
                            {
                                LoaderImg.Close();
                                return;
                            }
                            BannerImg.Open();
                            BannerImg.sprite = UtilityManager.Instance.GetExistFile(Application.persistentDataPath + "/" + tableName);
                            BannerImg.sprite.name = tableName;
                            LoaderImg.Close();
                        }
                        else
                            StartCoroutine(GetIconFromServer(ImageUrl, tableName));
                    }
                    else
                    {
                        LoaderImg.Close();
                        BannerImg.Close();
                        txtBanner.Open();
                    }
                *//*
            }
            else
            {
                Ludo_UIManager.instance.messagePanel.DisplayMessage(GetLeaderboardResultDataResp.message);
            }            
        });*/
    }

    

    /*
    public Sprite CreateSprite(Texture2D tex)
    {
        if (tex == null)
            return null;

        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);

    }*/


    public void Reset()
    {
        foreach (Transform item in dataParent)
        {
            Destroy(item.gameObject);
        }
    }

    #endregion

    #region PRIVATE_METHODS

    private string GetImageName(string val)
    {
        String[] parts = val.Split("/"[0]);
        string[] name = parts[parts.Length - 1].Split("."[0]);
        return name[0];
    }

    private void DownloadBannerImageIfFailed()
    {
        if (bannerUrl.Length > 0)
        {
            string tableName = GetImageName(bannerUrl);

            if (File.Exists(Application.persistentDataPath + "/" + tableName) && Directory.Exists(Path.GetDirectoryName(Application.persistentDataPath + "/" + tableName)))
            {
                if (BannerImg.sprite != null && BannerImg.sprite.name.Equals(tableName))
                {
                    LoaderImg.Close();
                    return;
                }
                BannerImg.Open();
                BannerImg.sprite = LudoUtilityManager.Instance.GetExistFile(Application.persistentDataPath + "/" + tableName);
                BannerImg.sprite.name = tableName;
                LoaderImg.Close();
            }
            else
                StartCoroutine(GetIconFromServer(bannerUrl, tableName));
        }
    }
    #endregion

    #region COROUTINES

    public IEnumerator GetIconFromServer(string url, string id)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(new Uri(url));

        yield return www.SendWebRequest();

        //if (www.isNetworkError || www.isHttpError)
        if (www.result == UnityWebRequest.Result.ProtocolError)
        {
            BannerImg.sprite = Ludo_UIManager.instance.assetOfGame.spDefaultBannerImage;
            BannerImg.Close();
            txtBanner.Open();
        }
        else
        {
            BannerImg.Open();
            string savePath = Application.persistentDataPath + "/" + id;

            Texture2D texture2D = ((DownloadHandlerTexture)www.downloadHandler).texture;

            LudoUtilityManager.Instance.saveImage(savePath, texture2D.EncodeToPNG());

            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);

            BannerImg.sprite = sprite;
            bannerDownloaded = true;

        }
        LoaderImg.Close();
        Debug.Log("close");

        Resources.UnloadUnusedAssets();
    }

    #endregion


    #region GETTER_SETTER


    #endregion
}
