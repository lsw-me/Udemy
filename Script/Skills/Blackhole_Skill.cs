using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackhole_Skill : Skill
{

    [SerializeField] private UI_SkillTreeSlot blackholeUnlockButton;
    public bool blackholeUnlocked {  get; private set; }

    [SerializeField] private int amountOfAttack;
    [SerializeField] private float cloneCooldown;
    [SerializeField] private float blackDuration;

    [Space]

    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;


    private Blackhole_Skill_Controller currentBlackhole;

   
    private void UnlockBlackhole()
    {
        if (blackholeUnlockButton.unlocked)
            blackholeUnlocked = true;   
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()  //在playerBlackholeState 中使用
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefab,player.transform.position,Quaternion.identity);

       //Blackhole_Skill_Controller newBlackHoleScript = newBlackHole.GetComponent<Blackhole_Skill_Controller>(); // 新版本，更新退出方式

        currentBlackhole =  newBlackHole.GetComponent<Blackhole_Skill_Controller>();

        //newBlackHoleScript.SetupBlackhole(maxSize,growSpeed,shrinkSpeed,amountOfAttack,cloneCooldown); //同上

        currentBlackhole.SetupBlackhole(maxSize, growSpeed, shrinkSpeed, amountOfAttack, cloneCooldown,blackDuration);
    }

    protected override void Start()
    {
        base.Start();


        blackholeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBlackhole);
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillCompleted()
    {
        if(!currentBlackhole)  //没有使用技能时候直接返回
            return false;

        if(currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;  //无效当前currentBlackhole
            return true;
        }
        return false;
    }

    public float GetBlackholeRadius()
    {

        return maxSize / 2;
    }

    protected override void CheckUnlock()
    {
        UnlockBlackhole();
    }
}
