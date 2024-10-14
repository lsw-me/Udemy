using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : Entity
{
    //һ�θı䣬����player ��enemy�кܶ�һ���Ĵ��� �ı�̳�Entity
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;


    public bool isBusy {  get; private set; }

    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;//�����صĳ����
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Dash info")]
    //[SerializeField] private float dashCooldown; ����SkillManager ��ɾ��
    //private float dashUsageTimer; ����SkillManager ��ɾ��
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;

    public float dashDir {  get; private set; }

    //�Ķ�  Collision ���У��ƶ���entity��

    // �Ķ�filp ���

    //�ƶ� ���

    public SkillManager skill {  get; private set; }
    public GameObject sword { get; private set; } //���ý�������


    #region States
    public PlayerStateMachine stateMachine {  get; private set; } //ֻ��

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
        airState  = new PlayerAirState(this, stateMachine, "Jump"); //�Ӿ���airState��jumpState û����
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump  = new PlayerWallJumpState(this, stateMachine, "Jump"); //ǽ����

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSword = new PlayerAimSwordState(this, stateMachine,"AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole = new PlayerBlackholeState(this, stateMachine, "Jump");               //��� ������Ծ����

        deadState = new PlayerDeadState(this, stateMachine, "Die");

    }
    protected override void Start() //��ʼʱ��û��״̬�����Գ�ʼ����������һ�� �����ʼIdle ״̬
    {

        base.Start(); //����Entity ����


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


        if (Input.GetKeyDown(KeyCode.Alpha1))  // ������
            Inventory.instance.UseFlask();
    }
    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)  //����״̬ʹ�ã����ٻ�������
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);

    }
    protected override void ReturnDefaultSpeed() //�ָ���ʼ�ٶ�
    {
        base.ReturnDefaultSpeed(); //�����������ö����ٶȵĴ���

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;

    }

    public void AssignNewSword(GameObject _newSword)   //����ֻ����һ�ѽ�
    {
        sword = _newSword;
    }
    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    //��Ϊ�޸����˳���ʽ ������ExitBlackHoleAbility û�����ˣ����������Ҳ�ɾ���������� Ҳ��Ӱ��
    public void ExitBlackHoleAbility() //�����˳��ڶ�����
    {
        stateMachine.ChangeState(airState);
    }


    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger(); //��animator �е��ã�ʵ�ֹ���������������л�״̬

    private void CheckForDashInput()       //dash ���Ȼ��ʵʱ���� ������������״̬���л���dash״̬
    {
        //dashUsageTimer -= Time.deltaTime;     //dash  ��������cd 
       
        if (IsWallDetected()) //վ��ǽ�ߺ�wallSlide ״̬�޷�dash
            return;

        if (skill.dash.dashUnlock == false)  //skill tree control
            return;



        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            //dashUsageTimer = dashCooldown; ����skillmanager ����ȥ�����������ʹ�÷�ʽ,������ģʽ  ��SkillManager
            dashDir = Input.GetAxisRaw("Horizontal");

            if(dashDir == 0)                    //dash ʱ�����û�����뷽����ôĬ�ϳ����泯����dash
                dashDir = facingDir;


            stateMachine.ChangeState(dashState);
        }
    }
    //�ٶȴ����ƶ�

    //�Ķ�ͬ�� �ƶ���ײ���뵽Entity

    //filp ����

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
