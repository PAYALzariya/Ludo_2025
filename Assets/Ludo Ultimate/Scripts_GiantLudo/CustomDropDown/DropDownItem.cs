using TMPro;
using UnityEngine;

public class DropDownItem : MonoBehaviour
{
    #region Variables

    [Header("Item Data Ref")]
    public TMP_Text LebelTextRef;

    #endregion    

    #region Management Methods

    public void OnClickButton()
    {
      
    }

    public void SetItemData(string labelString)
    {
        LebelTextRef.text = labelString;
    }

    #endregion

}
