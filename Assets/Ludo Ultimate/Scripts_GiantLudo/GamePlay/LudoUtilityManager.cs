using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using BestHTTP.SocketIO;
//using UnityEngine.Purchasing;

public class LudoUtilityManager : MonoBehaviour
{
    #region PUBLIC_VARIABLES

    public static LudoUtilityManager Instance;
  
    [Header("States Options")]
    public List<string> states = new List<string>();

    #endregion

    #region PRIVATE_VARIABLES
    [SerializeField] private Texture2D texture2dSharingImage = null;
    private string filePathOfTexture2dSharingImage = "";
    #endregion

    #region DELEGATE_CALLBACKS

    void Awake()
    {
        Instance = this;
    }
    #endregion

    #region PUBLIC_METHODS

    public void MoveObject(Transform obj, Vector3 fromPos, Vector3 toPos, float time)
    {
        StartCoroutine(MoveObjectSmoothly(obj, fromPos, toPos, time));
    }

    public void MoveObject(Transform obj, Vector3 toPos, float time)
    {
        StartCoroutine(MoveObjectSmoothly(obj, obj.position, toPos, time));
    }

    public void ChangeScale(Transform obj, Vector3 fromScale, Vector3 toScale, float time)
    {
        StartCoroutine(ChangeScaleAnimation(obj, fromScale, toScale, time));
    }

    public void ScaleObject(Transform obj)
    {
        //StartCoroutine(ScaleObjectSmoothly(obj));
    }
    public void RotateObject(Transform obj, Vector3 fromRotation, Vector3 toRotation, float time)
    {
        StartCoroutine(RotateObjectSmoothly(obj, fromRotation, toRotation, time));
    }
/*
    #region Unity IAP callbacks
    public void OnIAPSuccess(int coinsToAdd, string receipt, string transactionID)
    {
        StartCoroutine(CallPurchaseEventAfterReconnection(coinsToAdd, receipt, transactionID));
    }

    private IEnumerator CallPurchaseEventAfterReconnection(int coinsToAdd, string receipt, string trxnID)
    {
        yield return new WaitForSeconds(1);
        while (Game.Lobby.socketManager.State != BestHTTP.SocketIO.SocketManager.States.Open)
        {
            Debug.LogError($"In While {Game.Lobby.socketManager.State.ToString()} ");
            yield return new WaitForSeconds(1);
        }

        Debug.Log($"After a while...");
        Ludo_UIManager.instance.OpenLoader(false);
        Ludo_UIManager.instance.socketManager.AuthenticatePlaystorePurchase(coinsToAdd, receipt, trxnID, ResponseOfInApp);
    }

    private void ResponseOfInApp(Socket socket, Packet packet, object[] args)
    {
        Debug.Log($"ResponseOfInApp Response: {packet}");
        InAppResponse<InAppResult> response = JsonUtility.FromJson<InAppResponse<InAppResult>>(packet.GetPacketString());

        if (response.status == InAppResponse.STATUS_SUCCESS)
        {
            Ludo_UIManager.instance.messagePanel.DisplayMessage(response.message);
            return;
        }
        else
        {
            Ludo_UIManager.instance.messagePanel.DisplayMessage(response.message);
            return;
        }

    }
    #endregion

*/


    public Sprite GetExistFile(string savePath)
    {
        byte[] imageBytes = loadImage(savePath);

        Texture2D texture;
        texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        return sprite;
    }
    public string DecimalFormat(double myNumber)
    {
        var s = string.Format("{0:0.00}", myNumber);

        if (s.EndsWith("00"))
        {
            return ((int)myNumber).ToString();
        }
        else
        {
            return s;
        }
    }

    public void DownloadImageNormal(string url, Image imgSource, bool displayLoader = true)
    {
        StartCoroutine(DownloadImageIEnum(url, imgSource, displayLoader));
    }

    public void DownloadImage(string url, Image imgSource, bool displayLoader = true, bool dontUpdateProfile = false)
    {
        StartCoroutine(GetImage(url, imgSource, displayLoader, dontUpdateProfile));
    }

    public void DownloadBannerImage(string url, Image imgSource, bool displayLoader = true)
    {
      //  StartCoroutine(GetBannerImage(url, imgSource, displayLoader));
    }

