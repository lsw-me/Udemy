using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{

    private Transform sword;
    public PlayerCatchSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        sword = player.sword.transform;

        player.fX.PlayDustFx();

        if (player.transform.position.x - sword.position.x > 0 && player.facingDir == 1)
            player.Filp();
        else if (player.transform.position.x - sword.position.x < 0 && player.facingDir == -1)
            player.Filp();

        rb.velocity = new Vector2(player.swordReturnImpact * -player.facingDir,rb.velocity.y); //������һ������Ľӵ������
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .1f);
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

    }
}
