using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinnerData : MonoBehaviour
{
    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    // [Header("Transforms")]

    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    [Header("Images")]
    public Sprite[] ranks;
    public Image Bg;

    // Stephen Update
    public Image Img_Medal;
    public Sprite[] medalImg;
    public Sprite playerBG;

    [Header("Text")]
    public TextMeshProUGUI PlayerRank;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI PlayerMobile;
    public TextMeshProUGUI PlayerChips;
    public LeaderboardResult Data;
    //[Header ("Prefabs")]

    //[Header ("Enums")]


    //[Header ("Variables")]

    #endregion

    #region PRIVATE_VARIABLES

    #endregion

    #region UNITY_CALLBACKS

    void OnEnable()
    {

    }
    void OnDisable()
    {
        //Reset();
    }

    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS
    public void SetData(LeaderboardResult resData, int i)
    {
        this.Data = resData;
        Reset();
        int counter = i % ranks.Length;
        //Debug.Log("counter => " + counter);
        Bg.sprite = ranks[counter];
        PlayerRank.text = resData.rank.ToString();
        PlayerName.text = resData.name;
        PlayerMobile.text = resData.mobile;
        PlayerChips.text = resData.amount.ToString();

        switch (resData.rank)
        {
            case 1:
            case 2:
            case 3:
                Img_Medal.sprite = medalImg[(resData.rank - 1)];
                Img_Medal.SetNativeSize();
                break;
            default:
                Img_Medal.gameObject.SetActive(false);
                break;
        }

        if (ServerSocketManager.instance.playerId.Equals(resData._id))
        {
            // player id matches to our User ID, Change to show our Data
            Bg.sprite = playerBG;
            Bg.SetNativeSize();
        }
    }

    #endregion

    #region PRIVATE_METHODS
    private void Reset()
    {
        PlayerName.text = "";
        PlayerMobile.text = "";
        PlayerChips.text = "";
        PlayerRank.text = "";
    }
    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER


    #endregion
}