    public byte[] loadImage(string path)
    {
        byte[] dataByte = null;

        //Exit if Directory or File does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            //Debug.LogWarning("Directory does not exist");
            return null;
        }

        if (!File.Exists(path))
        {
            //Debug.Log("File does not exist");
            return null;
        }

        try
        {
            dataByte = File.ReadAllBytes(path);
            //Debug.Log("Loaded Data from: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Load Data from: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }

        return dataByte;
    }

    public string GetSharingImageFilePath()
    {
        if (filePathOfTexture2dSharingImage == "")
        {
            string imageName = "ludogaint-share.png";
            if (File.Exists(Application.temporaryCachePath + "/" + imageName))
                File.Delete(Application.temporaryCachePath + "/" + imageName);

            filePathOfTexture2dSharingImage = Path.Combine(Application.temporaryCachePath, imageName);
            File.WriteAllBytes(filePathOfTexture2dSharingImage, texture2dSharingImage.EncodeToPNG());
        }
        return filePathOfTexture2dSharingImage;
    }

    private IEnumerator GetImage(string url, string _id, Image imgSource, bool displayLoader)
    {
        GameObject loader = null;

        if (displayLoader)
        {
            // Instantiate the loader.
            loader = Instantiate(Ludo_UIManager.instance.downloadImageLoaderPrefab) as GameObject;

            // Make child of the Image.
            loader.transform.SetParent(imgSource.transform, false);
            loader.transform.localPosition = Vector3.zero;
        }

        string savePath = Application.persistentDataPath + "/" + _id;
        if (File.Exists(savePath) && Directory.Exists(Path.GetDirectoryName(savePath)))
        {
            if (imgSource.sprite.name.Equals(_id))
                yield break;

            byte[] imageBytes = loadImage(savePath);

            Texture2D texture;
            texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            sprite.name = _id;
            imgSource.sprite = sprite;
        }
        else
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(new Uri(url));

            yield return www.SendWebRequest();

            //if (www.isNetworkError || www.isHttpError)
            if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                imgSource.sprite = Ludo_UIManager.instance.assetOfGame.spDefaultBannerImage;
            }
            else
            {
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                saveImage(savePath, myTexture.EncodeToPNG());

                Sprite sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.zero);
                imgSource.sprite = sprite;
            }
        }

        //	Destroys the loader object on process complete.
        Destroy(loader);

    }
    public void saveImage(string path, byte[] imageBytes)
    {
        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        try
        {
            File.WriteAllBytes(path, imageBytes);
            //Debug.Log("Saved Data to: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }


    public string GetPacketString(BestHTTP.SocketIO.Packet packet)
    {
        JSONArray arr = new JSONArray(packet.ToString());
        if (arr.length() >= 2)
        {
            return arr.getString(1);
        }
        return arr.length() > 0 ? arr.getString(0) : "";// (arr.length() - 1);
    }
    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
    public void UpdateHorizontalLayout(GameObject gameObject)
    {
        StartCoroutine(UpdateHorizontalLayoutIenum(gameObject));
    }

    IEnumerator UpdateHorizontalLayoutIenum(GameObject gameObject)
    {
        yield return new WaitForEndOfFrame();
        try
        {
            ContentSizeFitter contentSizeFitter = gameObject.transform.parent.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter)
            {
                StartCoroutine(UpdateHorizontalLayoutEnumerator(contentSizeFitter));
            }
        }
        catch (Exception e)
        {
            print(e);
        }
    }
    private IEnumerator UpdateHorizontalLayoutEnumerator(ContentSizeFitter contentSizeFitter)
    {
        yield return new WaitForEndOfFrame();
        try
        {
            Canvas.ForceUpdateCanvases();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            if (contentSizeFitter)
                contentSizeFitter.SetLayoutHorizontal();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)contentSizeFitter.transform);
            Canvas.ForceUpdateCanvases();
        }
        catch (Exception e)
        {
            Debug.Log("print => " + e.ToString());
        }
    }


    /*
    public void DistributeCard(Transform obj, Vector3 fromPos, Vector3 toPos, float time, Action action)
    {
        StartCoroutine(DistributeCardAnimation(obj, fromPos, toPos, time, action));
    }*/



    public string GetHandRank(string handRank)
    {
        return AddSpacesToSentence(handRank, true).ToUpper();
    }

    public string GetDecimalStringValue(double value)
    {
        return (Math.Round(value, 3)).ToString();
    }

    public float GetDecimalFloatValue(float value)
    {
        float ff;
        ff = Mathf.Round(value * 100) / 100;
        return ff;
    }

    public string GetDecimalzeroStringValue(double value)
    {
        return (Math.Round(value, 0)).ToString();
    }

    public string GetActionName(int actionNumber)
    {
        string actionString = "";

        if (actionNumber == 0)
        {
            actionString = "Small Blind";
        }
        else if (actionNumber == 1)
        {
            actionString = "Big Blind";
        }
        else if (actionNumber == 2)
        {
            actionString = "Check";
        }
        else if (actionNumber == 3)
        {
            actionString = "Bet";
        }
        else if (actionNumber == 4)
        {
            actionString = "Call";
        }
        else if (actionNumber == 5)
        {
            actionString = "";
        }
        else if (actionNumber == 6)
        {
            actionString = "Fold";
        }
        else if (actionNumber == 7)
        {
            actionString = "Timeout";
        }
        else if (actionNumber == 8)
        {
            actionString = "All-in";
        }
        else if (actionNumber == 9)
        {
            actionString = "Straddle";
        }
        else
        {
            actionString = "";
        }

        return actionString;
    }

    public string GetShortenName(string name)
    {
        if (name.Length > 8)
        {
            return name.Substring(0, 8) + "..";
        }
        else
        {
            return name;
        }
    }

    public string GetOSName()
    {
#if UNITY_ANDROID
        return "android";
#elif UNITY_IOS
		return "ios";
#else
        return "other";
#endif
    }

    public string GetApplicationVersion()
    {
        return Application.version;
    }

    public string GetApplicationVersionWithOS()
    {
#if UNITY_EDITOR
        return "v" + Application.version + "u";
#elif UNITY_ANDROID
		return "v" + Application.version + "a";	
#elif UNITY_IOS
		return "v" + Application.version + "i";	
#else
        return "";
#endif
    }
    /*
    public Sprite GetAvatarById(int index)
    {
        if (index >= Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite.Length)
            return Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[0];
        else
            return Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[index];
    }*/

    public GameStartDataResp GetNewRoomObjectClone(GameStartDataResp roomObject)
    {
        GameStartDataResp newCloneObject = new GameStartDataResp();
        newCloneObject.boardId = roomObject.boardId;
        newCloneObject.nameSpace = roomObject.nameSpace;
        newCloneObject.tableId = roomObject.tableId;
        newCloneObject.uniqueId = roomObject.uniqueId;


        return newCloneObject;
    }

    public void OpenLink(string url)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
			//ExternalCallClass.Instance.OpenUrl(url);
			//Application.ExternalEval("window.open(\"" + url + "\",\"_blank\")");
