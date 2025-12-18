using BestHTTP.SecureProtocol.Org.BouncyCastle.Bcpg;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistrationManager : MonoBehaviour
{
    public static RegistrationManager instance = null;

    public GameObject registrationScreen, loginScreen,signUpScreen, forgotPasswordScreen, resetPasswordScreen, verificationCodeScreen;
    public InputField registrationFirstrName, registrationLastName ,registrationUserName, registrationEmail, registrationPassword;
    public InputField loginUserEmail, loginPassword;
  
    public Text popUpText, wrongPasswordText, statusText;

    public string registrationType = "";

  
    [Header("Forgot Password")]
    public InputField verificationCodeInputField;
    public Button GetVerificationCode;
    
    private float timer = 1;
    private string verificationCode = string.Empty;


    private string deviceId = "";

    private void Awake()
    {
        if (null== instance)
        {
            instance = this;
        }
/*
        //Getting Android Device ID
#if UNITY_ANDROID
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
        deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");

        string dId = SystemInfo.deviceUniqueIdentifier;

        statusText.text = "Android ID: " + deviceId + " ******** Device Unique ID: " + dId;

#elif UNITY_IPHONE
        deviceId = SystemInfo.deviceUniqueIdentifier;
#endif*/
    }

    private void OnEnable()
    {
        verificationCodeInputField.interactable = true;
        popUpText.gameObject.SetActive(false);
        wrongPasswordText.gameObject.SetActive(false);
        forgotPasswordScreen.SetActive(false);
        if (DataManager.instance.IsLoggedIn == "true")
        {
            print("login done refresh token:::" + DataManager.instance.refreshToken);
            print("login done access token:::" + DataManager.instance.AccessToken);
            RefreshTokenAccess();
        }
        else
        {


            if (GlobalGameManager.instance.isLoginShow)
            {
                registrationScreen.SetActive(false);
                loginScreen.SetActive(true);
                signUpScreen.SetActive(false);
            }
            else
            {
                registrationScreen.SetActive(true);
                loginScreen.SetActive(false);
                signUpScreen.SetActive(false);
            }
        }
    }

    

/*
    private void FixedUpdate()
    {
        if (timer > 1)
        {
            verificationCodeInputField.interactable = true;
            timer -= Time.deltaTime;
            GetVerificationCode.transform.GetChild(0).GetComponent<Text>().text = "Resend After " + timer.ToString("f0") + "s";
        }
        else if (timer < 1)
        {
            GetVerificationCode.interactable = true;
            verificationCodeInputField.transform.Find("WrongVeriCode").gameObject.SetActive(false);
            GetVerificationCode.transform.GetChild(0).GetComponent<Text>().text = "Resend";
            GetVerificationCode.transform.GetChild(0).GetComponent<Text>().color = new Color32(140, 224, 240, 255);
        }

        if (verificationCodeInputField.text.Length > 0 )
        {
            forgotPasswordScreen.transform.Find("Submit").GetComponent<Button>().interactable = true;
        }
        else
        {
            forgotPasswordScreen.transform.Find("Submit").GetComponent<Button>().interactable = false;
        }
    }

*/
    public void OnValueChangedEmail()
    {
        if (forgotPasswordScreen.transform.Find("WrongEmail").gameObject.activeSelf)
        {
            forgotPasswordScreen.transform.Find("WrongEmail").GetComponent<Text>().text = "";
            forgotPasswordScreen.transform.Find("WrongEmail").gameObject.SetActive(false);
        }
    }

    public void OnClickOnButton(string eventName)
    {
       // SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "openRegistration":
                {
                    ResetSignupScreen();
                    ResetLoginScreen();
                    ResetVerifyScreen();
                    registrationScreen.SetActive(true);

                    resetPasswordScreen.SetActive(false);
                    forgotPasswordScreen.SetActive(false);
                    loginScreen.SetActive(false);
                    signUpScreen.SetActive(false);
                    verificationCodeScreen.SetActive(false);
                }
                break;
            case "openSignUp":
                {
                    ResetSignupScreen();
                    ResetLoginScreen();
                    ResetVerifyScreen();
                    signUpScreen.SetActive(true);

                    resetPasswordScreen.SetActive(false);
                    forgotPasswordScreen.SetActive(false);
                    registrationScreen.SetActive(false);
                    loginScreen.SetActive(false);
                    verificationCodeScreen.SetActive(false);
                }
                break;
            case "openLogin":
                {
                    ResetSignupScreen();
                    ResetLoginScreen();
                    ResetVerifyScreen();

                    loginScreen.SetActive(true);

                    resetPasswordScreen.SetActive(false);
                    forgotPasswordScreen.SetActive(false);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(false);
                    verificationCodeScreen.SetActive(false);
                }
                break;

            case "openVerification":
                {
                  //  ResetSignupScreen();
                  //  ResetLoginScreen();
                    ResetVerifyScreen();

                    verificationCodeScreen.SetActive(true) ;

                    loginScreen.SetActive(false);
                    resetPasswordScreen.SetActive(false);
                    forgotPasswordScreen.SetActive(false);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(false);
                }
                break;

            case "forgotpwd":
                {
                    resetPasswordScreen.SetActive(false);
                    loginScreen.SetActive(false);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(false);
                    verificationCodeScreen.SetActive(false);
                    forgotPasswordScreen.SetActive(true);
                    forgotPasswordScreen.transform.Find("Submit").GetComponent<Button>().interactable = false;
                }
                break;

            case "closeForgotPwd":
                {
                    resetPasswordScreen.SetActive(false);
                    forgotPasswordScreen.SetActive(false);
                    ResetForgotPwdScreen();
                    loginScreen.SetActive(true);
                    verificationCodeScreen.SetActive(false);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(false);
                }
                break;

            case "closeLogin":
                {
                 //   ResetSignupScreen();
                    ResetLoginScreen();
                    ResetVerifyScreen();

                    signUpScreen.SetActive(true);

                    forgotPasswordScreen.SetActive(false);
                    loginScreen.SetActive(false);
                    registrationScreen.SetActive(false);
                    resetPasswordScreen.SetActive(false);
                    verificationCodeScreen.SetActive(false);
                }
                break;

            case "closeSignup":
                {
                    ResetSignupScreen();
                    ResetLoginScreen();
                    ResetVerifyScreen();
                    registrationScreen.SetActive(true);

                    signUpScreen.SetActive(false);
                    loginScreen.SetActive(false);
                    forgotPasswordScreen.SetActive(false);
                    resetPasswordScreen.SetActive(false);
                    verificationCodeScreen.SetActive(false);
                }
                break;
            case "closeVerify":
                {
                    ResetSignupScreen();
                    ResetLoginScreen();
                    ResetVerifyScreen();
                    registrationScreen.SetActive(true);

                    signUpScreen.SetActive(false);
                    loginScreen.SetActive(false);
                    forgotPasswordScreen.SetActive(false);
                    resetPasswordScreen.SetActive(false);
                    verificationCodeScreen.SetActive(false);
                }
                break;

            case "closeResetPassword":
                {
                    forgotPasswordScreen.SetActive(false);
                    //  ResetResetPwdScreen();
                    loginScreen.SetActive(true);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(false);
                    resetPasswordScreen.SetActive(false);
                }
                break;
            case "back":
                {
                    OnClickOnBack();
                }
            break;


            case "submit":
                {
                    if (loginScreen.activeInHierarchy)
                    {
                        string error;
                        registrationType = "custom";

                        if (!UtilityGame.IsValidEmail(loginUserEmail.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            break;
                        }
                        else if (!UtilityGame.IsValidPassword(loginPassword.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));

                            break;
                        }
                        else
                        {

                            LoginUSer();
                            /*string requestData = "{\"email\":\"" + loginUserEmail.text + "\"," +
                               "\"password\":\"" + *//*tmp_loginPassword*//*loginPassword.text*//*"123456"*//* + "\"}";

                         
                            WebServices.instance.SendRequest(RequestType.Login, requestData,false, true, OnServerResponseFound);*/
                        }
                    }
                    else if (signUpScreen.activeInHierarchy)
                    {
                        string error;
                        registrationType = "custom";
                        if (!UtilityGame.IsValidUserName(registrationFirstrName.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            break;
                        }
                        else if (!UtilityGame.IsValidUserName(registrationLastName.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            break;
                        }
                        else if (!UtilityGame.IsValidUserName(registrationUserName.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            break;
                        }
                        else if (!UtilityGame.IsValidEmail(registrationEmail.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            break;
                        }
                        else if (!UtilityGame.IsValidPassword(registrationPassword.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));

                            break;
                        }

                        else
                        {

                            string requestData = "{\"email " +
                                "" + registrationEmail.text + "\"," +
                            "\"password\":\"" + registrationPassword.text + "\"," +
                              "\"username\":\"" + registrationUserName.text + "\"," +
                                "\"firstName\":\"" + registrationFirstrName.text + "\"," +
                            "\"lastName\":\"" + registrationLastName.text + "\"}";

                            WebServices.instance.SendRequest(RequestType.SendSignupOTP, requestData, false, true, OnServerResponseFound);
                        }
                    }
                   
                    else if (verificationCodeScreen.activeInHierarchy)
                    {
                        if (verificationCodeInputField.text.Length<=6)
                        {
                          
                            string requestData = "{\"email\":\"" +registrationEmail.text + "\"," +
                                
                                "\"otp\":\"" + verificationCodeInputField.text + "\"," +
                                "\"password\":\"" + registrationPassword.text + "\"," +
                                "\"username\":\"" + registrationUserName + "\"," +
                                 "\"firstName\":\"" + registrationFirstrName + "\"," +
                                "\"lastName\":\"" +registrationLastName+ "\"}";

                            WebServices.instance.SendRequest(RequestType.VerifyOTP, requestData,false, true, OnServerResponseFound);
                        }
                        else
                        {
                            verificationCodeInputField.transform.Find("WrongVeriCode").GetComponent<Text>().text = "Wrong verification code";
                            verificationCodeInputField.transform.Find("WrongVeriCode").GetComponent<Text>().color = Color.red;
                            verificationCodeInputField.transform.Find("WrongVeriCode").gameObject.SetActive(true);
                        }
                    }
                }
                  
            break;


            



            default:
            Debug.LogError("Unhandled eventName found = "+eventName);
            break;
        }
    }

  
    public void LoginWithGoogle()
    {
    //    GoogleManager.instance.SignInWithGoogle();
    }

    public void LoginWithFacebook()
    {
        //FacebookLogin.instance.FBlogin();
      //  FacebookManager.instance.SignInWithFB();
    }

    public void GetVerificationCodeOnEmail()
    {
        if (forgotPasswordScreen.transform.Find("Email").GetComponent<InputField>().text.Length > 0)
        {
            forgotPasswordScreen.transform.Find("WrongEmail").gameObject.SetActive(false);
            FetchOTPOnEmail();
        }
        else
        {
            forgotPasswordScreen.transform.Find("WrongEmail").GetComponent<Text>().text = "*Content cannot be empty";
            forgotPasswordScreen.transform.Find("WrongEmail").gameObject.SetActive(true);
        }
    }

    void FetchOTPOnEmail()
    {
        string requestData = "{\"email\":\"" + forgotPasswordScreen.transform.Find("Email").GetComponent<InputField>().text + "\"}";

       // MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
      //  WebServices.instance.SendRequest(RequestType.ForgotPassword, requestData, true, OnServerResponseFound);
    }

    private void ResetLoginScreen()
    {
        loginUserEmail.text = "";
        loginPassword.text = "";

    }
    private void ResetVerifyScreen()
    {
        /*tmp_registrationUserName*/
        verificationCodeInputField.text = "";
     
    }

    private void ResetSignupScreen()
    {
        /*tmp_registrationUserName*/
        registrationUserName.text = "";
        registrationFirstrName.text = "";
        registrationLastName.text = "";
        /*tmp_registrationPassword*/
        registrationPassword.text = "";
        /*tmp_registrationConfirmPassword*/
        registrationEmail.text = "";
    }
    private void ResetForgotPwdScreen()
    {
        timer = 1;

        verificationCodeInputField.text = "";
        verificationCodeInputField.transform.Find("WrongVeriCode").gameObject.SetActive(false);
        verificationCodeInputField.transform.Find("WrongVeriCode").GetComponent<Text>().text = "";

        GetVerificationCode.interactable = true;
        GetVerificationCode.transform.GetChild(0).GetComponent<Text>().text = "Get Verification Code";

        forgotPasswordScreen.transform.Find("Email").GetComponent<InputField>().text = "";
        forgotPasswordScreen.transform.Find("WrongEmail").GetComponent<Text>().text = "";
    }

    public Sprite EyeOff, EyeOn;
    public Image RegisterPasswordEye, LoginPasswordEye, NewPasswordEye;

  
    public void LoginEyeClick() {
        if (this./*tmp_loginPassword*/loginPassword != null)
        {
            if (this./*tmp_loginPassword*/loginPassword.contentType == /*TMP_InputField.ContentType.Password*/InputField.ContentType.Password)
            {
                LoginPasswordEye.sprite = EyeOn;
                this./*tmp_loginPassword*/loginPassword.contentType = /*TMP_InputField.ContentType.Standard*/InputField.ContentType.Standard;
            }
            else
            {
                LoginPasswordEye.sprite = EyeOff;
                this./*tmp_loginPassword*/loginPassword.contentType = /*TMP_InputField.ContentType.Password*/InputField.ContentType.Password;
            }

            this./*tmp_loginPassword*/loginPassword.ForceLabelUpdate();
        }
    }
    public void signupEyeClick()
    {
        if (this./*tmp_loginPassword*/registrationPassword != null)
        {
            if (this./*tmp_loginPassword*/registrationPassword.contentType == /*TMP_InputField.ContentType.Password*/InputField.ContentType.Password)
            {
                RegisterPasswordEye.sprite = EyeOn;
                this./*tmp_loginPassword*/registrationPassword.contentType = /*TMP_InputField.ContentType.Standard*/InputField.ContentType.Standard;
            }
            else
            {
                RegisterPasswordEye.sprite = EyeOff;
                this./*tmp_loginPassword*/registrationPassword.contentType = /*TMP_InputField.ContentType.Password*/InputField.ContentType.Password;
            }

            this./*tmp_loginPassword*/registrationPassword.ForceLabelUpdate();
        }
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage, int statusCode)
    {
        //   MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);
        Debug.Log("Reg " + errorMessage);
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
               // MainMenuController.instance.ShowMessage(errorMessage);
            }

            return;
        }

            Debug.Log($"Response for {requestType}, statusCode: {statusCode}, errorMessage: {errorMessage}");
        if (requestType == RequestType.Login)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (statusCode == 200)
            {
                // Success logic

                StartCoroutine(MsgForVideo(data["message"].ToString(), 2f));
            }
            else if (statusCode == 400)
            {
                StartCoroutine(MsgForVideo("Bad Request - check your input", 2f));
            }
            else if (statusCode == 401)
            {
                StartCoroutine(MsgForVideo("Unauthorized - check login or token", 2f));
            }
            else if (statusCode == 500)
            {
                StartCoroutine(MsgForVideo("Server error - try again later", 2f));
            }
            else
            {
                StartCoroutine(MsgForVideo("Unexpected error (" + statusCode + ")", 2f));
            }



           /* if (data["success"].ToString() == "1")
            {

                *//*string requestData = "{\"userName\":\"" + registrationUserName.text + "\"," +
                         "\"userPassword\":\"" + registrationPassword.text + "\"," +
                         "\"registrationType\":\"custom\"}";

              //  MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
                WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);
*/

                /*    ResetLoginScreen();
                    ResetSignupScreen();*//*

                //     StartCoroutine(MsgForVideo("Registered Successfully", 1.5f));

                loginScreen.SetActive(false);
                registrationScreen.SetActive(false);

            }
            else
            {

                StartCoroutine(MsgForVideo("User already exist", 1.5f));
                loginScreen.SetActive(true);
                registrationScreen.SetActive(false);
            }*/
        }
        else if (requestType == RequestType.VerifyOTP)
        {
            Debug.Log("Response => VerifyOTP: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);
            if (statusCode == 200)
            {
                OnClickOnButton("openLogin");
                // Success logic
                StartCoroutine(MsgForVideo(data["message"].ToString(), 4f));
                // StartCoroutine(MsgForVideo("Success VerifyOTP", 2f));
            }
            else
            {
                StartCoroutine(MsgForVideo(errorMessage, 2f));
            }
           /* if (data["success"].ToString() == "1")
            {
                PlayerGameDetails playerData = Utility.ParsePlayerGameData(data);

              
                PlayerManager.instance.SetPlayerGameData(playerData);
                //  MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
                // MainMenuController.instance.SwitchToMainMenu();
            }
            else
            {
                wrongPasswordText.gameObject.SetActive(true);
                StartCoroutine(MsgForVideo("Incorrect password or username does not exist", 1.5f));
            }*/
        }
        else if (requestType == RequestType.SendSignupOTP)
        {
            Debug.Log("Response => SendSignupOTP: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);
            if (statusCode == 200)
            {
                // Success logic
                StartCoroutine(MsgForVideo(data["message"].ToString(), 4f));
                OnClickOnButton("openVerification");
                //  StartCoroutine(MsgForVideo("Success Rigister", 2f));
            }
            else
            {
                StartCoroutine(MsgForVideo(errorMessage, 2f));
            }
           

           /* if (data["Code"].Equals(true))
            {

                if (data["otp"] != null)
                    verificationCode = data["otp"].ToString();

                timer = 60;
                GetVerificationCode.interactable = false;
                verificationCodeInputField.transform.Find("WrongVeriCode").GetComponent<Text>().text = "Verification code sent";
                verificationCodeInputField.transform.Find("WrongVeriCode").GetComponent<Text>().color = GetVerificationCode.transform.GetChild(0).GetComponent<Text>().color;
                verificationCodeInputField.transform.Find("WrongVeriCode").gameObject.SetActive(true);
            }
            else
            {
                //forgotPassword.SetActive(false);
                //MainMenuController.instance.ShowMessage(data["response"].ToString());
                ////ResetLoginScreen();
                //OnClickOnButton("forgotpwd");
                forgotPasswordScreen.transform.Find("WrongEmail").GetComponent<Text>().text = "Email address has yet to be linked";
                forgotPasswordScreen.transform.Find("WrongEmail").gameObject.SetActive(true);
            }*/
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhandled server requestType found  " + requestType);
#endif
        }

    }


    public void OnClickOnBack()
    {
        if (PlayerManager.instance.IsLogedIn())
        {
        //    MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
        }
        else
        {
                GlobalGameManager.instance.CloseApplication();
            /*MainMenuController.instance.ShowMessage("Do you really want to quit?", () => {
            }, () => { });*/
        }
    }

    IEnumerator MsgForVideo(string msg, float delay)
    {
        popUpText.gameObject.SetActive(true);
        popUpText.text = msg;
        yield return new WaitForSeconds(delay);
        popUpText.gameObject.SetActive(false);
        wrongPasswordText.gameObject.SetActive(false);
    }

    //TaskWebserivce

    public async void LoginUSer()
    {

        var requestData = new LoginRequest
        {  
            email = loginUserEmail.text,
         
            password = loginPassword.text
        };

        string requestJson = JsonUtility.ToJson(requestData);


        try
        {
            Debug.Log("Sending request to LoginRequest...");
           
            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.Login,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: false
            );


            if (response.IsSuccess)
            {

                AuthSuccessResponse authResponse = JsonUtility.FromJson<AuthSuccessResponse >(response.Text);

                Debug.Log($"SUCCESS! LoginRequest '{response.Text}'");
                Debug.Log($"SUCCESS! RegisterRequest '{authResponse.accessToken}' created with ID: {authResponse.user}");
                DataManager.instance.IsLoggedIn = "true";
                authResponse.UpdatData();
                SceneManager.LoadScene(Scenes.MainScreen.ToString());


            }
            else
            {

                Debug.LogError($"SERVER ERROR ({response.StatusCode}): {response.Text}");

            }
        }
        catch (WebServiceException e)
        {
            // This catches network failures (no internet, timeout, etc.).
            Debug.LogError($"REQUEST FAILED ({e.StatusCode}): {e.ErrorMessage}");
            // Show a generic "Could not connect" error popup.
        }
        finally
        {
            // This block always runs, whether the request succeeded or failed.
            // It's the perfect place to re-enable the UI.
            //createRoomButton.interactable = true;
        }
    }
    public async void RefreshTokenAccess()
    {



        var requestData = new RefreshTokenRequest
        {
            refreshToken = DataManager.instance. refreshToken


        };

        string requestJson = JsonUtility.ToJson(requestData);

        Debug.Log("Sending request to refreshTokenRequest..." + requestData.refreshToken);
        try
        {
           

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.Refresh,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: true
            );


            if (response.IsSuccess)
            {

                RefreshTokenResponse authResponse = JsonUtility.FromJson<RefreshTokenResponse>(response.Text);

                Debug.Log($"SUCCESS! refreshTokenRequest '{response.Text}'");
                authResponse.UpdatData();

                SceneManager.LoadScene(Scenes.MainScreen.ToString());


            }
            else
            {

                Debug.LogError($"SERVER ERROR ({response.StatusCode}): {response.Text}");

            }
        }
        catch (WebServiceException e)
        {
            // This catches network failures (no internet, timeout, etc.).
            Debug.LogError($"REQUEST FAILED ({e.StatusCode}): {e.ErrorMessage}");
            // Show a generic "Could not connect" error popup.
        }
        finally
        {
            // This block always runs, whether the request succeeded or failed.
            // It's the perfect place to re-enable the UI.
            //createRoomButton.interactable = true;
        }
    }
}