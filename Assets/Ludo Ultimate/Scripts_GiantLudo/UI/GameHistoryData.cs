using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameHistoryData : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    //[Header ("Transforms")]


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    //[Header ("Images")]


    [Header("Text")]
    public TextMeshProUGUI date;
    public TextMeshProUGUI Game;
    public TextMeshProUGUI win;
    public TextMeshProUGUI coins;
    public gameHistoryData Data;


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
    public void SetData(gameHistoryData ResultData)
    {
        Reset();
        this.Data = ResultData;
        date.text = ResultData.date;
        Game.text = ResultData.amount.ToString();
        win.text = ResultData.win;
        coins.text = ResultData.coins.ToString();
    }




    #endregion

    #region PRIVATE_METHODS

    private void Reset()
    {
        date.text = "";
        Game.text = "";
        win.text = "";
        coins.text = "";
    }
    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER


    #endregion



}