#else
        Application.OpenURL(url);
#endif
    }

    public void ClearLoginData()
    {
        PlayerPrefs.SetString("USERNAME", "");
        PlayerPrefs.SetString("PASSWORD", "");
        PlayerPrefs.SetInt("REMEMBER_ME", 0);
        PlayerPrefs.SetInt("AutoLogin", 0);

    }

    public string GetAppStoreLink()
    {
#if UNITY_IOS
        return Ludo_Constants.StoreLinks.appleStoreAppLink;
#else        
        return Ludo_Constants.StoreLinks.playStoreAppLink;
#endif
    }

    public string GetNormalWinningShareText(double winningAmount = 0)
    {
        //return "Dear Friend, Use My Code " + Ludo_UIManager.instance.assetOfGame.SavedLoginData.refferal_code +
        //    "  & Get 10 Joining Bonus For Real Game Play, Download Ludo Gaint Mobile App Click On Link - " + Ludo_UIManager.instance.assetOfGame.SavedLoginData.updatedAPKLink
        //    + " Reffer & Earn " + Ludo_UIManager.instance.assetOfGame.SavedLoginData.refer
        //    + "% Life Time Commission On Game Played.";

        if (winningAmount > 0)
            return "I Win Rs. " + winningAmount + " On Ludo Gaint, Use My Code :-  (" + Ludo_UIManager.instance.assetOfGame.SavedLoginData.refferal_code + ") & Get 10 Joining Bonus For Real Money Game Play,\n\nDownload App Click Now👇\n" + Ludo_UIManager.instance.assetOfGame.SavedLoginData.updatedAPKLink;
        else
            return "Dear Friend, Use My Code :-  (" + Ludo_UIManager.instance.assetOfGame.SavedLoginData.refferal_code + ") & Get 10 Joining Bonus For Real Money Game Play,\n\nDownload App Click Now👇\n" + Ludo_UIManager.instance.assetOfGame.SavedLoginData.updatedAPKLink;
    }
    #endregion

    #region PRIVATE_METHODS

  

    private string AddSpacesToSentence(string text, bool preserveAcronyms)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
                if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                    (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                    i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }

    #endregion

    #region COROUTINES

    private IEnumerator MoveObjectSmoothly(Transform obj, Vector3 fromPos, Vector3 toPos, float time)
    {
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * (1 / time);
            if (obj != null && obj.gameObject.activeInHierarchy)
            {
                obj.position = Vector3.Lerp(fromPos, toPos, i);
            }
            yield return 0;
        }
    }

    private IEnumerator ChangeScaleAnimation(Transform obj, Vector3 fromScale, Vector3 toScale, float time)
    {
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * (1 / time);
            if (obj != null && obj.gameObject.activeInHierarchy)
            {
                obj.localScale = Vector3.Lerp(fromScale, toScale, i);
            }
            yield return 0;
        }
    }

    private IEnumerator ScaleObjectSmoothly(Transform obj, bool animate)
    {
        float i = 0;


        while (i < 1)
        {
            i += Time.deltaTime * (1 / 0.2f);
            if (obj != null && obj.gameObject.activeInHierarchy)
            {
                obj.position = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, i);
            }
            yield return 0;
        }
    }
    private IEnumerator RotateObjectSmoothly(Transform obj, Vector3 fromRotation, Vector3 toRotation, float time)
    {
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * (1 / time);
            obj.eulerAngles = Vector3.Lerp(fromRotation, toRotation, i);
            yield return 0;
        }

        obj.eulerAngles = toRotation;
    }

    private IEnumerator DownloadImageIEnum(string url, Image imgSource, bool displayLoader)
    {

        print("url => " + url);
        GameObject loader = null;
        if (displayLoader)
        {

            // Instantiate the loader.
            loader = Instantiate(Ludo_UIManager.instance.downloadImageLoaderPrefab) as GameObject;

            // Make child of the Image.
            loader.transform.SetParent(imgSource.transform, false);
            loader.transform.localPosition = Vector3.zero;
        }

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(new Uri(url));
        yield return www.SendWebRequest();

        //if (www.isNetworkError || www.isHttpError)
        if (www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            if (imgSource)
                imgSource.sprite = Ludo_UIManager.instance.assetOfGame.spDefaultImage;
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            Sprite mySprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.zero);
            if (imgSource)
                imgSource.sprite = mySprite;
        }


        //	Destroys the loader object on process complete.
        if (loader)
            Destroy(loader);
    }

    private IEnumerator GetImage(string url, Image imgSource, bool displayLoader, bool dontUpdateProfile)
    {
        print("url => " + url);
        GameObject loader = null;
        if (displayLoader)
        {

            // Instantiate the loader.
            loader = Instantiate(Ludo_UIManager.instance.downloadImageLoaderPrefab) as GameObject;

            // Make child of the Image.
            loader.transform.SetParent(imgSource.transform, false);
            loader.transform.localPosition = Vector3.zero;
        }

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(new Uri(url));
        yield return www.SendWebRequest();

        //if (www.isNetworkError || www.isHttpError)
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            imgSource.sprite = Ludo_UIManager.instance.assetOfGame.spDefaultImage;
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite mySprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.zero);
            imgSource.sprite = mySprite;

            if (dontUpdateProfile == false)
            {
                Ludo_UIManager.instance.uploadimageProfilePic = mySprite;
            }
        }


        //	Destroys the loader object on process complete.
        Destroy(loader);

        /*  UnityWebRequest www = new UnityWebRequest(url);

          yield return www;

        if (www.error == null && www.texture != null)
         {
             Sprite sp = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
             imgSource.sprite = sp;
         }
         else
         {
             //	If there is error in retrieving the image, display default sprite.

         }*/
    }

  

    internal string ConvertToCommaSeparatedValue(double amount)
    {
        throw new NotImplementedException();
    }

    internal string ConvertDoubleDecimals(double amount)
    {
        throw new NotImplementedException();
    }

    /*  private IEnumerator DistributeCardAnimation(Transform obj, Vector3 fromPos, Vector3 toPos, float time, Action action)
      {
          float i = 0;
          while (i < 1)
          {
              i += Time.deltaTime * (1 / time);
              obj.position = Vector3.Lerp(fromPos, toPos, i);

              //			obj.localScale = Vector3.Lerp (Vector3.zero, Vector3.one, i);
              yield return 0;
          }
          action();
      }
      */
    #endregion

    #region IAPCalls

    //public void IAPCall (Product product, decimal price, int chips)
    //{
    //	StartCoroutine (IAPCallIEnum (product, price, chips));
    //}

    //private IEnumerator IAPCallIEnum (Product product, decimal price, int chips)
    //{
    //	yield return new WaitForSeconds (1);
    //	while (Game.Lobby.socketManager.State != BestHTTP.SocketIO.SocketManager.States.Open) {
    //		print ("Game.Lobby.socketManager.State: " + Game.Lobby.socketManager.State.ToString ());
    //		yield return new WaitForSeconds (1);
    //	}
    //	Ludo_UIManager.Instance.SocketGameManager.InAppPurchaseSuccess (product, price, chips, (socket, packet, args) => {
    //		Debug.Log ("!!!!!!!!packet result....... " + packet.ToString ());
    //		JSONArray arr = new JSONArray (packet.ToString ());
    //		string Source;
    //		Source = arr.getString (arr.length () - 1);
    //		var resp1 = Source;
    //		PokerEventResponse<IAPGettingResponseData> iapGettingResponseData = JsonUtility.FromJson<PokerEventResponse<IAPGettingResponseData>> (resp1);
    //		Ludo_UIManager.Instance.assetOfGame.SavedLoginData.chips = iapGettingResponseData.result.totalChips;
    //		Ludo_UIManager.Instance.LobbyScreeen.txtChips.text = Ludo_UIManager.Instance.assetOfGame.SavedLoginData.chips.ConvertToCommaSeparatedValueColor ();
    //		Ludo_UIManager.Instance.LobbyScreeen.PanelMyAccount.ProfilePanel.Chips = iapGettingResponseData.result.totalChips;
    //		Ludo_UIManager.Instance.DisplayMessagePanel (iapGettingResponseData.message, () => {
    //			Ludo_UIManager.Instance.HidePopup ();
    //		});
    //	});
    //}

    #endregion

    #region GETTER_SETTER

    public List<string> States
    {
        get
        {
            return states;
        }
        set
        {
            states = value;
        }
    }

    #endregion
}

