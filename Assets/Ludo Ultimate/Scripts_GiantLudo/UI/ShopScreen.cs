using UnityEngine;
using BestHTTP.SocketIO;

public class ShopScreen : MonoBehaviour
{
    #region Public_Variables      
    public Transform Parent;    
    #endregion    

    #region  Unity_Callback

    private void OnEnable()
    {
        ServerSocketManager.instance.rootSocket.On("ClosePaymentPage", ResponseOfClosePaymentPage);
    }    

    private void OnDisable()
    {
        ServerSocketManager.instance.rootSocket.Off("ClosePaymentPage", ResponseOfClosePaymentPage);

        foreach (Transform tr in Parent)
        {
            Destroy(tr.gameObject);
        }
    }

    private void ResponseOfClosePaymentPage(Socket socket, Packet packet, params object[] args)
    {
        int arrLen = new JSONArray(packet.ToString()).length();
        Debug.Log("ResponseOfClosePaymentPage : " + packet.ToString());
        Debug.Log("ClosePaymentPage Broadcast");
        TransactionData transactionData = JsonUtility.FromJson<TransactionData>(packet.GetPacketString());

        CloseWindow();

        Ludo_UIManager.instance.assetOfGame.SavedLoginData.isFirstDeposit = transactionData.isFirstDeposit;
    //    Ludo_UIManager.instance.socketManager.UpdateDepositStatus(transactionData.transactionId, UpdateDepositStatusResponse);
    //    Ludo_UIManager.instance.transactionComplete.Open();
      //  Ludo_UIManager.instance.transactionComplete.SetData(transactionData);        
    }

    private void UpdateDepositStatusResponse(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("UpdateDepositStatus Done");
        CloseWindow();
    }    

    #endregion

    #region Public_Methods
    public void CloseWindow()
    {
      //  Voxel_WebViewManager.Instance.HideWebView();
        this.Close();
    }

    public void SetData(string url)
    {
      //  Ludo_UIManager.instance.storeScreen.Close();
        Ludo_UIManager.instance.homeScreen.Close();
        this.Open();        
      //  Voxel_WebViewManager.Instance.LoadCardFromJs(url);
    }    

    #endregion


}

