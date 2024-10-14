using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{

    public int comboCounter { get; private set; }// attack combo           ����������һ�ι�������effect
    private float lastTimeAttacked;
    private float comboWindow = 2;         //reset combo
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //AudioManager.instance.PlaySFX(2); //������Ч��Ȼ����

        xInput = 0;  // ���bug ��58��Ƶ  �޸����������ϵ�bug

        if (comboCounter > 2 || Time.time >= lastTimeAttacked +comboWindow)
            comboCounter = 0; //���ù�������


        player.anim.SetInteger("ComboCounter",comboCounter);  //���빥��״̬�л���Ӧ����
                                                              //player.anim.speed = 1.2f;

        float attackDir = player.facingDir;  //����ʱ��ʱ�ı乥������

        if (xInput != 0)
            attackDir = xInput;

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y); //���ù���ʱ����΢���ƶ������ݹ���������ͬ���� ʹ��attackMovement���ơ�


        stateTimer = .15f; //����stateTimer ʹ�����й��Եĸо�����������ֹͣ
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .15f);

        //player.anim.speed = 1;

        comboCounter++;
        lastTimeAttacked = Time.time; // ��¼��󹥻�ʱ��
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0) //����.1f���ӳ�ģ�����
           player.SetZeroVelocity();// ����������⣬���ǹ���ʱ���޷��ƶ�
        if (triggerCalled)  //���������� �л�����
            stateMachine.ChangeState(player.idleState);
    }
}
