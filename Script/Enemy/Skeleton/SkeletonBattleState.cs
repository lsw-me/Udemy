using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    private Transform player;
    private Enemy_Skeleton enemy;
    private int moveDir;
    public SkeletonBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        //player = GameObject.Find("Player").transform; 更新不使用find,利用单例模式
        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);
    }


    public override void Update()
    {
        base.Update();

        if(enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime; //这个状态的持续时间（仇恨）
            if(enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if(CanAttack())
                    stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            if(stateTimer < 0 ||Vector2.Distance(player.transform.position ,enemy.transform.position ) > 10 )
                stateMachine.ChangeState(enemy.idleState); 
        }

        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if(player.position.x < enemy.transform.position.x)
            moveDir = -1;

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }
    private bool CanAttack()
    {
        if(Time.time>= enemy.lastTimeAttacked +enemy.attackCooldown )
        {

            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown,enemy.maxAttackCooldown); //随机时间攻击
            enemy.lastTimeAttacked = Time.time;
            return true;
        }
        return false;
    }
}
