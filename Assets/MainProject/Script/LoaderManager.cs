using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
public class LoaderManager : MonoBehaviour
{



    [Header("UI References")]

    public GameObject spinner;
    public Image progressFill;
    public TMP_Text loadingText;

    [Header("Animation Settings")]
    public float fadeDuration = 0.5f;
    public float spinnerSpeed = 180f;
    public float dotAnimationInterval;
    public float progressFillSpeed = 0.5f;
    private Tween textTween;

   

    private Tween spinnerTween;

    public void ShowLoader(string message)
    {
        loadingText.text = message;

       
        if (spinner != null)
        {
          
            spinnerTween?.Kill();

       
            spinnerTween = spinner.transform
                .DORotate(new Vector3(0, 0, -360f), 1f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        AnimateLoadingText(message);
    }



    public void HideLoader()
    {
        spinnerTween?.Kill();
        textTween?.Kill();

       

        gameObject.SetActive(false);
    }

    private void AnimateLoadingText(string massage)
    {
        textTween = DOTween.To(() => 0, x =>
        {
            string dots = new string('.', x % 4);
            loadingText.text = massage + dots;
        }, 3, 1f).SetLoops(-1, LoopType.Restart);
    }

    public void LoadSceneWithProgress(string sceneName)
    {
        ShowLoader("Loading");
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    internal void ShowLoaderWithFakeProgress(string message, float timeout = 10f)
    {
        ShowLoader(message);
        StartCoroutine(LoaderProgressRoutine(timeout));
    }
    private IEnumerator LoadSceneAsync(string sceneName)
    {


        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float currentProgress = 0f;

        while (!op.isDone)
        {
            float targetProgress = Mathf.Clamp01(op.progress / 0.9f);

   
            DOTween.To(() => currentProgress, x =>
            {
                currentProgress = x;
                progressFill.fillAmount = currentProgress;
            }, targetProgress, progressFillSpeed);

            if (targetProgress >= 1f)
            {
                yield return new WaitForSeconds(dotAnimationInterval);
                op.allowSceneActivation = true;
            }

            yield return null;
        }

        HideLoader();
    }

    

    private IEnumerator LoaderProgressRoutine(float timeout)
    {
        progressFill.fillAmount = 0f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            elapsed += Time.deltaTime;

            float target = Mathf.Lerp(0f, 0.9f, elapsed / timeout);
            progressFill.fillAmount = target;

            yield return null;
        }
    }
  
}



