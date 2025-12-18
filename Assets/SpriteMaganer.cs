using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Purchasing.MiniJSON;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Crmf;

public class SpriteMaganer : MonoBehaviour
{
  public ImageValidatorAndDownloader imageValidatorAndDownloader;
  public Sprite default_RoomProfileSprite;
  public Sprite default_ProfileSprite;
  public Sprite default_NotfoundSprite; 
  public Sprite default_countrySprite;
  public Sprite default_levelSprite;
  public Sprite default_currencySprite;
  public Sprite default_frameSprite;
  public Sprite default_GiftSprite;
  public Sprite Locksprite;
  public Sprite unlocksprite;
    public string prfoileurltosendServer;
    public LoaderManager loaderManager;
    public WarningPanel warningPanel;
  internal async UniTask UploadImageToServer(byte[] imageData, string imgetype, string contenttype, string size)
  {
    //"profile-images" ,"image/jpeg" ,"2097152"

    var data = new Dictionary<string, object>
     { { "imageType",imgetype },
      { "contentType", contenttype},
      { "fileSize", size } };
    string jsonResponse = Json.Serialize(data);
    Debug.Log("Sending request to UploadImageToServer..." + ":::Dictionary::" + jsonResponse);
    try
    {


      ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
          requestCode: RequestType.uploadImage,
          httpMethod: "POST",
          requestData: jsonResponse,
          addAuthHeader: true
      );


      if (response.IsSuccess)
      {


        updloadimageResponse responsedata = JsonUtility.FromJson<updloadimageResponse>(response.Text);
        Debug.Log($"<color=green>SUCCESS! UploadImageToServer</color> " + responsedata.data);

        if (string.IsNullOrEmpty(responsedata.data.signedUrl))
        {
          Debug.LogError("Cannot proceed to Step 2: The signedUrl is empty.");
          return;
        }
        else
        {
          bool issuccesreponse = await UploadFileWithSignedUrl(responsedata.data.signedUrl, imageData, contenttype);
          if (issuccesreponse)
          {
            prfoileurltosendServer = responsedata.data.publicUrl;
          }
        }

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
  private async UniTask<bool> UploadFileWithSignedUrl(string signedUrl, byte[] imageData, string contentType)
  {
    using (var uwr = new UnityWebRequest(signedUrl, "PUT"))
    {
      uwr.uploadHandler = new UploadHandlerRaw(imageData);
      uwr.downloadHandler = new DownloadHandlerBuffer();
      uwr.SetRequestHeader("Content-Type", contentType);
      await uwr.SendWebRequest();

      // As you noted, the response here is uwr.result, not a URL in the body
      return uwr.result == UnityWebRequest.Result.Success;
    }
  }
  internal void ResetprfoileurltosendServer()
  {
    prfoileurltosendServer = "";
}
    internal void LoaderForLoadSceneWithProgress(string sceneName)
    {
        loaderManager.gameObject.SetActive(true);
        loaderManager.LoadSceneWithProgress(sceneName);
    }

    internal void ShowLoaderWithProgress(string message, float timeout = 10f)
    {
        loaderManager.gameObject.SetActive(true);
        loaderManager.ShowLoaderWithFakeProgress(message, timeout);
    }
    internal void DisplayWarningPanel(string message)
    {
        warningPanel.gameObject.SetActive(true);
        warningPanel.ShowPopup(message);
    }
}
