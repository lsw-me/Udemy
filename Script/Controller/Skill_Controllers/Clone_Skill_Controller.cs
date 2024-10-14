using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{

    private Player player;

    private SpriteRenderer sr; //获得组件控制阿尔法通道，然后实现残影透明效果

    private Animator anim;
    [SerializeField] private float colorLosingSpeed; //颜色蜕色系数

    private float cloneTimer;
    [SerializeField] private float attackMutiplier;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Transform closestEnemy;
    private int facingDir = 1;

    private bool canDuplicateClone; //可以复制克隆
    private float chanceToDuplicate;

    private void Awake()
    {
        sr =  GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();    
    }
    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if(cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1,sr.color.a - (Time.deltaTime * colorLosingSpeed));
            if(sr.color.a <= 0 )
                Destroy(gameObject); //透明后删除
        }
    }
    public void SetupClone(Transform _newTransform,float _cloneDuration,bool _canAttack ,Vector3 _offset,Transform _closestEnemy,bool _canDuplicate,float _chanceToDuplicate,Player _player,float _attackMutiplier) //增加大招克隆的偏差  增加 倍率
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3)); // Random.Range 包括最小值，不包括最大值
       
        player = _player;
        transform.position = _newTransform.position + _offset;
        cloneTimer =_cloneDuration;
        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        attackMutiplier = _attackMutiplier;

        FaceCloseTarget();
    }


    // 这里因为clone动画中也有和playe一样的trigger 所以我们直接使用相同的函数名称，但是功能不同来实现
    private void AnimationTrigger()
    {
        cloneTimer = -.1f;   //设置负值， cloneTimer < 0 clone 颜色改变 变消失，攻击后触发
    }

    private void AttackTrigger()
    {
        //攻击触发器， attackCheck 圆中可能会有很多，所以全部注册

        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (Collider2D hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //player.stats.DoDamage(hit.GetComponent<CharacterStats>()); 因为player.stats 中获取是CharacterStats,为了使用PlayerStats 中的 CloneDoDamage 函数所以注释掉 
                // hit.GetComponent<Enemy>().DamageEffect();  //如果遍历的过程中hit能获取到敌人组件，触发伤害函数   p102改了

                hit.GetComponent<Entity>().SetupKnockbackDir(transform); //传递攻击的transform 相当于伤害来源方向

                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.CloneDoDamage(enemyStats, attackMutiplier);

                if(player.skill.clone.canApplyOnHitEffect)  //和playerAnimationTrigger中的一部分一样，技能能用就应用这个效应
                {
                    ItemData_Equipment weponData = Inventory.instance.GetEquipment(EquipmentType.Wepon);  //解决上边bug
                    if (weponData != null)
                    {
                        weponData.Effect(hit.transform);
                    }
                }
                if (canDuplicateClone)
                {
                    if(Random.Range(0,100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreatClone(hit.transform, new Vector3(.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    private void FaceCloseTarget() //clone 朝向最近敌人
    {

        //这里由于Crystal 也需要找最近的敌人，这部分代码改到Skill 类中，然后在setup中直接将最近的敌人的transform传进，所以这里可以直接使用
        //详情见Skill 类中  Transform FindClosestEnemy(Transform _checkTransfom) 方法


        if (closestEnemy != null)  //如果有 并且敌人在左边转变方向
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;                              //更新方向，让canDuplicateClone 技能出来的克隆体实现左右方向的变化
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
