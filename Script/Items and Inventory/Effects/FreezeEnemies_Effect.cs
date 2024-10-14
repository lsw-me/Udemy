using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Freeze enemies effect", menuName = "Data/Item effect/Freeze enemies")]
public class FreezeEnemies_Effect : ItemEffect
{

    [SerializeField] private float durdtion;


    public override void ExecuteEffect(Transform  _transform)   //从玩家身边生成，freeze 敌人
    {

        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats.currentHealth > playerStats.GetMaxHealthValue() * .1f)
            return;

        if (!Inventory.instance.CanUseArmor())
            return;


        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, 2);

        foreach (Collider2D hit in colliders)
        {
            hit.GetComponent<Enemy>()?.FreezeTimeFor(durdtion);
        }
    }
}
