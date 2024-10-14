using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Heal effect", menuName = "Data/Item effect/Heal Effect")]
public class Heal_Effect : ItemEffect
{

    [Range(0,1f)]
    [SerializeField] private float healPercent;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        // what wang to do?    1. get player stats 2.how match to heal  3 .do heal

        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent);

        playerStats.IncreaseHealthBy(healAmount);          //调用加血方法
    }
}
