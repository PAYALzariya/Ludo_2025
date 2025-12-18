using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class RotateWheel : MonoBehaviour
{

    public Vector3 rotation;
    public GameObject plr;


    private void OnEnable()
    {
        InvokeRepeating("scalemanage", 1, 0.9f);
    }
    private void OnDisable()
    {
        plr.transform.localScale = Vector3.one;
        CancelInvoke();

    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation * Time.deltaTime);
    }


    void scalemanage()
    {
        if (plr != null)
        {
            StartCoroutine(MoveFromPositionToPositionXY(0.7f, plr.transform));
        }
        else
        {
            CancelInvoke();
        }
    }


    public IEnumerator MoveFromPositionToPositionXY(float time, Transform from)
    {
        var wait = new WaitForEndOfFrame();
        float t = 0;

        while (t < 1)
        {
            from.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.25f, Get0to0From0to1(t));
            t += Time.deltaTime / time;

            yield return null;

        }
        from.localScale = Vector3.one;
    }

    public static float Get0to0From0to1(float f)
    {
        f = Mathf.Clamp(f, 0, 1);
        return Mathf.Sin(f * Mathf.PI);
    }
}
