using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReferEarnData : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    //[Header ("Transforms")]


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    //[Header ("Images")]


    [Header("Text")]
    public TextMeshProUGUI date;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI totalGames;
    public TextMeshProUGUI totalCommission;
    public referResultItemData ReferData;

    //[Header ("Prefabs")]

    //[Header ("Enums")]


    //[Header ("Variables")]

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

    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS

    public void SetData(referResultItemData ResultData)
    {
        Reset();
        this.ReferData = ResultData;
        date.text = ResultData.createdAt;
        PlayerName.text = ResultData.username;
        totalGames.text = ResultData.totalPlayed.ToString();
        totalCommission.text = ResultData.totalEarn.ToString();
    }

    #endregion

    #region PRIVATE_METHODS

    private void Reset()
    {
        date.text = "";
        PlayerName.text = "";
        totalGames.text = "";
        totalCommission.text = "";
    }
    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER


    #endregion



}
