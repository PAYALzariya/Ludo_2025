using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeatinvitationReceived : MonoBehaviour
{
    public SeatInvitationReceived seatInvitationReceived;
    public TMP_Text roomname, invitedBy;
    public Button acceptBtn, declinebtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roomname.text = ChatManager.instance.CurrentRoom.data.room.name;
        invitedBy.text = ChatManager.instance.CurrentRoom.data.host.username;

    }

   public void OnClickAccept()
    {
        EmitInvitePopUp("accept");
    }
    public void OnClickDecline()
    {
        EmitInvitePopUp("decline");
    }
    internal void EmitInvitePopUp(string accept)
    {
        var data = new Dictionary<string, object> { { "roomId", seatInvitationReceived.data. roomId },
        { "seatNumber", seatInvitationReceived.data.seatNumber },
        { "action",accept }};
        LudoSocketManager.Instance.EmitWithAck<user_apply_seatSuccessResponse>(GameConstants.EmitEvents.userRespondSeatInvitation, payloadData: data
              , onAckResponse: (response) =>
              {
                  if (response != null && response.success)
                  {
                      Destroy(gameObject);
                      Debug.Log($" onAckResponse user_apply_seat received: {response.success} items");
                  }
                  else
                  {
                      Debug.LogWarning("Failed to get user_apply_seat ");
                  }
              }
      );
    }
}
