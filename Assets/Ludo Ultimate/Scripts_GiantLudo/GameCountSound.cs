using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCountSound : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(StartGameCall(1f));
    }
    private void OnDisable()
    {
        StopCoroutine("StartGameCall");
    }

    public IEnumerator StartGameCall(float timer)
    {
        Ludo_UIManager.instance.soundManager.InGameStartedOnce();
        yield return new WaitForSeconds(timer);
        /*         Ludo_UIManager.instance.soundManager.InGameStartOnce();
                yield return new WaitForSeconds(timer);
                Ludo_UIManager.instance.soundManager.InGameStartOnce();
                yield return new WaitForSeconds(timer);
                Ludo_UIManager.instance.soundManager.InGameStartedOnce(); */
    }
}
