using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateBar : MonoBehaviour
{
    [SerializeField] Image fillImageBack; //缓冲血槽
    [SerializeField] Image fillImageFront;//真实血槽

    [SerializeField] bool delayFill = true;
    [SerializeField] float fillDelay = 0.5f;

    [SerializeField] float fillspeed = 0.1f;//血槽变化速度(变化到终点的时长)

    float currentFillAmount;
    protected float targetFillAmount;

    float t;//协程中的计时器

    WaitForSeconds waitForDelayFill;

    Coroutine bufferedFillingCoroutine;

    Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public virtual void Initialize(float currentValue, float maxValue)
    {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;
        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    public void UpdateStats(float currentValue, float maxValue)
    {
        targetFillAmount = currentValue / maxValue;

        if (bufferedFillingCoroutine != null)
        {
            StopCoroutine(bufferedFillingCoroutine);
        }

        //if stats reduce 当状态值减少时
        if (currentFillAmount > targetFillAmount)
        {
            //fill image front = target fill amount 真实血条的值 = 目标填充值
            fillImageFront.fillAmount = targetFillAmount;
            //slowly reduce fill image back's fill amount 慢慢减少缓冲血槽的值到目标值
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageBack));
            
            return;
        }
        //if stats increase  当状态值增加时
        if (currentFillAmount < targetFillAmount)
        {
            //fill image back = target fill amount 缓冲血槽的值 = 目标填充值
            fillImageBack.fillAmount = targetFillAmount;
            //slowy increase fill image front's fill amount 再慢慢增加真实血条的值
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageFront));
        }
    }

    protected virtual IEnumerator BufferedFillingCoroutine(Image image)
    {
        if (delayFill)
        {
            yield return waitForDelayFill;
        }


        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * fillspeed;
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount,t);
            image.fillAmount = currentFillAmount;

            yield return null;
        }


    }


}
