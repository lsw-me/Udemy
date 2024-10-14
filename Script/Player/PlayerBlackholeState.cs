using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackholeState : PlayerState
{

    private float flyTime = .4f;
    private bool skillUsed;
    private float defautGravity; 

    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        defautGravity = player.rb.gravityScale;
        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;
    } 

    public override void Exit()
    {
        base.Exit();
        player.rb.gravityScale = defautGravity;
        PlayerManager.instance.player.fX.MakeTransprent(false);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 15);
        if(stateTimer < 0)
        {
            rb.velocity = new Vector2(0,-.1f);
            if(!skillUsed)
            {
                //Debug.Log("CAST  BLACKHOLE");
                if(player.skill.blackhole.CanUseSkill()) 
                    skillUsed = true;
            }
        }

        if (player.skill.blackhole.SkillCompleted())
            stateMachine.ChangeState(player.airState);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////We exit state in blackhole skills controller when all of the attacks are over   <-  （后续被修改了，见相关部分注释）///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
