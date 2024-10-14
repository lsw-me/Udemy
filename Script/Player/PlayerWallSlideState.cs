using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player.IsWallDetected() == false)
            stateMachine.ChangeState(player.airState);

        if (Input.GetKeyDown(KeyCode.Space)) //滑墙到进入跳跃状态
        {
            stateMachine.ChangeState(player.wallJump);
            return;                                            //避免执行下面逻辑打断wallJump，所以return
        }
        if(xInput != 0 && player.facingDir != xInput)  //判断是否输入以及输入是否和面朝方向一样
            stateMachine.ChangeState(player.idleState);

        if(yInput < 0 ) //如果有向下方向的输入，下滑速度不变，反之下滑速度减慢
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y * .7f);


        if (player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);
    }
}
