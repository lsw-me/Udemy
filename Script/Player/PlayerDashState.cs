using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
   

    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //SkillManager.instance.clone.CreatClone(player.transform); //SkillManager 产生残影  在player中 skill  = SkillManager.instance;  所以这里可以写端点
        //player.skill.clone.CreatClone(player.transform,new Vector3(0,0));//SkillManager 产生残影   ps 后续增加了方法修改写法
        
        player.skill.dash.CloneOnDash(); //技能树 后 改
        stateTimer = player.dashDuration; //dash CD

        player.stats.MakeInvincible(true);
        

    }

    public override void Exit()
    {
        base.Exit();
        player.skill.dash.CloneOnArrival(); 
        player.SetVelocity(0, rb.velocity.y);
        player.stats.MakeInvincible(false);
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsGroundDetected() && player.IsWallDetected())  //解决dash到墙上状态切换延迟问题
            stateMachine.ChangeState(player.wallSlide);



        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if(stateTimer < 0)
            stateMachine.ChangeState(player.idleState);

        player.fX.CreateAfterImage();
    }
}
