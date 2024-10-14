using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.skill.sword.DotsActive(true); //生成剑的时候就可以关闭预测线了
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .2f); //协程让人物暂时无法移动，在扔剑和接剑的时候
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity(); //瞄准时候有滑步bug 这样就不能移动了


        if (Input.GetKeyDown(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        if (player.transform.position.x - mousePosition.x > 0 && player.facingDir ==1)
            player.Filp();
        else if(player.transform.position.x - mousePosition.x < 0 && player.facingDir == -1)
            player.Filp();

    }
}
