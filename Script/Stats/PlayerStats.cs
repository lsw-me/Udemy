using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{

    private Player player;
    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
       // player.DamageEffect();
    }

    protected override void Die()
    {
        base.Die();
        player.Die();

        GameManager.instance.lostCurrencyAmount = PlayerManager.instance.currency;
        PlayerManager.instance.currency = 0;

        GetComponent<PlayerItemDrop>().GenerateDrop();
    }


    protected override void DecreaseHealthyBy(int _damage)
    {
        base.DecreaseHealthyBy(_damage);

        if(_damage > GetMaxHealthValue() * .3f)
        {
            player.SetupKnockbackPower(new Vector2(10,6));
            //AudioManager.instance.PlaySFX(这里放下标)但是我没做audio部分 pass
        }

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if (currentArmor != null)
            currentArmor.Effect(player.transform);
    }
    public override void OnEvasion()
    {
        //Debug.Log(" player avoid attack  is work");
        player.skill.dodge.CreateMirageOnDodge();
    }

    public void CloneDoDamage(CharacterStats _targetStats,float _mutiplier) //和普通的没什么区别，这里是图方便 不用重新是因为要改很多
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if(_mutiplier > 0 )
            totalDamage = Mathf.RoundToInt(totalDamage * _mutiplier);  //倍率

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage); //检查 护甲值，进行伤害计算

        _targetStats.TakeDamage(totalDamage);

        DoMagicDamage(_targetStats);
    }
}
