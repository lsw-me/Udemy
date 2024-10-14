using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState   //玩家状态基础
{
    protected PlayerStateMachine stateMachine;
    protected Player player;

    protected Rigidbody2D rb;

    protected float xInput;
    protected float yInput; 
    private string animBoolName;

    protected float stateTimer; //  状态Timer dash 和攻击中使用到了
    protected bool triggerCalled;//
    public  PlayerState(Player _player,PlayerStateMachine _stateMachine,string _animBoolName) //  构造函数，下划线代表传入变量  
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter ()
    {
        triggerCalled = false;
        player.anim.SetBool(animBoolName,true);
        rb  = player.rb;
    }
    public virtual void Update ()
    {
        stateTimer -= Time.deltaTime;

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        player.anim.SetFloat("yVelocity",rb.velocity.y);
    }
    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()  //动画触发器
    {
        triggerCalled = true;
    }
}
