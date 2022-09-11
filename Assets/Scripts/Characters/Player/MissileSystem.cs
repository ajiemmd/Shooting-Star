using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    [SerializeField] int defaultAmount = 5;

    [SerializeField] float cooldownTime = 1f;

    [SerializeField] GameObject missilePrefab = null;

    [SerializeField] AudioData launchSFX = null;

    bool isReady = true;

    int amount;

    private void Awake()
    {
        amount = defaultAmount;
    }

    private void Start()
    {
        MissileDisplay.UpdateAmountText(amount);
    }

    public void Launch(Transform muzzleTransform)
    {
        if (amount == 0 || !isReady) return; //播放没导弹可发射的提升音效 或 使UI动态变化来提醒玩家没导弹了。

        isReady = false;
        //对象池中取一个导弹
        PoolManager.Release(missilePrefab,muzzleTransform.position);
        //播放导弹发射音效
        AudioManager.Instance.PlayRandomSFX(launchSFX);
        amount--;
        MissileDisplay.UpdateAmountText(amount);

        if (amount == 0)
        {
            MissileDisplay.UpdateCoolDownImage(1f);//没导弹时，图标变红
        }
        else
        {
            //导弹发射进入冷却
            StartCoroutine(CooldownCoroutine());
        }
    }

    IEnumerator CooldownCoroutine()
    {
        var cooldownValue = cooldownTime;

        while(cooldownValue > 0f)
        {
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime, 0f);
            MissileDisplay.UpdateCoolDownImage(cooldownValue / cooldownTime);
            yield return null;
        }

        isReady = true;
    }
    
}
