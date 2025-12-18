using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if GOOGLE_SIGNIN
using Google;
#endif

public class Settings : MonoBehaviour
{
    #region Public_Variables
    [Header("Images")]
    public Image Music;
    public Image SoundButton;
    public Image VibrationButton;


    #endregion

    #region Private_Variables
    [Header("GameObject")]
    [SerializeField] private GameObject gameObjectShare;
    #endregion

    #region  Unity_Callback
    private void OnEnable()
    {
        RefreshButtonPreventionFunction();
    }

    private void RefreshButtonPreventionFunction()
    {
        bool isWalletEnable = Ludo_UIManager.instance.assetOfGame.SavedLoginData.isWalletEnable;

        // gameObjectShare.SetActive(isWalletEnable);
    }
    #endregion

    #region Private_Methods
    void musicOnOffBool(int OnOff)
    {
        if (OnOff.Equals(0))
        {
            Music.Close();
        }
        else
        {
            Music.Open();
        }

    }
    void SoundOnOffBool(int OnOff)
    {
        if (OnOff.Equals(0))
        {
            SoundButton.Close();
        }
        else
        {
            SoundButton.Open();
        }

    }

    void VibrationOnOffBool(int OnOff)
    {
        if (OnOff.Equals(0))
        {
            VibrationButton.Close();
        }
        else
        {
            VibrationButton.Open();
        }

    }
    #endregion

    #region Public_Methods
    public void ClosePanel()
    {
        Ludo_UIManager.instance.homeScreen.Open();
        this.Close();
    }

    public void SetData()
    {
        SoundOnOffBool(PlayerPrefs.GetInt("Sound"));
        VibrationOnOffBool(PlayerPrefs.GetInt("Vibration"));
        musicOnOffBool(PlayerPrefs.GetInt("bgMusic"));
        this.Open();
    }
    public void MusicButtonTap()
    {

        if (PlayerPrefs.GetInt("bgMusic") == 1)
        {
            PlayerPrefs.SetInt("bgMusic", 0);
            Ludo_UIManager.instance.soundManager.stopBgSound();
        }
        else
        {
            PlayerPrefs.SetInt("bgMusic", 1);
            Ludo_UIManager.instance.soundManager.PlayBgSound();
        }
        Debug.Log("bgMusic = >" + PlayerPrefs.GetInt("bgMusic"));
        musicOnOffBool(PlayerPrefs.GetInt("bgMusic"));
    }
    public void SoundButtonTap()
    {

        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            PlayerPrefs.SetInt("Sound", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Sound", 1);
        }
        Debug.Log("Sound = >" + PlayerPrefs.GetInt("Sound"));
        SoundOnOffBool(PlayerPrefs.GetInt("Sound"));
    }
    public void VibrationButtonTap()
    {

        if (PlayerPrefs.GetInt("Vibration") == 1)
        {
            PlayerPrefs.SetInt("Vibration", 0);

        }
        else
        {
            PlayerPrefs.SetInt("Vibration", 1);
        }
        Debug.Log("Vibration = >" + PlayerPrefs.GetInt("Vibration"));
        VibrationOnOffBool(PlayerPrefs.GetInt("Vibration"));
    }
    public void ShareButtonTap()
    {

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
        //StartCoroutine(TakeSSAndShare());
        //string details = "Dear Friend, Use My Code " + Ludo_UIManager.instance.assetOfGame.SavedLoginData.refferal_code +
        //    "  & Get 10 Joining Bonus For Real Game Play, Download Ludo Gaint Mobile App Click On Link - " + UtilityManager.Instance.GetAppStoreLink()
        //    + " Reffer & Earn " + Ludo_UIManager.instance.assetOfGame.SavedLoginData.refer
        //    + "% Life Time Commission On Game Played.";

        //string details = s1 + " " + LudoConstants.LudoAPI.BaseUrl;

        string sharingImagePath = LudoUtilityManager.Instance.GetSharingImageFilePath();
//        NativeShare nativeShare = new NativeShare().SetText(LudoUtilityManager.Instance.GetNormalWinningShareText(Ludo_UIManager.instance.assetOfGame.SavedLoginData.win));
       // if (sharingImagePath != "") nativeShare.AddFile(sharingImagePath);
      //  nativeShare.Share();
#endif

    }
    public void OpenGameRules()
    {
        Ludo_UIManager.instance.gameRules.Open();
    }
    public void OpenLanguageSelection()
    {
        this.Close();
        Ludo_UIManager.instance.languageSelection.Open();
    }
    public void OpenPrivacyPolicy()
    {
        this.Close();
        Ludo_UIManager.instance.homeScreen.Close();
        Ludo_UIManager.instance.policy.Open();
    }

    public void OpenTNC()
    {
        this.Close();
        Ludo_UIManager.instance.homeScreen.Close();
        Ludo_UIManager.instance.tnc.Open();
    }

    public void Logout()
    {
        this.Close();
       // Game.Lobby.socketManager.Close();
        Ludo_UIManager.instance.homeScreen.Close();
     //   Game.Lobby.socketManager.Open();
    //    Game.Lobby.ConnectToSocket();
        Ludo_UIManager.instance.ProfilePic = 0;
       /* PlayerPrefs.DeleteKey(Ludo_Constants.LudoConstants.PlayerName);
        PlayerPrefs.DeleteKey(Ludo_Constants.LudoConstants.PlayerNumber);
        PlayerPrefs.DeleteKey(Ludo_Constants.LudoConstants.PlayerEmail);
        //Ludo_UIManager.instance.loginScreen.Open();
        PlayerPrefs.SetInt(Ludo_Constants.LudoConstants.UserLogin, Ludo_Constants.LudoConstants.loggedOut);*/

#if GOOGLE_SIGNIN

        GoogleSignIn.DefaultInstance.SignOut();
        if (UIManager.instance.assetOfGame.SavedLoginData.isGmailLogin)
        {
            UIManager.instance.loginScreen.OnSignOut();
            GoogleSignIn.DefaultInstance.SignOut();
            if (GoogleSignIn.DefaultInstance != null)
            {
                GoogleSignIn.DefaultInstance.SignOut();
            }
        }

#endif
        Ludo_UIManager.instance.assetOfGame.SavedLoginData.name = "";
    }
    #endregion

    #region Coroutine
    private IEnumerator TakeSSAndShare()
    {
        StartCoroutine(stopFalse());
        yield return new WaitForEndOfFrame();
        //yield return new WaitForSeconds(0.2f);

        //Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //ss.Apply();

        /*	Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
          texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
          texture.LoadRawTextureData(texture.GetRawTextureData());
          texture.Apply();
          //sendTexture(texture, messageToSend);

          string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
          File.WriteAllBytes(filePath, texture.EncodeToPNG());*/

        string details = "Dear Friend, Use My Code " + Ludo_UIManager.instance.assetOfGame.SavedLoginData.refferal_code +
            "  & Get 10 Joining Bonus For Real Game Play, Download Ludo Gaint Mobile App Click On Link - " + LudoUtilityManager.Instance.GetAppStoreLink()
            + " Reffer & Earn " + Ludo_UIManager.instance.assetOfGame.SavedLoginData.refer
            + "% Life Time Commission On Game Played.";

        //string details = s1 + " " + LudoConstants.LudoAPI.BaseUrl;

        //new NativeShare().AddFile("").SetSubject(Application.productName).SetText(details).Share();
    }


    IEnumerator stopFalse()
    {
        Ludo_UIManager.instance.isUploadImage = true;
        yield return new WaitForSeconds(2f);
        Ludo_UIManager.instance.isUploadImage = false;
    }
    #endregion
}
