using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TopPlayerInfo : MonoBehaviour
{
    #region Public_Variables
    #endregion

    #region Private_Variables
    public TextMeshProUGUI playerName;
    public Image profileImage;
    public TextMeshProUGUI txtCoins;
    #endregion

    #region  Unity_Callback
    #endregion

    #region Private_Methods
    private void SetPlayerName()
    {
        string pName = Ludo_UIManager.instance.assetOfGame.SavedLoginData.username;

        if (pName.Contains(" "))
        {
            playerName.text = pName.Split(' ')[0];
        }
        else
        {
            if (pName.Length >= 9)
            {
                playerName.text = $"{pName.Substring(0, 9)}...";
            }
            else
            {
                playerName.text = pName;
            }
        }
    }
    #endregion

    #region Public_Methods
    public void SetPlayerData()
    {
        SetPlayerName();
        txtCoins.text = Ludo_UIManager.instance.assetOfGame.SavedLoginData.totalBalance.ToString();
        //txtCoins.text = UtilityManager.Instance.DecimalFormat(Ludo_UIManager.instance.assetOfGame.SavedLoginData.totalBalance);//.ToString();
        /* string dpUrl = LudoConstants.LudoConstants.GetImageUrl + Ludo_UIManager.instance.assetOfGame.SavedLoginData.profilePic;

         Debug.Log($"Downloading Image from {dpUrl}");
         StartCoroutine(DownloadImage(dpUrl, (sprite) =>
         {
             profileImage.sprite = sprite;
             profileImage.preserveAspect = true;
         }));*/
    }
    #endregion

    #region Coroutine
    private IEnumerator DownloadImage(string url, Action<Sprite> callback)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"{request.error}");
            yield break;
        }

        Texture2D t = DownloadHandlerTexture.GetContent(request);
        Rect r = new Rect(0, 0, t.width, t.height);
        Vector2 pivot = Vector2.one * 0.5f;
        callback(Sprite.Create(t, r, pivot));
    }
    #endregion
}