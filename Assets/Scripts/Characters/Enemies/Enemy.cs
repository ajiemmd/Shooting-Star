using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeReference] int scorePoint = 100;//敌人死亡奖励得分

    [SerializeField] int deathEnergyBonus = 3;//敌人死亡奖励的能量

    public override void Die()
    {
        ScoreManager.Instance.AddScore(scorePoint);
        PlayerEnergy.Instance.Obtain(deathEnergyBonus);
        EnemyManager.Instance.RemoveFromList(gameObject);
        base.Die();
    }
}
