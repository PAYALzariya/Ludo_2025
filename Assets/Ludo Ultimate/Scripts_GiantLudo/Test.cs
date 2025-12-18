using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    #region Public_Variables
    public int startNumber = 128512;
    public int total = 100;
    public Text text;
    #endregion

    #region Private_Variables
    //string s = "";
    #endregion

    #region  Unity_Callback
    private void OnEnable()
    {
        Debug.Log("Hello");
    }
    #endregion

    #region Private_Methods

    #endregion

    #region Public_Methods
    public void JustOpen()
    {
        this.Open();
    }
    #endregion

    #region Coroutine
    #endregion
}
