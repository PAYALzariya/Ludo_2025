using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnlineUnlockTablePopup : MonoBehaviour
{
    #region PUBLIC_VARIABLES
    #endregion

    #region PRIVATE_VARIABLES
    #endregion

    #region UNITY_CALLBACKS
    #endregion

    #region DELEGATE_CALLBACKS
    #endregion

    #region PUBLIC_METHODS
    public void OpenAddCash()
    {
      //  Ludo_UIManager.instance.storeScreen.Open();
        ClosePanel();
    }

    public void ClosePanel()
    {
        //Ludo_UIManager.instance.playOnline.Close();
        this.Close();

    }
    #endregion

    #region PRIVATE_METHODS
    #endregion

    #region COROUTINES
    #endregion

    #region GETTER_SETTER
    #endregion
}
