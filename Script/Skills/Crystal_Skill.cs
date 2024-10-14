using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal mirage")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneInstaedButton;  //�±ߵĶ������ں�ui skill tree �󶨵�button
    private bool cloneInsteadOfCrystal;


    [Header("Crystal simple")]
    [SerializeField] private UI_SkillTreeSlot unlockedCrystalButton;
    public bool crystalUnlocked {  get; private set; }

    [Header("Explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;
    private bool canExplode;


    [Header("Moving crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;
     private bool canMoveToTarget;
    [SerializeField] private float moveSpeed;


    [Header("Multi stacking crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMutiStackButton;
    private bool canUseMulitStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;  //ʱ����û���꼼�ܾ�ֱ��cd
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();



    protected override void Start()
    {
        base.Start();

        unlockedCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInstaedButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMutiStackButton.GetComponent<Button>().onClick.AddListener(UnlockMultiStack);


    }

    #region unlockSkill region

    protected override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
        UnlockMultiStack();
    }
    private void  UnlockCrystal()
    {
        if(unlockedCrystalButton.unlocked)
            crystalUnlocked = true;
    }

    private void UnlockCrystalMirage()
    {
        if(unlockCloneInstaedButton.unlocked)
            cloneInsteadOfCrystal = true;
    }

    private void UnlockExplosiveCrystal()
    {
        if(unlockExplosiveButton.unlocked)
            canExplode = true;
    }


    private void UnlockMovingCrystal()
    {
        if(unlockMovingCrystalButton.unlocked)
            canMoveToTarget = true;
    }

    private void UnlockMultiStack()
    {
        if(unlockMutiStackButton.unlocked)
            canUseMulitStacks = true;
    }
    #endregion

    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMutiStacks()) //���ˮ����֧
            return;


        if (currentCrystal == null)
        {
            CreatCrystal();
        }
        else
        {
            if (canMoveToTarget)  //��ˮ�����ڿ����ƶ������˵ķ�֧ʱ�����ﲻ�ܺ�ˮ������λ��
                return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;  //���ﴫ��
            currentCrystal.transform.position = playerPos;   //switch player and crystal

            if(cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreatClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal(); //crystal 2 model explode or destory
            }    
        }
    }

    public void CreatCrystal()  //����ˮ���ĺ���
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();
        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToTarget, moveSpeed, FindClosestEnemy(currentCrystal.transform),player);


        currentCrystalScript.ChooseRandomEnemy(); //ѡ���������
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();


    private bool CanUseMutiStacks() //�ж��ܲ���ʹ��������ܷ�֧
    {
        if(canUseMulitStacks)
        {
            cooldown = 0;
            if(crystalLeft.Count >0) //list ���� ������list�����һ��
            {

                if (crystalLeft.Count == amountOfStacks)
                    Invoke("ResetAbility",useTimeWindow); //���ܲ��Ͻ�����ͽ�Cd��

                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position,Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);
                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration,canExplode,canMoveToTarget,moveSpeed,FindClosestEnemy(newCrystal.transform), player);

                if(crystalLeft.Count <= 0)
                {
                    // cooldown skill and refill crystal 

                    cooldown = multiStackCooldown;
                    RefillCrystal();
                }
                return true;
            }
        }
        return false;
    }
    private void  RefillCrystal() //crystal �б����
    {

        int amountToAdd = amountOfStacks - crystalLeft.Count; //�����������ʹ�ô��ڣ�list�оͻ��в����crystalprefab �����������ӹ̶�������amountOfStacks �ͻᳬ����Χ���������������Ҫ��Ӷ���crystal
        for (int i = 0; i< amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab); //����
        }
    }

    private void ResetAbility()
    {
        if(cooldownTimer> 0 )
            return;

        cooldownTimer = multiStackCooldown;
        RefillCrystal();

    }
}
