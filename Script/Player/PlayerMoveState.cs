using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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


        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        //if (xInput == 0 || player.IsWallDetected())//增加player.IsWallDetected(） 检测到墙时候切换状态，不希望人物冲着墙一直跑  bug 此时移动会出现动画来回切换
        if (xInput == 0 || player.IsWallDetected())
            stateMachine.ChangeState(player.idleState);
    }
}
