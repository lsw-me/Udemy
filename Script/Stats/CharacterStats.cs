using System.Collections;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelegence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightingDamage
}


public class CharacterStats : MonoBehaviour
{

    private EntitiFX fx;

    [Header("Major stats")]
    public Stat strength; // 1 point increase damage by 1 and crit.power by 1%
    public Stat agility; // ����  1 point  increase evasion by 1% and crit.chance by 1%
    public Stat intelligence; // ����  1 point  increase magic damage by 1% and magic resistance by 3  ħ���˺�
    public Stat vitality; // ֻ�ܼ� health  û��1���5Ѫ��


    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor; //����
    public Stat evasion;//����


    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;        //������
    public Stat critPower;            //�����˺�  default 150%
    public Stat magicResistance;


    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;


    public bool isIgnited; //��ȼ״̬  �����˺�
    public bool isChilled;//����״̬   ���ͻ��� 20%
    public bool isShocked; // ѣ��״̬    ����׼ȷ��


    [SerializeField] private float allmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer; 
    private float shockedTimer;


    private float ignitedDamageCooldown = .3f;
    private float ignitedDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;

 // ������ֵ 

    public int currentHealth;

    public System.Action onHealChanged;

    public bool isDead { get; private set; }//�ж��Ƿ�����״̬  113 ��Ϊpublic ��ΪҪ�������������ܼ�����Ʒ
    public bool isInvincible {  get; private set; }//�޵��ж�

    private bool isVulnerable;  //sword ��һ������


    protected virtual void Start()
    {
        critPower.SetDefaultValue(150); //����Ĭ�ϱ���
        currentHealth = GetMaxHealthValue();


        fx = GetComponent<EntitiFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        ignitedDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;
        if (chilledTimer < 0)
            isChilled = false;
        if (shockedTimer < 0)
            isShocked = false;

        if(isIgnited)     //����Ѻ���if�е�&& isIgnited�ó����ж��ˣ����㿴
            ApplyIgniteDamage();

    }

