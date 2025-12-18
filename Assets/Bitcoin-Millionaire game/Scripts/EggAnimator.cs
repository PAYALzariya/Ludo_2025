
using UnityEngine;
using UnityEngine.UI;

public class EggAnimator : MonoBehaviour
{
    public Image eggSprite;
    public Sprite[] eggStages;

    public int maxTaps = 100;

    public void UpdateEgg(int tapsRemaining)
    {
        int stageCount = eggStages.Length;
        int stage = Mathf.Clamp((maxTaps - tapsRemaining) * stageCount / maxTaps, 0, stageCount - 1);
       // Debug.Log("stage: " + stage);
        eggSprite.sprite = eggStages[stage];
    }
}
