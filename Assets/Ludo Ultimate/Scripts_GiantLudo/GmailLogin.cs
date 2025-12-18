using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if GOOGLE_SIGNIN
using Google;
#endif
#if FIREBASE_SDK
using Firebase.Auth;
#endif
using System;
using System.Threading.Tasks;

public class GmailLogin : MonoBehaviour
{
    public TMP_Text StatusTxt;

#if GOOGLE_SIGNIN
    private GoogleSignInConfiguration configuration;
#endif


    private string webClientId = "920810123521-bfhkmtoqoeuo45t8kp9mnd3gia2eojib.apps.googleusercontent.com";//"744664575372-9o8eqnvpc513k8s36vgol0m26r0iobuc.apps.googleusercontent.com"; 
#if FIREBASE_SDK
    private FirebaseAuth auth;
#endif
    void OnEnable()
    {
#if UNITY_ANDROID || UNITY_IOS && !UNITY_EDITOR
        StartCoroutine(GmailAuthCheck(1f));
#endif
    }

    IEnumerator GmailAuthCheck(float timer)
    {
        StatusTxt.text = "Loader Open..";
        yield return new WaitForSeconds(1f);

#if GOOGLE_SIGNIN
            configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
#endif


        CheckFirebaseDependencies();
        yield return new WaitForSeconds(timer);
        StatusTxt.text = "Loader close..";
    }

    private void CheckFirebaseDependencies()
    {
#if FIREBASE_SDK
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {

            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                auth = FirebaseAuth.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }

    });
#endif
    }


#if GOOGLE_SIGNIN

    public void OnGmailBtn()
    {
        StatusTxt.text = "Button Clicked..";
        OnGmailLogin();
    }

    private void OnGmailLogin()
    {
        StatusTxt.text = "OnGmailLogin Method..";
        Invoke(nameof(SignInWithGoogle), 1);
    }

    public void SignInWithGoogle()
    {
        StatusTxt.text = "SignInWithGoogle process started..";
        Invoke(nameof(OnSignIn), 2);
    }
    
    private void OnSignIn()
    {
        StatusTxt.text = "OnSignIn..";
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        StatusTxt.text = "OnAuthenticationFinished..";
        if (task.IsFaulted)
        {
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string authErrorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    authErrorCode = String.Format("AuthError.{0}: ",
                      ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                }

                StatusTxt.text = "Login process failed, please try again..";

                string code = ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString();
                Debug.Log(code);
            }
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("gmail login Cancelled call");
            StatusTxt.text = "gmail login Cancelled call..";

        }
        else
        {
            Debug.LogError("gmail login else part call:0- " + task);
            Debug.LogError("gmail logintask.Result.IdToken- " + task.Result.IdToken);

            StartCoroutine(SignInWithGoogleOnFirebase(task.Result.IdToken));
        }
    }
#endif



    private IEnumerator SignInWithGoogleOnFirebase(string idToken)
    {

        Debug.LogError("idToken = " + idToken);
        StatusTxt.text = "Firebase Siging started....";
        yield return new WaitForSeconds(0);
#if GOOGLE_SIGNIN
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                UIManager.instance.OpenLoader(false);
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                UIManager.instance.OpenLoader(false);
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }


            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1}) {2} {3} {4}",
                newUser.DisplayName, newUser.UserId,
                newUser.PhoneNumber, newUser.Email,
                newUser.PhotoUrl.ToString());
            //GmailLoginCallBack(task1);
            // gmailDataSet(newUser);
            StatusTxt.text = "checkMobileNumberSocket....";
            // UIManager.instance.socketManager.checkMobileNumberSocket(newUser.DisplayName, newUser.PhoneNumber, newUser.Email, newUser.UserId, newUser.PhotoUrl.ToString(), true, ResponseOfSocialSocket);
        });
#endif
    }

}
