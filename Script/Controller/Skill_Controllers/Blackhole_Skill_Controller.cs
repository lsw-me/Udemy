using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;


    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;           //��Ϊprivate ʹ��Setup��������
    private float blackholeTimer;


    private bool canShrink; //����������
    private bool canGrow = true;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;
    private bool playerCanDisaper = true;   // ����ڶ���ʧʱ���ٴΰ�R����͸������


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
        cloneAttackCooldown = _cloneAttackCooldown;  //setup�������ò�����������Ϊprivate

        blackholeTimer =_blackDuration;


        if (SkillManager.instance.clone.crystalInstallOfClone)
            playerCanDisaper = false;

    }
   
    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;//�ڶ�������ʱ��
        blackholeTimer -= Time.deltaTime; //���QTE��ʱ

        if(blackholeTimer<0)                      //�±߼�ʱ���൱��QTE�ļ�ʱ��û�����ʧ���ˣ���
        {
            blackholeTimer = Mathf.Infinity;   //ȷ��ִֻ��һ��

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
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);//������ a �� b ֮�䰴 t �������Բ�ֵ�� public static Vector2 Lerp (Vector2 a, Vector2 b, float t);
        }
        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime); //��С�ߴ�
            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }

    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return;

        DestoryHotKeys();  //�������Ѿ��������ȼ����ڰ�R��ʱ��ɾ��
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

            if(SkillManager.instance.clone.crystalInstallOfClone)  //�����ʹ�ô���ʱ��crystalInstallOfClone ���Ǵ���clone �򴴽�crystal ���������������
            {
                SkillManager.instance.crystal.CreatCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else //����CreatClon 
            {
                SkillManager.instance.clone.CreatClone(targets[randomIndex], new Vector3(xOffset, 0));
            }

            amountOfAttack--;

            if (amountOfAttack <= 0)
            {
                Invoke("FinishBlackHoleAbility",.5f);  //invoke �ӳ� �ڶ�������amountOfAttack <= 0 ��������ʧ����Ļ˵���м�������������ʵ�ʹ�����������amountOfAttack����� ������û��
                //˳������ӳ�ʱ����˻��ǻ��кڶ�״̬����������clone���ڹ��������
            }
        }
    }

    private void FinishBlackHoleAbility()
    {
        DestoryHotKeys();
        playerCanExitState = true;
        //PlayerManager.instance.player.ExitBlackHoleAbility();  //�����˳��ڶ�״̬  p81�������playerCanExitState �����˳�
        canShrink = true;
        cloneAttackReleased = false;
    }

    private void DestoryHotKeys() //ɾ���ȼ�
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
            other.GetComponent<Enemy>().FreezeTime(true); //����ʱ��

            CreateHotKey(other);

        }
    }

    private void OnTriggerExit2D(Collider2D other) => other.GetComponent<Enemy>()?.FreezeTime(false);

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.GetComponent<Enemy>() != null)
    //    {
    //        other.GetComponent<Enemy>().FreezeTime(false); //����ʱ��

    //    }
    //}

    private void CreateHotKey(Collider2D other)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("Not enough hot keys in a key code list");
            return;
        }

        if (!canCreateHotKeys)  //�����ֹ��hotkey֮���е��˽��� �����´�����hotkey�޷������������������ͷ��ռ�����ʱ�򣬲�������hotkey
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, other.transform.position + new Vector3(0, 2), Quaternion.identity);

        creatHotKey.Add(newHotKey); // ����һ��HotKey ����list �������ɾ��

        KeyCode chooseKey = keyCodeList[Random.Range(0, keyCodeList.Count)];

        keyCodeList.Remove(chooseKey);

        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();

        newHotKeyScript.SetHotKey(chooseKey, other.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
