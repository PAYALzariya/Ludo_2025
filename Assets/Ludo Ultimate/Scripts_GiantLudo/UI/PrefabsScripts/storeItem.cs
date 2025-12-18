using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BestHTTP;
using System;

public class storeItem : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    //[Header ("Transforms")]


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    [Header("Images")]
    public Image img;


    [Header("Text")]
    public TextMeshProUGUI txtAmount;
    public TextMeshProUGUI txtPrice;

    [Header("Sprites")]
    public Sprite spriteSelection;
    public Sprite spriteNormal;
    public Sprite spriteExtraBonus;

    //[Header ("Enums")]    

    [Header("Variables")]
    public ShopDetailsData Data;
    #endregion

    #region PRIVATE_VARIABLES
    private int index = -1;
    #endregion

    #region UNITY_CALLBACKS
    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS
  



    public void SetData(ShopDetailsData Playerdata, int index)// (RoomsListing.Room data, int i)
    {
        this.index = index;
        this.Data = Playerdata;
        txtAmount.text = Playerdata.coin.ToString();
        txtPrice.text = Playerdata.amount.ToString();

        this.Open();

        if (!Ludo_UIManager.instance.assetOfGame.SavedLoginData.isFirstDeposit)
        {
            if (Data.isExtraBonus)
            {
                img.sprite = spriteExtraBonus;
            }
            else
            {
                img.sprite = spriteNormal;
            }
        }
        else
        {
            if (Data.amount > Data.coin)
            {
                img.sprite = spriteExtraBonus;
            }
            else
            {
                img.sprite = spriteNormal;
            }
        }

    }

    #endregion

    #region PRIVATE_METHODS

    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER

    public int Amount
    {
        get
        {
            return Data.amount;
        }
    }

    public int Coins
    {
        get
        {
            return Data.coin;
        }
    }

    public string Id
    {
        get
        {
            return Data._id;
        }
    }

    public bool Selection
    {
        set
        {
            if (!Ludo_UIManager.instance.assetOfGame.SavedLoginData.isFirstDeposit)
            {
                img.sprite = value == true ? spriteSelection : Data.isExtraBonus ? spriteExtraBonus : spriteNormal;
            }
            else
            {
                img.sprite = value == true ? spriteSelection : (Data.amount > Data.coin) ? spriteExtraBonus : spriteNormal;
            }
        }
    }
    #endregion



}
