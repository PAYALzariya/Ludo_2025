using System.Collections;
using UnityEngine;

public class BackgroundEventManager : MonoBehaviour
{

    #region PUBLIC_VARIABLES
    public static BackgroundEventManager instance = null;
    #endregion

    #region PRIVATE_VARIABLES
    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        StartCoroutine(CallProfileEvent());
    }
    #endregion

    #region DELEGATE_CALLBACKS
    #endregion

    #region PUBLIC_METHODS

    public void GetProfileEventCall()
    {
        Ludo_UIManager.instance.socketManager.GetProfile((socket, packet, args) =>
        {
            if (Ludo_UIManager.instance.maintenancePanel.isActiveAndEnabled || packet.ToString() == "[null]")
                return;

            //Debug.Log("Background GetProfile  : " + packet.ToString());
            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp1 = Source;

            PokerEventResponse<PlayerDetails> resp = JsonUtility.FromJson<PokerEventResponse<PlayerDetails>>(resp1);

            if (resp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
            {
                //Debug.Log("Playerprofile response : " + resp.result);
                Ludo_UIManager.instance.assetOfGame.SavedLoginData.username = resp.result.username;
                Ludo_UIManager.instance.assetOfGame.SavedLoginData.email = resp.result.email;                
                Ludo_UIManager.instance.assetOfGame.SavedLoginData.totalBalance = resp.result.totalBalance;
                Ludo_UIManager.instance.homeScreen.Chips = resp.result.totalBalance;
              
                Ludo_UIManager.instance.assetOfGame.SavedLoginData.win = resp.result.win;
            //    Ludo_UIManager.instance.homeScreen.txtBonusWalletChips.text = resp.result.bonusAmount.ToString();
            //    Ludo_UIManager.instance.homeScreen.playerInfo.SetPlayerData();
            }
            else
            {
                print("GetProfile fail");
            }
        });
    }

    public void CheckServerMaitenanceStatus()
    {
        Debug.Log("CheckServerMaitenanceStatus");
      /*  Ludo_UIManager.instance.socketManager.GetServerMaintenanceStatus((socket, packet, args) =>
        {
            Debug.Log("CheckServerMaitenanceStatus response: " + packet.ToString());
            EventResponse<ServerMaintenanceData> response = JsonUtility.FromJson<EventResponse<ServerMaintenanceData>>(packet.GetPacketString());

   
        });*/
    }
    #endregion

    #region PRIVATE_METHODS
    #endregion

    #region COROUTINES
    IEnumerator CallProfileEvent()
    {
        /*while (true)
        {
            if (ServerSocketManager.instance != null && ServerSocketManager.instance.rootSocket.IsOpen && ServerSocketManager.instance.playerId != ""
               )
            {
                GetProfileEventCall();
            }*/
            yield return new WaitForSeconds(5);
        //}
    }
    #endregion

    #region GETTER_SETTER
    #endregion
}
