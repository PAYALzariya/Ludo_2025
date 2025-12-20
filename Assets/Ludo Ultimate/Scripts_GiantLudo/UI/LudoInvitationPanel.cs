using UnityEngine;

public class LudoInvitationPanel : MonoBehaviour
{
  //  public ChatPanel chatPanel;
    public CommonUserProfileItem CommonUserProfileItem;
    public Transform content;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       /* ChatParticipantData participantData;
        HomePanel.instance.chatManager.ChatParticipantDataList.TryGetValue(userid, out participantData);
        ChatRoomInvit seatitem;
        participantsSeatList.TryGetValue(seatno, out seatitem);
*/
    }
    internal CommonUserProfileItem CloneOnlineUserItem(ChatParticipantData userdata)
    {
        CommonUserProfileItem item = Instantiate(CommonUserProfileItem);
        item.gameObject.SetActive(true);
        item.participantData = userdata;
        item.SetUserProfile();
        return item;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
