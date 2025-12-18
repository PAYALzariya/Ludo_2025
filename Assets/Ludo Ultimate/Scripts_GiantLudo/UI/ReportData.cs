using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReportData : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI date;
    public TextMeshProUGUI purchase;
    public TextMeshProUGUI win;
    public TextMeshProUGUI bet;
    public TextMeshProUGUI refer;
    public TextMeshProUGUI other;
    public ReportListData.Result Data;

    public void SetData(ReportListData.Result ResultData)
    {
        Reset();
        this.Data = ResultData;
        date.text = ResultData.date.ToString();
        purchase.text = ResultData.purchase.ToString();
        win.text = ResultData.win.ToString();
        bet.text = ResultData.bet.ToString();
        refer.text = ResultData.reffer.ToString();
        other.text = ResultData.bonus.ToString();
    }
    private void Reset()
    {
        date.text = "";
        purchase.text = "";
        win.text = "";
        bet.text = "";
        refer.text = "";
        other.text = "";
    }
}