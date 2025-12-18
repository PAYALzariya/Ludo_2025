using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class refferalData : MonoBehaviour
{
    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    // [Header("Transforms")]

    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    //[Header ("Images")]


    [Header("Text")]
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
    public void SetData(LeaderboardResult resData)
    {
        this.Data = resData;
        Reset();
        PlayerName.text = resData.name;
        PlayerMobile.text = resData.mobile;
        PlayerChips.text = resData.amount.ToString();
    }

    #endregion

    #region PRIVATE_METHODS
    private void Reset()
    {
        PlayerName.text = "";
        PlayerMobile.text = "";
        PlayerChips.text = "";
    }
    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER


    #endregion
}
