using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrizeDistribution : MonoBehaviour
{
    #region Public_Variables
    #endregion

    #region Private_Variables

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI entryFee;
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    [SerializeField] private Transform player3;
    [SerializeField] private Transform player4;

    public TextMeshProUGUI plr1;
    public TextMeshProUGUI plr2;
    public TextMeshProUGUI plr3;
    public TextMeshProUGUI plr4;
    #endregion

    #region  Unity_Callback
    private void OnEnable()
    {

    }
    #endregion

    #region Private_Methods
    #endregion

    #region Public_Methods
    public void OpenPrizeDistribution(string tName, float player1WinPrice, float player2WinPrice, string eFee)
    {
        print("tname => " + tName.ToString());

        title.text = tName;
        entryFee.text = eFee;
        if (tName.Equals("1v1"))
        {
            plr1.text = player1WinPrice.ToString();
            plr2.text = "0";
            plr3.text = "0";
            plr4.text = "0";
            player1.gameObject.SetActive(true);
            player2.gameObject.SetActive(true);
            player3.gameObject.SetActive(false);
            player4.gameObject.SetActive(false);
        }
        else
        {
            plr1.text = player1WinPrice.ToString();
            plr2.text = player2WinPrice.ToString();
            plr3.text = "0";
            plr4.text = "0";
            player1.gameObject.SetActive(true);
            player2.gameObject.SetActive(true);
            player3.gameObject.SetActive(true);
            player4.gameObject.SetActive(true);
        }

        /*  switch (t.numbers)
          {
              case 1:
                  title.text = t.name;
                  player2.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "0.00";
                  player3.gameObject.SetActive(false);
                  player4.gameObject.SetActive(false);
                  break;

              case 2:
                  title.text = t.name;
                  player2.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "49.00";
                  player3.gameObject.SetActive(true);
                  player4.gameObject.SetActive(true);
                  break;
          }*/
        this.Open();
    }
    public void closebuttonTap()
    {
        this.Close();
    }
    #endregion

    #region Coroutine
    #endregion
}