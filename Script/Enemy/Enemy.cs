using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{

    [SerializeField]protected LayerMask whatIsPlayer;

    [Header("Stunned info")]
    public float stunDuration;
    public Vector2 stunDriection;
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage; //反击提示 ps ：危 

    [Header("Move info")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime;// 相当于仇恨时间
    private float defaultMoveSpeed;


    [Header("Attack info")]
    public float attackDistance;
    public float attackCooldown;
    public float minAttackCooldown;
    public float maxAttackCooldown;
    [HideInInspector]public float lastTimeAttacked;
    public EnemyStateMachine stateMachine { get; private set; }

    public string lastAnimBoolName {  get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
        defaultMoveSpeed = moveSpeed;
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

    }

    public virtual void AssignLastAnimName(string _animBoolName)  //不同死亡动画不同，所以这里暂时记录 用来死亡时候播放动画
    {
        lastAnimBoolName = _animBoolName;
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);


        Invoke("ReturnDefaultSpeed", _slowDuration);

    }
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
    }


    public virtual void FreezeTime(bool _timeFrozzen)  //冻结时间 用在使用技能的时候
    {
        if(_timeFrozzen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1; 
        }
    }

    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimeCoroutine(_duration));

    protected virtual IEnumerator FreezeTimeCoroutine(float _seconds) //冻结几秒时间
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds); //停止时间

        FreezeTime(false);
    }

    #region Counter Attack
    public virtual void OpenCounterAttackWindow() //开启弹
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }
    public virtual void CloseCounterAttackWindow() //关闭弹
    {
        canBeStunned = false;
        counterImage.SetActive(false);  
    }
    #endregion

    public virtual bool CanBeStunned()  //在具体skeleton中重写
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }
    public virtual void AnimationFinshTrigger() => stateMachine.currentState.AnimationFinshTrigger();

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir,50,whatIsPlayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position ,new Vector3(transform.position.x + attackDistance *facingDir ,transform.position.y));
    }
}
