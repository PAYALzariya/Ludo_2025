using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceController : MonoTemplate
{
    #region PublicVariables
    public static OnDiceResult OnResult;
    public int diceValue;

    public Button btnDiceRollPrevention = null;
    #endregion

    #region PrivateVariables

    [SerializeField] private Sprite[] animationSprites;

    [Header("Debuging Only")]
    [SerializeField] private bool b;
    [SerializeField] private int custom;
    public Sprite DefaultSprite;

    [Header("Debug and Testing")]
    [SerializeField] private Toggle debugMode;
    [SerializeField] private InputField diceNumber;
    [SerializeField] private GameObject debugParent;

    PlayerData player;
    public Image diceValueImage;

    private int steps = 0;

    private bool isDiceRolling = false;
    #endregion

    #region UnityCallback

    void OnEnable()
    {
        //debugMode.Close();
        //diceNumber.Close();
        //debugMode.isOn = false;
        //diceNumber.text = "0";

        diceValueImage = GetComponent<Image>();
        GameController.OnDebugModeStart += OpenDebugTool;

        if (btnDiceRollPrevention)
            btnDiceRollPrevention.Open();
    }

    void OnDisable()
    {
        GameController.OnDebugModeStart -= OpenDebugTool;

        isDiceRolling = false;
    }
    #endregion

    #region PublicMethods
    public void SetDiceValueToDefault()
    {
        diceValueImage.sprite = DefaultSprite;
    }
    public void RollDice()
    {
        GetComponent<Button>().interactable = false;
        if (btnDiceRollPrevention)
            btnDiceRollPrevention.Open();

        steps = UnityEngine.Random.Range(1, 7);

#if UNITY_EDITOR
        if (b)
            steps = custom;
#endif

        if (debugMode.isOn)
            steps = Convert.ToInt16(diceNumber.text);

        RollDiceStart();
    }

    public void SetDiceResult(int result)
    {
        if (result < 1 || result > 6)
            result = 6;
        Sprite s = GetGameScreen.GetDiceValue(result);
        StopCoroutine("DiceAnimation");
        isDiceRolling = false;
        diceValueImage.sprite = s;
    }
    public void RollDiceStart()
    {
        StartCoroutine(DiceAnimation(2, SetDiceValue));
    }

    Coroutine routine;
    public int StopDiceTimecounter = 6;

    private string previousDiceKey = "";

    public void StartRollingCoroutine(string diceKey)
    {
        if (diceKey != "" && previousDiceKey == diceKey)
        {
            Debug.Log("Roll dice duplicate event call & handled");
            return;
        }

    //    Debug.Log("XXXX previousDiceKey: " + previousDiceKey);
   //     Debug.Log("XXXX diceKey: " + diceKey);


        previousDiceKey = diceKey;

        if (isDiceRolling)
            return;

        isDiceRolling = true;
        int randomNumber = UnityEngine.Random.Range(6, 7);
        //StopDiceTimecounter = 6;
        //    Debug.Log(randomNumber);
        routine = StartCoroutine(DiceAnimation(randomNumber, () =>
        {
            //StartRollingCoroutine();
            //my comment SetDiceValueToDefault();
            SetDiceResult(diceValue);
            isDiceRolling = false;
        }));

        //if (routine != null)
        //{
        //    StopCoroutine(routine);
        //}
    }

    public void StopRollingCoroutine()
    {
        StopCoroutine(routine);
        //Ludo_UIManager.instance.soundManager.DiceMovementSoundOff();        
        isDiceRolling = false;
    }

    public void StopRollingAndSetValue(int value, string pId)
    {
        Debug.Log("stop roll");
        StopCoroutine("DiceAnimation");
        int randomNumber = UnityEngine.Random.Range(4, 6);
        StartCoroutine(DiceAnimation(6, () =>
        {
            SetDiceResult(value);
        }));
        //if (!pId.Equals(Ludo_UIManager.instance.assetOfGame.SavedLoginData.playerId))
        //{
        //    print("if");
        //}
        //else
        //{
        //    print("else => " + value);
        //    SetDiceResult(value);
        //}
    }
    #endregion

    #region PrivateVariables
    private void OpenDebugTool(bool b)
    {
       

        if (b)
        {
            debugParent.SetActive(true);
        }
        else
        {
            debugParent.SetActive(false);
        }
    }

    private void SetDiceValue()
    {
        SetDiceResult(steps);
        OnResult?.Invoke(steps);
    }
    #endregion

    #region Coroutine
    private IEnumerator DiceAnimation(int stopcounter, Action callback)
    {

        Ludo_UIManager.instance.soundManager.DiceMovementSoundOnce();
        var wait = new WaitForSeconds(0.005f);
        for (int j = 0; j < stopcounter; j++)
        {
            for (int i = 0; i < animationSprites.Length; i++)
            {
                diceValueImage.sprite = animationSprites[i];
                yield return wait;
            }
        }
        yield return wait;
        Ludo_UIManager.instance.soundManager.DiceMovementSoundOff();

        callback();
    }
    #endregion

    #region GetterSetter
    public PlayerData SetPlayer
    {
        set
        {
            player = value;
        }
    }

    #endregion
}
public delegate void OnDiceResult(int result);