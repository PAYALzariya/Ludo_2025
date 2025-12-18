using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
public class SaveSystem : MonoBehaviour
{
    const string KEY_BTC = "BTC";
    const string KEY_BPS = "BPS";
    const string KEY_CLICK = "ClickValue";
    private void Awake()
    {
        // Delay loading until end of frame so CurrencyManager exists
        StartCoroutine(LoadNextFrame());
    }

    private IEnumerator LoadNextFrame()
    {
        yield return null; // wait 1 frame
        if (CurrencyManager.Instance != null)
            Load();
        else
            Debug.LogError("CurrencyManager.Instance still not found!");
    }

    void OnApplicationQuit()
    {
        Save();
    }

    public void Save()
    {
        if (CurrencyManager.Instance != null)
        {
            PlayerPrefs.SetString(KEY_BTC, CurrencyManager.Instance.Bitcoins.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.SetString(KEY_BPS, CurrencyManager.Instance.GetBps().ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.SetString(KEY_CLICK, CurrencyManager.Instance.BitcoinsPerClick.ToString(CultureInfo.InvariantCulture));
        }

        // Save ShopItems owned counts
        var allItems = Resources.LoadAll<ShopItem>("ShopItems");
        foreach (var si in allItems)
        {
            PlayerPrefs.SetInt("owned_" + si.id, si.owned);
        }

        PlayerPrefs.Save();
        Debug.Log("Saved game");
    }

    public void Load()
    {
        if (CurrencyManager.Instance == null) return;

        double btc = 0;
        double.TryParse(PlayerPrefs.GetString(KEY_BTC, "0"), NumberStyles.Float, CultureInfo.InvariantCulture, out btc);
        CurrencyManager.Instance.SetBitcoins(btc);

        double bps = 0;
        double.TryParse(PlayerPrefs.GetString(KEY_BPS, "0"), NumberStyles.Float, CultureInfo.InvariantCulture, out bps);
        CurrencyManager.Instance.SetBps(bps);

        double clickVal = 1;
        double.TryParse(PlayerPrefs.GetString(KEY_CLICK, "1"), NumberStyles.Float, CultureInfo.InvariantCulture, out clickVal);
        CurrencyManager.Instance.SetClickValue(clickVal);

        // Load owned counts
        var allItems = Resources.LoadAll<ShopItem>("ShopItems");
        foreach (var si in allItems)
        {
            si.owned = PlayerPrefs.GetInt("owned_" + si.id, si.owned);
        }

        Debug.Log("Loaded game");
    }
    #region APICall
    IEnumerator ErrorText(string text2, int time)
    {
        BCGUIManager.Instance?.errorDialogText.gameObject.SetActive(true);
        BCGUIManager.Instance.errorDialogText.text = text2;

        yield return new WaitForSeconds(time);

        BCGUIManager.Instance?.errorDialogText.gameObject.SetActive(false);
    }
    internal async void BCGamePostplayerProgress(BCG_GameGetDataRoot datatUpdateRoot)
    {

        // Direct JSON string
        // string requestJson = $"{{\"receiverId\":\"{receiverId}\"}}";
        string requestJson = JsonUtility.ToJson(datatUpdateRoot);
        try
        {
            // Debug.Log("Sending request to Friend_request_delete...");

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.bcg_amePostplayerProgress,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: true
            );

            Debug.Log($"EggGamePostplayerProgress '{response}'");

            if (response.IsSuccess)
            {
                Debug.Log($"SUCCESS! BCGGamePostplayerProgress '{response.Text}'");
            }
            else
            {

                Debug.LogError($"SERVER ERROR BCGGamePostplayerProgress ({response.StatusCode}): {response.Text}");

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
    internal async void BCGGameReset()
    {

        string requestJson = $"{{\"gameId\":\"{CommonDataFatch.instance.bcg_GameGetData.data.games[0].gameId}\",\"confirmReset\":true}}";

        //  string requestJson = JsonUtility.ToJson(eggDatatUpdateRoot);
        try
        {
            // Debug.Log("Sending request to Friend_request_delete...");

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.bcg_amePostplayerProgress,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: true
            );

            Debug.Log($"BCGGameReset '{response}'");

            if (response.IsSuccess)
            {
                Debug.Log($"SUCCESS! BCGGameReset '{response.Text}'");
                CurrencyManager.Instance.ResetGame();
            }
            else
            {

                Debug.LogError($"SERVER ERROR BCGGameReset ({response.StatusCode}): {response.Text}");

            }
        }
        catch (WebServiceException e)
        {
            // This catches network failures (no internet, timeout, etc.).
            Debug.LogError($"REQUEST FAILED BCGGameReset ({e.StatusCode}): {e.ErrorMessage}");
            // Show a generic "Could not connect" error popup.
        }
        finally
        {
            // This block always runs, whether the request succeeded or failed.
            // It's the perfect place to re-enable the UI.
            //createRoomButton.interactable = true;
        }
    }
    #endregion
}


// class of egg cracker game
// Root of egg game get data
[System.Serializable]
public class BCG_GameGetDataRoot
{
    public bool success;
    public BCG_GameData data;
}
[System.Serializable]
public class BCG_GameData
{
    public List<BCG_Game> games;
    public double totalCoins;
    public double totalEarnings;
}
[System.Serializable]
public class BCG_Game
{
    public string gameId;
    public string gameName;
    public double coins;
    public double clickPower;
    public double totalClicks;
    public double currentLevel;
    public DateTime lastPlayedAt;
}
[System.Serializable]
public class BCG_DatatUpdateRoot
{
    public string gameId;
    public double coins;
    public double clickPower;
    public double totalClicks;
    public double currentLevel;
    public BCG_SessionData sessionData;
}
[System.Serializable]
public class BCG_SessionData
{
    public double clicks;
    public double coinsEarned;
    public int playTime;
}
[System.Serializable]
public class BCG_buyItemRoot
{
    public string gameId;
    public string itemType;
    public string itemId;
    public int quantity;
}


