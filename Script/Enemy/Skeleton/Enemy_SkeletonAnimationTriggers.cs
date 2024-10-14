using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SkeletonAnimationTriggers : MonoBehaviour
{
    private Enemy_Skeleton enemy => GetComponentInParent<Enemy_Skeleton>();

    private void AnimationTrigger()
    {
        enemy.AnimationFinshTrigger();
    }
    private void Damage()
    {
        //÷¼÷Ã¹¥»÷
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position,enemy.attackCheckRadius);

        foreach (Collider2D collider in colliders)
        {
            if(collider.GetComponent<Player>() !=null)
            {
                PlayerStats target = collider.GetComponent<PlayerStats>();
                enemy.stats.DoDamage(target);
                //collider.GetComponent<Player>().Damage();
            }
        }
    }

    private void OpenCounterWindow() => enemy.OpenCounterAttackWindow();
    private void CloseCounterWindow() => enemy.CloseCounterAttackWindow();
}
