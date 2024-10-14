using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;



    private bool canRotate = true;
    private bool isReturning;


    private float freezeTimeDuration;
    private float returnSpeed = 12;

    [Header("Pierce info")]
    private float pierceAmount;

    [Header("Bounce info")]
    private float bounceSpeed;
    private bool isBouncing; //剑弹跳
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStooped;
    private bool isSpinning;


    private float hitTimer;
    private float hitCooldown;

    private float spinDirection; //旋转移动方向


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (canRotate)
        {
            transform.right = rb.velocity;
        }

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1) //距离太近直接清除
                player.CatchTheSword();
        }

        BounceLogic();

        SpinLogic();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isReturning)
            return;

        if (other.GetComponent<Enemy>() != null)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            SwordSkillDamage(enemy);

        }

        SetupTargetsForBounce(other);

        StuckInto(other);

    }
    public void SetupSword(Vector2 _dir, float _grivityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)//传递参数
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;
        rb.velocity = _dir;
        rb.gravityScale = _grivityScale;
        if (pierceAmount <= 0)
            anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);
        Invoke("DestoryMe", 7);
    }
    public void SetupBounce(bool _isBouncing, int _amountOfBounces, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounces;
        bounceSpeed = _bounceSpeed;

        enemyTarget = new List<Transform>();
    }
    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }
    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDurtion, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDurtion;
        hitCooldown = _hitCooldown;
    }
    private void SetupTargetsForBounce(Collider2D other)  //设置弹跳目标
    {
        if (other.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (Collider2D hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }
    }
    private void DestoryMe()  //用来删除飞出去超时的剑
    {
        Destroy(gameObject);
    }

    private void StopWhenSpinning()
    {
        wasStooped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void SpinLogic() //spin 逻辑
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStooped)
            {
                StopWhenSpinning();
            }

            if (wasStooped)
            {
                spinTimer -= Time.deltaTime;

                //transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);
                //不注释也挺好，看情况 两个效果都不错


                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;       //持续检查旋转spin伤害
                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (Collider2D hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            //hit.GetComponent<Enemy>().Damage();  改成下面函数
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                        }
                    }

                }
            }
        }
    }

    private void BounceLogic() //Bounce 逻辑
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

                //enemyTarget[targetIndex].GetComponent<Enemy>().Damage();
                //enemyTarget[targetIndex].GetComponent<Enemy>().StartCoroutine("FreezeTimeFor",freezeTimeDuration);  这两句改成上边函数

                targetIndex++;
                bounceAmount--;


                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }
                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }

        }
    }


    public void ReturnSword() //剑返回
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //rb.isKinematic = false;  // 修bug 剑飞出去途中收回时候直接小时bug  p71
        transform.parent = null;  //设置剑运动刚体状态和父
        isReturning = true;

        //sword.skill.setcooldown; //如果设置技能cd的话 
    }

    private void SwordSkillDamage(Enemy enemy) //设置伤害 
    {

        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        //enemy.DamageEffect();
        player.stats.DoDamage(enemyStats);

        if(player.skill.sword.timeStopUnlock)  //技能树补充
            enemy.FreezeTimeFor(freezeTimeDuration);

        if (player.skill.sword.volnurableUnlock)
            enemyStats.MakeVulnerableFor(freezeTimeDuration);


        ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet); //护符装备效果  这里装备护符之后，sword Skill就会有护符设置的效果
        if (equipedAmulet != null)
        {
            equipedAmulet.Effect(enemy.transform);
        }


    }


    private void StuckInto(Collider2D other)  //用来设置剑撞到 other 卡住
    {
        if (pierceAmount > 0 && other.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false; //停止旋转
        cd.enabled = false; // 关闭 碰撞

        rb.isKinematic = true;  //启用了 isKinematic，则力、碰撞或关节将不再影响刚体。 将完全由动画或脚本通过改变 transform.position 来控制。这种状态下的刚体也被称为运动学刚体
        rb.constraints = RigidbodyConstraints2D.FreezeAll;  //constraints 属性是一个用于限制 Rigidbody2D 移动和旋转的设置。它是一个 RigidbodyConstraints2D 枚举，可以组合多种约束来控制刚体的运动。
                                                            //RigidbodyConstraints2D.FreezeAll  是一个特殊的约束设置，它同时冻结刚体的位置和旋转。 无法在水平或垂直方向上移动。将无法旋转。

        GetComponentInChildren<ParticleSystem>().Play();
        
        if (isBouncing && enemyTarget.Count > 0) //弹跳不结束动画
            return;
        anim.SetBool("Rotation", false);

        transform.parent = other.transform; //变化父对象 变为碰到的物体的子对象
    }
}
