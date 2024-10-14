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
    private bool isBouncing; //������
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

    private float spinDirection; //��ת�ƶ�����


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

            if (Vector2.Distance(transform.position, player.transform.position) < 1) //����̫��ֱ�����
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
    public void SetupSword(Vector2 _dir, float _grivityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)//���ݲ���
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
    private void SetupTargetsForBounce(Collider2D other)  //���õ���Ŀ��
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
    private void DestoryMe()  //����ɾ���ɳ�ȥ��ʱ�Ľ�
    {
        Destroy(gameObject);
    }

    private void StopWhenSpinning()
    {
        wasStooped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void SpinLogic() //spin �߼�
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
                //��ע��Ҳͦ�ã������ ����Ч��������


                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;       //���������תspin�˺�
                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (Collider2D hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            //hit.GetComponent<Enemy>().Damage();  �ĳ����溯��
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                        }
                    }

                }
            }
        }
    }

    private void BounceLogic() //Bounce �߼�
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

                //enemyTarget[targetIndex].GetComponent<Enemy>().Damage();
                //enemyTarget[targetIndex].GetComponent<Enemy>().StartCoroutine("FreezeTimeFor",freezeTimeDuration);  ������ĳ��ϱߺ���

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


    public void ReturnSword() //������
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //rb.isKinematic = false;  // ��bug ���ɳ�ȥ;���ջ�ʱ��ֱ��Сʱbug  p71
        transform.parent = null;  //���ý��˶�����״̬�͸�
        isReturning = true;

        //sword.skill.setcooldown; //������ü���cd�Ļ� 
    }

    private void SwordSkillDamage(Enemy enemy) //�����˺� 
    {

        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        //enemy.DamageEffect();
        player.stats.DoDamage(enemyStats);

        if(player.skill.sword.timeStopUnlock)  //����������
            enemy.FreezeTimeFor(freezeTimeDuration);

        if (player.skill.sword.volnurableUnlock)
            enemyStats.MakeVulnerableFor(freezeTimeDuration);


        ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet); //����װ��Ч��  ����װ������֮��sword Skill�ͻ��л������õ�Ч��
        if (equipedAmulet != null)
        {
            equipedAmulet.Effect(enemy.transform);
        }


    }


    private void StuckInto(Collider2D other)  //�������ý�ײ�� other ��ס
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

        canRotate = false; //ֹͣ��ת
        cd.enabled = false; // �ر� ��ײ

        rb.isKinematic = true;  //������ isKinematic����������ײ��ؽڽ�����Ӱ����塣 ����ȫ�ɶ�����ű�ͨ���ı� transform.position �����ơ�����״̬�µĸ���Ҳ����Ϊ�˶�ѧ����
        rb.constraints = RigidbodyConstraints2D.FreezeAll;  //constraints ������һ���������� Rigidbody2D �ƶ�����ת�����á�����һ�� RigidbodyConstraints2D ö�٣�������϶���Լ�������Ƹ�����˶���
                                                            //RigidbodyConstraints2D.FreezeAll  ��һ�������Լ�����ã���ͬʱ��������λ�ú���ת�� �޷���ˮƽ��ֱ�������ƶ������޷���ת��

        GetComponentInChildren<ParticleSystem>().Play();
        
        if (isBouncing && enemyTarget.Count > 0) //��������������
            return;
        anim.SetBool("Rotation", false);

        transform.parent = other.transform; //�仯������ ��Ϊ������������Ӷ���
    }
}
