using BestHTTP.JSON;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using TMPro; 
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignUpManager : MonoBehaviour
{
    public GameObject signUpPanel;

    public GameObject signInPanel;
    public TMP_InputField loginUserEmail;
    public TMP_InputField loginPassword;
    public EmailBindlingPanel emailBindlingPanel;
    public FBLogin fBLoginmanager;
    private void OnEnable()
    {
        
    
        
        if (DataManager.instance.IsLoggedIn == "true")
        {
            print("login done refresh token:::" + DataManager.instance.refreshToken);
            print("login done access token:::" + DataManager.instance.AccessToken);
           RefreshTokenAccess().Forget(); 
        }
        else
        {

            signUpPanel.SetActive(true); 
           
           
        }
    }

    internal async UniTask RefreshTokenAccess()
    {



        var requestData = new RefreshTokenRequest
        {
            refreshToken = DataManager.instance.refreshToken


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
                Debug.Log($"SUCCESS! refreshTokenRequest '{response}'");
                authResponse.UpdatData();

                LoadMainScreenScenes();
               // SceneManager.LoadScene(Scenes.MainScreen.ToString());


            }
            else
            {

                Debug.LogError($"SERVER ERROR ({response.StatusCode}): {response.Text}");

            }
        }
        catch (WebServiceException e)
        {
            
            Debug.LogError($"REQUEST FAILED ({e.StatusCode}): {e.ErrorMessage}");
            
        }
        finally
        {
            
        }
    }
    void LoadMainScreenScenes()
    {
        DataManager.instance.spriteMaganer.LoaderForLoadSceneWithProgress(Scenes.MainScreen.ToString());
    }
    internal async void LoginUSer()
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

                AuthSuccessResponse authResponse = JsonUtility.FromJson<AuthSuccessResponse>(response.Text);

                Debug.Log($"SUCCESS! LoginRequest '{response.Text}'");
                Debug.Log($"SUCCESS! RegisterRequest '{authResponse.accessToken}' created with ID: {authResponse.user}");
                DataManager.instance.IsLoggedIn = "true";
                authResponse.UpdatData();
                DataManager.instance.spriteMaganer.loaderManager.HideLoader();
                LoadMainScreenScenes();
                // SceneManager.LoadScene(Scenes.MainScreen.ToString());


            }
            else
            {
                DataManager.instance.spriteMaganer.loaderManager.HideLoader();
                DataManager.instance.spriteMaganer.DisplayWarningPanel(response.Text);
                Debug.LogError($"SERVER ERROR ({response.StatusCode}): {response.Text}");

            }
        }
        catch (WebServiceException e)
        {
           
            DataManager.instance.spriteMaganer.loaderManager.HideLoader();
            Debug.Log("error code:::"+ e.StatusCode);
            Debug.Log("error message:::" + e.Message);
            if (e.Message.Contains("401"))
            {

            DataManager.instance.spriteMaganer.DisplayWarningPanel("Enter Vaild credentials");
            }
            else
            {
                DataManager.instance.spriteMaganer.DisplayWarningPanel(e.ErrorMessage);
            }
                Debug.Log($"REQUEST FAILED ({e.StatusCode}): {e.ErrorMessage}");
            // Show a generic "Could not connect" error popup.
        }
        finally
        {
            // This block always runs, whether the request succeeded or failed.
            // It's the perfect place to re-enable the UI.
            //createRoomButton.interactable = true;
        }
    }

    public void LoginWithGoogle()
    {
        //    GoogleManager.instance.SignInWithGoogle();
    }

    public void LoginWithFacebook()
    {
        fBLoginmanager.Login();
    }

    public void OnClickEmailLoginButton()
    {
        signInPanel.SetActive(true);
        signInPanel.transform.SetAsLastSibling();
    }
    public void OnClickLoginButton()
    {
        if (string.IsNullOrEmpty(loginUserEmail.text) || string.IsNullOrEmpty(loginPassword.text))
        {
            DataManager.instance.spriteMaganer.DisplayWarningPanel("Please enter all the fields");
            return;
        }
        else
        {
            DataManager.instance.spriteMaganer.ShowLoaderWithProgress("Login");
            LoginUSer();

        }




    }


    public void OnForgotPasswordButtonclick()
    {
        emailBindlingPanel.gameObject.SetActive(true);
        emailBindlingPanel.transform.SetAsLastSibling();
        emailBindlingPanel.OpenPanels("OTP");
    }
}
