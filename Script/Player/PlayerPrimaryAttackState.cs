using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{

    public int comboCounter { get; private set; }// attack combo           公开最后最后一次攻击产生effect
    private float lastTimeAttacked;
    private float comboWindow = 2;         //reset combo
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //AudioManager.instance.PlaySFX(2); //攻击音效虽然不搭

        xInput = 0;  // 解决bug 看58视频  修复攻击方向上的bug

        if (comboCounter > 2 || Time.time >= lastTimeAttacked +comboWindow)
            comboCounter = 0; //重置攻击连段


        player.anim.SetInteger("ComboCounter",comboCounter);  //进入攻击状态切换对应动画
                                                              //player.anim.speed = 1.2f;

        float attackDir = player.facingDir;  //攻击时随时改变攻击方向

        if (xInput != 0)
            attackDir = xInput;

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y); //设置攻击时有轻微的移动，根据攻击段数不同设置 使用attackMovement控制。


        stateTimer = .15f; //设置stateTimer 使攻击有惯性的感觉，不会立刻停止
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .15f);

        //player.anim.speed = 1;

        comboCounter++;
        lastTimeAttacked = Time.time; // 记录最后攻击时间
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0) //有着.1f的延迟模拟惯性
           player.SetZeroVelocity();// 解决滑步问题，但是攻击时候无法移动
        if (triggerCalled)  //触发器触发 切换动画
            stateMachine.ChangeState(player.idleState);
    }
}
