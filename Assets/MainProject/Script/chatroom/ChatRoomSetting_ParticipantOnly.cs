using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class ChatRoomSetting_ParticipantOnly : MonoBehaviour
{
    public Transform onlineuserParent;

    public RawImage roomprofile;
    public TMP_Text roomname;
    public TMP_Text roomcode;
    void OnEnable()
    {
        roomprofile.texture = HomePanel.instance.chatManager.CurrentRoom.data.room.roomImageAsset.SpriteAssset.texture;
        roomcode.text = "<b>Room Code: </b>" + HomePanel.instance.chatManager.CurrentRoom.data.room.roomCode;
        roomname.text = "<b>Room Name: </b>" + HomePanel.instance.chatManager.CurrentRoom.data.room.name;
    }

}
