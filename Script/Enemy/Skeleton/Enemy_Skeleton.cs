using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Skeleton : Enemy
{

    #region States
    public SkeletonIdleState idleState {  get; private set; }
    public SkeletonMoveState moveState { get; private set; }
    public SkeletonBattleState battleState { get; private set; }
    public SkeletonAttackState attackState { get; private set; }
    public SkeletonStunnedState stunnedState { get; private set; }

    public SkeletonDeadState deadState { get; private set; }
    #endregion
    protected override void Awake()
    {
        base.Awake();
        idleState = new SkeletonIdleState(this,stateMachine,"Idle",this);
        moveState = new SkeletonMoveState(this, stateMachine, "Move", this);
        battleState = new SkeletonBattleState(this, stateMachine, "Move", this); //发现玩家 移动 所以用move就行
        attackState = new SkeletonAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SkeletonStunnedState(this, stateMachine, "Stunned", this);
        deadState = new SkeletonDeadState(this, stateMachine, "Idle", this);

    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        /*
        if(Input.GetKeyDown(KeyCode.U))
        {
            stateMachine.ChangeState(stunnedState);
        }
        */
    }

    public override bool CanBeStunned()  //检查能否反击
    {
        if(base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState); //如果可以被stunnd 改变skeleton状态
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }
}
