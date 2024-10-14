using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{

    private Animator anim =>GetComponent<Animator>();
    private CircleCollider2D cd =>GetComponent<CircleCollider2D>(); 

    private Player player;
    private float crystalExistTimer;


    private bool canExplode;
    private bool canMove;
    private float moveSpeed;


    private bool canGrow;
    private float growSpeed = 5;
    private Transform closestTarget;

    [SerializeField] private LayerMask whatIsEnemy;

    public void SetupCrystal(float _crystalDuration,bool _canExplode ,bool _canMove ,float _moveSpeed,Transform _closestTarget,Player _player)
    {
        player = _player;
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }


    public void  ChooseRandomEnemy()
    {
        float  radius = SkillManager.instance.blackhole.GetBlackholeRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius,whatIsEnemy);

        if(colliders.Length >0) // 确保碰撞中有东西
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform; 
    }
    public void Update()
    {

        crystalExistTimer -= Time.deltaTime;

        if (crystalExistTimer < 0)
        {
            FinishCrystal();
        }
        if (canMove && closestTarget != null) //这里应该有找不到目标报错bug  加上  &&closestTarget!=null
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position,moveSpeed *Time.deltaTime); //crystal move to target
            if (Vector2.Distance(transform.position, closestTarget.position) < 1) //距离近之后执行下边逻辑
            { 
                FinishCrystal();
                canMove = false;
            }
        }
        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3) , growSpeed * Time.deltaTime);
    }

    private void AnimationExplodeEvent()
    {
        //攻击触发器， attackCheck 圆中可能会有很多，所以全部注册

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,cd.radius);

        foreach (Collider2D hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //hit.GetComponent<Enemy>().DamageEffect();  //如果遍历的过程中hit能获取到敌人组件，触发伤害函数  102 更新
                hit.GetComponent<Entity>().SetupKnockbackDir(transform); //传递攻击的transform 相当于伤害来源方向
                player.stats.DoMagicDamage(hit.GetComponent<CharacterStats>());

                ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet); //护符装备效果  这里装备护符之后，crystal就会有护符设置的效果
                if (equipedAmulet != null)
                {
                    equipedAmulet.Effect(hit.transform);
                }

            }
        }
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode"); //trigger 
        }
        else
            SetDeStory();
    }

    public void SetDeStory() => Destroy(gameObject);
}
