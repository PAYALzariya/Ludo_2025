using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class privateJoiners : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    //[Header ("Transforms")]


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    [Header("Images")]
    public Sprite DefaultSprite;
    public Image ProfilePic;

    [Header("Text")]
    public TextMeshProUGUI PlayerName;

    //[Header ("Prefabs")]

    //[Header ("Enums")]


    [Header("Variables")]
    public JoinersItem data;
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

    }
    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS
    public void setData(JoinersItem Details)
    {
        this.data = Details;
        PlayerName.text = Details.playerName;

        ProfilePic.sprite = Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[Details.avatar];
        if (Details.profilePic != null && !Details.profilePic.Equals("default.png") && Details.profilePic != "")
        {
            string getImageUrl =  Details.profilePic;
            LudoUtilityManager.Instance.DownloadImage(getImageUrl, ProfilePic, true, true);
        }
        else
        {
            ProfilePic.sprite = Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[Details.avatar];
        }
    }

    public void ResetData()
    {
        PlayerName.text = "";
        ProfilePic.sprite = DefaultSprite;
    }

    #endregion

    #region PRIVATE_METHODS

    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER


    #endregion



}
