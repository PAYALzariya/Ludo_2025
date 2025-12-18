using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class SlotReel : MonoBehaviour
{
    #region PUBLIC_VARIABLES

    public int reelNumber;
    public List<Reel> reels;

    #endregion

    #region PRIVATE_VARIABLES

    #endregion

    #region UNITY_CALLBACKS

    // Use this for initialization
    void Start()
    {
    }

    void OnEnable()
    {
        GenerateDuplicateReels();
        Reel.resetReel += HandleResetReel;
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanel.reels[0].reels[0].transform.localPosition = Vector3.zero;
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForThree.reels[0].reels[0].transform.localPosition = Vector3.zero;
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForThree.reels[1].reels[0].transform.localPosition = new Vector3(0f, -800f, 0f);
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.reels[0].reels[0].transform.localPosition = Vector3.zero;
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.reels[1].reels[0].transform.localPosition = new Vector3(0f, -400f, 0f);
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.reels[2].reels[0].transform.localPosition = new Vector3(0f, -607f, 0f);
    }

    void OnDisable()
    {
        //   reels[0].transform.localPosition = Vector3.zero;
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanel.reels[0].reels[0].transform.localPosition = Vector3.zero;
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForThree.reels[0].reels[0].transform.localPosition = Vector3.zero;
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForThree.reels[1].reels[0].transform.localPosition = new Vector3(0f, -800f, 0f);
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.reels[0].reels[0].transform.localPosition = Vector3.zero;
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.reels[1].reels[0].transform.localPosition = new Vector3(0f, -400f, 0f);
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.reels[2].reels[0].transform.localPosition = new Vector3(0f, -607f, 0f);
        Reel.resetReel -= HandleResetReel;
        SetReelsToInitialPos();
        DestroyReels();
    }

    #endregion

    #region DELEGATE_CALLBACKS

    private void HandleResetReel(int reelNumber)
    {
        if (reelNumber != this.reelNumber)
            return;

        ResetReel();
    }

    #endregion

    #region PUBLIC_METHODS

    public void StartReelAnimation(float waitTime)
    {
        StopAllCoroutines();

        for (int i = 0; i < reels[0].reelImagesList.Count; i++)
        {
            reels[3].reelImagesList[i].sprite =reels[1].reelImagesList[i].sprite = reels[2].reelImagesList[i].sprite = reels[0].reelImagesList[i].sprite;
        }

        StartCoroutine("MoveSlotColumn", waitTime);
        //Invoke("RemoveDefaultImg", 0.8f);

    }

    public void StopReelAnimation(float waitTime)
    {
        StartCoroutine("StopAnimtion", waitTime);
    }

    public void ResetReel()
    {
        Reel tempReel = reels[0];
        reels.Remove(tempReel);
        tempReel.transform.localPosition = GetNewPosition();
        reels.Add(tempReel);
    }

    public void SetReelsToInitialPos()
    {
        transform.localPosition = Vector3.zero;
        for (int i = 0; i < reels.Count; i++)
        {
            reels[i].transform.localPosition = GetResetPosition(i);
        }
    }

    //public void speedIncrese()
    //{
    //    StartCoroutine(IncreaseSpeed());
    //}
    #endregion

    #region PRIVATE_METHODS

    private void RemoveDefaultImg()
    {
        foreach (Reel rl in reels)
        {
            rl.defaultImg.gameObject.SetActive(false);
            rl.boxcollision.size = new Vector2(rl.boxcollision.size.x, 1600f);
        }
    }

    private void GenerateDuplicateReels()
    {
       // Debug.Log("generate duplicate reels");
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanel.reels[0].reels[0].transform.localPosition = Vector3.zero;

        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForThree.reels[0].reels[0].transform.localPosition = Vector3.zero;
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForThree.reels[1].reels[0].transform.localPosition = new Vector3(0f, -800f, 0f);

        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.reels[0].reels[0].transform.localPosition = Vector3.zero;
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.reels[1].reels[0].transform.localPosition = new Vector3(0f, -400f, 0f);
        Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.reels[2].reels[0].transform.localPosition = new Vector3(0f, -607f, 0f);

        for (int i = 0; i < 3; i++)
        {
            GameObject s = Instantiate(reels[0].gameObject) as GameObject;
            s.transform.SetParent(transform);
            s.transform.localScale = Vector3.one;

            s.transform.localPosition = GetNewPosition();

            reels.Add(s.GetComponent<Reel>());
        }

        System.Random r = new System.Random();
        List<int> randomNumbers = new List<int>();
        for (int i = 0; i < reels[0].reelSpritesList.Count; i++)
        {
            randomNumbers.Add(i);
        }
        randomNumbers = randomNumbers.OrderBy(x => r.Next()).ToList();

        for (int i = 0; i < reels[0].reelImagesList.Count; i++)
        {
            reels[0].reelImagesList[i].sprite = reels[0].reelSpritesList[randomNumbers[i]];
        }

        for (int i = 0; i < reels[1].reelImagesList.Count; i++)
        {
            reels[1].reelImagesList[i].sprite = reels[1].reelSpritesList[randomNumbers[i]];
        }

        for (int i = 0; i < reels[2].reelImagesList.Count; i++)
        {
            reels[2].reelImagesList[i].sprite = reels[2].reelSpritesList[randomNumbers[i]];
        }
       // reels[0].transform.localPosition = new Vector3(0f,-797f,0f);
    }

    void DestroyReels()
    {       
        Destroy(reels[1].gameObject);
        Destroy(reels[2].gameObject);
        Destroy(reels[3].gameObject);
        reels.RemoveAt(3);
        reels.RemoveAt(2);
        reels.RemoveAt(1);
    }

    private Vector3 GetNewPosition()
    {
        float height = reels[0].GetComponent<RectTransform>().sizeDelta.y;
        return new Vector3(reels[0].transform.localPosition.x, (reels[reels.Count - 1].transform.localPosition.y + height), reels[0].transform.localPosition.z);
    }

    private Vector3 GetResetPosition(int index)
    {
    //    reels[0].transform.localPosition = Vector3.zero;
  //  Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForThree.reels[0].reels[0].transform.localPosition = Vector3.zero;
  //  Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForThree.reels[0].reels[1].transform.localPosition = new Vector3(0f,-800f,0f);
        float height = reels[0].GetComponent<RectTransform>().sizeDelta.y;

        if (index == 0)
            return new Vector3(reels[0].transform.localPosition.x, 0f, reels[0].transform.localPosition.z);
        else
            return new Vector3(reels[0].transform.localPosition.x, (reels[index - 1].transform.localPosition.y + height), reels[0].transform.localPosition.z);
    }

    private void SetCenterReelImage()
    {
        /*
		LuckyDrawResponse luckyDraw = Ludo_UIManager.instance.SSearchingPanel.luckyDrawPanel.luckyDrawResponse;
		int randomNumber = GetRandomNumberForNoPrizeSprite ();

		for (int i = 0; i < reels.Count; i++) {
			Image imgCenter = reels [i].reelImagesList [4];

//			Debug.Log ("THIS IS CENTER  : " + reels [i].reelImagesList [4], reels [i].reelImagesList [4]);
			Ludo_UIManager.Instance.gamePanel.ProfilePicAI.sprite = reels [i].reelImagesList [4].sprite; 
			Ludo_UIManager.Instance.playerAiNo = i;
			
			if (luckyDraw.prize == LudoConstants.APIResponse.LuckyDrawBigPrize)
				imgCenter.sprite = Ludo_UIManager.Instance.SSearchingPanel.luckyDrawPanel.spBigWin;
			else if (luckyDraw.prize == LudoConstants.APIResponse.LuckyDrawStandardPrize)
				imgCenter.sprite = Ludo_UIManager.Instance.SSearchingPanel.luckyDrawPanel.SpRandomSelectedFruit;
		}

		*/
    }

    private int GetRandomNumberForNoPrizeSprite()
    {
        int randomNumber = 0;
        /*
        int randomNumber = Random.Range(0, 4);//Ludo_UIManager.Instance.SSearchingPanel.luckyDrawPanel.spNoWinList.Count);
        while (Ludo_UIManager.Instance.SSearchingPanel.luckyDrawPanel.NoPrizeSpriteGenerated.Contains(randomNumber))
        {
            randomNumber = Random.Range(0, 4);//Ludo_UIManager.Instance.SSearchingPanel.luckyDrawPanel.spNoWinList.Count);
        }
        Ludo_UIManager.Instance.SSearchingPanel.luckyDrawPanel.NoPrizeSpriteGenerated.Add(randomNumber);
        */
        return randomNumber;
    }

    #endregion

    #region COROUTINES

    private IEnumerator MoveSlotColumn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        StartCoroutine("IncreaseSpeed");
        while (true)
        {
            //if (Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanel.gameObject.activeInHierarchy)
            //{
            //    transform.localPosition += Vector3.down * Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanel.MovementSpeed;
            //}
            //else
            if (Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForThree.gameObject.activeInHierarchy)
            {
                transform.localPosition += Vector3.down * Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForThree.MovementSpeed;
            }
            else if (Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.gameObject.activeInHierarchy)
            {
                transform.localPosition += Vector3.down * Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.MovementSpeed;
            }
            else
            {
                transform.localPosition += Vector3.down * Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanel.MovementSpeed;
            }

            yield return 0;
        }
    }

    public IEnumerator IncreaseSpeed()
    {
        yield return new WaitForSeconds(5f);
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime;

            if (Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanel.gameObject.activeInHierarchy)
            {
                Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanel.MovementSpeed = Mathf.Lerp(-10, 25, i);
            }
            else if (Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForThree.gameObject.activeInHierarchy)
            {
                Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForThree.MovementSpeed = Mathf.Lerp(-10, 25, i);
            }
            else if (Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.gameObject.activeInHierarchy)
            {
                Ludo_UIManager.instance.PlayerSearchPanel.luckyDrawPanelForFour.MovementSpeed = Mathf.Lerp(-10, 25, i);
            }
            yield return 0;
        }
    }

    private IEnumerator StopAnimtion(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        StopCoroutine("MoveSlotColumn");
        StopCoroutine("IncreaseSpeed");

        float yPos = 0;
        yPos = transform.localPosition.y.RoundTo(1800f);
        transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
        transform.localPosition = Vector3.zero;

        SetCenterReelImage();

        //		LudoSoundManager.Instance.PlayLuckyDrawStopReelSound ();

        if (reelNumber == 3)
        {
            /*if (Ludo_UIManager.Instance.SSearchingPanel.luckyDrawPanel.luckyDrawResponse == null || Ludo_UIManager.Instance.SSearchingPanel.luckyDrawPanel.luckyDrawResponse.prize != LudoConstants.APIResponse.LuckyDrawNoPrize) {
				Ludo_UIManager.Instance.SSearchingPanel.luckyDrawPanel.imgHeaderBG.sprite = Ludo_UIManager.Instance.SSearchingPanel.luckyDrawPanel.spWinBG;

				foreach (Image img in Ludo_UIManager.Instance.SSearchingPanel.luckyDrawPanel.imgHeaderTextList)
					img.sprite = Ludo_UIManager.Instance.SSearchingPanel.luckyDrawPanel.spYouWinText;
			}*/

            //LudoSoundManager.Instance.StopLuckyDrawSpinSound ();
            yield return new WaitForSeconds(1f);
            //			Ludo_UIManager.Instance.dashboardPanel.luckyDrawPanel.DisplayWinImage ();
        }
    }

    #endregion
}