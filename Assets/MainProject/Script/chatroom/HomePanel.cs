
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct HomeScreenFields
{
    public Image userprofile;
    public Image userprofilFrame;
}
public class HomePanel : MonoBehaviour
{
    public static HomePanel instance;
    public UserDataPlayer userDataPlayer;
    public HomeScreenFields homeScreenFields;

    [Header(" All Home  Screens Button Panels ")]
    public ChatManager chatManager;
    public FriendsManager friendsManager;
    public EventManager eventManager;
    public MePanel mePanel;
    public PurchasePanel purchasePanel;
    public SettingPanel settingPanel;
    public RechargePanel rechargePanel;
    public UserProfilePanel userProfilePanel;

    [Header(" All Top bar  Button data ")]
    public List<RawImage> userProfileImgList = new List<RawImage>();

    public TMP_Text cointxt;
    public TMP_Text dimondtxt;
    private void OnEnable()
    {
        Debug.Log("HomePanel OnEnable...");
      
        UpdateHomeScreenData();
        LoardHomeScreenButtonPanelData();
        cointxt.text = DataManager.instance.coin.ToString();
        dimondtxt .text = DataManager.instance.dimond.ToString();
    }
    void UpdateHomeScreenData()
    {
        GetUseProfile(DataManager.instance.userId);
    }
    private void Awake()
    {
        instance = this;
    }
    public void SceneLoad(int i)
    {
        SceneManager.LoadScene(i);
    }
    internal void OpenPanel(GameObject panel)
    {
        panel.gameObject.SetActive(true);
        panel.transform.SetAsLastSibling();
    }
  
    void CloseAllHomeButtonPanel()
    {
        chatManager.popularChatRoomListPanel.gameObject.SetActive(false);
        friendsManager.gameObject.SetActive(false);
        eventManager.gameObject.SetActive(false);
        mePanel.gameObject.SetActive(false);
        purchasePanel.gameObject.SetActive(false);
        settingPanel.gameObject.SetActive(false);
        rechargePanel.gameObject.SetActive(false);
        userProfilePanel.gameObject.SetActive(false);
    }
    public void OnHomePanelButtonClicked(string buttonName)
    {
        CloseAllHomeButtonPanel();
        switch (buttonName.ToString().ToLower())
        {
            case "userprofile":
                OpenPanel(userProfilePanel.gameObject);
                break;
            case "recharge":
                OpenPanel(rechargePanel.gameObject);
                break;
            case "purchase":
                OpenPanel(purchasePanel.gameObject);
                break;
            case "setting":
                OpenPanel(settingPanel.gameObject);
                break;
            case "event":
                OpenPanel(eventManager.gameObject);
                break;
            case "chat":
                Debug.Log("Chat Button Clicked...");
                chatManager.SendGetPopularChatListRequest(true);

                break;
            case "friend":  
                OpenPanel(friendsManager.gameObject);
                break;         
            case "me":
                OpenPanel(mePanel.gameObject);
                break;          
            default:
                Debug.LogWarning($"Unknown button name: {buttonName}");
                break;
        }
    }
   
    internal async void GetUseProfile(string userid)
    {
        Debug.Log("GetUseProfile...");





        var requestData = new UserProfileRequest
        {
            uid = userid


        };

        string requestJson = JsonUtility.ToJson(requestData);
        string urlTemplate = GameConstants.GAME_URLS[(int)RequestType.UserProfile_GetProfileByUserID];
        string apiUrl = GameConstants.FormatUrl(urlTemplate, ("uid", userid));
        // Use apiUrl for your API call 
        try
        {
            Debug.Log("Sending request to GetUseProfile...+" + requestJson + "::apiUrl:::" + apiUrl);

            ApiResponse response = await TaskWebServices.instance.SendRequestAsync(
                requestCode: RequestType.UserProfile_GetProfileByUserID,
                httpMethod: "GET",
                requestData: requestJson,
                addAuthHeader: true,
                 customUrl: apiUrl
            );



            Debug.Log($"GGOT GetUseProfile '{response.Text}'");



            if (response.IsSuccess)
            {


                userDataPlayer = JsonUtility.FromJson<UserDataPlayer>(response.Text);
                DataManager.instance.uniqueId = userDataPlayer.uniqueId.ToString();
               Debug.Log($"User Name : <color=white>{userDataPlayer.username}</color>");
            }
            else
            {

                Debug.LogError($"SERVER ERROR ({response.StatusCode}): {response.Text}");

            }
        }
        catch (WebServiceException e)
        {
            // This catches network failures (no internet, timeout, etc.).
            Debug.LogError($"REQUEST FAILED ({e.StatusCode}): {e.ErrorMessage}");
            // Show a generic "Could not connect" error popup.
        }
        finally
        {
            // This block always runs, whether the request succeeded or failed.
            // It's the perfect place to re-enable the UI.
            //createRoomButton.interactable = true;
        }
    }
    void LoardHomeScreenButtonPanelData()
    {
        Debug.Log("LoardHomeScreenButtonPanelData...");
        chatManager.SendGetPopularChatListRequest(false);
       
    }
    public void OnBackButtonClick(GameObject panelobject)
    {
        panelobject.SetActive(false);
    }
    public void OnclickPlayLudoButton()
    {
        DataManager.instance.spriteMaganer.LoaderForLoadSceneWithProgress(GameConstants.ProjectScene.LudoGame);
    }
}
