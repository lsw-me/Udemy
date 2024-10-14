using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{

    private Player player => GetComponentInParent<Player>();
    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        //攻击触发器， attackCheck 圆中可能会有很多，所以全部注册

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (Collider2D hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();
                if (_target != null)                           // p117 最后bug
                    player.stats.DoDamage(_target);


            /*    //hit.GetComponent<Enemy>().Damage();  //如果遍历的过程中hit能获取到敌人组件，触发伤害函数 p90 更新 所有的damage()函数都要重构
                //hit.GetComponent<CharacterStats>().TakeDamage(player.stats.damage.GetValue()); // p89 更新伤害  后续重构为一个函数


                //执行逻辑 inventory  拿到武器，执行item效果
                //Inventory.instance.GetEquipment(EquipmentType.Wepon).Effect(_target.transform);  //bug 不装武器也会调用，会有一个报错 没有实例化的报错
            */   
                
                
                ItemData_Equipment weponData = Inventory.instance.GetEquipment(EquipmentType.Wepon);  //解决上边bug
                if (weponData != null)
                {
                    weponData.Effect(_target.transform);
                }

            }
        }
    }





    private void ThrowSword()
    {
        SkillManager.instance.sword.CreatSword();
    }
}
