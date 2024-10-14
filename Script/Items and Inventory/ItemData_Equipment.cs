using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EquipmentType
{
    Wepon,  //����
    Armor,  //װ��
    Amulet,  //����
    Flask  //ƿ�ӣ���
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")] //��unity�д����˵�

public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;


    [Header("Unique effect")]
    public ItemEffect[] itemEffects;
    public float itemCooldown;  //cd��ȴ

    //[TextArea]
    //public string itemEffectDescription;  //149 ȥ��


    [Header("Major  info")]
    public int strength;
    public int agility;
    public int intelgenace;
    public int vitality;

    [Header("Offensive stats")]
    public int damage;
    public int critChance;
    public int critPower;

    [Header("Defensive info")]
    public int health;
    public int armor;
    public int evasion; //����
    public int magicResistance;

    [Header("Magic stats")]
    public int fireDamage;
    public int icrDamage;
    public int lightingDamage;


    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterials;


    private int descriptionLength;      //������������̶���С��


    public void Effect(Transform _enemyPosition)
    {
        foreach (var item in itemEffects)
        {
            item.ExecuteEffect( _enemyPosition);
        }
    }



    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>(); 


        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelgenace);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(health);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(icrDamage);
        playerStats.lightingDamage.AddModifier(lightingDamage);

    }
    public void RemoveModifiers() 
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();


        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelgenace);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.maxHealth.RemoveModifier(health);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(icrDamage);
        playerStats.lightingDamage.RemoveModifier(lightingDamage);
    }


    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength,"Strength");
        AddItemDescription(agility,"Agility");
        AddItemDescription(intelgenace, "Intelgenace");
        AddItemDescription(vitality,"Vitality");

        AddItemDescription(damage,"Damage");
        AddItemDescription(critChance,"Crit.Chance");
        AddItemDescription(critPower,"Crit.Power");

        AddItemDescription(health,"Health");
        AddItemDescription(evasion,"Evasion");
        AddItemDescription(armor,"Armor");
        AddItemDescription(magicResistance,"Magic Resist.");

        AddItemDescription(fireDamage,"Fire Damage");
        AddItemDescription(icrDamage,"Ice Damage");
        AddItemDescription(lightingDamage,"Lighting dmg.");


        for (int i = 0; i < itemEffects.Length; i++)
        {
            if (itemEffects[i ].effectDescription.Length> 0)
            {
                sb.AppendLine();
                sb.AppendLine("Unique: "+ itemEffects[i].effectDescription);
                descriptionLength++;
            }
        }


        if (descriptionLength < 5 )                                  //���ݲ���5��ȴ�������Ӽ��У���֤�������Сһ��
        {
            for (int i = 0; i < 5 - descriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }

        //if (itemEffectDescription.Length > 0  )  //141 ȥ��
        //{
        //    sb.AppendLine();
        //    sb.Append(itemEffectDescription);
        //}

        return sb.ToString();
    }
    private void AddItemDescription(int _value,string _name)
    {
        if (_value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();


            if (_value > 0)
                sb.AppendLine("+ " + _value + " " + _name );


            descriptionLength++;
        }
    }

}
