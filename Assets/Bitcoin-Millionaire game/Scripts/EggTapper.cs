
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

[System.Serializable]
public class TapTool
{
    public string name;
    public int damage;
    public Sprite icon;
    public AudioClip sound;
    public GameObject crackEffectPrefab;  // New
}

public class EggTapper : MonoBehaviour
{
    [Header("Tap System")]
    public int tapsRemaining = 10000;
    public TextMeshProUGUI tapText;

    [Header("Tools")]
    public List<TapTool> tools;
    private int currentToolIndex = 0;
    public List<Button> toolButtons;

    [Header("Audio")]
    public AudioSource tapSound;

    [Header("Visuals")]
    public EggAnimator eggAnimator;

    [Header("Tool Hit Animation")]
    public Image swingToolImage;                   // UI image to show tool sprite
    public Sprite[] toolSprites;                   // Sprites for each tool
    public RectTransform swingToolTransform;       // Move this to swing the tool
    public float swingDuration = 0.4f;
    public GameObject Winpanel;

    private DateTime lastUpdateTime;
  //  public EggDatatUpdateRoot eggDatatUpdateRoot;
    void Start()
    {
        tapsRemaining = PlayerPrefs.GetInt("TapsLeft", tapsRemaining);
        // Load last update time from PlayerPrefs (persistent storage)
        if (PlayerPrefs.HasKey("LastUpdateTime"))
        {
            long binaryTime = Convert.ToInt64(PlayerPrefs.GetString("LastUpdateTime"));
            lastUpdateTime = DateTime.FromBinary(binaryTime);
        }
        else
        {
            lastUpdateTime = DateTime.UtcNow;
            SaveLastUpdateTime();
        }
        UpdateUI();
        // Start waiting until GameId is ready
      //  StartCoroutine(WaitForGameIdAndStartUpdates());
    }
/*
    private IEnumerator WaitForGameIdAndStartUpdates()
    {
        // wait until gameId is not null or empty
        while (string.IsNullOrEmpty(CommonDataFatch.instance.eggGameGetData.data.games[0].gameId))
        {
            yield return null; // wait one frame
        }

        Debug.Log("✅ GameId is ready, starting auto updates...");

        // Start repeating updates every 5 minutes
        InvokeRepeating(nameof(SendClickDataToServer), 1f, 10f);
    }*/
    private void SendClickDataToServer()
    {
     //   eggDatatUpdateRoot.gameId = CommonDataFatch.instance.eggGameGetData.data.games[0].gameId;
      //  eggDatatUpdateRoot.totalClicks = tapsRemaining;
//
      //  EggCrackerGameManager.instance.EggGamePostplayerProgress(eggDatatUpdateRoot);

        Debug.Log("Auto update every 1 minutes sent to server");
       
        SaveLastUpdateTime();
    }
    private void SaveLastUpdateTime()
    {
        lastUpdateTime = DateTime.UtcNow;
        PlayerPrefs.SetString("LastUpdateTime", lastUpdateTime.ToBinary().ToString());
        PlayerPrefs.Save();
    }
  
    private void CheckAndSendClickData()
    {
        // Only update if 5 minutes have passed
       
            Debug.Log("Update click data 5 true ");
           // StartCoroutine(SendClickData());
        SaveLastUpdateTime();
    }

    public void SelectTool(int index)
    {
        currentToolIndex = index;
    }

    public void TapEgg()
    {
        if (tapsRemaining <= 0) return;

        var tool = tools[currentToolIndex];

        // Apply damage
        tapsRemaining--;// tool.damage;
        tapsRemaining = Mathf.Max(tapsRemaining, 0);
        PlayerPrefs.SetInt("TapsLeft", tapsRemaining);

        // Play sound
        tapSound.clip = tool.sound;
        tapSound.Play();

        // Crack animation
        eggAnimator.UpdateEgg(tapsRemaining);

        // Shake tool button
        StartCoroutine(ShakeButton(this.GetComponent<Button>()));

        // Tool fly-in hit animation
      //  StartCoroutine(SwingToolAnimation(currentToolIndex));

        // Update counter
        UpdateUI();

        // Occasionally show ad
        if (UnityEngine.Random.Range(0, 25) == 5)
        {
            Debug.Log("Occasionally show ad");
            AdsManager._adsManager.ShowInterstitialAd();//.ShowRewardedAd();
        }
    }

    void UpdateUI()
    {
        tapText.text = "Taps Left: " + FormatNumber(tapsRemaining);
        if (tapsRemaining <= 0)
        {
           
           CheckAndSendClickData();
            Winpanel.SetActive(true);
        }
        else
        {
            Winpanel.SetActive(false);
        }
    }
    public  void winReward()
    {
        tapsRemaining =100;
        Debug.Log("Reward geting");
        PlayerPrefs.SetInt("TapsLeft", tapsRemaining);
        Winpanel.SetActive(false);
       // EggCrackerGameManager.instance.EggGameReset();
        // Crack animation
        eggAnimator.UpdateEgg(tapsRemaining);
        UpdateUI();
    }
    string FormatNumber(long number)
    {
        if (number >= 1_000_000_000)
            return (number / 1_000_000_000f).ToString("0.#") + "B";
        else if (number >= 1_000_000)
            return (number / 1_000_000f).ToString("0.#") + "M";
        else if (number >= 1_000)
            return (number / 1_000f).ToString("0.#") + "K";
        else
            return number.ToString();
    }

    IEnumerator ShakeButton(Button button, float duration = 0.2f, float magnitude = 10f)
    {
        RectTransform rt = button.GetComponent<RectTransform>();
        Vector3 originalPos = rt.anchoredPosition;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            rt.anchoredPosition = originalPos + new Vector3(x, y,0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = originalPos;
    }

    IEnumerator SwingToolAnimation(int toolIndex)
    {
        swingToolImage.sprite = toolSprites[toolIndex];
        swingToolImage.gameObject.SetActive(true);

        Vector2 startPos = new Vector2(0, 700);
        Vector2 endPos = new Vector2(0, 100);

        float elapsed = 0;
        while (elapsed < swingDuration)
        {
            swingToolTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / swingDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        swingToolTransform.anchoredPosition = endPos;
        yield return new WaitForSeconds(0.1f);

        swingToolImage.gameObject.SetActive(false);
    }
    
}