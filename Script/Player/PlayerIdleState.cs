using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.SetZeroVelocity();  //写法影响滑行，进入idle状态时候速度置0；
    }

    public override void Exit()
    { 
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(xInput != 0 &&!player.isBusy && !player.IsWallDetected()|| xInput *player.facingDir < 0)  //解决墙边移动动画冲突bug              
            stateMachine.ChangeState(player.moveState);
    }
}
