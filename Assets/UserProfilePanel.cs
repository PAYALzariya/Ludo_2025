using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserProfilePanel : MonoBehaviour
{
    public RawImage coverimage;
    public Image userprofileimage;
    public Image frameimage;
    public Image levelimage;
    public Image countryimage;

    public TMP_Text usernameTxt;
    public TMP_Text useridTxt;
    public TMP_Text countryTxt;
    string pickcoverimagePath;
    private void OnEnable()
    {
        usernameTxt.text= HomePanel.instance.userDataPlayer.username;
        useridTxt.text= "ID: " +HomePanel.instance.userDataPlayer.uniqueId.ToString();
        countryTxt.text=  HomePanel.instance.userDataPlayer.country.ToString();
      
    }
    public void OnCoverPicEditButon()
    {
        //HomePanel.instance.chatManager.PickImageFromGallery(coverimage, pickcoverimagePath);
        HomePanel.instance.chatManager.OnChooseImageButtonClicked(coverimage);

    }
}
