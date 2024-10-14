using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{

    private Player player;

    private SpriteRenderer sr; //���������ư�����ͨ����Ȼ��ʵ�ֲ�Ӱ͸��Ч��

    private Animator anim;
    [SerializeField] private float colorLosingSpeed; //��ɫ��ɫϵ��

    private float cloneTimer;
    [SerializeField] private float attackMutiplier;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Transform closestEnemy;
    private int facingDir = 1;

    private bool canDuplicateClone; //���Ը��ƿ�¡
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
                Destroy(gameObject); //͸����ɾ��
        }
    }
    public void SetupClone(Transform _newTransform,float _cloneDuration,bool _canAttack ,Vector3 _offset,Transform _closestEnemy,bool _canDuplicate,float _chanceToDuplicate,Player _player,float _attackMutiplier) //���Ӵ��п�¡��ƫ��  ���� ����
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3)); // Random.Range ������Сֵ�����������ֵ
       
        player = _player;
        transform.position = _newTransform.position + _offset;
        cloneTimer =_cloneDuration;
        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        attackMutiplier = _attackMutiplier;

        FaceCloseTarget();
    }


    // ������Ϊclone������Ҳ�к�playeһ����trigger ��������ֱ��ʹ����ͬ�ĺ������ƣ����ǹ��ܲ�ͬ��ʵ��
    private void AnimationTrigger()
    {
        cloneTimer = -.1f;   //���ø�ֵ�� cloneTimer < 0 clone ��ɫ�ı� ����ʧ�������󴥷�
    }

    private void AttackTrigger()
    {
        //������������ attackCheck Բ�п��ܻ��кܶ࣬����ȫ��ע��

        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (Collider2D hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //player.stats.DoDamage(hit.GetComponent<CharacterStats>()); ��Ϊplayer.stats �л�ȡ��CharacterStats,Ϊ��ʹ��PlayerStats �е� CloneDoDamage ��������ע�͵� 
                // hit.GetComponent<Enemy>().DamageEffect();  //��������Ĺ�����hit�ܻ�ȡ����������������˺�����   p102����

                hit.GetComponent<Entity>().SetupKnockbackDir(transform); //���ݹ�����transform �൱���˺���Դ����

                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.CloneDoDamage(enemyStats, attackMutiplier);

                if(player.skill.clone.canApplyOnHitEffect)  //��playerAnimationTrigger�е�һ����һ�����������þ�Ӧ�����ЧӦ
                {
                    ItemData_Equipment weponData = Inventory.instance.GetEquipment(EquipmentType.Wepon);  //����ϱ�bug
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

    private void FaceCloseTarget() //clone �����������
    {

        //��������Crystal Ҳ��Ҫ������ĵ��ˣ��ⲿ�ִ���ĵ�Skill ���У�Ȼ����setup��ֱ�ӽ�����ĵ��˵�transform�����������������ֱ��ʹ��
        //�����Skill ����  Transform FindClosestEnemy(Transform _checkTransfom) ����


        if (closestEnemy != null)  //����� ���ҵ��������ת�䷽��
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;                              //���·�����canDuplicateClone ���ܳ����Ŀ�¡��ʵ�����ҷ���ı仯
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
