using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class skipStepScreen : MonoBehaviour
{

    #region PUBLIC_VARIABLES

    //[Header ("Gamobjects")]

    //[Header ("Transforms")]


    //[Header ("ScriptableObjects")]


    //[Header ("DropDowns")]


    [Header("Images")]
    public Sprite green;
    public Sprite red;
    public Image[] SkipSteps;

    //[Header ("Text")]


    //[Header ("Prefabs")]

    //[Header ("Enums")]


    //[Header ("Variables")]

    #endregion

    #region UNITY_CALLBACKS

    // Use this for initialization
    void OnEnable()
    {
        StartCoroutine(stoppopup(4f));
    }

    void OnDisable()
    {
        Reset();
    }
    
    #endregion

    #region PUBLIC_METHODS

    public void closeButtonTap()
    {
        this.Close();
    }

    public void SetData(string playerid, string boardId)
    {
        Ludo_UIManager.instance.socketManager.GetPlayerDefaultTurnCount(playerid, boardId, (socket, packet, args) =>
         {
             Debug.Log(Ludo_Constants.LudoEvents.GetPlayerDefaultTurnCount + " respnose  : " + packet.ToString());
             /*
                          JSONArray arr = new JSONArray(packet.ToString());
                          string Source;
                          Source = arr.getString(arr.length() - 1);
                          var resp = Source;*/

             JSONArray arr = new JSONArray(packet.ToString());
             string Source = arr.getString(arr.length() - 1); // this already gives you the inner JSON array as string
             JSONArray jsonArr = new JSONArray(Source);
             string firstObj = jsonArr.getString(0); // extract the first object inside the array


             PokerEventResponse<skipstepsresp> DataResp = JsonUtility.FromJson<PokerEventResponse<skipstepsresp>>(firstObj);

             if (DataResp.status.Equals(Ludo_Constants.LudoAPI.KeyStatusSuccess))
             {
                 SetSteps(DataResp.result.turnCount);
                 this.Open();
             }
             else
             {
                 Ludo_UIManager.instance.messagePanel.DisplayMessage(DataResp.message);
                 this.Close();
             }

         });
    }

    #endregion

    #region PRIVATE_METHODS

    private void Reset()
    {
        foreach (Image img in SkipSteps)
        {
            img.sprite = green;
        }
    }

    private void SetSteps(int Id)
    {
        Reset();
        if (Id.Equals(0))
        {
            Reset();
            return;
        }
        else
        {
            for (int i = 0; i <= Id - 1; i++)
            {
                SkipSteps[i].sprite = red;
            }
        }
    }

    #endregion

    #region COROUTINES

    public IEnumerator stoppopup(float timer)
    {
        yield return new WaitForSeconds(timer);
        this.closeButtonTap();
    }

    #endregion

}
