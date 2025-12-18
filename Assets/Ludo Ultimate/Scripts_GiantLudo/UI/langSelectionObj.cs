using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//using I2.Loc;

public class langSelectionObj : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    //[Header ("Transforms")]


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    //[Header ("Images")]


    [Header("Text")]
    public TextMeshProUGUI langText;
    public Sprite selectedSprite;
    public Sprite normalSprite;
    [SerializeField] public GameObject bPanel;
    public int thisObjNo;
    

    //[Header ("Prefabs")]

    //[Header ("Enums")]


    //[Header ("Variables")]

    #endregion

    #region PRIVATE_VARIABLES

    #endregion

    #region UNITY_CALLBACKS
    // Use this for initialization
    void OnEnable()
    {
        
    }
    void OnDisable()
    {

    }

    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS

    public void SetData(int i)
    {
        langText.text = Ludo_UIManager.instance.languageSelection.Languages[i];
        thisObjNo = i;
    }
    public void OnSelect()
    {
        Ludo_UIManager.instance.languageSelection.Reset();
        bPanel.GetComponent<Image>().sprite = selectedSprite;
        PlayerPrefs.SetInt("MultiLanguage", thisObjNo);
   //     LocalizationManager.CurrentLanguage = Ludo_UIManager.instance.languageSelection.Languages[thisObjNo];
    }
    #endregion

    #region PRIVATE_METHODS

    private void Reset()
    {
        
    }
    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER


    #endregion



}
