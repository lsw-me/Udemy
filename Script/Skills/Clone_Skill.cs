using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{


    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Clone attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMutiplier;  //clone 攻击力为 本体30%
    private bool canAttack;                       //后续技能树部分需要修改  如上


    [Header("Aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMutiplier;
    public bool canApplyOnHitEffect{ get; private set; }


    /*
    //[SerializeField] private bool creatCloneOnDashStart;   //unlock in skill tree      技能树合并更改了使用方式
    //[SerializeField] private bool creatCloneOnDashOver;
    //[SerializeField] private bool canCreateCloneOnCounterAttack;  //反击部分 unlock in skill tree
    */

    [Header("Multiple clone")]
    [SerializeField] private UI_SkillTreeSlot multipleUnlockButton;
    [SerializeField] private float multipleCloneAttackMutiplier;
    [SerializeField] private float chanceToDuplictae;
    private bool canDuplicateClone; //可以复制克隆 很厉害的技能

    [Header("Crystal install of clone")]
    [SerializeField] private UI_SkillTreeSlot crystalInsteadUnlockButton;
    public bool crystalInstallOfClone {  get; private set; }


    protected override void Start()
    {
        base.Start();


        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMutiClone);
        crystalInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }


    #region Unlock region  --skill tree

    protected override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockAggresiveClone();
        UnlockMutiClone();
        UnlockCrystalInstead();

    }


    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = cloneAttackMutiplier;

        }
    }
    private void  UnlockAggresiveClone()
    {
        if(aggresiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = aggresiveCloneAttackMutiplier;

        }
    }
    private void UnlockMutiClone()
    {
        if(multipleUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multipleCloneAttackMutiplier;
        }
    }
    private void UnlockCrystalInstead()
    {
        if(crystalInsteadUnlockButton.unlocked)
        {
            crystalInstallOfClone = true;
        }
    }

    #endregion




    public void CreatClone(Transform _ClonePosition , Vector3 _offset)
   {

        if(crystalInstallOfClone)
        {
            SkillManager.instance.crystal.CreatCrystal();  //根据是否创建水晶更改创建克隆      
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().
            SetupClone(_ClonePosition,cloneDuration,canAttack,_offset,FindClosestEnemy(newClone.transform),canDuplicateClone, chanceToDuplictae,player,attackMultiplier); //137额外传递attackMultiplier
    }



    //合并技能树的时候将这些更改位置，移动到dash中 移动后做了调整
    //public void CreatCloneOnDashStart()
    //{
    //    if (creatCloneOnDashStart)
    //    {
    //        CreatClone(player.transform, Vector3.zero);
    //    }
    //}

    //public void CreatCloneOnDashOver()
    //{
    //    if (creatCloneOnDashOver)
    //    {
    //        CreatClone(player.transform, Vector3.zero);
    //    }
    //}

    public void CreatCloneWithDelay(Transform _enemyTransform)  //原名CreatCloneOnCounterAttack
    {
        //if (canCreateCloneOnCounterAttack) //技能树控制不需要这个bool 判断了
        StartCoroutine(CloneDelayCorotine(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCorotine(Transform _transform, Vector3 _offset) //延迟
    {
        yield return new WaitForSeconds(.4f);
            CreatClone(_transform, _offset);
    }
}
