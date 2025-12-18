using BestHTTP.SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathElement : MonoBehaviour
{

    #region  PublicVariables
    public PathType startPointOf = PathType.none;
    public ElementType type = ElementType.mortalLand;
    public bool stared = false;
    public bool isHome = false;
    public List<PawnControl> pawns;

    #endregion

    #region PrivateVariables
    private HorizontalLayoutGroup layoutGroup;
    private RectTransform rectTransform;
    #endregion

    #region UnityCallback
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        ClearPawns();
    }
    void OnDisable()
    {
        ClearPawns();
    }
    #endregion

    #region PublicMethods
    public Transform AddPawn(PawnControl pawn)
    {
        pawns.Add(pawn);
        Transform t = pawn.transform.parent;
        pawn.transform.SetParent(transform);
     //   Debug.LogError("Come more goti here add");
        ReAdjustPawns();
        return t;
    }

    public void RemovePawn(PawnControl pawn)
    {
        int i = pawns.FindIndex(x => x == pawn);

        if (i > -1)
        {
            pawns[i].transform.SetParent(pawn.parent);
           
            //pawns[i].transform.parent = null;
            pawns[i].transform.localScale = new Vector3(1f, 1f, 1f);
            pawns.RemoveAt(i);
        }

        ReAdjustPawns();
    }

    public bool CheckForKill(PawnControl pawn)
    {
        Debug.Log("NEED to killll");
        if (type == ElementType.immortalLand)
        {
            ReAdjustPawns();
            return false;
        }
        Dictionary<PlayerData, int> playerPawn = new Dictionary<PlayerData, int>();
        foreach (PawnControl itemcount in pawns)
        {
            if (playerPawn.ContainsKey(itemcount.player))
            {
                playerPawn[itemcount.player] += 1;
            }
            else
            {
                playerPawn.Add(itemcount.player, 1);
            }
        }
        List<PawnControl> templist = new List<PawnControl>();
        foreach (PawnControl item in this.pawns)
        {
            if (item == pawn)
            {
                continue;
            }

            if (item.player != pawn.player && (playerPawn[item.player] < 2))//% 2 != 0))
            {
                Debug.Log("NEED to killll   2");
                item.Kill();
                templist.Add(item);
                item.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        foreach (PawnControl item in templist)
        {
            RemovePawn(item);
        }
        ReAdjustPawns();

        return templist.Count > 0;
    }
    #endregion

    #region PrivateMethods
    private void ClearPawns()
    {
        if (pawns == null)
        {
            pawns = new List<PawnControl>();
        }
        else
        {
            pawns.Clear();
        }
    }
    private void RefreshPawnListFromChildren()
    {
        pawns.Clear();
        foreach (Transform t in transform)
        {
            PawnControl pc = t.GetComponent<PawnControl>();
            if (pc != null)
            {
                pawns.Add(pc);
            }
        }
    }
    public void ReAdjustPawns()
    {

        int length = pawns.Count;
   //     RefreshPawnListFromChildren();
        if (length == 0)
        {
            Destroy(layoutGroup);
            return;
        }
        else
        {
            AddHrLayout();

            /*if ((length > 1) || (type == ElementType.home))
              {

              }
              else
              {
                  foreach (PawnControl item in pawns)
                  {
                      item.transform.localScale = Vector3.one;
                  }
              }*/
            float f = (0.1f * (length - 1));
            foreach (PawnControl item in pawns)
            {
                item.transform.localScale = Vector3.one - (Vector3.one * f);


            }
        //    Debug.Log("Call ReAdjustPawns " + length);
            // SAFE LOOP (length - 1)
           if ( pawns.Count >=1 && length >=2 )
            {
                for (int i = 0; i < length - 1; i++)
                {
                    Debug.Log($"--- Checking Pair Index {i} & {i + 1} ---");

                    PawnControl p1 = null;
                    PawnControl p2 = null;

                    // Try to get from list
                    if (i < pawns.Count)
                    {
                        p1 = pawns[i];
                        Debug.Log($"List p1 = {(p1 != null ? p1.name : "NULL")}");
                    }

                    if (i + 1 < pawns.Count)
                    {
                        p2 = pawns[i + 1];
                        Debug.Log($"List p2 = {(p2 != null ? p2.name : "NULL")}");
                    }

                    // Fallback: get from transform children
                    if (p1 == null)
                    {
                        if (i < transform.childCount)
                        {
                            p1 = transform.GetChild(i).GetComponent<PawnControl>();
                            Debug.Log($"Fallback child p1 = {(p1 != null ? p1.name : "NULL")}");
                        }
                        else
                        {
                            Debug.LogWarning($"NO CHILD FOUND for p1 index {i}");
                        }
                    }

                    if (p2 == null)
                    {
                        if ((i + 1) < transform.childCount)
                        {
                            p2 = transform.GetChild(i + 1).GetComponent<PawnControl>();
                            Debug.Log($"Fallback child p2 = {(p2 != null ? p2.name : "NULL")}");
                        }
                        else
                        {
                            Debug.LogWarning($"NO CHILD FOUND for p2 index {i + 1}");
                        }
                    }

                    // If still null, skip
                    if (p1 == null || p2 == null)
                    {
                        Debug.LogError($"❌ Skipping kill check at index {i} because p1 or p2 is NULL");
                        continue;
                    }

                    Debug.Log("p1 PlayerID= " + p1.myPlayerID +" Token=" +p1.Index);
                    Debug.Log("p2 PlayerID="+  p2.myPlayerID +" Token=" + p2.Index);

                    // Different player?
                    if (p1.myPlayerID != p2.myPlayerID)
                    {
                        Debug.Log("✔ Different players found!");

                        // Different token?
                        if (p1.Index != p2.Index)
                        {
                            Debug.Log("✔ Different tokens found → Kill Condition!");

                            string attackerPlayerId = "0";
                            int attackerTokenId = 0;
                            string killedPlayerId = "0";
                            int killedTokenId = 0;

                            string myId =(ServerSocketManager.instance.playerId);

                            // Who is attacker?
                            if (p1.myPlayerID == myId)
                            {
                                Debug.Log("Attacker = p1 (local player)");
                                attackerPlayerId = myId.ToString();
                                attackerTokenId = p1.Index;

                                killedPlayerId = p2.myPlayerID;
                                killedTokenId = p2.Index;
                            }
                            else if (p2.myPlayerID == myId)
                            {
                                Debug.Log("Attacker = p2 (local player)");
                                attackerPlayerId = myId.ToString();
                                attackerTokenId = p2.Index;

                                killedPlayerId = p1.myPlayerID;
                                killedTokenId = p1.Index;
                            }
                            else
                            {
                                Debug.LogWarning("⚠ No local player involved → skip kill emit");
                                continue;
                            }

                            Debug.Log($"🔥 Sending Kill Emit:  attacker={attackerPlayerId}/{attackerTokenId},  killed={killedPlayerId}/{killedTokenId}");

                            Ludo_UIManager.instance.socketManager.KillPlayer(
                                attackerPlayerId,
                                attackerTokenId,
                                killedPlayerId,
                                killedTokenId,
                                ResponseOfPlayerKill
                            );
                        }
                        else
                        {
                            Debug.Log("Same token index → no kill.");
                        }
                    }
                    else
                    {
                        Debug.Log("Same player → no kill.");
                    }
                }
            }
            else
            {
         //       Debug.Log("Call else length " + length);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
    private void ResponseOfPlayerKill(Socket socket, Packet packet, object[] args)
    {
        Debug.Log(Ludo_Constants.LudoEvents.PlayerAction + " respnose  : " + packet.ToString());
    }
    private void AddHrLayout()
    {
        if (layoutGroup != null)
            return;

        layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
    private void SetPositionOfSinglePawn()
    {
        PawnControl pc = pawns[0];
        pc.transform.position = transform.position;
        pc.transform.localScale = Vector3.one;
    }
    #endregion
}

public enum PathType
{
    topRight,
    topLeft,
    bottomRight,
    bottomLeft,
    none,
}

public enum ElementType
{
    immortalLand,
    mortalLand,
    diversen,
    home,
}