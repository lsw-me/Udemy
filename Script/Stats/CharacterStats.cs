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
    public Stat agility; // 敏捷  1 point  increase evasion by 1% and crit.chance by 1%
    public Stat intelligence; // 智力  1 point  increase magic damage by 1% and magic resistance by 3  魔法伤害
    public Stat vitality; // 只能加 health  没加1点加5血量


    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor; //护甲
    public Stat evasion;//闪避


    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;        //暴击率
    public Stat critPower;            //暴击伤害  default 150%
    public Stat magicResistance;


    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;


    public bool isIgnited; //点燃状态  持续伤害
    public bool isChilled;//寒冷状态   降低护甲 20%
    public bool isShocked; // 眩晕状态    降低准确度


    [SerializeField] private float allmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer; 
    private float shockedTimer;


    private float ignitedDamageCooldown = .3f;
    private float ignitedDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;

 // 管理数值 

    public int currentHealth;

    public System.Action onHealChanged;

    public bool isDead { get; private set; }//判断是否死亡状态  113 改为public 因为要让任务死亡不能捡起物品
    public bool isInvincible {  get; private set; }//无敌判断

    private bool isVulnerable;  //sword 的一个被动


    protected virtual void Start()
    {
        critPower.SetDefaultValue(150); //设置默认爆伤
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

        if(isIgnited)     //这个把函数if中的&& isIgnited拿出来判断了，方便看
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
    /// 增加buff效果，有提升buff的药水一类的
    /// </summary>
    /// <param name="_modifier">增强效果</param>
    /// <param name="_duration"> buff持续时间</param>
    /// <param name="_statToModify">增强的能力</param>
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
        bool criticalStrike = false;  //暴击为false

        if (TargetCanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform); //传递攻击的transform 相当于伤害来源方向

        int totalDamage = damage.GetValue() + strength.GetValue();
        
        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            //Debug.Log("total crit damage is " + totalDamage);
            criticalStrike = true; //暴击为真
        }

        fx.CreateHitFx(_targetStats.transform,criticalStrike);


        totalDamage = CheckTargetArmor(_targetStats, totalDamage); //检查 护甲值，进行伤害计算
      
         _targetStats.TakeDamage(totalDamage);


        // 如果后续武器有附魔buff则可以造成魔法伤害
        DoMagicDamage(_targetStats);  //测试魔法伤害时候放这里的    120 又写了一个这个  如果不想在普通攻击中附加魔法伤害就删掉
    }


    #region Magical  damage and ailements
    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();



        int totalMagicDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();   //三种魔法加上智力

        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);  //检查魔法攻击部分

        _targetStats.TakeDamage(totalMagicDamage);



        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)  //解决下边判断都没有0的问题， 三者全为0 返回
            return;

        AttemptToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightingDamage);

    }

    private  void AttemptToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage; //这里寻找三个伤害最大的，然后施加
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
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f)); //fire damage 的20%

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * .1f));

        _targetStats.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock); //根据bool施加不同元素
    }



    public void ApplyAliments(bool _ignite,bool _chill, bool _shock)
    {
        //if (isIgnited || isChilled || isShocked)  //暂时状态只能施加一次   更新写法
        //    return;

        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChilled = !isIgnited && !isChilled && !isShocked;
        bool canApplyShocked = !isIgnited && !isChilled; //因为雷电状态特殊设置，我们设计的是雷电状态再次被施加状态，会像周围最近的敌人施加一个闪电


        if (_ignite && canApplyIgnite)
        {
            isIgnited  = _ignite;
            ignitedTimer = allmentsDuration;  //燃烧时间为4秒

            fx.IgniteFxFor(allmentsDuration);

        }
        if (_chill && canApplyChilled)
        {
            chilledTimer = allmentsDuration;
            isChilled = _chill;


            float slowPercentage = .2f;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, allmentsDuration);   //这里需要留意，问题：GetComponent<Entity>是get那个上边的组件，为什么最后会应用player中override的方法而不是父类中的方法
            fx.ChillFxFor(allmentsDuration);
        }
        if(_shock &&canApplyShocked)
        {
            if(!isShocked)  //首次被施加状态
            {
                ApplyShock(_shock);
            }
            else  //已经被施加状态下，进行闪电的传递
            {
                if (GetComponent<Player>() != null)  //解决敌人攻击我，然后自己被雷击的bug
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

    private void HitNearestTargetWithShockStrike() //这个函数执行了找最近的敌人，然后发射闪电传输
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25); //半径内的所有碰撞

        float closestDistance = Mathf.Infinity;

        Transform closestEnemy = null;

        foreach (Collider2D hit in colliders)               //遍历找最近的enemy
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)//因为闪电是shocked 对象中产生的，所以最近的敌人是他自己，加一个距离判断防止自己打自己
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null) //如果不想让受击的为被攻击的敌人可以删掉这两行
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

            //currentHealth -= igniteDamage;  // 这里没有使用 takeDamage()   重新写了一下  再次重新，把收到伤害和更新血量的事件整到一个新的函数 中

            if (currentHealth < 0 && !isDead)
                Die();

            ignitedDamageTimer = ignitedDamageCooldown;
        }
    }

    public void  SetupIgniteDamage(int _damage )=> igniteDamage = _damage;//点火状体伤害
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

        if (currentHealth < 0 && !isDead)        //这里判断状态，防止死了因为燃烧伤害一直有判断受伤
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

    public void MakeInvincible(bool _invincible) => isInvincible = _invincible; //无敌判断
 


    public virtual void IncreaseHealthBy(int _amount)  //血瓶 等有加血效果 时写的方法 
    {
        currentHealth += _amount;

        if(currentHealth > GetMaxHealthValue())  //最多不超过最大血量
            currentHealth = GetMaxHealthValue();


        if(onHealChanged !=null)
            onHealChanged();       //触发事件更新ui
    }


    protected virtual void DecreaseHealthyBy(int _damage)
    {

        if (isVulnerable) //脆弱技能
            _damage = Mathf.RoundToInt( _damage * 1.1f);
        currentHealth -= _damage;

        if (onHealChanged != null)
        {
            onHealChanged();            //这里触发事件
        }

    }


    #region Stats calculations

    public virtual void OnEvasion() //这里什么都不做  转到->playerstats
    {

    }

    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue(); //总闪避等于基础闪避加上敏捷能力

        if (isShocked)
            totalEvasion += 20; //增加20% 的闪避

        if (Random.Range(0, 100) < totalEvasion)
        {
            //Debug.Log("Miss");
            _targetStats.OnEvasion();
            return true;
        }
        return false;
    }  //闪避

    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage) //检查 护甲
    {
        if(_targetStats.isChilled)  //寒冷状态减护甲
            totalDamage -= Mathf.RoundToInt( _targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage -= _targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);//避免护甲大于伤害然后为负值开始加血
        return totalDamage;
    }
    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    protected bool CanCrit() //判断暴击
    {
        int totalCritChance = critChance.GetValue() + agility.GetValue(); //暴击率加上敏捷属性？ 好怪的设置

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



    public Stat GetStat(StatType _statType) //无敌了   
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
