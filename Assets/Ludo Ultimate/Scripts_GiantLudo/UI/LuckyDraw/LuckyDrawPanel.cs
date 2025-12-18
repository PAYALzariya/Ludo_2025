using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LuckyDrawPanel : MonoBehaviour
{
    #region PUBLIC_VARIABLES

    public List<SlotReel> reels;

    [Header("Image/Text")]
    public Image UserImage;
    public Image OpponantImage;
    public TextMeshProUGUI myUserNameTxt;
    public TextMeshProUGUI opponentUserNameforTwoTxt;
    #endregion

    #region PRIVATE_VARIABLES

    public float _movementSpeed;

    private Sprite _spRandomSelectedFruit;
    private bool _isReelSpinning;

    #endregion

    #region UNITY_CALLBACKS

    void OnEnable()
    {

    }

    void OnDisable()
    {
        OpponantImage.transform.parent.transform.parent.gameObject.SetActive(false);
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(StopReelAfterSometime());
        }
        //		Ludo_UIManager.Instance.dashboardPanel.SelectMenuButton (null);
    }

    #endregion

    #region DELEGATE_CALLBACKS

    #endregion

    #region PUBLIC_METHODS
    //public void increaseSpinSpeed()
    //{
    //    reels[0].speedIncrese();
    //}
    public void OnSpinButtonTap()
    {
        OpponantImage.transform.parent.transform.parent.gameObject.SetActive(false);
        IsReelSpinning = true;
        _movementSpeed = 2f;
        reels[0].StartReelAnimation(1 * .1f);
        //StartCoroutine(StopReelAfterSometime());
    }

    public void setOpponantData(List<Joinersusers> JoinersPLayers)
    {

        foreach (Joinersusers jr in JoinersPLayers)
        {
            if (jr.id.Equals(ServerSocketManager.instance.playerId))
            {

                myUserNameTxt.text = jr.playerName;
                if (jr.profilePic != null && !jr.profilePic.Equals("default.png") && jr.profilePic != "")
                {
                    //string getImageUrl = LudoConstants.LudoConstants.GetBaseUrl + jr.profilePic;
                    //UtilityManager.Instance.DownloadImage(getImageUrl, UserImage, true);
                }
                else
                {
                    UserImage.sprite = Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[jr.avatar];
                }

            }
            else
            {
                opponentUserNameforTwoTxt.text = jr.playerName;
                if (jr.profilePic != null && !jr.profilePic.Equals("default.png") && jr.profilePic != "")
                {
                    string getImageUrl = jr.profilePic;
                    LudoUtilityManager.Instance.DownloadImage(getImageUrl, OpponantImage, true, true);
                }
                else
                {
                    OpponantImage.sprite = Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[jr.avatar];
                }
                OpponantImage.transform.parent.transform.parent.gameObject.SetActive(true);
                StartCoroutine(StopReelAfterSometime());
            }
        }
    }
    public void OnWinOkButtonTap()
    {
        if (!IsReelSpinning)
            this.Close();
    }

    #endregion

    #region PRIVATE_METHODS

    #endregion

    #region COROUTINES

    private IEnumerator StopReelAfterSometime()
    {
        yield return new WaitForSeconds(0.5f);
        //yield return new WaitForSeconds(Random.Range(2f, 5f));
        IsReelSpinning = false;

        for (int i = 0; i < reels.Count; i++)
        {
            reels[i].StopReelAnimation(i * .5f);
        }
    }

    private IEnumerator StopReelIfErrorOrNoResponseTillSometime()
    {
        yield return new WaitForSeconds(Ludo_Constants.Tag.StopReelAfterTimeIfError);

        if (IsReelSpinning)
        {
            IsReelSpinning = false;
            for (int i = 0; i < reels.Count; i++)
            {
                reels[i].StopReelAnimation(i * .15f);
            }
        }
    }


    #endregion

    #region GETTER_SETTER

    public float MovementSpeed
    {
        get
        {
            return _movementSpeed;
        }
        set
        {
            _movementSpeed = value;
        }
    }

    public Sprite SpRandomSelectedFruit
    {
        get
        {
            return _spRandomSelectedFruit;
        }
    }

    public bool IsReelSpinning
    {
        get
        {
            return _isReelSpinning;
        }
        set
        {
            _isReelSpinning = value;
        }
    }



    #endregion
}