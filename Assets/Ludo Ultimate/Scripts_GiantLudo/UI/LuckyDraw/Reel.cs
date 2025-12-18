using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Reel : MonoBehaviour
{
    #region PUBLIC_VARIABLES

    public int reelNumber;
    public Image defaultImg;
    public BoxCollider2D boxcollision;
    public List<Image> reelImagesList;
    public List<Sprite> reelSpritesList;

    public VerticalLayoutGroup reelVerticleLayoutGroup;

    #endregion

    #region PRIVATE_VARIABLES

    #endregion

    #region DELEGATES

    public delegate void ResetReel(int reelNumber);

    public static event ResetReel resetReel;

    public static void FireResetReel(int reelNumber)
    {
        if (resetReel != null)
            resetReel(reelNumber);
    }

    #endregion

    #region UNITY_CALLBACKS

    // Use this for initialization
    void OnEnable()
    {
        boxcollision = this.GetComponent<BoxCollider2D>();
        defaultImg.gameObject.SetActive(true);
        foreach (Image img in reelImagesList)
        {
            img.sprite = reelSpritesList[Random.Range(0, reelSpritesList.Count - 1)];
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(Ludo_Constants.Tag.LuckyDrawReelResetter))
            FireResetReel(reelNumber);
    }

    #endregion

    #region DELEGATE_CALLBACKS

    #endregion

    #region PUBLIC_METHODS

    #endregion

    #region PRIVATE_METHODS

    #endregion

    #region COROUTINES

    #endregion
}