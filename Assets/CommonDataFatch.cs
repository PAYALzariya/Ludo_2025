using System.Collections;
using UnityEngine;

public class CommonDataFatch : MonoBehaviour
{
    public static CommonDataFatch instance;
    public BCG_GameGetDataRoot bcg_GameGetData;
    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WaitForGameIdAndStartUpdates());
    }
    private IEnumerator WaitForGameIdAndStartUpdates()
    {
        // wait until gameId is not null or empty
        while (DataManager.instance.AccessToken ==null||
               string.IsNullOrEmpty(DataManager.instance.AccessToken))
        {
            yield return null; // wait one frame
        }

        Debug.Log("✅ GameId is ready, starting auto updates...");

        // Start repeating updates every 5 minutes
        EggGameGetplayerProgress();
    }
    internal async void EggGameGetplayerProgress()
    {
        Debug.Log("EggGameGetplayerProgress...");
        string requestJson = "";
        try
        {
            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.bcg_GameGetplayerProgress,
                httpMethod: "GET",
                requestData: requestJson,
                addAuthHeader: true
            );
            if (response.IsSuccess)
            {
                Debug.Log($"SUCCESS! EggGameGetplayerProgress '{response.Text}'");
                // parse into wrapper
                bcg_GameGetData = JsonUtility.FromJson<BCG_GameGetDataRoot>(response.Text);
            }
            else
            {
                Debug.LogError($"SERVER ERROR EggGameGetplayerProgress  ({response.StatusCode}): {response.Text}");
            }
        }
        catch (WebServiceException e)
        {
            Debug.LogError((e.ErrorMessage, 2));
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
