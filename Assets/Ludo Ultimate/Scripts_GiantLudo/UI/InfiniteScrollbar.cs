using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
//using DanielLochner.Assets.SimpleScrollSnap;
using System.IO;
using UnityEngine.Networking;

public class InfiniteScrollbar : MonoBehaviour
{
    #region Variables
  //  [SerializeField] private SimpleScrollSnap _scrollSnap;
    [SerializeField] private List<ScrollableContain> contains;

    public RectTransform ContainerRect;
    public GameObject ContainerObj;
    public BannerDataResult _data;

    private bool isBannerLoaded = false;
    private int bannerLoadCount;
    private int totalBanner;
    #endregion

    #region Unity Default Methods
    void Awake()
    {
        GetAndSetBannerData();
    }

    void Start()
    {
    }

    void OnEnable()
    {
        if (isBannerLoaded)
        {
            if (IsInvoking(nameof(AutoScrolling)))
                CancelInvoke(nameof(AutoScrolling));

            Invoke(nameof(AutoScrolling), _data.SliderTime);
        }
    }

    void OnDisable()
    {
        if (IsInvoking(nameof(AutoScrolling)))
            CancelInvoke(nameof(AutoScrolling));
    }
    #endregion

    #region Server Details
    private void GetAndSetBannerData()
    {
       /* Ludo_UIManager.instance.socketManager.GetBannerList((socket, packet, args) =>
        {
            Debug.Log("GetBannerList  : " + packet.ToString());
            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventResponse<BannerDataResult> BannerDataResp = JsonUtility.FromJson<PokerEventResponse<BannerDataResult>>(resp);

            if (BannerDataResp.status.Equals(LudoConstants.LudoAPI.KeyStatusSuccess))
            {
                _scrollSnap.enabled = true;
                _data = BannerDataResp.result;
                totalBanner = _data.bannerData.Count;
                // Debug.LogError($"_data == {totalBanner}");

                for (int i = 0; i < totalBanner; i++)
                {
                    ScrollableContain bannerObj = new ScrollableContain();

                    GameObject obj = Instantiate<GameObject>(ContainerObj, ContainerRect.transform);
                    obj.transform.localRotation = Quaternion.identity;

                    bannerObj.URL_Redirect = _data.bannerData[i].url;
                    bannerObj.Img_Contain = obj.GetComponent<Image>();
                    string bannerImgName = GetImageName(_data.bannerData[i].image);

                    if (File.Exists(Application.persistentDataPath + "/" + bannerImgName) && Directory.Exists(Path.GetDirectoryName(Application.persistentDataPath + "/" + bannerImgName)))
                    {
                        bannerObj.Img_Contain.sprite = LudoUtilityManager.Instance.GetExistFile(Application.persistentDataPath + "/" + bannerImgName);
                        bannerObj.Img_Contain.sprite.name = bannerImgName;
                        isBannerLoaded = true;
                    }
                    else
                    {
                        // Updating banner download process to main process.
                        // UtilityManager.Instance.DownloadBannerImage(_data.bannerData[i].image, bannerObj.Img_Contain, true);
                        StartCoroutine(GetBannerFromServer(_data.bannerData[i].image, bannerImgName, bannerObj.Img_Contain, true));
                    }

                    if (bannerObj.URL_Redirect == "support")
                    {
                        obj.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            // Open Support screen
                            Ludo_UIManager.instance.homeScreen.OpenSupportScreen();
                        });
                    }
                    else
                    {
                        obj.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            OnBannerTap(bannerObj.URL_Redirect);
                        });
                    }
                    
                    contains.Add(bannerObj);
                }



                if (isBannerLoaded)
                {
                    if (IsInvoking(nameof(AutoScrolling)))
                        CancelInvoke(nameof(AutoScrolling));

                    Invoke(nameof(AutoScrolling), _data.SliderTime);
                }
            }
            else
            {
                Ludo_UIManager.instance.messagePanel.DisplayMessage(BannerDataResp.message);
            }
        });*/
    }

    private string GetImageName(string val)
    {
        String[] parts = val.Split("/"[0]);
        string[] name = parts[parts.Length - 1].Split("."[0]);
        return name[0];
    }

    private IEnumerator GetBannerFromServer(string url, string bannerImgName, Image img_Contain, bool displayLoader)
    {
        Debug.Log("Getting home banner from server.. ");
        GameObject loader = null;
        if (displayLoader)
        {
            // Instantiate the loader.
            loader = Instantiate(Ludo_UIManager.instance.downloadImageLoaderPrefab) as GameObject;

            // Make child of the Image.
            loader.transform.SetParent(img_Contain.transform, false);
            loader.transform.localPosition = Vector3.zero;
        }

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(new Uri(url));
        yield return www.SendWebRequest();

        //if (www.isNetworkError || www.isHttpError)
        if (www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            img_Contain.sprite = Ludo_UIManager.instance.assetOfGame.spDefaultImage;
        }
        else
        {
            string savePath = Application.persistentDataPath + "/" + bannerImgName;
            Texture2D texture2D = ((DownloadHandlerTexture)www.downloadHandler).texture;

            LudoUtilityManager.Instance.saveImage(savePath, texture2D.EncodeToPNG());

            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            img_Contain.sprite = sprite;
            bannerLoadCount++;
        }
        Destroy(loader);

        // Debug.LogError("bannerLoadCount = " + bannerLoadCount + " =totalBanner= " + totalBanner);

        if (bannerLoadCount == totalBanner)
        {
            isBannerLoaded = true;

            if (IsInvoking(nameof(AutoScrolling)))
                CancelInvoke(nameof(AutoScrolling));

            Invoke(nameof(AutoScrolling), _data.SliderTime);
        }

        Resources.UnloadUnusedAssets();
    }

    private void AutoScrolling()
    {
       // _scrollSnap.GoToNextPanel();
        if (IsInvoking(nameof(AutoScrolling)))
            CancelInvoke(nameof(AutoScrolling));

        Invoke(nameof(AutoScrolling), _data.SliderTime);
    }
    #endregion

    #region Custom Button Events

    public void OnBannerTap(string bannerURL)
    {
        if (isBannerLoaded)
            Application.OpenURL(bannerURL);
    }

    public void OnPointerDown()
    {
        if (IsInvoking(nameof(AutoScrolling)))
            CancelInvoke(nameof(AutoScrolling));
    }

    public void OnPointerUp()
    {
        if (IsInvoking(nameof(AutoScrolling)))
            CancelInvoke(nameof(AutoScrolling));

        Invoke(nameof(AutoScrolling), _data.SliderTime);
    }
    #endregion

}


#region Custom Class

[System.Serializable]
public class ScrollableContain
{
    public string URL_Redirect;
    public Image Img_Contain;
}

#endregion

