using UnityEngine;
using System.Collections;

public class HitStop : MonoBehaviour
{
    bool isWaiting;

    public void HitStopEffect(float duration)
    {
        if (isWaiting)
            return;

        isWaiting = true;
        StartCoroutine(HitStopCoroutine(duration));
    }

    private IEnumerator HitStopCoroutine(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        isWaiting = false;
    }
}