    public void MakeVulnerableFor(float _duration)
    {
        StartCoroutine(VulnerableCorutine(_duration));
    }
    private IEnumerator VulnerableCorutine(float _duration)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duration);

        isVulnerable = false;
    }




    /// <summary>
    /// ����buffЧ����������buff��ҩˮһ���
    /// </summary>
    /// <param name="_modifier">��ǿЧ��</param>
    /// <param name="_duration"> buff����ʱ��</param>
    /// <param name="_statToModify">��ǿ������</param>
    public virtual void IncreaseStatBy(int _modifier,float _duration,Stat _statToModify)     
    {
        // what to do ?  1.start corototuine for stat increase 
        StartCoroutine(StatModCoroutine(_modifier,_duration,_statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statToModify.RemoveModifier(_modifier);
    }



    public virtual void DoDamage(CharacterStats  _targetStats) 
    {
        bool criticalStrike = false;  //����Ϊfalse

        if (TargetCanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform); //���ݹ�����transform �൱���˺���Դ����

        int totalDamage = damage.GetValue() + strength.GetValue();
        
        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            //Debug.Log("total crit damage is " + totalDamage);
            criticalStrike = true; //����Ϊ��
        }

        fx.CreateHitFx(_targetStats.transform,criticalStrike);


        totalDamage = CheckTargetArmor(_targetStats, totalDamage); //��� ����ֵ�������˺�����
      
         _targetStats.TakeDamage(totalDamage);


        // ������������и�ħbuff��������ħ���˺�
        DoMagicDamage(_targetStats);  //����ħ���˺�ʱ��������    120 ��д��һ�����  �����������ͨ�����и���ħ���˺���ɾ��
    }


    #region Magical  damage and ailements
    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();



        int totalMagicDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();   //����ħ����������

        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);  //���ħ����������

        _targetStats.TakeDamage(totalMagicDamage);



        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)  //����±��ж϶�û��0�����⣬ ����ȫΪ0 ����
            return;

        AttemptToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightingDamage);

    }

    private  void AttemptToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage; //����Ѱ�������˺����ģ�Ȼ��ʩ��
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;


        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .5f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f)); //fire damage ��20%

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * .1f));

        _targetStats.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock); //����boolʩ�Ӳ�ͬԪ��
    }



    public void ApplyAliments(bool _ignite,bool _chill, bool _shock)
    {
        //if (isIgnited || isChilled || isShocked)  //��ʱ״ֻ̬��ʩ��һ��   ����д��
        //    return;

        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChilled = !isIgnited && !isChilled && !isShocked;
        bool canApplyShocked = !isIgnited && !isChilled; //��Ϊ�׵�״̬�������ã�������Ƶ����׵�״̬�ٴα�ʩ��״̬��������Χ����ĵ���ʩ��һ������


        if (_ignite && canApplyIgnite)
        {
            isIgnited  = _ignite;
            ignitedTimer = allmentsDuration;  //ȼ��ʱ��Ϊ4��

            fx.IgniteFxFor(allmentsDuration);

        }
        if (_chill && canApplyChilled)
        {
            chilledTimer = allmentsDuration;
            isChilled = _chill;


            float slowPercentage = .2f;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, allmentsDuration);   //������Ҫ���⣬���⣺GetComponent<Entity>��get�Ǹ��ϱߵ������Ϊʲô����Ӧ��player��override�ķ��������Ǹ����еķ���
            fx.ChillFxFor(allmentsDuration);
        }
        if(_shock &&canApplyShocked)
        {
            if(!isShocked)  //�״α�ʩ��״̬
            {
                ApplyShock(_shock);
            }
            else  //�Ѿ���ʩ��״̬�£���������Ĵ���
            {
                if (GetComponent<Player>() != null)  //������˹����ң�Ȼ���Լ����׻���bug
                    return;

                HitNearestTargetWithShockStrike();

                //find closest targrt ,only among the enemy

            }
        }
    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;
         
        shockedTimer = allmentsDuration;
        isShocked = _shock;

        fx.ShockFxFor(allmentsDuration);
    }

    private void HitNearestTargetWithShockStrike() //�������ִ����������ĵ��ˣ�Ȼ�������紫��
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25); //�뾶�ڵ�������ײ

        float closestDistance = Mathf.Infinity;

        Transform closestEnemy = null;

        foreach (Collider2D hit in colliders)               //�����������enemy
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)//��Ϊ������shocked �����в����ģ���������ĵ��������Լ�����һ�������жϷ�ֹ�Լ����Լ�
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null) //����������ܻ���Ϊ�������ĵ��˿���ɾ��������
            {
                closestEnemy = transform;
            }

        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShocckStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());


        }
    }

    private void ApplyIgniteDamage()
    {
        if (ignitedDamageTimer < 0 )
        {
           // Debug.Log("Take burn damage" + igniteDamage);

            DecreaseHealthyBy(igniteDamage);

            //currentHealth -= igniteDamage;  // ����û��ʹ�� takeDamage()   ����д��һ��  �ٴ����£����յ��˺��͸���Ѫ�����¼�����һ���µĺ��� ��

            if (currentHealth < 0 && !isDead)
                Die();

            ignitedDamageTimer = ignitedDamageCooldown;
        }
    }

    public void  SetupIgniteDamage(int _damage )=> igniteDamage = _damage;//���״���˺�
    public void SetupShockStrikeDamage(int _damage) =>shockDamage = _damage;

    #endregion
   
    
    
    
    public virtual void TakeDamage(int _damage)
    {
        if (isInvincible)
            return;
        DecreaseHealthyBy(_damage);

        //Debug.Log(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth < 0 && !isDead)        //�����ж�״̬����ֹ������Ϊȼ���˺�һֱ���ж�����
        {
            Die();
        }
    }
    protected virtual void Die()
    {
       
        isDead = true;
        // dei logic 
    }

    public void KillEntity()
    {
        if (!isDead)
            Die();
    }

    public void MakeInvincible(bool _invincible) => isInvincible = _invincible; //�޵��ж�
 


    public virtual void IncreaseHealthBy(int _amount)  //Ѫƿ ���м�ѪЧ�� ʱд�ķ��� 
    {
        currentHealth += _amount;

        if(currentHealth > GetMaxHealthValue())  //��಻�������Ѫ��
            currentHealth = GetMaxHealthValue();


        if(onHealChanged !=null)
            onHealChanged();       //�����¼�����ui
    }


    protected virtual void DecreaseHealthyBy(int _damage)
    {

        if (isVulnerable) //��������
            _damage = Mathf.RoundToInt( _damage * 1.1f);
        currentHealth -= _damage;

        if (onHealChanged != null)
        {
            onHealChanged();            //���ﴥ���¼�
        }

    }


    #region Stats calculations

    public virtual void OnEvasion() //����ʲô������  ת��->playerstats
    {

    }

    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue(); //�����ܵ��ڻ������ܼ�����������

        if (isShocked)
            totalEvasion += 20; //����20% ������

        if (Random.Range(0, 100) < totalEvasion)
        {
            //Debug.Log("Miss");
            _targetStats.OnEvasion();
            return true;
        }
        return false;
    }  //����

    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage) //��� ����
    {
        if(_targetStats.isChilled)  //����״̬������
            totalDamage -= Mathf.RoundToInt( _targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage -= _targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);//���⻤�״����˺�Ȼ��Ϊ��ֵ��ʼ��Ѫ
        return totalDamage;
    }
    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    protected bool CanCrit() //�жϱ���
    {
        int totalCritChance = critChance.GetValue() + agility.GetValue(); //�����ʼ����������ԣ� �ùֵ�����

        if(Random.Range(0,100) <= totalCritChance)
        {
            return true;
        }
        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f; //change to percent
        //Debug.Log("total crit power %" + totalCritPower);


        float critDamage = _damage * totalCritPower;
        //Debug.Log("crit damage before round up " + damage);

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue() => maxHealth.GetValue() + vitality.GetValue() * 5;


    #endregion



    public Stat GetStat(StatType _statType) //�޵���   
    {
        if (_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelegence) return intelligence;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.damage) return damage;
        else if (_statType == StatType.critChance) return critChance;
        else if (_statType == StatType.critPower) return critPower;
        else if (_statType == StatType.health) return maxHealth;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.magicRes) return magicResistance;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.lightingDamage) return lightingDamage;

        return null;
    }

}
