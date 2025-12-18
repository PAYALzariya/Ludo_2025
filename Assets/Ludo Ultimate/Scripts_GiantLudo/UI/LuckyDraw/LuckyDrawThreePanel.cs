using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LuckyDrawThreePanel : MonoBehaviour
{
    #region PUBLIC_VARIABLES

    public List<SlotReel> reels;

    [Header("Image/Text")]
    public Image UserImage;
    public Image[] OpponantImages;
    public TextMeshProUGUI myUserNameTxt;
    public TextMeshProUGUI[] opponentUserTxt;

    #endregion

    #region PRIVATE_VARIABLES

    public float _movementSpeed;

    public bool _isReelSpinning;

    #endregion

    #region UNITY_CALLBACKS

    void OnEnable()
    {
        myUserNameTxt.text = Ludo_UIManager.instance.assetOfGame.SavedLoginData.username;
    }

    void OnDisable()
    {
        //		Ludo_UIManager.Instance.dashboardPanel.SelectMenuButton (null);
        ResetUser();
    }

    #endregion

    #region DELEGATE_CALLBACKS

    #endregion

    #region PUBLIC_METHODS
    public void setOpponantData(List<Joinersusers> JoinersPLayers)
    {

        if (JoinersPLayers.Count > 0)
        {
            if (JoinersPLayers.Count.Equals(1))
            {
                if (JoinersPLayers[0].id.Equals(ServerSocketManager.instance.playerId))
                {

                    myUserNameTxt.text = JoinersPLayers[0].playerName;

                }
            }

            else
            {
                int oppCounter = 0;
                for (int i = 0; i < JoinersPLayers.Count; i++)

                {
                    if (JoinersPLayers[i].id.Equals(ServerSocketManager.instance.playerId))
                    {
                        myUserNameTxt.text = JoinersPLayers[i].playerName;
                    }
                    else
                    {
                        opponentUserTxt[oppCounter].text = JoinersPLayers[i].playerName;
                        //OpponantImages[oppCounter].sprite = Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[JoinersPLayers[i].avatar];
                        if (!OpponantImages[oppCounter].transform.parent.transform.parent.gameObject.activeInHierarchy)
                        {
                            if (JoinersPLayers[i].profilePic != null && !JoinersPLayers[i].profilePic.Equals("default.png") && JoinersPLayers[i].profilePic != "")
                            {
                                string getImageUrl =  JoinersPLayers[i].profilePic;
                                LudoUtilityManager.Instance.DownloadImage(getImageUrl, OpponantImages[oppCounter], true, true);
                            }
                            else
                            {
                                OpponantImages[oppCounter].sprite = Ludo_UIManager.instance.assetOfGame.profileAvatarList.profileAvatarSprite[JoinersPLayers[i].avatar];
                            }
                        }
                        OpponantImages[oppCounter].transform.parent.transform.parent.gameObject.SetActive(true);
                        OnStopReelAnimation(oppCounter);
                        oppCounter++;

                    }
                }
            }

        }
    }
    private void ResetUser()
    {
        foreach (Image opp in OpponantImages)
        {
            opp.transform.parent.transform.parent.gameObject.SetActive(false);
        }
        reels[0].SetReelsToInitialPos();
        reels[1].SetReelsToInitialPos();
        //OnStopReelAnimation(0);
        //OnStopReelAnimation(1);

    }
    public void OnSpinButtonTap()
    {
        IsReelSpinning = true;
        _movementSpeed = 2f;
        foreach (SlotReel rl in reels)
        {
            rl.StartReelAnimation(1 * .1f);
        }

        //StartCoroutine(StopReelAfterSometime());
    }
    public void OnStopReelAnimation(int Id)
    {
        StartCoroutine(StopReelAfterSometime(Id));

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

    private IEnumerator StopReelAfterSometime(int id)
    {
        yield return new WaitForSeconds(0.5f);
        //yield return new WaitForSeconds(Random.Range(2f, 5f));
        IsReelSpinning = false;

        reels[id].StopReelAnimation(id * .5f);
        //for (int i = 0; i < reels.Count; i++)
        //{
        //}
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