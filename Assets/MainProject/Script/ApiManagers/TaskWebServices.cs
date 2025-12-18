

using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;


public readonly struct ApiResponse
{
    public readonly long StatusCode;
    public readonly string Text;
    public readonly byte[] RawData;

   
    public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

    public ApiResponse(UnityWebRequest request)
    {
        StatusCode = request.responseCode;
        Text = request.downloadHandler?.text;
        RawData = request.downloadHandler?.data;
    }
}


public class WebServiceException : System.Exception
{
    public long StatusCode { get; }
    public string ErrorMessage { get; }
    public string ResponseText { get; }

   
    public WebServiceException(UnityWebRequest request)
        : base($"API Error: {request.error} (Status: {request.responseCode})")
    {
        StatusCode = request.responseCode;
        ErrorMessage = request.error;
        ResponseText = request.downloadHandler?.text;
    }

    // Constructor for errors that happen before a request is sent (e.g., No Internet)
    public WebServiceException(string customMessage, long customStatusCode)
        : base($"API Pre-flight Error: {customMessage} (Status: {customStatusCode})")
    {
        StatusCode = customStatusCode;
        ErrorMessage = customMessage;
        ResponseText = null;
    }
}




public class TaskWebServices : MonoBehaviour
{
   
    public static TaskWebServices instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    

    public async UniTask<ApiResponse> SendRequestAsync(
        RequestType requestCode,
        string httpMethod = "POST",
        string requestData = null,
        bool addAuthHeader = false,
        int timeOut = 30,
        int retryLimit = 6,
        string customUrl = null)
    {
        if (!IsInternetAvailable())
        {
            Debug.LogError("--> PRE-FLIGHT CHECK FAILED: No Internet.");
            throw new WebServiceException("Internet Connection not available.", -1);
        }

        string url = customUrl ?? GameConstants
            .GAME_URLS[(int)requestCode];
        byte[] bodyData = string.IsNullOrEmpty(requestData) ? null : System.Text.Encoding.UTF8.GetBytes(requestData);
        Debug.Log("url::"+ url);
        for (int i = 0; i < retryLimit; i++)
        {
            Debug.Log($"--> LOOP START: Iteration {i + 1}/{retryLimit}");

            using (var request = new UnityWebRequest(url, httpMethod.ToUpper()))
            {
                
                if (bodyData != null && (httpMethod.ToUpper() == "POST" || httpMethod.ToUpper() == "PUT"|| httpMethod.ToUpper() == "GET"))
                {
                    request.uploadHandler = new UploadHandlerRaw(bodyData);
                }
                request.downloadHandler = new DownloadHandlerBuffer();
                request.timeout = timeOut;
                request.SetRequestHeader("Content-Type", "application/json");
                if (addAuthHeader)
                {
                    request.SetRequestHeader("Authorization", "Bearer " + DataManager.instance. AccessToken);
                }

                try
                {
                   // Debug.Log("--> TRY BLOCK: About to send web request...");
                    await request.SendWebRequest().ToUniTask();
                //    Debug.Log("--> TRY BLOCK: Web request has completed.");

                    // Check the result code IMMEDIATELY after the await.
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"--> SUCCESS! Code: {request.responseCode}, Response: {request.downloadHandler.text}");

                       // Debug.Log("--> SUCCESS: About to create ApiResponse object...");
                        var apiResponse = new ApiResponse(request);
                      //  Debug.Log("--> SUCCESS: ApiResponse object created. About to return.");

                        return apiResponse; // This should be the final exit.
                    }
                    else
                    {
                        // This handles cases where the request completed but with an HTTP error (4xx, 5xx)
                        Debug.LogError($"--> HTTP ERROR! Result: {request.result}, Code: {request.responseCode}, Error: {request.error}");

                      //  Debug.Log("--> HTTP ERROR: About to create WebServiceException...");
                        var exception = new WebServiceException(request);
                      //  Debug.Log("--> HTTP ERROR: WebServiceException created. About to throw.");
                        DataManager.instance.spriteMaganer.DisplayWarningPanel(requestCode +"::"+ request.error);
                        throw exception; 
                    }
                }
                catch (Exception e)
                {
                    // This will catch the exception we just threw OR a genuine network error.
                  //  Debug.LogError($"--> CATCH BLOCK: An exception was caught. Type: {e.GetType()}. Message: {e.Message}");

                    //  Retry Logic
                    bool shouldRetry = false;
                    if (e is UnityWebRequestException webEx)
                    {
                        bool isNetworkError = webEx.UnityWebRequest.result == UnityWebRequest.Result.ConnectionError;
                        bool isTempServerError = webEx.UnityWebRequest.responseCode >= 502 && webEx.UnityWebRequest.responseCode <= 504;
                        if (isNetworkError || isTempServerError)
                        {
                            shouldRetry = true;
                        }
                    }

                    if (shouldRetry && i < retryLimit - 1)
                    {
                        Debug.LogWarning("--> CATCH BLOCK: Temporary error detected. Retrying..."+e.Message);
                        await UniTask.Delay(1000);
                    }
                    else
                    {
                        Debug.LogError("--> CATCH BLOCK: Permanent error or out of retries. Rethrowing to caller.");
                        DataManager.instance.spriteMaganer.DisplayWarningPanel(requestCode + "::" + e.Message);
                        if (e is WebServiceException) throw; 
                        else throw new WebServiceException(e.Message, -3); 
                    }
                }
            }
          //  Debug.Log($"--> LOOP END: Iteration {i + 1} finished.");
        }

        Debug.LogError("--> FATAL: Loop finished without returning or throwing. This should be impossible.");
        throw new WebServiceException("Request failed after all retries.", -2);
    }
    public bool IsInternetAvailable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
}