public static class MyExtension
{
    /// <summary>
    /// Convert string to camel case.
    /// </summary>
    /// <returns>The camel case string.</returns>
    /// <param name="str">String.</param>
    public static string ToPascalCase(this string str)
    {
        string[] words = str.Split(' ');
        string newString = "";

        foreach (string s in words)
        {
            newString += s.ToCharArray()[0].ToString().ToUpper() + s.Substring(1) + " ";
        }
        return newString;
    }

    /// <summary>
    /// Converts amount to comma separated value.
    /// </summary>
    /// <returns>The to comma separated value.</returns>
    /// <param name="amount">Amount.</param>
    public static string ConvertToCommaSeparatedValue(this long amount)
    {
        return amount.ToString("#,##0");
    }
    public static string ConvertToCommaSeparatedValueColor(this double value)
    {
        string amt = value.ToString("###,###,##0.00");

        string[] splitString = amt.Split('.');
        string newString = splitString[0];

        if (splitString.Length > 1)
        {
            newString += "<color=\"yellow\">." + splitString[1] + "</color>";
            //newString += "<color=#FF000A>.<size=80%>"+ splitString[1] + "</size></color>"; //color code example
        }
        return newString;
    }
    /// <summary>
    /// Open the specified component.
    /// </summary>
    /// <param name="component">Component.</param>
    public static void Open(this MonoBehaviour component)
    {
        if (component.gameObject != null)
            component.gameObject.SetActive(true);
    }

