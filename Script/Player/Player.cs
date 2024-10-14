using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : Entity
{
    //一次改变，由于player 和enemy有很多一样的代码 改变继承Entity
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;


    public bool isBusy {  get; private set; }

    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;//剑返回的冲击力
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Dash info")]
    //[SerializeField] private float dashCooldown; 更新SkillManager 后删除
    //private float dashUsageTimer; 更新SkillManager 后删除
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;

    public float dashDir {  get; private set; }

    //改动  Collision 共有，移动到entity中

    // 改动filp 相关

    //移动 组件

    public SkillManager skill {  get; private set; }
    public GameObject sword { get; private set; } //设置剑的数量


    #region States
    public PlayerStateMachine stateMachine {  get; private set; } //只读

    public PlayerIdleState idleState {  get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }
    public PlayerWallJumpState  wallJump { get; private set; }
    public PlayerDashState dashState { get; private set; }



    public PlayerCounterAttackState counterAttack { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }

    public PlayerAimSwordState aimSword { get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }

    public PlayerBlackholeState blackHole { get; private set; } 

    public PlayerDeadState deadState { get; private set; }


    #endregion


    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this,stateMachine,"Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState  = new PlayerAirState(this, stateMachine, "Jump"); //视觉上airState和jumpState 没区别
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump  = new PlayerWallJumpState(this, stateMachine, "Jump"); //墙上跳

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSword = new PlayerAimSwordState(this, stateMachine,"AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole = new PlayerBlackholeState(this, stateMachine, "Jump");               //起飞 利用跳跃动画

        deadState = new PlayerDeadState(this, stateMachine, "Die");

    }
    protected override void Start() //初始时候没有状态，所以初始化函数调用一次 赋予初始Idle 状态
    {

        base.Start(); //父类Entity 方法


        skill  = SkillManager.instance;

        stateMachine.Initialize(idleState);


        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
       

    }
    protected override void Update()
    {
        if (Time.timeScale == 0)
            return;

        base.Update();

        stateMachine.currentState.Update();

        CheckForDashInput();



        //Crystal check input
        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
            skill.crystal.CanUseSkill();


        if (Input.GetKeyDown(KeyCode.Alpha1))  // 测试用
            Inventory.instance.UseFlask();
    }
    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)  //寒冷状态使用，减速缓慢人物
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);

    }
    protected override void ReturnDefaultSpeed() //恢复初始速度
    {
        base.ReturnDefaultSpeed(); //父类中有设置动画速度的代码

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;

    }

    public void AssignNewSword(GameObject _newSword)   //人物只能扔一把剑
    {
        sword = _newSword;
    }
    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    //因为修改了退出方式 ，所以ExitBlackHoleAbility 没有用了，但是这里我不删除它，保留 也不影响
    public void ExitBlackHoleAbility() //用来退出黑洞技能
    {
        stateMachine.ChangeState(airState);
    }


    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger(); //在animator 中调用，实现攻击动画播放完毕切换状态

    private void CheckForDashInput()       //dash 检测然后实时调用 方便我们任意状态下切换到dash状态
    {
        //dashUsageTimer -= Time.deltaTime;     //dash  技能内置cd 
       
        if (IsWallDetected()) //站在墙边和wallSlide 状态无法dash
            return;

        if (skill.dash.dashUnlock == false)  //skill tree control
            return;



        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            //dashUsageTimer = dashCooldown; 由于skillmanager 更新去掉这句代码更换使用方式,利用例模式  和SkillManager
            dashDir = Input.GetAxisRaw("Horizontal");

            if(dashDir == 0)                    //dash 时候如果没有输入方向，那么默认朝向面朝方向dash
                dashDir = facingDir;


            stateMachine.ChangeState(dashState);
        }
    }
    //速度代码移动

    //改动同上 移动碰撞代码到Entity

    //filp 代码

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    protected override void SetupZeroKnockBackPower()
    {
        knockbackPower = new Vector2(0, 0);

    }
}
