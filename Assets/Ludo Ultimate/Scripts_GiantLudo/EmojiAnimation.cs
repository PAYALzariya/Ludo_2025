using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class EmojiAnimation : MonoBehaviour
{
    #region PUBLIC_VARIABLES

    public float speedController = 1;
    public int emojiId;
    public Image selectimg;
    public List<Sprite> emojiFramesList;
    public bool pauseOnLastFrame = false;
    public float pauseWaitingTime = 0;
    private bool emojiInit = false;
    #endregion

    #region PRIVATE_VARIABLES

    public Image imgEmoji;

    #endregion

    #region UNITY_CALLBACKS

    void OnEnable()
    {
        //imgEmoji = GetComponent<Image>();
        if (emojiId == 0)
            selectimg.Open();
        else
            selectimg.Close();
       if(emojiInit)
            PlayEmojiAnimation();
    }

    void OnDisable()
    {
        StopEmojiAnimation();
    }

    #endregion

    #region DELEGATE_CALLBACKS

    #endregion

    #region PUBLIC_METHODS
    public void setDatAndOpen(List<Sprite> listdata, int Id, float sController)
    {
        emojiFramesList.Clear();
       
        foreach (Sprite s in listdata)
        {
            emojiFramesList.Add(s);
        }
        speedController = sController;
        emojiId = Id;
        PlayEmojiAnimation();
        emojiInit = true;
    }
    public void OnSendEmojiButtonTap()
    {
        Ludo_UIManager.instance.gamePlayScreen.playerDetailspanel.EmojiId = this.emojiId;
        Ludo_UIManager.instance.gamePlayScreen.playerDetailspanel.DeselectEmoji();
        this.selectimg.Open();
    }

    public void PlayEmojiAnimation()
    {
        StopCoroutine("PlayAnim");
        StartCoroutine("PlayAnim");
    }

    public void StopEmojiAnimation()
    {
        StopCoroutine("PlayAnim");
    }

    #endregion

    #region PRIVATE_METHODS

    #endregion

    #region COROUTINES

    private IEnumerator PlayAnim()
    {
        float waitOnFrames = .025f / speedController;

        while (true)
        {
            int index = 0;
            while (index++ < emojiFramesList.Count - 1)
            {
                imgEmoji.sprite = emojiFramesList[index];
                yield return new WaitForSeconds(waitOnFrames);
            }

            if (pauseOnLastFrame)
                yield return new WaitForSeconds(pauseWaitingTime);
        }
    }

    #endregion
}