    /// <summary>
    /// Close the specified component.
    /// </summary>
    /// <param name="component">Component.</param>
    public static void Close(this MonoBehaviour component)
    {
        if (component.gameObject != null)
            component.gameObject.SetActive(false);
    }

    /// <summary>
    /// Convert string to json array to packet json string
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public static string JSONString(this Packet packet)
    {
        JSONArray arr = new JSONArray(packet.ToString());
        return arr.getString(arr.length() - 1);
    }

    /// <summary>
    /// Converts to comma separated value.
    /// </summary>
    /// <returns>The to comma separated value.</returns>
    /// <param name="value">Value.</param>
    public static string ConvertToCommaSeparatedValue(double value)
    {
        string amt = value.ToString("###,###,##0.00");
        //amt = "$" + amt;
        return amt;//.Replace (',', '.');
    }

    public static string ConvertToCommaSeparatedValueBuyIn(this long value)
    {
        string amt = value.ToString("###,##0.00");
        //		amt =  amt;
        return amt;//.Replace (',', '.');
    }

    public static string ConvertToCommaSeparatedValue(this int value)
    {
        string amt = value.ToString("###,###,###");
        //amt = "$" + amt;
        return amt;//.Replace (',', '.');
    }

    public static string ConvertToCommaSeparatedValue(this float value)
    {
        string amt = value.ToString("###,###,##0.00");
        return amt;//.Replace (',', '.');
    }

