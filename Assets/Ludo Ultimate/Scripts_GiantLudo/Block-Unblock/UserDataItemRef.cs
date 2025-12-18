using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserDataItemRef : MonoBehaviour
{
    #region Variables

    [Header("Data ref")]
    private string playerId;
    public TMP_Text UserNameTextRef;
    public TMP_Text UserNumberTextRef;
    public Button ButtonRef;
    public TMP_Text ButtonTextRef;
    public Image UserProfilePicRef;

    #endregion    

    #region Initialize item

    public void InitializeItem(string playerIdRef, string UserName, string UserNumber, string buttonName, int IndexForMethod)
    {
        playerId = playerIdRef;
        UserNameTextRef.text = UserName;
        UserNumberTextRef.text = UserNumber;
        ButtonTextRef.text = buttonName;        
        ButtonRef.onClick.AddListener(() =>
        {
            // Set Button Method 
            OnButtonClick(IndexForMethod);
        });
    }

    public void OnButtonClick(int Index)
    {
       /* // Check for Index of the user method
        if (Index == 0) // Block User
        {
            Ludo_UIManager.instance.OpenLoader(true);
            Ludo_UIManager.instance.socketManager.BlockPlayer(playerId, (socket, packet, args) =>
            {
                Debug.Log($"BlockPlayer response : {packet.ToString()} ");
                JSONArray arr = new JSONArray(packet.ToString());
                string Source;
                Source = arr.getString(arr.length() - 1);
                var resp = Source;
                PokerEventListResponse<FoundPlayerdata> Resp = JsonUtility.FromJson<PokerEventListResponse<FoundPlayerdata>>(resp);
                if (Resp.status.Equals(LudoConstants.LudoAPI.KeyStatusSuccess))
                {
                    //Debug.Log("User has been blocked.");
                    Ludo_UIManager.instance.messagePanel.DisplayMessage(Resp.message);
                    if (this.transform.parent.childCount <= 1)
                    {
                        Debug.Log("ResetFoundUserList");
                        //Ludo_UIManager.instance.BlockUsersScreen.ResetFoundUserList();
                    }
                    Destroy(this.gameObject);
                }
                else
                {
                    //Debug.Log("No Blocked User Data List Found");
                    Ludo_UIManager.instance.messagePanel.DisplayMessage(Resp.message);
                }                
                Ludo_UIManager.instance.OpenLoader(false);
            });            
        }
        else if (Index == 1) // Unblock User
        {
            Ludo_UIManager.instance.OpenLoader(true);
            Ludo_UIManager.instance.socketManager.UnBlockPlayer(playerId, (socket, packet, args) =>
            {
                Debug.Log($"UnBlockPlayer response : {packet.ToString()} ");
                JSONArray arr = new JSONArray(packet.ToString());
                string Source;
                Source = arr.getString(arr.length() - 1);
                var resp = Source;
                PokerEventListResponse<FoundPlayerdata> Resp = JsonUtility.FromJson<PokerEventListResponse<FoundPlayerdata>>(resp);
                if (Resp.status.Equals(LudoConstants.LudoAPI.KeyStatusSuccess))
                {
                    //Debug.Log("User has been unblocked.");
                    Ludo_UIManager.instance.messagePanel.DisplayMessage(Resp.message);
                    if (this.transform.parent.childCount <= 1)
                    {
                        Debug.Log("ResetFoundUserList");
                        //  Ludo_UIManager.instance.BlockUsersScreen.ResetBlockUserList();
                    }
                    Destroy(this.gameObject);
                }
                else
                {
                    //Debug.Log("No Blocked User Data List Found");
                    Ludo_UIManager.instance.messagePanel.DisplayMessage(Resp.message);
                }
                Ludo_UIManager.instance.OpenLoader(false);                
            });            
        }*/
    }

    #endregion

}
