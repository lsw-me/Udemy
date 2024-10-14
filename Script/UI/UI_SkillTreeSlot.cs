using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,ISaveManager
{
    private UI ui;
    private Image skillImage;

    [SerializeField]private string skillName;

    [SerializeField] private int skillCost; 

    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;



    public bool unlocked;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLock;

    private void Awake()
    {
        
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot()); //执行顺序相关 132 中移动
    }
    private void Start()
    {

        ui =GetComponentInParent<UI>();
        skillImage = GetComponent<Image>();

        skillImage.color = lockedSkillColor;


        if(unlocked)       //解决读取数据技能解锁但是ui 不更新（bug: 读取数据后skilltree上unlock 为true 但是skilltree上图标暗色 ）
            skillImage.color = Color.white;

    }

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI-" + skillName;
    }



    public void UnlockSkillSlot()
    {
        if (PlayerManager.instance.HaveEnoughMoney(skillCost) == false) //弹幕提示 不能在这里判断，否则有可能钱白扣了 
            return;

        for (int i = 0; i < shouldBeUnlocked.Length; i++) //检查应该解锁的技能
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }

        for (int i = 0; i < shouldBeLock.Length; i++) //检查应该锁住的技能
        {
            if (shouldBeLock[i].unlocked ==true)
            {
                Debug.Log("Cannot unlock skill");
                return; 
            }
        }

        unlocked = true;
        skillImage.color = Color.white;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillDescription,skillName,skillCost);

        //这里原来有根据鼠标位置调整uiTooltip 但是bug （有时候会超出位置）   换到ui tooltip中了
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    public void LoadData(GameData _data)
    {
        //Debug.Log("Load skill tree");
        if(_data.skillTree.TryGetValue(skillName,out bool value))
        {
            unlocked = value;
        }

    }

    public void SaveData(ref GameData _data) //这里不像inventory一样清除，不然每次保存都清除，但是技能会学很多，会导致最终只存一个技能
    {
        if(_data.skillTree.TryGetValue(skillName,out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
        {
            _data.skillTree.Add(skillName, unlocked);
        }
    }
}