    public static string ConvertDoubleDecimalsToValue(this double value)
    {
        string amt = value.ToString("###,###,##0.00");
        return amt;//.Replace (',', '.');
    }

    public static string ConvertFloatDecimalsToValue(this float value)
    {
        string amt = value.ToString("###,###,##0.00");
        return amt;//.Replace (',', '.');
    }

    public static Double ConvertDoubleDecimals(this double value)
    {
        //		double amt = value.ToString ("0.00");
        double amt = Math.Round(value * 100.0) / 100.0;
        return amt;//.Replace (',', '.');
    }

    public static string ConvertUserNameToStarString(this string value)
    {
        /*for (int i = 1; i < value.Length - 1; i++) {	
			value = value.Remove (i, 1);
			value = value.Insert (i, "*");
		}*/
        return value;
    }

    public static string GetDateFromString(this string value)
    {
        double temp = double.Parse(value);

        System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        dateTime = dateTime.AddMilliseconds(temp);

        //		return dateTime.ToString ("yyyy/M/d");
        return dateTime.ToString("dd/MM");
    }

    public static string GetTimeFromString(this string value)
    {
        double temp = double.Parse(value);

        System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        dateTime = dateTime.AddMilliseconds(temp);

        return dateTime.ToString("hh:mm");
    }

    public static string UTCTimeStringToLocalTime(this string value)
    {
        DateTime utcDateTime = DateTime.Parse(value);
        return utcDateTime.ToLocalTime().ToString();
    }

    /// <summary>
    /// Rounds to.
    /// </summary>
    /// <returns>The to.</returns>
    /// <param name="value">Value.</param>
    /// <param name="roundToValue">Round to value.</param>
    public static float RoundTo(this float value, float roundToValue)
    {
        return (float)Math.Round(value / roundToValue) * roundToValue;
    }

    /// <summary>
    /// Rounds to.
    /// </summary>
    /// <returns>The to.</returns>
    /// <param name="value">Value.</param>
    /// <param name="roundToValue">Round to value.</param>
    public static long RoundTo(this long value, long roundToValue)
    {
        return (long)Mathf.Round(value / roundToValue) * roundToValue;
    }


    /// <summary>
    /// Converts to the enum.
    /// </summary>
    /// <returns>The enum.</returns>
    /// <param name="value">Value.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    #region Number To Short String Format

    private static readonly int charA = Convert.ToInt32('a');

    private static readonly Dictionary<int, string> units = new Dictionary<int, string> {
        { 0, "" },
        { 1, "K" },
        { 2, "M" },
        { 3, "B" },
        { 4, "T" }
    };

    public static string FormatNumberUS(this long value)
    {
        if (value < 1d)
        {
            return "0";
        }

        var n = (int)Math.Log(value, 1000);
        var m = value / Math.Pow(1000, n);
        var unit = "";

        if (n < units.Count)
        {
            unit = units[n];
        }
        else
        {
            var unitInt = n - units.Count;
            var secondUnit = unitInt % 26;
            var firstUnit = unitInt / 26;
            unit = Convert.ToChar(firstUnit + charA).ToString() + Convert.ToChar(secondUnit + charA).ToString();
        }

        // Math.Floor(m * 100) / 100) fixes rounding errors
        return (Math.Floor(m * 100) / 100)/*.ToString("0.##")*/ + unit;
    }

