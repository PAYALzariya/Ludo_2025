 using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FBLogin : MonoBehaviour
{
  


    void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            Debug.Log("Facebook SDK Initialized Successfully.");
            
            // Check if we are already logged in
            if (FB.IsLoggedIn)
            {
                Debug.Log("Facebook user is already logged in.");
                // Get user's name and profile picture
                GetProfile();
            }
        }
        else
        {
            Debug.LogError("Failed to Initialize the Facebook SDK.");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void Login()
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("User is already logged in.");
            GetProfile();
            return;
        }

        // Define the permissions we want to ask for
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;
            
            // Print current access token's User ID
            Debug.Log("User ID: " + aToken.UserId);
            
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log("Permission: " + perm);
            }

            Debug.Log("Facebook login successful!");

            // Now get the user's profile information
            GetProfile();
        }
        else
        {
            Debug.LogError("User cancelled login or it failed.");
            if (result.Error != null)
            {
                Debug.LogError("Error: " + result.Error);
            }
        }
    }

    public void Logout()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
            Debug.Log("User logged out successfully.");
        }
        else
        {
            Debug.Log("User is not logged in.");
        }
    }


    public void GetProfile()
    {
        
        FB.API("/me?fields=name,picture.width(100).height(100)", HttpMethod.GET, ProfileCallback);
    }

    private void ProfileCallback(IGraphResult result)
    {
        Debug.Log("result error ::" + result.Error);
        Debug.Log("result::" + result.RawResult);
        /*if (string.IsNullOrEmpty(result.Error) && result.ResultDictionary != null)
        {
            string userName = result.ResultDictionary["name"].ToString();
            Debug.Log("User Name: " + userName);

        }
        else
        {
            Debug.LogError("Failed to get profile: " + result.Error);
        }*/
    }
}

