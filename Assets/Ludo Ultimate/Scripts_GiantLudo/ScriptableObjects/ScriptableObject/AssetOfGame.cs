using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class AssetOfGame : ScriptableObject
{
    public GetWithdrawData getWithdrawData;
    public PlayerDetails SavedLoginData = null;
    public GameStartDataResp gameStartDataResp;
    public ProfileAvatarList profileAvatarList;
    public TransactionData transactionData;
    public string uniqueId;
    public Sprite spDefaultImage;
    public Sprite spDefaultBannerImage;

    public void SetPlayerDetails(PlayerDetails details)
    {
        /*
        SavedLoginData = details;
        if (!details.isGmailLogin)
        {
            PlayerPrefs.SetString(LudoConstants.LudoConstants.PlayerNumber, details.phno);
        }
        */

        SavedLoginData = details;
        //if (!details.isGmailLogin)
        //{
        //    //    PlayerPrefs.SetString(LudoConstants.LudoConstants.PlayerNumber, details.phno);
        //}        
       /* PlayerPrefs.SetString(Ludo_Constants.LudoConstants.PlayerNumber, details.phno);
        PlayerPrefs.SetString(Ludo_Constants.LudoConstants.PlayerEmail, details.email);
        PlayerPrefs.SetString(Ludo_Constants.LudoConstants.PlayerName, details.username);*/

    }
    //public GameObjects SettedGameObjects;
}

[Serializable]
public class ProfileAvatarList
{
    public Sprite[] profileAvatarSprite;
}