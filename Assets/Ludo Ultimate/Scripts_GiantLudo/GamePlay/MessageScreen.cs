using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageScreen : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    [Header("Transforms")]
    public Transform Parent;

    [Header("ScriptableObjects")]
    public defaultMessageData messageDetailsObj;
    #endregion

    #region PRIVATE_VARIABLES
    private string previousEventResponse = "";
    #endregion

    #region UNITY_CALLBACKS
    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS
    public void CloseButtonTap()
    {
        this.Close();
    }
    public void SetAndOpen()
    {        
        Ludo_UIManager.instance.socketManager.messageList(Ludo_UIManager.instance.gamePlayScreen.currentRoomData.boardId, (socket, packet, args) =>
         {
             Debug.Log(Ludo_Constants.LudoEvents.MessageList + " respnose  : " + packet.ToString());

             if (previousEventResponse == packet.ToString())
                 return;             
             
             Reset();

             Ludo_UIManager.instance.OpenLoader(false);
             JSONArray arr = new JSONArray(packet.ToString());
             string Source;
             Source = arr.getString(arr.length() - 1);
             var resp = Source;

             PokerEventResponse<MessageListResult> GetLeaderboardResultDataResp = JsonUtility.FromJson<PokerEventResponse<MessageListResult>>(resp);

             if (GetLeaderboardResultDataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
             {
                 for (int i = 0; i < GetLeaderboardResultDataResp.result.messageList.Count; i++)
                 {
                     defaultMessageData messageDetailsObjPrefab = Instantiate(messageDetailsObj) as defaultMessageData;
                     messageDetailsObjPrefab.Setdata(GetLeaderboardResultDataResp.result.messageList[i]);
                     messageDetailsObjPrefab.transform.SetParent(Parent, false);
                 }

                 previousEventResponse = packet.ToString();
             }
             else
             {
                 Ludo_UIManager.instance.messagePanel.DisplayMessage(GetLeaderboardResultDataResp.message);
             }
         });
        this.Open();
    }


    #endregion

    #region PRIVATE_METHODS
    void Reset()
    {
        foreach (Transform item in Parent)
        {
            Destroy(item.gameObject);
        }
    }
    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER


    #endregion



}
