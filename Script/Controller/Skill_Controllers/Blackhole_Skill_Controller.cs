using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;


    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;           //改为private 使用Setup函数传参
    private float blackholeTimer;


    private bool canShrink; //控制球收缩
    private bool canGrow = true;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;
    private bool playerCanDisaper = true;   // 解决黑洞消失时候再次按R人物透明问题


    private int amountOfAttack = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;


    private List<Transform> targets = new List<Transform>();
    private List<GameObject> creatHotKey = new List<GameObject>();

    public bool playerCanExitState {  get; private set; }
    
    public void SetupBlackhole(float _maxSize,float _growSpeed,float _shrinkSpeed, int _amountOfAttack, float _cloneAttackCooldown,float _blackDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttack = _amountOfAttack;
        cloneAttackCooldown = _cloneAttackCooldown;  //setup函数设置参数，变量改为private

        blackholeTimer =_blackDuration;


        if (SkillManager.instance.clone.crystalInstallOfClone)
            playerCanDisaper = false;

    }
   
    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;//黑洞攻击计时器
        blackholeTimer -= Time.deltaTime; //玩家QTE计时

        if(blackholeTimer<0)                      //下边计时器相当于QTE的计时，没整完就失败了！！
        {
            blackholeTimer = Mathf.Infinity;   //确保只执行一次

            if (targets.Count > 0)
            {
                ReleaseCloneAttack();
            }
            else
                FinishBlackHoleAbility();
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();

        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);//在向量 a 与 b 之间按 t 进行线性插值。 public static Vector2 Lerp (Vector2 a, Vector2 b, float t);
        }
        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime); //减小尺寸
            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }

    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return;

        DestoryHotKeys();  //这里是已经按下了热键，在按R的时候删除
        cloneAttackReleased = true;
        canCreateHotKeys = false;


        if(playerCanDisaper)
        {
            PlayerManager.instance.player.fX.MakeTransprent(true);
            playerCanDisaper = false;
        }

    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttack >0)
        {
            cloneAttackTimer = cloneAttackCooldown;
            // create clone
            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;

            if (Random.Range(0, 100) > 50)
                xOffset = 2;
            else
                xOffset = -2;

            if(SkillManager.instance.clone.crystalInstallOfClone)  //如果在使用大招时候crystalInstallOfClone 不是创建clone 则创建crystal 并且随机攻击敌人
            {
                SkillManager.instance.crystal.CreatCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else //否则CreatClon 
            {
                SkillManager.instance.clone.CreatClone(targets[randomIndex], new Vector3(xOffset, 0));
            }

            amountOfAttack--;

            if (amountOfAttack <= 0)
            {
                Invoke("FinishBlackHoleAbility",.5f);  //invoke 延迟 黑洞不会在amountOfAttack <= 0 后立刻消失，弹幕说会有继续产生替身导致实际攻击次数大于amountOfAttack的情况 这里我没改
                //顺便如果延迟时间短了还是会有黑洞状态结束，人物clone扔在攻击的情况
            }
        }
    }

    private void FinishBlackHoleAbility()
    {
        DestoryHotKeys();
        playerCanExitState = true;
        //PlayerManager.instance.player.ExitBlackHoleAbility();  //这里退出黑洞状态  p81更新添加playerCanExitState 用于退出
        canShrink = true;
        cloneAttackReleased = false;
    }

    private void DestoryHotKeys() //删除热键
    {
        if (creatHotKey.Count < 0)
            return;
        for (int i = 0; i < creatHotKey.Count; i++)
        {
            Destroy(creatHotKey[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Enemy>() != null)
        {
            other.GetComponent<Enemy>().FreezeTime(true); //冻结时间

            CreateHotKey(other);

        }
    }

    private void OnTriggerExit2D(Collider2D other) => other.GetComponent<Enemy>()?.FreezeTime(false);

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.GetComponent<Enemy>() != null)
    //    {
    //        other.GetComponent<Enemy>().FreezeTime(false); //冻结时间

    //    }
    //}

    private void CreateHotKey(Collider2D other)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("Not enough hot keys in a key code list");
            return;
        }

        if (!canCreateHotKeys)  //这里防止按hotkey之后有敌人进入 导致新创建的hotkey无法被消除，因此在添加释放终极技能时候，不会增加hotkey
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, other.transform.position + new Vector3(0, 2), Quaternion.identity);

        creatHotKey.Add(newHotKey); // 创建一个HotKey 加入list 方便后续删除

        KeyCode chooseKey = keyCodeList[Random.Range(0, keyCodeList.Count)];

        keyCodeList.Remove(chooseKey);

        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();

        newHotKeyScript.SetHotKey(chooseKey, other.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
