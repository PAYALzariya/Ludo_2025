using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_LanguageSelection : MonoBehaviour
{
    #region Public_Variables
    public string[] Languages;
    [SerializeField]
    public langSelectionObj landSelObj;
    [SerializeField] public GameObject objContent;
    public langSelectionObj[] langOBJ;
    #endregion

    #region Private_Variables
    [Header("ScrollRect")]
    [SerializeField] private ScrollRect scrollRect;
    
    #endregion

    #region  Unity_Callback
    private void OnEnable()
    {
        //scrollRect.verticalNormalizedPosition = 1;

        for (int i = 0; i < Languages.Length; i++)
        {
            //langOBJ[i].langText.text = Languages[i];
            langOBJ[i].SetData(i);
            if (i == PlayerPrefs.GetInt("MultiLanguage"))
            {
                langOBJ[i].bPanel.GetComponent<Image>().sprite = langOBJ[i].selectedSprite;
            }
            else
            {
                langOBJ[i].bPanel.GetComponent<Image>().sprite = langOBJ[i].normalSprite;
            }
        }
    }
    private void OnDisable()
    {
        //Reset();
    }
    #endregion

    #region Private_Methods
    void CallEvent()
    {
        
    }
    public void Reset()
    {
        for (int i = 0; i < Languages.Length; i++)
        {
            langOBJ[i].bPanel.GetComponent<Image>().sprite = langOBJ[i].normalSprite;
        }
    }
    public void CloseButtonTap()
    {
        this.Close();
        Ludo_UIManager.instance.settings.Open();
    }
    #endregion

    #region Public_Methods
    #endregion

    #region Coroutine
    #endregion
}
