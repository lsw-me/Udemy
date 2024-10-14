using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "Buff effect", menuName = "Data/Item effect/Buff effect")]
public class BuffEffect : ItemEffect
{
    // 这里 决定增加什么 stat,增加多少，持续多长时间
    private PlayerStats stats;
    [SerializeField] private StatType buffType;
    [SerializeField] private int buffAmount;
    [SerializeField] private int buffDuration;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        stats.IncreaseStatBy(buffAmount,buffDuration,stats.GetStat(buffType));           //大概方便统一管理
    }
}
