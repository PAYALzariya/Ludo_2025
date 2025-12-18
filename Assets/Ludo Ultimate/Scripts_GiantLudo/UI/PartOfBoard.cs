using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartOfBoard : MonoTemplate
{
    #region PublicVariables
    public Image WinnerPosition;
    public Image PlayerLeftImage;
    public PawnControl[] pawns;
    #endregion

    #region  PrivateVariables
    [SerializeField] private GameObject highlightPart;
    public Animator highlightPartobj;
    [SerializeField] public Transform pawnsParent;

    [Header("Editor Position Change and Color")]
    [SerializeField] private int colorIndex;
    [SerializeField] private int position;
    private float initialAngle = 90f;

    public List<Vector3> oldPOsitions;
    #endregion

    #region UnityCallback
    void OnEnable()
    {
        //highlightPartobj = this.gameObject.GetComponent<Animator>();
        PawnControl.OnPawnPathCompletes += OnPawnPathCompletes;
        WinnerPosition.Close();
        PlayerLeftImage.Close();
    }
    /*void OnValidate()
    {
        if (Application.isEditor)
            SetPartAtPosition(position);
    }
    */
    void OnDisable()
    {
        ResetBoard();
    }
    #endregion

    #region PublicMethods
    public void SetPartAtPosition(int i)
    {
        float angle = i * 90 * -1;
        Vector3 angles = transform.eulerAngles;
        angles.z = initialAngle + angle;
        transform.eulerAngles = angles;

        angles = pawnsParent.localEulerAngles;
        angles.z = (initialAngle + angle) * -1;
        pawnsParent.localEulerAngles = angles;
        WinnerPosition.transform.localEulerAngles = angles;
        position = i;
    }

    public void OpenPawns(bool flag, int pid, PlayerData pplayers)
    {
        //Debug.Log($"Opening the pawns: <color=red>{flag}</color>");
        for (int i = 0; i < pawns.Length; i++)
        {
            PawnControl item = pawns[i];
            if (flag)
            {
                item.Open();
                //item.startPosition = item.transform;
                item.startPosition = pplayers.pawnsInitPos[i];
            }
            else
            {
                item.Close();
            }
        }
        /* Debug.Log($"Value: {position} || {flag}", gameObject);
        pawnsParent.GetComponent<RectTransform>().anchoredPosition = (flag) ? GetGameScreen.pawnOpenPosition : GetGameScreen.pawnClosePositions[position]; */
    }
    public void SetNewParent(Transform parent)
    {
        if (oldPOsitions == null)
            oldPOsitions = new List<Vector3>();

        oldPOsitions.Clear();
        foreach (PawnControl item in pawns)
        {
            oldPOsitions.Add(item.transform.position);
            item.transform.SetParent(parent);
        }
    }
    #endregion

    #region PrivateMethods
    public void HighlightBoard(bool open)
    {
        if (open)
        {
            //highlightPart.SetActive(true);
            highlightPartobj.Play("boardLineAnimation");

        }
        else
        {
            highlightPartobj.Play("Idle");
            //highlightPart.SetActive(false);
        }
    }

    public void ResetBoard()
    {
        //Debug.Log($"Resetting the whole board");


        // Debug.LogError("In Reset board..11 ");
        for (int i = 0; i < pawns.Length; i++)
        {
            PawnControl pawn = pawns[i];
            // Debug.LogError("In Reset board..22 ");

            if (pawn.transform.parent == pawnsParent)
                continue;

            // Debug.LogError("In Reset board..33 ");
            pawn.transform.SetParent(pawnsParent);
            // Debug.LogError("In Reset board..44 ");
            pawn.ResetPawn(oldPOsitions[i]);
        }

        if (colorIndex == 1)
        {
            WinnerPosition.rectTransform.anchoredPosition = new Vector3(-57f, 153f, 0f);
            //   PlayerLeftImage.rectTransform.anchoredPosition = new Vector3(-57f, 153f, 0f);
        }

    }

    void OnPawnPathCompletes(PathElement pathElement, PawnControl pawnControl, bool b)
    {
        int temp = Array.FindIndex(pawns, x => x == pawnControl);
        if (temp > -1)
        {
            //richa      HighlightBoard(false);
        }
    }
    #endregion

    #region  GetterSetter
    public int ColorIndex => colorIndex;
    public int Position => position;
    public PawnControl[] GetPawns => pawns;
    #endregion
}
