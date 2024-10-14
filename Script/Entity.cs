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

    public SpriteRenderer sr { get; private set; } //����Ϊ������������͸������ȹ���
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


    public int knockbackDir {  get; private set; } //������� �����ܻ��������� 

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;


    public System.Action onFlipped;    //Ϊ�˽����תʱ��Ѫ��Ҳ��ת�������  ����Event�����   Action ϵͳ�Դ����޷���ֵ��ί��

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
        //fX.StartCoroutine("FlashFX");  102 �����ɾ���� ��ֻ�ڳ����˺���ʱ��ʹ�ã��ŵ�Characterstats���� �Ҽǵ��ʼ����Ϊû��д�����˺��Ĳ��֣��������յ������жϵ�ʱ�򴥷��������ع����ⲿ�ִ���
        StartCoroutine("HitKnockback");
    }

    public virtual void SetupKnockbackDir(Transform _damageDirection) //�˺���Դ����
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
        rb.velocity = new Vector2(0, 0);  //����0�ٶ�
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnock)
            return; //������ʱ��ͣ��
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FilpController(_xVelocity);
    }
    #endregion

    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos()  //��������ⷶΧ����
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

        if(onFlipped != null)  //����onFlippedEvent?.Invoke();
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

    // public void MakeTransprent(bool _transprent)  //�ڶ�����ʱ����� ͬʱ����Ϊ����Gameobject���� player��͸�� �ƶ���EntityFX ������




    public virtual void Die()
    {

    }
}
