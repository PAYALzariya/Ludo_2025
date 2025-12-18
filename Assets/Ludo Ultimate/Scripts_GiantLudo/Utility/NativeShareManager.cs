using System;
using System.Collections;
using System.IO;
using UnityEngine;
//using static NativeShare;

public class NativeShareManager : MonoBehaviour
{
    #region PUBLIC_VARIABLES
    public static NativeShareManager Instance = null;
    #endregion

    #region PRIVATE_VARIABLES
    [Header("Canvas")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private string ScreenShotPath;

    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    #endregion

    #region DELEGATE_CALLBACKS
    #endregion

    #region PUBLIC_METHODS
    public void TakeScreenshotOfElement(RectTransform rectTransform, string path)
    {
        ScreenShotPath = path;
        StartCoroutine(TakeScreenshotOfElementIEnum(rectTransform));
    }

   /* public void ShareText(string message, ShareResultCallback callback = null)
    {
        NativeShare nativeShare = new NativeShare();
        nativeShare.SetText(message);
        if (callback != null)
            nativeShare.SetCallback(callback);
        nativeShare.Share();
    }*/
    #endregion

    #region PRIVATE_METHODS
  /*  private void ShareFile(string filePath, string mimeType = null, string message = null, ShareResultCallback callback = null)
    {
        NativeShare nativeShare = new NativeShare();
        nativeShare.AddFile(filePath, mimeType);
        if (message != null)
            nativeShare.SetText(message);
        if (callback != null)
            nativeShare.SetCallback(callback);
        nativeShare.Share();
    }*/
    #endregion

    #region COROUTINES
    IEnumerator TakeScreenshotOfElementIEnum(RectTransform rectT)
    {
        RenderMode tmpRenderMode = canvas.renderMode;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        int width = Convert.ToInt32(rectT.rect.width);
        int height = Convert.ToInt32(rectT.rect.height);

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Vector2 temp = rectT.transform.position;
        // Debug.LogError("temp pos = " + temp);
        var startX = temp.x - width / 2;
        var startY = temp.y - height / 2;

        var tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
        tex.Apply();

        // string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");

        // if (File.Exists(LudoConstants.LudoConstants._screenShotPath))
        //     File.Delete(LudoConstants.LudoConstants._screenShotPath);

        Debug.Log("_screenShotPath.." + ScreenShotPath);
        File.WriteAllBytes(ScreenShotPath, tex.EncodeToPNG());
        Debug.Log("Screenshot Saved..");
        // To avoid memory leaks
        Destroy(tex);


        // #if UNITY_IOS
        //         new NativeShare().AddFile(filePath).SetText("Download virtual online bingo!").SetUrl("https://apps.apple.com/us/app/virtual-online-bingo/id1601797158")
        // #else
        //         new NativeShare().AddFile(filePath).SetText("Download virtual online bingo!").SetUrl("https://play.google.com/store/apps/details?id=com.virtualonlinebingo")
        // #endif
        //         .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
        //         .Share();

        yield return new WaitForEndOfFrame();
        canvas.renderMode = tmpRenderMode;
    }
    #endregion

    #region GETTER_SETTER
    #endregion
}
