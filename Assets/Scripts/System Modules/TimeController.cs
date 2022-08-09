using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : Singleton<TimeController>
{
    [SerializeField, Range(0f, 1f)] float bulletTimeScale = 0.1f;

    float defaultFixedDeltaTime;

    float t;

    protected override void Awake()
    {
        base.Awake();
        defaultFixedDeltaTime = Time.fixedDeltaTime;

    }

    public void BulletTime(float duration)
    {
        Time.timeScale = bulletTimeScale;
        StartCoroutine(nameof(SlowOutCoroutine), duration);
    }

    public void BulletTime(float induration, float outDuration)
    {
        StartCoroutine(SlowInAndOutCoroutine(induration,outDuration));
    }

    public void BulletTime(float inDuration, float keepingDuration, float outDuration)
    {
        StartCoroutine(SlowInKeepAndOutCoroutine(inDuration, keepingDuration, outDuration));
    }

    public void SlowIn(float duration)
    {
        StartCoroutine(SlowInCoroutine(duration));
    }

    public void SlowOut(float duration)
    {
        StartCoroutine(SlowOutCoroutine(duration));
    }
    
    /// <summary>
    /// 玩家进入子弹子弹时间后缓速持续一段时间
    /// </summary>
    /// <returns></returns>
    IEnumerator SlowInKeepAndOutCoroutine(float inDuration, float keepingDuration, float outDuration)
    {
        yield return StartCoroutine(nameof(SlowInCoroutine), inDuration);
        yield return new WaitForSecondsRealtime(keepingDuration);

        StartCoroutine(nameof(SlowOutCoroutine), outDuration);
    }



    IEnumerator SlowInAndOutCoroutine(float inDuration, float outDuration)
    {
        yield return StartCoroutine(nameof(SlowInCoroutine), inDuration);

        StartCoroutine(nameof(SlowOutCoroutine), outDuration);
    }

    IEnumerator SlowInCoroutine(float duration)
    {
        t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration; // 不能使用Time.deltaTime，会受到时间刻度影响
            Time.timeScale = Mathf.Lerp(1f, bulletTimeScale, t);
            Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;

            yield return null;
        }
    }


    IEnumerator SlowOutCoroutine(float duration)
    {
        t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration; // 不能使用Time.deltaTime，会受到时间刻度影响
            Time.timeScale = Mathf.Lerp(bulletTimeScale, 1f, t);
            Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;

            yield return null;
        }
    }

}
