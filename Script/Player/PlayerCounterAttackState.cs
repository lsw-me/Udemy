using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{

    private bool canCreateClone; //ȷ������ʱ�� ���㷴��������ˣ�Ҳֻ����һ��Clone
    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        canCreateClone = true;
        stateTimer = player.counterAttackDuration;
        player.anim.SetBool("SuccessfunCounterAttack", false);  //���ö���
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (Collider2D hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    stateTimer = 10;// any value bigger than 1
                    player.anim.SetBool("SuccessfunCounterAttack", true);

                    player.skill.parry.UseSkill();// �мܳɹ��ָ�Ѫ��


                    if (canCreateClone)
                    {
                        canCreateClone = false;
                        //player.skill.clone.CreatClone(hit.transform, new Vector3(2 * player.facingDir, 0)); ����Ϊ��һ��
                        //player.skill.clone.CreatCloneWithDelay(hit.transform);����Ϊ��һ��
                        player.skill.parry.MakeMirageOnParry(hit.transform);
                    }


                }
            }
        }
        if (stateTimer < 0 || triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
