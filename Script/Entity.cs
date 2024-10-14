using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Entity : MonoBehaviour
{

    #region Components

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntitiFX fX { get; private set; }

    public CharacterStats stats { get; private set; }

    public SpriteRenderer sr { get; private set; } //用来为所有物体设置透明隐身等功能
    public CapsuleCollider2D cd { get; private set; }

    #endregion

    [Header("Knockback  info")]
    [SerializeField] protected Vector2 knockbackPower;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnock;


    [Header(" Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;


    public int knockbackDir {  get; private set; } //用来解决 敌人受击方向问题 

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;


    public System.Action onFlipped;    //为了解决反转时候，血条也翻转的情况，  利用Event来解决   Action 系统自带的无返回值的委托

    protected virtual void Awake()
    { 

    }
    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();    
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fX = GetComponent<EntitiFX>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }
    protected virtual void Update()
    {

    }


    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {

    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public virtual void DamageImpact()
    {
        //fX.StartCoroutine("FlashFX");  102 把这个删掉了 ，只在承受伤害的时候使用，放到Characterstats中了 我记得最开始是因为没有写计算伤害的部分，所以在收到攻击判断的时候触发，现在重构了这部分代码
        StartCoroutine("HitKnockback");
    }

    public virtual void SetupKnockbackDir(Transform _damageDirection) //伤害来源方向
    {
        if (_damageDirection.position.x > transform.position.x)
            knockbackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knockbackDir = 1;

    }

    public void SetupKnockbackPower(Vector2 _knockbacckPower) => knockbackPower = _knockbacckPower;


    protected virtual IEnumerator HitKnockback()
    {
        isKnock = true;

        rb.velocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);
        yield return new WaitForSeconds(knockbackDuration);    
        isKnock  = false;
        SetupZeroKnockBackPower();

    }

    protected virtual void SetupZeroKnockBackPower()
    {

    }


    #region Velocity
    public void SetZeroVelocity()
    {
        if (isKnock)
            return;
        rb.velocity = new Vector2(0, 0);  //设置0速度
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnock)
            return; //被攻击时候停顿
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FilpController(_xVelocity);
    }
    #endregion

    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos()  //用来画检测范围的线
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y)); //x y Check
        Gizmos.DrawWireSphere(attackCheck.position,attackCheckRadius);
    }
    #endregion

    #region Filp
    public virtual void Filp() //filp player 
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight; // player filp , then update facingRight
        transform.Rotate(0, 180, 0);

        if(onFlipped != null)  //或者onFlippedEvent?.Invoke();
            onFlipped();
    }
    public virtual void FilpController(float _x) //control how to filp
    {
        if (_x > 0 && !facingRight)
            Filp();
        else if (_x < 0 && facingRight)
            Filp();
    }
    #endregion

    // public void MakeTransprent(bool _transprent)  //黑洞技能时候更新 同时可以为所有Gameobject设置 player变透明 移动到EntityFX 里面了




    public virtual void Die()
    {

    }
}
