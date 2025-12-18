using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommonUserProfileItem : MonoBehaviour
{
    public Image profileImage;
    public Image frameImage;
    public Image countryFlagImage;
    public Image levelImage;

    public TMP_Text userNameText;
    public TMP_Text userdIdText;
    public TMP_Text countryNameText;


    public Button button;
    public TMP_Text buttontext;
    internal ChatParticipantData participantData;

    public void SetUserProfile()
    {
        userdIdText.text = participantData.uniqueId.ToString(); ;
        userNameText.text = participantData.username;
        profileImage.sprite = participantData.profilePictureAsset.SpriteAssset;
        countryNameText.text = participantData.countryData.countryname;
        countryFlagImage.sprite = participantData.countryData.flagSprite;
        levelImage.sprite = participantData.levelData.levelSprite;
      

    }
}
