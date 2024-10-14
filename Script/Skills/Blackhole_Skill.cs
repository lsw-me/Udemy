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

    public override void UseSkill()  //��playerBlackholeState ��ʹ��
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefab,player.transform.position,Quaternion.identity);

       //Blackhole_Skill_Controller newBlackHoleScript = newBlackHole.GetComponent<Blackhole_Skill_Controller>(); // �°汾�������˳���ʽ

        currentBlackhole =  newBlackHole.GetComponent<Blackhole_Skill_Controller>();

        //newBlackHoleScript.SetupBlackhole(maxSize,growSpeed,shrinkSpeed,amountOfAttack,cloneCooldown); //ͬ��

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
        if(!currentBlackhole)  //û��ʹ�ü���ʱ��ֱ�ӷ���
            return false;

        if(currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;  //��Ч��ǰcurrentBlackhole
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
