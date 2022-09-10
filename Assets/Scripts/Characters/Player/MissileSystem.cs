using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    [SerializeField] GameObject missilePrefab = null;

    [SerializeField] AudioData launchSFX = null;


    public void Launch(Transform muzzleTransform)
    {
        //对象池中取一个导弹
        PoolManager.Release(missilePrefab,muzzleTransform.position);

        //播放导弹发射音效
        AudioManager.Instance.PlayRandomSFX(launchSFX);
    }
}
