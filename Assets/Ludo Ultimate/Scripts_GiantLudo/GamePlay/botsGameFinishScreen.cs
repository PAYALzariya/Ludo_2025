using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class botsGameFinishScreen : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    [Header("ScriptableObjects")]
    public BotPlayersWinnerObjec[] PlayersList;

    [Header("Text")]
    public TextMeshProUGUI Refer;
    [Space]
    [Space]
    public GamePlayScreen gamePlayScreen;
    public Transform boardParent;



    #endregion

    #region PRIVATE_VARIABLES

    #endregion

    #region UNITY_CALLBACKS
    // Use this for initialization
    void OnEnable()
    {
        StartCoroutine(bakctoHome());

    }
    void OnDisable()
    {
        StopCoroutine(bakctoHome());
    }

    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS
    public void SetData(PlayerGameWinBots WinData)
    {
        Reset();
        for (int i = 0; i < WinData.winners.Count; i++)
        {
            PlayersList[i].SetData(WinData.winners[i], WinData.isTournament);
        }

        // NativeShareManager.Instance.TakeScreenshotOfElement(gamePlayScreen.ScreenShotRect, LudoConstants.LudoConstants._screenShotPath);
        this.Open();
        Refer.gameObject.SetActive(WinData.isTournament);
    }
    #endregion

    #region PRIVATE_METHODS
    private void Reset()
    {
        foreach (BotPlayersWinnerObjec bots in PlayersList)
        {
            bots.Reset();
            bots.Close();
        }
    }
    #endregion

    #region COROUTINES
    public IEnumerator bakctoHome()
    {
        yield return new WaitForSeconds(5f);
        if (gameObject.activeInHierarchy)
        {
            Ludo_UIManager.instance.gamePlayScreen.LeaveRoomDone();
        }
    }


    #endregion


    #region GETTER_SETTER


    #endregion



}
