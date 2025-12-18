using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoTemplate
{

    #region PublicVariables
    public static OnPLayerComplete onPlayerGameComplete;
    public PathElement[] path;
    public PartOfBoard parthighlight;
    public Transform[] pawnsInitPos;
    public int id;
    public Animation ProfileHighlightAnim;
    #endregion

    #region PrivateVariable

    [Header("Animation")]
    [SerializeField] private Animation starAnimation;

    [Space]
    [SerializeField] private TextMeshProUGUI txtPlayerName;
    [SerializeField] private Image playerProfilePicture;
    [SerializeField] private Sprite defaultUserSprite;
    [SerializeField] private TurnTimer turnTimer;
    [SerializeField] private GameObject arrow;

    [Space]
    [SerializeField] private PawnControl[] pawns;
    [SerializeField] public DiceController die;

    [Space]
    [SerializeField] private PathType gamePathType = PathType.bottomLeft;
    private int maxSteps = 6;
    private int steps = 0;
    private float timeToComplete = 0;
    private int pawnCompleted = 0;
    //private int activePawnsCounter = 0;
    #endregion

    #region unityCallback
    void OnEnable()
    {
        HighlightPlayer(false);

        die.GetComponent<Image>().sprite = GetGameScreen.DiceDefaultImage;
        arrow.SetActive(false);
        //parthighlight.highlightPartobj.Play("Idle");

        die.SetPlayer = this;
        pawnCompleted = 0;
    }

    void OnDisable()
    {
        pawnCompleted = 0;
    }
    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            if (ProfileHighlightAnim.gameObject.activeSelf)
            {
                parthighlight.highlightPartobj.Play("boardLineAnimation");
            }
            else
            {
                parthighlight.highlightPartobj.Play("Idle");
            }
        }
    }
    #endregion

    #region PublicMethods
    public void StartTimer(float timerToComplete, bool OnTimer = false)
    {
        HighlightPlayer(true);
        timeToComplete = timerToComplete;
        DisableDice(true, true);
        //   SetAllPawnsOnTop();

        if (OnTimer)
        {
            turnTimer.StartTimer(timerToComplete);
        }
    }
    public void CLearPath()
    {
        foreach (PathElement item in path)
        {
            if (item.pawns != null)
            {
                item.pawns.Clear();
            }
        }
    }
    public void HighlightPlayer(bool open)
    {
        if (open)
        {
            ProfileHighlightAnim.Play();
            ProfileHighlightAnim.gameObject.SetActive(true);
       //     parthighlight.highlightPartobj.Play("boardLineAnimation");

        }
        else
        {
            ProfileHighlightAnim.Stop();
            ProfileHighlightAnim.gameObject.SetActive(false);
         //   parthighlight.highlightPartobj.Play("Idle");

        }
    }
    public void SetPlayerData()
    {
        path = GetGameScreen.GetPathManager.GetPathForPawns(gamePathType);
    }
    public void DisableDice(bool b = false, bool border = false)
    {
        if (border)
        {
            //  die.GetComponent<Image>().sprite = Ludo_UIManager.instance.gameScreen.defaultDice;
            parthighlight.highlightPartobj.Play("boardLineAnimation");
        }
        else
        {
            parthighlight.highlightPartobj.Play("Idle");

        }
        arrow.SetActive(b);
    
        die.GetComponent<Button>().interactable = b;

        HighlightPlayer(b);
    }

    public void HighlightPawns(int steps, bool onTimer)
    {
        this.steps = steps;
        turnTimer.StopTimer();
        List<PawnControl> highlighted = new List<PawnControl>();
        DisableDice(false, true);

        foreach (PawnControl item in pawns)
        {
            bool b = steps == maxSteps;
            PawnControl pc = item.HighlightPawn(b, steps);

            if (pc != null)
            {
                highlighted.Add(pc);
            }
        }

        HighlightPlayer(true);
        parthighlight.highlightPartobj.Play("boardLineAnimation");
        //richa
        parthighlight.HighlightBoard(true);
        if (highlighted.Count == 1)
        {
            highlighted[0].MovePawn();
        }
        else if (highlighted.Count > 1)
        {
            PathElement pathElement = null;
            bool isCheck = false;
            for (int i = 0; i < highlighted.Count; i++)
            {
                if (highlighted[i].lastPathElement == null)
                {
                    isCheck = false;
                    break;
                }
                if (pathElement != null)
                {
                    if (highlighted[i].lastPathElement == pathElement)
                    {
                        isCheck = true;
                    }
                    else
                    {
                        isCheck = false;
                        break;
                    }
                }
                else
                {
                    pathElement = highlighted[i].lastPathElement;
                }
            }
            if (isCheck)
            {
                highlighted[0].MovePawn();
            }
            if (onTimer)
            {
                //turnTimer.StartTimer(timeToComplete);
                parthighlight.highlightPartobj.Play("boardLineAnimation");
            }
        }
        else
        {
            
            //richa      parthighlight.HighlightBoard(false);
            HighlightPlayer(false);
            parthighlight.highlightPartobj.Play("Idle");

            GamePlayController.instance.TranspherTurnToNextPlayer();
        }

    }

    public void DisableAllPawns()
    {
        foreach (PawnControl item in pawns)
        {
            item.DeactivatePawn();
        }
    }

    public void StopTimer()
    {
        turnTimer.StopTimer();
    }

    public void AddPawnInHome(PawnControl pawn)
    {
        Ludo_UIManager.instance.soundManager.InHomeSoundOnce();
        pawnCompleted++;
        StartStarAnimation();
        StopStartAnimationAfterComplete();
        if (pawnCompleted >= 4)
        {
            onPlayerGameComplete?.Invoke(this);
            pawnCompleted = 0;
        }
    }

    public PathElement[] GetPathElements(int currentPos)
    {
        PathElement[] elements = path.Subsequence(currentPos, steps);
        return elements;
    }
    public PathElement[] reversePath(int currentPos)
    {
        PathElement[] pathel = path.SubsequenceReverse(currentPos);
        return pathel;
    }

    public void Test()
    {
        onPlayerGameComplete?.Invoke(this);
    }
    #endregion

    #region PrivateVariable
    private void SetAllPawnsOnTop()
    {
        foreach (PawnControl item in pawns)
        {
            item.transform.SetAsLastSibling();
        }
    }

    private void StartStarAnimation()
    {
        starAnimation.gameObject.SetActive(true);
        starAnimation.Play();
    }

    private void StopStartAnimationAfterComplete()
    {
        StartCoroutine(StopingStarAnimation());
    }
    #endregion


    #region Coroutine
    private IEnumerator StopingStarAnimation()
    {
        yield return new WaitForSeconds(1.2f);
        starAnimation.gameObject.SetActive(false);
    }
    #endregion

    #region GetterSetter
    public TurnTimer TurnTimer => turnTimer;
    public PathElement FirstStepPath => path[0];
    public int PathLength => path.Length;
    public string GetPlayerName => txtPlayerName.text;
    public PathType PathType => gamePathType;
    public PawnControl[] SetPawns
    {
        set
        {
            if (value.Length <= 0)
            {
                return;
            }

            pawns = value;

            foreach (PawnControl item in pawns)
            {
                item.player = this;
            }
        }
    }
    public string PlayerName
    {

        set
        {
            txtPlayerName.text = value;
        }
    }
    #endregion
}
public delegate void OnPLayerComplete(PlayerData data);