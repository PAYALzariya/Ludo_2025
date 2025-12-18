using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepLinkManager : MonoBehaviour
{
    #region Public_Variables
    #endregion

    #region Private_Variables
    #endregion

    #region  Unity_Callback

    void Awake()
    {
        Application.deepLinkActivated += OnDeepLinkActivated;
    }
    #endregion

    #region Private_Methods
    private void OnDeepLinkActivated(string s)
    {
        //should be: "ludo://gaintLudo?123456"
        Debug.Log($"{s}");
    }
    #endregion

    #region Public_Methods
    #endregion

    #region Coroutine
    #endregion
}