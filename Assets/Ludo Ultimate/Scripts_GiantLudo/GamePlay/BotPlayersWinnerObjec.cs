using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotPlayersWinnerObjec : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    //[Header ("Transforms")]


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    //[Header ("Images")]


    [Header("Text")]
    public TextMeshProUGUI txtPlayerName;
    public TextMeshProUGUI txtPlayerPrice;

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
        Reset();
        Ludo_UIManager.instance.gamePlayScreen.WinningParticle.SetActive(false);
    }

    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS
    public void SetData(Winners winnerData, bool isTournamentPlayer)
    {
        Reset();
        txtPlayerName.text = winnerData.playerName;
        txtPlayerPrice.text = winnerData.amount.ToString();
        txtPlayerPrice.gameObject.SetActive(isTournamentPlayer);

        Ludo_UIManager.instance.gamePlayScreen.WinningParticle.SetActive(false);
        Ludo_UIManager.instance.gamePlayScreen.WinningParticle.SetActive(true);
        this.Open();
    }

    public void Reset()
    {
        txtPlayerName.text = "";
        txtPlayerPrice.text = "";
    }

    #endregion

    #region PRIVATE_METHODS
    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER


    #endregion



}
