using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Emojis : MonoBehaviour
{
    #region Public_Variables
    public Image image;
    public Sprite[] emojis;
    #endregion

    #region Private_Variables
    #endregion

    #region  Unity_Callback
    private void OnEnable()
    {
        SetEmojis();
    }
    #endregion

    #region Private_Methods
    public void SetEmojis()
    {
        for (int i = 0; i < emojis.Length; i++)
        {
            Image img = Instantiate(image, Vector3.zero, Quaternion.identity, this.transform);
            img.gameObject.SetActive(true);
            img.transform.localScale = Vector3.one;
            img.sprite = emojis[i];
        }
    }
    #endregion

    #region Public_Methods
    #endregion

    #region Coroutine
    #endregion
}