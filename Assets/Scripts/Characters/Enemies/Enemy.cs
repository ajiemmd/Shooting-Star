using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeReference] int scorePoint = 100;//敌人死亡奖励得分

    [SerializeField] int deathEnergyBonus = 3;//敌人死亡奖励的能量

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO:目前为玩家和敌人撞到殉爆，可改为玩家掉血(后续添加功能过载时可直接撞碎小怪)
        //if (collision.gameObject.TryGetComponent<Player>(out Player player))
        //{
        //    player.Die();
        //    Die();
        //}
    }

    public override void Die()
    {
        ScoreManager.Instance.AddScore(scorePoint);
        PlayerEnergy.Instance.Obtain(deathEnergyBonus);
        EnemyManager.Instance.RemoveFromList(gameObject);
        base.Die();
    }
}
