using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreen : MonoTemplate
{
    #region Public_Variables
    #endregion

    #region Private_Variables
    #endregion

    #region  Unity_Callback
    private void OnEnable()
    {

    }
    #endregion

    #region Private_Methods
    #endregion

    #region Public_Methods
    public void CloseWindow()
    {
        this.Close();
    }

    public void LeaveTheGame()
    {
        GetMessagePanel.DisplayConfirmationMessage("are you sure you want to leave the game", (b) =>
        {
            if (b)
            {
                GameStaticData.commingBackFromGame = true;
                this.Close();
                GetGameScreen.Close();
                if (Ludo_UIManager.instance.gamePlayScreen)
                    Ludo_UIManager.instance.gamePlayScreen.Close();
                GetHomeScreen.Open();
            }
            else
            {
                CloseWindow();
            }
        });
    }
    #endregion

    #region Coroutine
    #endregion
}