    public static string FormatNumberUS(this double value)
    {
        if (value < 1d)
        {
            return "0";
        }

        var n = (int)Math.Log(value, 1000);
        var m = value / Math.Pow(1000, n);
        var unit = "";

        if (n < units.Count)
        {
            unit = units[n];
        }
        else
        {
            var unitInt = n - units.Count;
            var secondUnit = unitInt % 26;
            var firstUnit = unitInt / 26;
            unit = Convert.ToChar(firstUnit + charA).ToString() + Convert.ToChar(secondUnit + charA).ToString();
        }

        // Math.Floor(m * 100) / 100) fixes rounding errors
        return (Math.Floor(m * 100) / 100)/*.ToString("0.##")*/ + unit;
    }

    public static string DoubleToFloatToString(this double value)
    {
        float newValue = (float)value;

        return Math.Round(newValue, 2).ToString();
    }

    public static double floatToDouble(this float value)
    {
        return double.Parse(value.ToString());
    }

    public static float doubleToFloat(this double value)
    {
        return float.Parse(value.ToString());
    }

    #endregion
}

#region Dynamic Transform
/* Demo Code
 _ImgTransform.SetAnchor(AnchorPresets.TopRight);
 _ImgTransform.SetAnchor(AnchorPresets.TopRight,-10,-10);
 
 ImgTransform.SetPivot(PivotPresets.TopRight);

RectTransformExtensions.SetAnchor (textProductId.GetComponent<RectTransform> (), AnchorPresets.StretchAll, 0, 0);
*/
public enum AnchorPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottonCenter,
    BottomRight,
    BottomStretch,

    VertStretchLeft,
    VertStretchRight,
    VertStretchCenter,

    HorStretchTop,
    HorStretchMiddle,
    HorStretchBottom,

    StretchAll
}

public enum PivotPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottomCenter,
    BottomRight,
}

public static class RectTransformExtensions
{
    public static void SetAnchor(this RectTransform source, AnchorPresets allign, int offsetX = 0, int offsetY = 0)
    {
        source.anchoredPosition = new Vector3(offsetX, offsetY, 0);

        switch (allign)
        {
            case (AnchorPresets.TopLeft):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.TopCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 1);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.TopRight):
                {
                    source.anchorMin = new Vector2(1, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.MiddleLeft):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(0, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0.5f);
                    source.anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleRight):
                {
                    source.anchorMin = new Vector2(1, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }

            case (AnchorPresets.BottomLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 0);
                    break;
                }
            case (AnchorPresets.BottonCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 0);
                    break;
                }
            case (AnchorPresets.BottomRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.HorStretchTop):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
            case (AnchorPresets.HorStretchMiddle):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }
            case (AnchorPresets.HorStretchBottom):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.VertStretchLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.VertStretchCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.VertStretchRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.StretchAll):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
        }
    }

    public static void SetPivot(this RectTransform source, PivotPresets preset)
    {

        switch (preset)
        {
            case (PivotPresets.TopLeft):
                {
                    source.pivot = new Vector2(0, 1);
                    break;
                }
            case (PivotPresets.TopCenter):
                {
                    source.pivot = new Vector2(0.5f, 1);
                    break;
                }
            case (PivotPresets.TopRight):
                {
                    source.pivot = new Vector2(1, 1);
                    break;
                }

            case (PivotPresets.MiddleLeft):
                {
                    source.pivot = new Vector2(0, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleCenter):
                {
                    source.pivot = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleRight):
                {
                    source.pivot = new Vector2(1, 0.5f);
                    break;
                }

            case (PivotPresets.BottomLeft):
                {
                    source.pivot = new Vector2(0, 0);
                    break;
                }
            case (PivotPresets.BottomCenter):
                {
                    source.pivot = new Vector2(0.5f, 0);
                    break;
                }
            case (PivotPresets.BottomRight):
                {
                    source.pivot = new Vector2(1, 0);
                    break;
                }
        }
    }
}
#endregion