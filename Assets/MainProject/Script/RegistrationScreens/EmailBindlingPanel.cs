using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmailBindlingPanel : MonoBehaviour
{
    public List<TMP_InputField> passwordInputList=new List<TMP_InputField>();
    public TMP_InputField emailInput;
    public TMP_InputField  OTPInput;
    public GameObject bindingwithOTPPanel;
    public GameObject bindingwithPasswordPanel;
    public void OnEditPasswordInput()
    {
        
       

            if (passwordInputList[0].text == passwordInputList[1].text)
            {
            }
       


        
    }
    internal async void SendEmailBindingRquest()
    {



        var requestData = new EmailBindingRequest
        {
            email = emailInput.text


        };

        string requestJson = JsonUtility.ToJson(requestData);

        Debug.Log("Sending request to refreshTokenRequest..." + requestData.email);
        try
        {


            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.Refresh,
                httpMethod: "POST",
                requestData: requestJson,
                addAuthHeader: false
            );


            if (response.IsSuccess)
            {

                RefreshTokenResponse authResponse = JsonUtility.FromJson<RefreshTokenResponse>(response.Text);

                Debug.Log($"SUCCESS! refreshTokenRequest '{response.Text}'");
                authResponse.UpdatData();
                DataManager.instance.spriteMaganer.loaderManager.HideLoader();



            }
            else
            {

                Debug.LogError($"SERVER ERROR ({response.StatusCode}): {response.Text}");

            }
        }
        catch (WebServiceException e)
        {
            DataManager.instance.spriteMaganer.loaderManager.HideLoader();
            Debug.Log("error code:::" + e.StatusCode);
            Debug.Log("error message:::" + e.Message);
            if (e.Message.Contains("401"))
            {

                DataManager.instance.spriteMaganer.DisplayWarningPanel("Enter Vaild credentials");
            }
            else
            {
                DataManager.instance.spriteMaganer.DisplayWarningPanel(e.ErrorMessage);
            }
            Debug.LogError($"REQUEST FAILED ({e.StatusCode}): {e.ErrorMessage}");

        }
        finally
        {

        }
    }
    void ClosePanels()
    {
        bindingwithOTPPanel.SetActive(false);
        bindingwithPasswordPanel.SetActive(false);
    }
   internal void OpenPanels(string name)
    {
        ClosePanels();
        string panelName = name.ToLower();
        switch(panelName)
        {
            case "otp":
                bindingwithOTPPanel.SetActive(true);
               
                break;
            case "password":
                bindingwithOTPPanel.SetActive(false);
               
                break;
            default:
                Debug.LogError("Invalid panel name: " + name);
                break;
        }
    }
}
