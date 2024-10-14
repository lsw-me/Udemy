using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState  //PlayerGroundState Super state (include idle move)
{
    public PlayerGroundState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        //rb.velocity = new Vector2(0, 0);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();


        if (Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.blackholeUnlocked) //加上解锁技能的判断
            stateMachine.ChangeState(player.blackHole);

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlocked)  //player 没有剑  加上解锁技能的判断
            stateMachine.ChangeState(player.aimSword);

        if (Input.GetKeyDown(KeyCode.Q) && player.skill.parry.parryUnlocked)  //这里好像不解锁技能 也会有红光  如果能不解锁技能就看不到红光，学了技能就看得到就更有趣了
            stateMachine.ChangeState(player.counterAttack);

        if(Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttack);//攻击状态

        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //    stateMachine.ChangeState(player.dashState);  // dash 任意时刻 所以删掉这个

        if(!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);

        if(Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
            stateMachine.ChangeState(player.jumpState);
    }

    private bool HasNoSword()
    {
        if(!player.sword) //有剑执行 28行
        {
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword(); //否则不执行28行，返回false
        return false;
    }
}
