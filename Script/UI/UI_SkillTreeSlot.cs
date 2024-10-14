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
        
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot()); //ִ��˳����� 132 ���ƶ�
    }
    private void Start()
    {

        ui =GetComponentInParent<UI>();
        skillImage = GetComponent<Image>();

        skillImage.color = lockedSkillColor;


        if(unlocked)       //�����ȡ���ݼ��ܽ�������ui �����£�bug: ��ȡ���ݺ�skilltree��unlock Ϊtrue ����skilltree��ͼ�갵ɫ ��
            skillImage.color = Color.white;

    }

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI-" + skillName;
    }



    public void UnlockSkillSlot()
    {
        if (PlayerManager.instance.HaveEnoughMoney(skillCost) == false) //��Ļ��ʾ �����������жϣ������п���Ǯ�׿��� 
            return;

        for (int i = 0; i < shouldBeUnlocked.Length; i++) //���Ӧ�ý����ļ���
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }

        for (int i = 0; i < shouldBeLock.Length; i++) //���Ӧ����ס�ļ���
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

        //����ԭ���и������λ�õ���uiTooltip ����bug ����ʱ��ᳬ��λ�ã�   ����ui tooltip����
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

    public void SaveData(ref GameData _data) //���ﲻ��inventoryһ���������Ȼÿ�α��涼��������Ǽ��ܻ�ѧ�ܶ࣬�ᵼ������ֻ��һ������
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
