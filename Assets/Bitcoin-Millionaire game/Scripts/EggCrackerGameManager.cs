using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EggCrackerGameManager : MonoBehaviour
{
    public static EggCrackerGameManager instance;
    public GameObject t;
    public TextMeshProUGUI errorDialogText;

    public BCG_DatatUpdateRoot eggDatatUpdateRoot;
    private void Awake()
    {
        instance = this;
      
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public void SceneLoad(int i)
    {
        SceneManager.LoadScene(i);
    }
  /*  #region api call
    IEnumerator ErrorText(string text, int time)
    {
        errorDialogText.gameObject.SetActive(true);
        errorDialogText.text = text;
        yield return new WaitForSeconds(time);

        errorDialogText.gameObject.SetActive(false);
    }
    
    internal async void EggGamePostplayerProgress(EggDatatUpdateRoot eggDatatUpdateRoot)
    {
      
        // Direct JSON string
       // string requestJson = $"{{\"receiverId\":\"{receiverId}\"}}";
         string requestJson = JsonUtility.ToJson(eggDatatUpdateRoot);
        try
        {
           // Debug.Log("Sending request to Friend_request_delete...");

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.EggGamePostplayerProgress,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: true
            );

            Debug.Log($"EggGamePostplayerProgress '{response}'");

            if (response.IsSuccess)
            {
                Debug.Log($"SUCCESS! EggGamePostplayerProgress '{response.Text}'");
            }
            else
            {

                Debug.LogError($"SERVER ERROR EggGamePostplayerProgress ({response.StatusCode}): {response.Text}");

            }
        }
        catch (WebServiceException e)
        {
            // This catches network failures (no internet, timeout, etc.).
            Debug.LogError($"REQUEST FAILED EggGamePostplayerProgress ({e.StatusCode}): {e.ErrorMessage}");
            // Show a generic "Could not connect" error popup.
        }
        finally
        {
            // This block always runs, whether the request succeeded or failed.
            // It's the perfect place to re-enable the UI.
            //createRoomButton.interactable = true;
        }
    }
    internal async void EggGameReset()
    {
        
        string requestJson = $"{{\"gameId\":\"{CommonDataFatch.instance.eggGameGetData.data.games[0].gameId}\",\"confirmReset\":true}}";

        //  string requestJson = JsonUtility.ToJson(eggDatatUpdateRoot);
        try
        {
            // Debug.Log("Sending request to Friend_request_delete...");

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.EggGamePostplayerProgress,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: true
            );

            Debug.Log($"EggGameReset '{response}'");

            if (response.IsSuccess)
            {
                Debug.Log($"SUCCESS! EggGameReset '{response.Text}'");
            }
            else
            {

                Debug.LogError($"SERVER ERROR EggGameReset ({response.StatusCode}): {response.Text}");

            }
        }
        catch (WebServiceException e)
        {
            // This catches network failures (no internet, timeout, etc.).
            Debug.LogError($"REQUEST FAILED EggGameReset ({e.StatusCode}): {e.ErrorMessage}");
            // Show a generic "Could not connect" error popup.
        }
        finally
        {
            // This block always runs, whether the request succeeded or failed.
            // It's the perfect place to re-enable the UI.
            //createRoomButton.interactable = true;
        }
    }
    #endregion*/
}