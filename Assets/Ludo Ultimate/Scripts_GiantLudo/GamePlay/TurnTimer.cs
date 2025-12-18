using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TurnTimer : MonoBehaviour
{
    #region PublicVariables
    public static OnTimerEnds onTimerRunsOut;
    #endregion

    #region Private Variables
    [SerializeField] private Image imageToFill;
    private Coroutine coroutine;
    #endregion

    #region UnityCallback
    void Awake()
    {
        if (imageToFill == null)
            imageToFill = GetComponent<Image>();
    }
    #endregion

    #region PublicMethods
    public void StartTimer(float timer)
    {
        coroutine = StartCoroutine(Cust_Utility.FillWithTime(imageToFill, timer, OnTimerEnds));
    }
    public void StartTimerFromPoint(float currentTime, float totalTime)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(Cust_Utility.FillWithTime(imageToFill, currentTime, totalTime, () => { }));
    }
    public void StopTimer()
    {
        imageToFill.fillAmount = 0;
        if (coroutine == null)
            return;

        StopCoroutine(coroutine);
    }
    #endregion

    #region PrivateMethods
    private void OnTimerEnds()
    {
        onTimerRunsOut?.Invoke();
    }
    #endregion
}

public delegate void OnTimerEnds();