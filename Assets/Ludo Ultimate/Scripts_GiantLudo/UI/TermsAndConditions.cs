using System.Collections;
using System.Collections.Generic;
//using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TermsAndConditions : MonoBehaviour
{
    #region Public_Variables
    public TextMeshProUGUI Terms;
    public Loader loader;
    #endregion

    #region Private_Variables
    private bool isContentDownloaded = false;
    #endregion

    #region  Unity_Callback
    private void OnEnable()
    {
        //if (isContentDownloaded == false)
        Terms.text = "";
        setDataOpen();
    }
    private void OnDisable()
    {
        //Terms.text = "";
    }
    #endregion

    #region Private_Methods
    #endregion

    #region Public_Methods
    public void setDataOpen()
    {
        loader.Open();
     /*   Ludo_UIManager.instance.socketManager.TermsDetails((socket, packet, args) =>
        {
            
            Debug.Log("TermsDetails  : " + packet.ToString());

            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            CmsResult ShopDetailsDataResp = JsonUtility.FromJson<CmsResult>(resp);

            if (ShopDetailsDataResp.status.Equals(LudoConstants.LudoAPI.KeyStatusSuccess))
            {
                if (LocalizationManager.CurrentLanguageCode == "en")
                {
                    Terms.text = ShopDetailsDataResp.result.ToString();
                    isContentDownloaded = true;
                    loader.Close();
                }
                else
                {
                    GoogleTranslation.Translate(ShopDetailsDataResp.result.ToString(), "en", LocalizationManager.CurrentLanguageCode, OnTranslationReady);
                }
            }
            else
            {
                Ludo_UIManager.instance.messagePanel.DisplayMessage(ShopDetailsDataResp.message);
            }
        });*/
        this.Open();
    }
    public void CloseWindow()
    {        
        //Ludo_UIManager.instance.homeScreen.Open();
        Ludo_UIManager.instance.settings.Open();
        this.Close();
    }
    public void OnTranslationReady(string result, string error)
    {
        Terms.text = result;
        isContentDownloaded = true;
        loader.Close();
    }
    #endregion

    #region Coroutine
    #endregion
}
