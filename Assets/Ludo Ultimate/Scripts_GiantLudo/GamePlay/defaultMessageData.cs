using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class defaultMessageData : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    //[Header ("Transforms")]


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    //[Header ("Images")]


    [Header("Text")]
    public Text SetMessage;

    //[Header ("Prefabs")]

    //[Header ("Enums")]


    //[Header ("Variables")]

    #endregion

    #region PRIVATE_VARIABLES

    #endregion

    #region UNITY_CALLBACKS
    // Use this for initialization
    void OnEnable()
    {


    }
    void OnDisable()
    {
        //SetMessage.text = "";
    }

    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS
    public void Setdata(MessageListItem Item)
    {
        SetMessage.text = "";
        SetMessage.text = Item.message;

    }
    public void sendMessageButtonTap()
    {
        string selectedMessage = SetMessage.text;
        print("selectedMessage +>" + selectedMessage);
        Ludo_UIManager.instance.socketManager.sendMessage(Ludo_UIManager.instance.gamePlayScreen.currentRoomData.boardId,
           selectedMessage, (socket, packet, args) =>
            {
                Debug.Log(Ludo_Constants.LudoEvents.SendMessage +" respnose  : " + packet.ToString());
                Ludo_UIManager.instance.OpenLoader(false);
                JSONArray arr = new JSONArray(packet.ToString());
                string Source;
                Source = arr.getString(arr.length() - 1);
                var resp = Source;

                PokerEventResponse sendMessageResp = JsonUtility.FromJson<PokerEventResponse>(resp);

                if (sendMessageResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
                {
                    Ludo_UIManager.instance.gamePlayScreen.MessagePanel.CloseButtonTap();
                }
                else
                {
                    Ludo_UIManager.instance.messagePanel.DisplayMessage(sendMessageResp.message);
                }
            });
    }


    #endregion

    #region PRIVATE_METHODS

    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER


    #endregion



}
