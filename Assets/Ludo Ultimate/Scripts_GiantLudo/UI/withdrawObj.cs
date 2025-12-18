using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class withdrawObj : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    //[Header ("Transforms")]


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    //[Header ("Images")]


    [Header("Text")]
    public TextMeshProUGUI ID;
    public TextMeshProUGUI date;
    public TextMeshProUGUI amount;
    public TextMeshProUGUI Status;
    public getUserWithdrawListData Data;

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
    public void SetData(getUserWithdrawListData ResultData)
    {
        Reset();
        this.Data = ResultData;
        ID.text = ResultData.transactionId;
        date.text = ResultData.createdAt;
        amount.text = ResultData.amount.ToString();
        if (ResultData.adminstatus.Equals("confirmed") || ResultData.adminstatus.Equals("Confirmed"))
        {
            Status.text = ResultData.adminstatus;
            Status.color = Color.green;
        }
        else if (ResultData.adminstatus.Equals("rejected") || ResultData.adminstatus.Equals("Rejected"))
        {
            Status.text = ResultData.adminstatus;
            Status.color = Color.red;
        }
        else
        {
            Status.text = ResultData.adminstatus;
            Status.color = Color.yellow;
        }
    }

    #endregion

    #region PRIVATE_METHODS

    private void Reset()
    {
        ID.text = "";
        date.text = "";
        amount.text = "";
        Status.text = "";
    }
    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER


    #endregion



}
