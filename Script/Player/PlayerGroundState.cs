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


        if (Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.blackholeUnlocked) //���Ͻ������ܵ��ж�
            stateMachine.ChangeState(player.blackHole);

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlocked)  //player û�н�  ���Ͻ������ܵ��ж�
            stateMachine.ChangeState(player.aimSword);

        if (Input.GetKeyDown(KeyCode.Q) && player.skill.parry.parryUnlocked)  //������񲻽������� Ҳ���к��  ����ܲ��������ܾͿ�������⣬ѧ�˼��ܾͿ��õ��͸���Ȥ��
            stateMachine.ChangeState(player.counterAttack);

        if(Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttack);//����״̬

        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //    stateMachine.ChangeState(player.dashState);  // dash ����ʱ�� ����ɾ�����

        if(!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);

        if(Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
            stateMachine.ChangeState(player.jumpState);
    }

    private bool HasNoSword()
    {
        if(!player.sword) //�н�ִ�� 28��
        {
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword(); //����ִ��28�У�����false
        return false;
    }
}
