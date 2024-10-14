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
        player.skill.sword.DotsActive(true); //���ɽ���ʱ��Ϳ��Թر�Ԥ������
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .2f); //Э����������ʱ�޷��ƶ������ӽ��ͽӽ���ʱ��
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity(); //��׼ʱ���л���bug �����Ͳ����ƶ���


        if (Input.GetKeyDown(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        if (player.transform.position.x - mousePosition.x > 0 && player.facingDir ==1)
            player.Filp();
        else if(player.transform.position.x - mousePosition.x < 0 && player.facingDir == -1)
            player.Filp();

    }
}
