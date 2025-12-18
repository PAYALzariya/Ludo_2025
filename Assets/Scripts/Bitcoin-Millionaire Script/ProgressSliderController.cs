using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressSliderController : MonoBehaviour
{
    public static ProgressSliderController Instance;

    [Header("UI References")]
    public Slider slider;
    public TMP_Text milestoneText;
    public TMP_Text multiplierText;

    [Header("Settings")]
    public float fillSpeed = 1f;       // how fast slider fills
    public float decaySpeed = 0.15f;   // how fast slider empties when idle
    public float idleDelay = 1.5f;     // seconds before decay starts
    public double targetBTC = 100;     // milestone target
    public double milestoneMultiplier = 10; // next milestone grows ×10

    private double nextMilestone;
    private double lastBTC;
    private float lastClickTime;

    void Awake()
    {
        Instance = this;
        if (slider == null) slider = GetComponent<Slider>();
        nextMilestone = targetBTC;
        lastBTC = 0;
        lastClickTime = Time.time;
    }

    void Update()
    {
        if (CurrencyManager.Instance == null) return;

        double currentBTC = CurrencyManager.Instance.Bitcoins;

        // Calculate normalized progress
        float targetProgress = (float)(currentBTC / nextMilestone);
        targetProgress = Mathf.Clamp01(targetProgress);

        // Update slider value smoothly
        slider.value = Mathf.MoveTowards(slider.value, targetProgress, fillSpeed * Time.deltaTime);

        // Detect player clicking (BTC increase)
        if (currentBTC > lastBTC)
        {
            lastClickTime = Time.time;
        }

        // If player idle, start decaying slider
        if (Time.time - lastClickTime > idleDelay)
        {
            slider.value = Mathf.MoveTowards(slider.value, 0f, decaySpeed * Time.deltaTime);
        }

        // When slider fills fully, trigger multiplier
        if (slider.value >= 1f)
        {
            if (BTCMultiplierManager.Instance != null)
            {
                BTCMultiplierManager.Instance.OnClickBoost(2); // activate 2× boost
            }

        //    StartCoroutine(ShowMilestoneText($"Milestone Reached! {nextMilestone} BTC"));
            nextMilestone *= milestoneMultiplier; // next goal
            slider.value = 0f; // reset for next round
        }

        // Show multiplier text
        if (multiplierText != null)
            multiplierText.text = $"x{BTCMultiplierManager.Instance?.GetCurrentMultiplier() ?? 1}";

        lastBTC = currentBTC;
    }

    System.Collections.IEnumerator ShowMilestoneText(string msg)
    {
        milestoneText.gameObject.SetActive(true);
        milestoneText.text = msg;
        yield return new WaitForSeconds(1.5f);
        milestoneText.gameObject.SetActive(false);
    }

    public void ResetMultiplier()
    {
        if (multiplierText != null)
            multiplierText.text = "x1";
    }
}
