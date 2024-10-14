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
    [SerializeField] private float cloneAttackMutiplier;  //clone ������Ϊ ����30%
    private bool canAttack;                       //����������������Ҫ�޸�  ����


    [Header("Aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMutiplier;
    public bool canApplyOnHitEffect{ get; private set; }


    /*
    //[SerializeField] private bool creatCloneOnDashStart;   //unlock in skill tree      �������ϲ�������ʹ�÷�ʽ
    //[SerializeField] private bool creatCloneOnDashOver;
    //[SerializeField] private bool canCreateCloneOnCounterAttack;  //�������� unlock in skill tree
    */

    [Header("Multiple clone")]
    [SerializeField] private UI_SkillTreeSlot multipleUnlockButton;
    [SerializeField] private float multipleCloneAttackMutiplier;
    [SerializeField] private float chanceToDuplictae;
    private bool canDuplicateClone; //���Ը��ƿ�¡ �������ļ���

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
            SkillManager.instance.crystal.CreatCrystal();  //�����Ƿ񴴽�ˮ�����Ĵ�����¡      
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().
            SetupClone(_ClonePosition,cloneDuration,canAttack,_offset,FindClosestEnemy(newClone.transform),canDuplicateClone, chanceToDuplictae,player,attackMultiplier); //137���⴫��attackMultiplier
    }



    //�ϲ���������ʱ����Щ����λ�ã��ƶ���dash�� �ƶ������˵���
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

    public void CreatCloneWithDelay(Transform _enemyTransform)  //ԭ��CreatCloneOnCounterAttack
    {
        //if (canCreateCloneOnCounterAttack) //���������Ʋ���Ҫ���bool �ж���
        StartCoroutine(CloneDelayCorotine(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCorotine(Transform _transform, Vector3 _offset) //�ӳ�
    {
        yield return new WaitForSeconds(.4f);
            CreatClone(_transform, _offset);
    }
}
