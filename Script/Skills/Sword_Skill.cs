using System;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType //四种剑的类型
{
    Regular,
    Bounce,  //反弹
    Pierce,  //穿刺
    Spin //旋转
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;


    [Header("Bounce info")]
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Peirce info")]
    [SerializeField] private UI_SkillTreeSlot peirceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField]private float hitCooldown =.35f;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity;

    [Header("Skill info")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    public bool swordUnlocked{ get; private set; }
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;   //控制方向的力
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;


    [Header("Passive skill")]  //剑术的被动
    [SerializeField] private UI_SkillTreeSlot timeStopUnlockButton;
    public bool timeStopUnlock {  get; private set; }
    [SerializeField] private UI_SkillTreeSlot volnurableUnlockButton ;
    public bool volnurableUnlock {  get; private set; }



    private Vector2 finalDir; //预测轨迹


    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParents;

    private GameObject[] dots; //用来存储生成的dots


    protected override void Start()
    {
        base.Start();
        GenereateDots();
        SetupGraivty();   //开始时候设置重力会导致技能切换没办法实时更新， 也可能后续技能树设置之后剑的方式不能随时改变，但是这里方便Unity中检查移动到uodate中方便随时切换

        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        peirceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);
        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        volnurableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVolnurable);


    }
    protected override void Update()
    {

        SetupGraivty();
        if (Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < numberOfDots; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }


    #region unlockregion

    protected override void CheckUnlock()
    {
        UnlockSword();
        UnlockBounceSword();
        UnlockSpinSword();
        UnlockPierceSword();
        UnlockTimeStop();
        UnlockVolnurable();

    }

    private void UnlockTimeStop()
    {
        if(timeStopUnlockButton.unlocked)
            timeStopUnlock = true;
    }
    private void UnlockVolnurable()
    {
        if(volnurableUnlockButton.unlocked)
            volnurableUnlock =true;
    }

    private void  UnlockSword()
    {
        if(swordUnlockButton.unlocked)
        {
            swordType = SwordType.Regular; //设置默认防止错误，虽然前边有默认
            swordUnlocked = true;
        }
    }

    private void UnlockBounceSword()  //枚举控制不同种类的剑
    {
        if(bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
    }
    private void UnlockPierceSword()
    {
        if (peirceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
    }
    private void UnlockSpinSword()
    {
        if(spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
    }


    #endregion

    private void SetupGraivty() //设置不同方式的重力
    {
        if(swordType ==SwordType.Bounce)
            swordGravity = bounceGravity;
        else if(swordType ==SwordType.Pierce)
            swordGravity = pierceGravity;
        else if(swordType ==SwordType.Spin)
            swordGravity = spinGravity;
    }

    public void CreatSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
            newSwordScript.SetupBounce(true, bounceAmount,bounceSpeed);
        else if (swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(pierceAmount);
        else if (swordType == SwordType.Spin)
            newSwordScript.SetupSpin(true,maxTravelDistance,spinDuration,hitCooldown);




        newSwordScript.SetupSword(finalDir, swordGravity, player,freezeTimeDuration,returnSpeed);

        player.AssignNewSword(newSword); //在ground状态下完成检查是否已经有一把剑了

        DotsActive(false); //生成剑关闭
    }



    #region Aim region
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position; //玩家位置
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = mousePosition - playerPosition;
        return direction;
    }

    public void DotsActive(bool _isActive)    //在playerAimSwordState 状态进入时候打开 dotss
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenereateDots()
    {
        dots = new GameObject[numberOfDots];//数组
        for (int i = 0; i < numberOfDots; i++)//循环实例化dots
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParents);//Object.Instantiate 返回 Object 实例化的克隆对象。
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);
        return position;
    }
    #endregion

}
