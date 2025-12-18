
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CreateChatRoomPanel : MonoBehaviour
{
    public RawImage roomImage;
    public TMP_InputField roomNameInput;
   
    public bool isRoomPrivate;
    public int  maxparticipants ;
    



    public void OnAddProfileImage()
    {
        //HomePanel.instance.chatManager.PickImageFromGallery(roomImage,pickImagePath);
        HomePanel.instance.chatManager.OnChooseImageButtonClicked(roomImage);
      
    }

    public void OnCreateRoomButtonClicked()
    {
        
            string error = string.Empty;
            if (!UtilityGame.IsValidUserName(roomNameInput.text, out error))
            {
                Debug.LogError("roomName must be greater than 0");

            }
            else if (maxparticipants <= 0)
            {
                Debug.LogError("Max participants must be greater than 0");
            }
            else if (!roomImage.texture)
            {
                Debug.LogError("Please select a room image");
            }
            else
            {
                Debug.Log("DataManager.instance.profileurl::" + DataManager.instance.spriteMaganer.prfoileurltosendServer);
                HomePanel.instance.chatManager.SendCreateNewRoomRequest(roomNameInput.text, " ", maxparticipants, DataManager.instance.spriteMaganer.prfoileurltosendServer);
            }
                
    }

}
