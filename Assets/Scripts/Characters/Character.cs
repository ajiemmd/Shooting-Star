using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]GameObject deathVFX;

    [Header("----HEALTH----")]
    [SerializeField] protected float maxHealth;

    [SerializeField] bool showOnHeadHealthBar = true;

    [SerializeField] StateBar onHeadHealthBar;

    protected float health;

    protected virtual void OnEnable()
    {
        health = maxHealth;

        if (showOnHeadHealthBar)
        {
            ShowOnHeadHealthBar();
        }
        else
        {
            HideOnHealthBar();
        }
    }

    public void ShowOnHeadHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(true);
        onHeadHealthBar.Initialize(health, maxHealth);
    }

    public void HideOnHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(false);
    }


    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (showOnHeadHealthBar && gameObject.activeSelf)
        {
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }

        if (health <= 0f)
        {
            Die();
        }
    }


    public virtual void Die()
    {
        health = 0;
        PoolManager.Release(deathVFX, transform.position);
        gameObject.SetActive(false);
    }

    public virtual void RestoreHealth(float value)
    {
        if (health == maxHealth) return;


        //health += value;
        //health = Mathf.Clamp(health, 0f, maxHealth);
        health = Mathf.Clamp(health + value, 0f, maxHealth);

        if (showOnHeadHealthBar)
        {
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }

    }


    /// <summary>
    /// 受伤后在<paramref name="waitTime">秒内部受伤即可回复百分之<paramref name="percent">的血量
    /// </summary>
    /// <param name="waitTime"></param>
    /// <param name="percent"></param>
    /// <returns></returns>
    protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds waitTime, float percent)
    {
        //Debug.Log("回复携程");
        while(health < maxHealth)
        {
            yield return waitTime;
            RestoreHealth(maxHealth * percent);
        }
    }

    protected IEnumerator DamageOverTimeCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (health > 0f)
        {
            yield return waitTime;
            RestoreHealth(maxHealth * percent);
        }
    }



}
