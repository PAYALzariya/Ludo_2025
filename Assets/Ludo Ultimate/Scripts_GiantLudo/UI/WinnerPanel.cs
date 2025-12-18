using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerPanel : MonoBehaviour
{
    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    [Header("Transforms")]
    //public Transform[] SelectedGameParent;
    public GameObject[] SelectedGame;
    public Transform SelectedGameTableList;
    public WinnerData wDataObj;
    public int SelectedGames = 0;


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    //[Header ("Images")]


    //[Header("Text")]


    //[Header ("Prefabs")]

    //[Header ("Enums")]


    //[Header ("Variables")]

    #endregion

    #region PRIVATE_VARIABLES

    #endregion

    #region UNITY_CALLBACKS

    void OnEnable()
    {
        ResetSelectedgameButtons(0);
    }
    void OnDisable()
    {
        Reset();
    }

    #endregion

    #region DELEGATE_CALLBACKS


    #endregion

    #region PUBLIC_METHODS
    public void ResetSelectedgameButtons(int GameSelect)
    {
        foreach (GameObject Obj in SelectedGame)
        {
            Obj.SetActive(false);
        }

        SelectedGames = GameSelect;
        SelectedGame[GameSelect].SetActive(true);
        CallTableListApi();
    }

    public void Reset()
    {
        foreach (Transform item in SelectedGameTableList)
        {
            Destroy(item.gameObject);
        }
    }

    public void CallTableListApi()
    {
        if (SelectedGames.Equals(1))
        {
            callEvent("monthly");
        }
        else if (SelectedGames.Equals(2))
        {
            callEvent("all");
        }
        else
        {
            callEvent("daily");
        }
    }

    #endregion

    #region PRIVATE_METHODS
    private void callEvent(string recordType)
    {

        Ludo_UIManager.instance.OpenLoader(true);
    /*    Ludo_UIManager.instance.socketManager.LeaderBoard("win", recordType, (socket, packet, args) =>
        {
            Debug.Log("GetWithdrawData  : " + packet.ToString());
            Ludo_UIManager.instance.OpenLoader(false);
            JSONArray arr = new JSONArray(packet.ToString());
            string Source;
            Source = arr.getString(arr.length() - 1);
            var resp = Source;
            PokerEventResponse<LeaderboardResultData> GetLeaderboardResultDataResp = JsonUtility.FromJson<PokerEventResponse<LeaderboardResultData>>(resp);

            if (GetLeaderboardResultDataResp.status.Equals(LudoConstants.LudoAPI.KeyStatusSuccess))
            {
                Reset();
                for (int i = 0; i < GetLeaderboardResultDataResp.result.list.Count; i++)
                {

                    WinnerData wDataObjPrefabData = Instantiate(wDataObj) as WinnerData;
                    wDataObjPrefabData.SetData(GetLeaderboardResultDataResp.result.list[i], i);
                    wDataObjPrefabData.transform.SetParent(SelectedGameTableList, false);
                }

            }
            else
            {
                Ludo_UIManager.instance.messagePanel.DisplayMessage(GetLeaderboardResultDataResp.message);
            }
        });*/
    }
    #endregion

    #region COROUTINES



    #endregion


    #region GETTER_SETTER


    #endregion
}
