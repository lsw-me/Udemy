using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;

    private ItemDrop myDropSystem;// 敌人爆金币啦

    public Stat soulsDropAmount;


    [Header("Level details")]
    [SerializeField] private int level = 1; //给敌人增加 level 


    [Range(0f, 1f)]
    [SerializeField] private float percantageModifier =.4f; //百分比  


    protected override void Start()
    {
        soulsDropAmount.SetDefaultValue(100);
        ApplyLevelModifiers();

        base.Start();                                  //base 中maxhelth =currenthealth  更换顺序，应用modify 之后更新血量

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();

    }

    private void ApplyLevelModifiers()
    {
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(maxHealth);
        Modify(armor);
        Modify(evasion);
        Modify(magicResistance);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightingDamage);

        Modify(soulsDropAmount);

    }

    private void Modify(Stat _stat)                //不同等级施加不同百分比来增强敌人
    {
        for (int i = 1; i < level; i++)
        {
            float modifier = _stat.GetValue() * percantageModifier;

            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }


    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        //enemy.DamageEffect();
    }
    protected override void Die()
    {
        base.Die();
        enemy.Die();

        PlayerManager.instance.currency += soulsDropAmount.GetValue();
        myDropSystem.GenerateDrop(); //爆金币

        Destroy(gameObject, 5f);
    }
}
