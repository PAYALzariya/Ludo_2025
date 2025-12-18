using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PawnControl : MonoTemplate
{
    #region PublicMethods
    public string myPlayerID;
    public static OnPawnPathComplete OnPawnPathCompletes;
    public PlayerData player;
    public Transform parent;
    public Animation StarAnim;
    public int currentDistance = 0;
    #endregion

    #region privateVariables
    [SerializeField] public GameObject highlightAnim;
    [SerializeField] private Image imgPawn;

    private Button button;
    public PathElement lastPathElement;

    public Transform startPosition;

    public bool inStartingPosition = true;
    public bool inHome = false;
    public int pathIndex = 0;

    // player speed
    private const float pawnMovementSpeed = 0.2f;
    public PathElement[] elements;
    private Image imgButton;
    #endregion

    #region UnityCallback
    private void Awake()
    {
        imgButton = GetComponent<Image>();
    }

    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.interactable = false;
        highlightAnim.gameObject.SetActive(false);
        RayCastTarget = false;

        if (elements != null)
            elements.Clone();
    }
    private void OnDisable()
    {
        ResetPawn();
    }

    void OnDrawGizmos()
    {
        if (elements != null)
        {
            Cust_Utility.DrawPathElements(elements);
        }
    }
    #endregion

    #region PublicMethods
    public void SetData(Transform startPosition, bool flag)
    {
        this.startPosition = startPosition;
        this.gameObject.SetActive(flag);
        if (!flag)
        {
            this.transform.position = startPosition.position;
        }
        // Debug.LogError($"{this.name} = Pawn = {startPosition.position} <== LocalPos ==> {startPosition.GetComponent<RectTransform>().anchoredPosition3D}");
    }

    public PawnControl HighlightPawn(bool all, int steps)
    {
        if (!CheckForRemainingSteps(steps) || inHome)
        {
            return null;
        }

        if (all)
        {
            HighlightAndActiveButton();
            return this;
        }
        else if (!inStartingPosition)
        {
            HighlightAndActiveButton();
            return this;
        }
        else
        {
            return null;
        }
        // plr.parthighlight.highlightPartobj.Play("boardLineAnimation");
    }

    public void Kill(PathElement[] Repath = null)
    {
        Ludo_UIManager.instance.soundManager.TokenKillSoundOnce();
        float StepTimeKill = 0f;
        if (Repath == null)
        {
            StepTimeKill = 0.01f;
        }
        else
        {
            StepTimeKill = 0.01f / Repath.Length;
        }

        SendPawnToStartPosition(StepTimeKill, Repath);
        pathIndex = 0;
        lastPathElement = null;
        inStartingPosition = true;
    }

    public void DeactivatePawn()
    {
        button.interactable = false;
        highlightAnim.gameObject.SetActive(false);
        RayCastTarget = false;

        if (lastPathElement != null)
        {
            lastPathElement.ReAdjustPawns();
        }
    }

    public void MovePawn()
    {
        elements = player.GetPathElements(pathIndex);

        player.DisableAllPawns();

        SetPawnPathVariables();

        if (inStartingPosition)
        {
            SendToFirstStep();
        }
        else
        {
            SetPawnOnPath(elements);
        }
    }
    public void ResetPawn(Vector3 position)
    {
        ResetVariables();
        transform.position = position;
        transform.localScale = Vector3.one;
    }
    public void ResetPawn()
    {
        transform.localScale = Vector3.one;
    }

    public void SetIndex(int i)
    {
        Index = i;
    }

    public void AddPawnToPathElement(PathElement pe, bool isSoundCall = true)
    {
        lastPathElement = pe;
        parent = pe.AddPawn(this);
        if (pe.stared)
        {
            if (isSoundCall)
            {
                Ludo_UIManager.instance.soundManager.InstarsafeAreaOnce();
            }
        }
    }

    public void RemovePawnFromPathElement()
    {

        if (lastPathElement != null)
        {
            lastPathElement.RemovePawn(this);
        }

        transform.SetParent(parent);
    }
    #endregion

    #region  PrivateMethods
    private void ResetVariables()
    {
        pathIndex = 0;
        lastPathElement = null;
        inStartingPosition = true;
        inHome = false;
    }
    private void SendPawnToStartPosition(float StepTime, PathElement[] Arr = null)
    {
        Ludo_UIManager.instance.soundManager.TokenMoveSoundOnce();
        try
        {
            Ludo_UIManager.instance.soundManager.Vibrate();
        }
        catch
        {
            print("vibrate error");
        }
        if (Arr != null)
        {
            elements = Arr;
        }
        else
        {
            elements = player.reversePath(pathIndex);
        }

        /* if (Arr.Equals(null))
         {
             elements = player.reversePath(pathIndex);
         }
         else
         {
             elements = Arr;
         }
         */
        StartCoroutine(Cust_Utility.MoveToPath(transform, elements, pawnMovementSpeed / 10, () =>
          {

          }, () =>
          {
              StartCoroutine(Cust_Utility.MoveFromPositionToPositionXY(false, pawnMovementSpeed, transform, startPosition, () => { }));
          }));
    }

    private void SendToFirstStep()
    {
        pathIndex++;
        StartCoroutine(Cust_Utility.MoveFromPositionToPositionXY(false, pawnMovementSpeed, transform, lastPathElement.transform,
        () =>
        {
            parent = lastPathElement.AddPawn(this);
            inStartingPosition = false;
            OnPawnPathCompletes?.Invoke(lastPathElement, this, false);
        }));

    }

    private void SetPawnOnPath(PathElement[] elements)
    {


        StartCoroutine(Cust_Utility.MoveToPath(transform, elements, pawnMovementSpeed, () =>
        {
            Ludo_UIManager.instance.soundManager.TokenMoveSoundOnce();
            try
            {
                Ludo_UIManager.instance.soundManager.Vibrate();
            }
            catch
            {
                print("vibrate error");
            }
        }, () =>
        {
            pathIndex += elements.Length;
            lastPathElement = elements[elements.Length - 1];
            transform.localScale = Vector3.one;
            parent = lastPathElement.AddPawn(this);

            if (pathIndex >= player.PathLength)
            {
                inHome = true;
                player.AddPawnInHome(this);
            }
            OnPawnPathCompletes?.Invoke(lastPathElement, this, inHome);
        }));
    }

    private void HighlightAndActiveButton()
    {
        transform.localScale = Vector3.one;
        if (Ludo_UIManager.instance.gamePlayScreen.gameObject.activeInHierarchy)
        {
            StartCoroutine(OpenHighLightAnimation(0.5f));
        }
        else if (Ludo_UIManager.instance.LocalGameScreen.gameObject.activeInHierarchy)
        {
            button.interactable = true;
            highlightAnim.gameObject.SetActive(true);
            RayCastTarget = true;
        }
    }
    IEnumerator OpenHighLightAnimation(float timer)
    {

        yield return new WaitForSeconds(timer);
        button.interactable = true;
        highlightAnim.gameObject.SetActive(true);
        RayCastTarget = true;
    }

    private void SetPawnPathVariables()
    {
        if (lastPathElement != null)
        {
            lastPathElement.RemovePawn(this);
            transform.parent = parent;
        }

        lastPathElement = player.FirstStepPath;
        player.TurnTimer.StopTimer();
    }
    public bool CheckForRemainingSteps(int steps)
    {
        if (player == null)
            return true;

        int n = pathIndex + steps;
        if (n > player.PathLength)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool IsHighlighted()
    {
        return highlightAnim.activeSelf;
    }

    #endregion


    #region GetterSetter
    public int Index { get; private set; }
    public int PathIndex => pathIndex;

    private bool RayCastTarget
    {
        set
        {
            if (imgButton)
                imgButton.raycastTarget = value;
            else
                Debug.LogError("Custom Error: imgButton not found");

            if (imgPawn)
                imgPawn.raycastTarget = value;
            else
                Debug.LogError("Custom Error: imgPawn not found");
        }
    }
    #endregion

    #region Coroutines
    //public IEnumerator pawnScallar()
    //{
    //    var wait = new WaitForEndOfFrame();
    //    float t = 0;

    //    /*  while (t <= 1)
    //      {
    //          UtilityManager.Instance.ScaleObject(transform.gameObject, 2);
    //          target.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 2.5f, Get0to0From0to1(t));
    //          t += Time.deltaTime / time;

    //          yield return wait;
    //      }*/
    //}
    public IEnumerator StopStarAnim()
    {
        StarAnim.Stop();
        StarAnim.gameObject.SetActive(false);
        yield return new WaitForSeconds(0f);
    }

    #endregion
}

public delegate void OnPawnPathComplete(PathElement lastElement, PawnControl pawn, bool isComplete);

/* [CanEditMultipleObjects]
[CustomEditor(typeof(PawnControl))]
public class PawnEditor : Editor
{

    public override void OnInspectorGUI()
    {
        PawnControl pc = target as PawnControl;
        base.OnInspectorGUI();
        if (GUILayout.Button("SetPath"))
        {
            Debug.Log($"{pc.name}");

            if (pc.GetComponent<LudoPawnController>() == null)
            {
                Debug.LogError($"There is no old data from LudoPawnController");
                return;
            }
            pc.SetPath = pc.GetComponent<LudoPawnController>().path;
        }
    }
} */