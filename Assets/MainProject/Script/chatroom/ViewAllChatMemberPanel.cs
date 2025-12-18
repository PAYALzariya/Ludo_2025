using System.Collections.Generic;
using UnityEngine;

public class ViewAllChatMemberPanel : MonoBehaviour
{
    public GameObject morepopup;
    public ViewChatMemeberItem viewChatMemeberItemPrefab;
    public Transform onlinememberParent;
    public Transform othermemberParnet;
    public List<ViewChatMemeberItem> allMemberList = new List<ViewChatMemeberItem>();
    public void OnClickButtonOnMoreButton(ViewChatMemeberItem member)
    {
        morepopup.transform.localPosition = member.transform.localPosition;
        morepopup.transform.position = member.transform.position;
        morepopup.SetActive(true);
    }
    public void OnClickButtonOffMoreButton()
    {
        morepopup.SetActive(false);
    }
    void CloneChatMember()
    {
        ViewChatMemeberItem cloneItem = Instantiate(viewChatMemeberItemPrefab);
        // cloneItem.transform.SetParent(itemParnet);
        allMemberList.Add(cloneItem);
        /*if (itemParnet == onlinememberParent)
        {
        }
        else
        {

        cloneItem.musicNameText.text = track.Title;
        cloneItem.isMyMusic = track.ISMyMusic;
        cloneItem.transform.localScale = Vector3.one;
        cloneItem.transform.position = musicListTransform.position;
        if (track.ISMyMusic)
        {
            cloneItem.button.image.sprite = addedMusicSprite;
            cloneItem.button.interactable = false;
        }
        else
        {

        }*/

    }
}
