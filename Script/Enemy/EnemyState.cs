using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyState
{
    protected EnemyStateMachine stateMachine; //敌人的状态机
    protected Enemy enemyBase;
    protected Rigidbody2D rb;


    protected string animBoolName;

    protected float stateTimer;
    protected bool triggerCalled;

    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName) //构造函数
    {
        this.enemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }
    public  virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Enter()
    {
        triggerCalled  = false;
        rb = enemyBase.rb;
        enemyBase.anim.SetBool(animBoolName,true);
    }
    public virtual void Exit() 
    {
        enemyBase.anim.SetBool(animBoolName,false);
        enemyBase.AssignLastAnimName(animBoolName);
    }

    public virtual void AnimationFinshTrigger()
    {
        triggerCalled=true;
    }
}
