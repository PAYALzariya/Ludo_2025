using UnityEngine;
using System.Collections;

public class BTCMultiplierManager : MonoBehaviour
{
    public static BTCMultiplierManager Instance;

    [Header("Multiplier Settings")]
    public int currentMultiplier = 1;
    public int maxMultiplier = 10;

    [Header("Boost Settings")]
    public float boostDuration = 5f; // seconds each boost lasts
    public float boostFadeTime = 1f; // smooth fade-out duration

    private bool isBoostActive = false;
    private float boostEndTime;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Check if boost expired
        if (isBoostActive && Time.time > boostEndTime)
        {
            EndBoost();
        }
    }

    /// <summary>
    /// Called from ProgressSliderController when slider fills completely
    /// </summary>
    public void OnClickBoost(int multiplier)
    {
        currentMultiplier = Mathf.Clamp(multiplier, 1, maxMultiplier);
        ActivateBoost();
    }

    void ActivateBoost()
    {
        isBoostActive = true;
        boostEndTime = Time.time + boostDuration;

        Debug.Log($"[BOOST ACTIVE] x{currentMultiplier} for {boostDuration} seconds!");
        // You can trigger UI glow or particle effect here
    }

    void EndBoost()
    {
        isBoostActive = false;
        currentMultiplier = 1; // reset back to normal
        Debug.Log("[BOOST ENDED] Multiplier reset to x1");

        if (ProgressSliderController.Instance != null)
            ProgressSliderController.Instance.ResetMultiplier();
    }

    /// <summary>
    /// Returns the multiplier to apply when adding BTC per click
    /// </summary>
    public int GetCurrentMultiplier()
    {
        if (isBoostActive)
            return currentMultiplier;
        else
            return 1;
    }
}
