using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public float cooldown;
    protected float cooldownTimer;

    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;

        CheckUnlock();
    }
    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime; 
    }

    protected virtual  void CheckUnlock()
    {
        // 用于解决读取数据后技能解锁但是无法使用的问题，在每个skill脚本中重写
    }

    public virtual bool CanUseSkill()
    {
        if(cooldownTimer < 0 )
        { 
            // Use Skill
            UseSkill();
            cooldownTimer = cooldown;    
            return true;
        }
        return false;
    }
    public virtual void UseSkill()
    {
        //do some skill spesific things 
    }

    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25); //半径内的所有碰撞

        float closestDistance = Mathf.Infinity;
        
        Transform closestEnemy = null;

        foreach (Collider2D hit in colliders)               //遍历找最近的enemy
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        return  closestEnemy;
    }
}

