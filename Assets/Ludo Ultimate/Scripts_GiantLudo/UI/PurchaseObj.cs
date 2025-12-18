using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PurchaseObj : MonoBehaviour
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
    public getPurchaseListData Data;

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
    public void SetData(getPurchaseListData ResultData)
    {
        Reset();
        this.Data = ResultData;
        ID.text = ResultData.orderId;
        date.text = ResultData.createdAt;
        amount.text = ResultData.amount.ToString();
        Status.text = ResultData.status;
        if (ResultData.status.Equals("true") || ResultData.status.Equals("true"))
        {
            Status.color = Color.green;
        }
        else
        {
            Status.color = Color.red;
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
