using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class WebServices : MonoBehaviour
{
	public static WebServices instance;

	void Awake()
	{
		//PlayerPrefs.DeleteAll();
		
		instance = this;
	}

	private void SendRequestToServer(APIRequests request)
	{
		if (request.retryCount >= GameConstants.API_RETRY_LIMIT)
		{
			Debug.Log("API retry count exceeded api code = " + request.requestCode);

			if (request.callbackMethod != null)
			{
                request.callbackMethod(request.requestCode, "", request.isShowErrorMessage, "Retry count exceeded.", -1);

            }
            else
			{
				Debug.LogError("Call back method is null ");
			}
			return;
		}
		++request.retryCount;
		request.requestMethod = WaitForServerResponse(request);
		StartCoroutine(request.requestMethod);
	}

    public void SendRequest(RequestType requestCode, string requestData, bool addAuthHeader,bool isShowErrorMessage, System.Action<RequestType, string, bool, string, int> callbackMethod = null, WWWForm formData = null, bool isGetMethod = false)
    {
        APIRequests request = new APIRequests();

		if (formData != null)
		{
			request.pdata = formData.data;
		}
		else
		{
			if (!string.IsNullOrEmpty(requestData))
			{
				request.pdata = System.Text.Encoding.ASCII.GetBytes(requestData.ToCharArray());
			}
			else
			{
				request.pdata = null;
			}
		}

		request.url = GameConstants.GAME_URLS[(int)requestCode];
		request.requestCode = requestCode;
		request.callbackMethod = callbackMethod;
		request.isShowErrorMessage = isShowErrorMessage;
		request.addAuthHeader =addAuthHeader;
		request.retryCount = 0;

//#if DEBUG_LOG
		//Debug.Log("Calling API sending data requestCode = " + request.requestCode + "  data = " + requestData);
		Debug.Log("Request API  URL ===> " + request.url+" requestCode ===> " + request.requestCode + "  data ===> " + requestData);
//#endif

		SendRequestToServer(request);
	}

	private IEnumerator WaitForServerResponse(APIRequests request)
	{
		if (!IsInternetAvailable())
		{
			if (request.isShowErrorMessage)
			{
                request.callbackMethod(request.requestCode, "", request.isShowErrorMessage, "Internet Connection not available.", -1);

             //   request.callbackMethod(request.requestCode, "", request.isShowErrorMessage,request., "Internet Connection not available.");
			}

			yield break;
		}

		request.timerMethod = WaitForAPIResponse(request);
		Dictionary<string, string> headers = new Dictionary<string, string>();
		headers.Add("Content-Type", "application/json");
		if (request.addAuthHeader)
		{
           // request.s headers.Add("Authorization", "Bearer " + DataManager.instance.GetAccessToken().ToString());
        }
		//Debug.LogError(request.url);
		WWW www = new WWW(request.url, request.pdata, headers);

		StartCoroutine(request.timerMethod);

		yield return www;

		StopCoroutine(request.timerMethod);

//#if DEBUG_LOG
		Debug.Log("Response requestCode ---> " + request.requestCode + "    data ---> " + www.text);
//#endif


		if (request.callbackMethod != null)
		{
			string errorMessage = "";

			if (www.error != null)
			{
				errorMessage = www.error;
			}
            //else if (string.IsNullOrEmpty(www.text))
            //{
            //	errorMessage = "Server response not found, please check your internet connection is working properly";
            //}
            //Debug.LogWarning("Request was:" + request.url + " " + request.requestCode +" Response   " + www.text);
            int statusCode = 0;
            if (www.responseHeaders != null && www.responseHeaders.ContainsKey("STATUS"))
            {
                string[] statusParts = www.responseHeaders["STATUS"].Split(' ');
                if (statusParts.Length >= 2)
                    int.TryParse(statusParts[1], out statusCode);
            }

            request.callbackMethod(request.requestCode, www.text, request.isShowErrorMessage, errorMessage, statusCode);

        }

        www.Dispose();
		www = null;
	}

	private IEnumerator WaitForAPIResponse(APIRequests request)
	{
		yield return new WaitForSeconds(GameConstants.API_TIME_OUT_LIMIT);

		StopCoroutine(request.requestMethod);
		SendRequestToServer(request);
	}

	public bool IsInternetAvailable()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}

		return true;
	}



	//DEV_CODE Method for independent request
	public IEnumerator POSTRequestData(string uri, string json, System.Action<string, bool, string> callbackOnFinish)
	{
		UnityWebRequest uwr = new UnityWebRequest(uri, "POST");

		Debug.Log("Request : " + uri);

		if (json != "")
		{
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
			uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
		}

		uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

		uwr.SetRequestHeader("Content-Type", "application/json");

		yield return uwr.SendWebRequest();

		if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
		{
			callbackOnFinish("", true, uwr.error);
		}
		else
		{
			callbackOnFinish(uwr.downloadHandler.text, false, "");
		}
	}
}


[System.Serializable]
public class APIRequests
{
	public byte[] pdata;
	public string url;
	public bool isShowErrorMessage;
	public RequestType requestCode;
	public IEnumerator requestMethod, timerMethod;

	public int retryCount;
    public System.Action<RequestType, string, bool, string, int> callbackMethod;
    public bool addAuthHeader = false;

}


[System.Serializable]
public enum RequestMethod
{
	Get,
	Put,
	PostWithFormData
}


[System.Serializable]
public class LoginData
{
	public string userName;
	public string password;
}
