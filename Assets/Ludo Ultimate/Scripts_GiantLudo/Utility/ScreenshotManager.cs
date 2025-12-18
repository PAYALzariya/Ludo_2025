using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{

    public string ssName = "apple";
    public int index = 1;

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string sharingImagePath = Path.Combine(Application.dataPath, "imageName.png");
            ScreenCapture.CaptureScreenshot(sharingImagePath);
            Debug.LogError(sharingImagePath);
        }
    }
#endif
}
