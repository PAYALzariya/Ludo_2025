using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeatSettings_HostOnly : MonoBehaviour
{
    public List<Button> Allbuttons = new List<Button>();
   
    void OnEnable()
    {
        SetButton();
    }
    internal void SetButton()
    {

        EnableButons();
        if (HomePanel.instance.chatManager.chatPanel.Selectdseatitem.IsSeatLocked)
        {
            Allbuttons[2].gameObject.SetActive(false);
        }
        else
        {
            Allbuttons[3].gameObject.SetActive(false);
        }
    }
  
    void EnableButons()
    {
         for (int i = 0; i < Allbuttons.Count; i++)
        {
             Allbuttons[i].gameObject.SetActive(true);
        }
 
    }
    void OnmicButtonClicked()
    {
        
    }
    void OnInviteButtonclick()
    {
        HomePanel.instance.chatManager.inviteManager.hostSendRequestToOtherPanel.gameObject.SetActive(true);
        HomePanel.instance.chatManager.inviteManager.hostSendRequestToOtherPanel.transform.SetAsLastSibling();
    }
    void OnMuteMicButtonclick()
    {
        if (HomePanel.instance.chatManager.chatPanel.Selectdseatitem.isSeated)
        {
            HomePanel.instance.chatManager.voiceChatManager.ToggleMicrophone();
        }
    }

    void OnLockMicButtonClick()
    {

        Emitseatlockmic();
       
    }
    void OnUnlockMicButtonClick()
    {
        Debug.Log("unlockmic");
        EmitseatUnlockmic();
        
    }
    void OnCancelButtonClick()
    {

    }
    public void OnSeatSettingsHost_Only_ButtonsClick(TMP_Text buttontxt)
    {
        string buttonname = buttontxt.text.ToLower().Replace(" ", "");
        Debug.Log("buttonanme::" +buttonname);
        switch (buttonname)
        {

            case "onmic":
                OnmicButtonClicked();
                break;
            case "invite":
                OnInviteButtonclick();
                break;
            case "lockmic":
                OnLockMicButtonClick();
                break;
            case "unlockmic":
                OnUnlockMicButtonClick();
                break;
            case "mute":
                OnMuteMicButtonclick();
                break;
            case "cancel":
                OnCancelButtonClick();
                break;

            default:
                Debug.Log("No matching button action found.");
                break;


        }
        gameObject.SetActive(false);
    }

    //   =================================sockt Function=============================
    internal void Emitseatlockmic()
    {
        var data = new Dictionary<string, object> { { "roomId",HomePanel.instance.chatManager.CurrentRoom.data.room.id },
            { "seatNumber", HomePanel.instance.chatManager.chatPanel.Selectdseatitem.seatNo.ToString()} };
        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.EmitEvents.seatlockmic, payloadData: data
              , onAckResponse: (response) =>
         {
             if (response != null && response.success)
             {

                 Debug.Log($" onAckResponse seatlockmic received: {response.success} items");
             }
             else
             {
                 Debug.LogWarning("Failed to get seatlockmic ");
             }
         }
      );
    }

    internal void EmitseatUnlockmic()
    {
        var data = new Dictionary<string, object> { { "roomId",HomePanel.instance.chatManager.CurrentRoom.data.room.id },
            { "seatNumber", HomePanel.instance.chatManager.chatPanel.Selectdseatitem.seatNo.ToString()} };
        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.EmitEvents.seatunlockmic, payloadData: data
              , onAckResponse: (response) =>
         {
             if (response != null && response.success)
             {

                 Debug.Log($" onAckResponse  received: {response.success} items");
             }
             else
             {
                 Debug.LogWarning("Failed to get reponse ");
             }
         }
      );
    }

    internal void OnSeatLockedAndUnlock(LockedSeat lockedseat,bool islocked)
    {
        string seatno = lockedseat.data. seatNumber;
        Debug.Log("OnSeatLocked::" + HomePanel.instance.chatManager.chatPanel.participantsSeatList.ContainsKey(seatno));
        if (HomePanel.instance.chatManager.chatPanel.participantsSeatList.ContainsKey(seatno))
        {  Debug.Log(" get OnSeatLocked ::" +HomePanel.instance.chatManager.chatPanel.participantsSeatList[seatno].isSeated);
            if (!HomePanel.instance.chatManager.chatPanel.participantsSeatList[seatno].isSeated)
            {
                if (islocked)
                {
                    HomePanel.instance.chatManager.chatPanel.participantsSeatList[seatno].centerIcon.sprite =
                DataManager.instance.spriteMaganer.Locksprite;
                }
                else
                {
                    HomePanel.instance.chatManager.chatPanel.participantsSeatList[seatno].centerIcon.sprite =
                DataManager.instance.spriteMaganer.unlocksprite;
                }
                
                HomePanel.instance.chatManager.chatPanel.participantsSeatList[seatno].IsSeatLocked = islocked;
                SetButton();
            }
            else
            {

            }

        }
        else
        {

        }
        

    }
        
    
}
