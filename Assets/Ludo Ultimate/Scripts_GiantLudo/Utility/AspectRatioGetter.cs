using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioGetter : MonoBehaviour
{
    #region Variables
    public static AspectRatioGetter Instance;
    // public static event Action<DeviceStyle> OnAspectRationGet;
    public DeviceStyle deviceStyle = DeviceStyle.Normal;
    public float currentAspectRatio;

    #endregion

    #region Unity defaul methods
    void Awake()
    {
        if (Instance == null)
            Instance = this;

        GetAspectRatio();
    }

    private void GetAspectRatio()
    {

        currentAspectRatio = (float)Screen.height / (float)Screen.width;
        // > 1.8 tall devices
        // <= 1.8 normal devices

        if (currentAspectRatio < 1.8f)
        {
            // this will be normal device
            deviceStyle = DeviceStyle.Normal;
        }
        else if (currentAspectRatio >= 1.8f)
        {
            // this will be tall device
            deviceStyle = DeviceStyle.Tall;
        }


        // OnAspectRationGet?.Invoke(deviceStyle);
    }


    #endregion


}

[System.Serializable]
public enum DeviceStyle { Normal, Tall }
