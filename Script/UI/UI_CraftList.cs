using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftList : MonoBehaviour,IPointerDownHandler
{

    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private GameObject craftSlotPrefab;

    [SerializeField] private List<ItemData_Equipment> craftEquipment;
    //[SerializeField] private List<UI_CraftSlot> craftSlots;
    void Start()
    {
        transform.parent.GetChild(0).GetComponent<UI_CraftList>().SetupCraftList(); // ��ʼ����������ʼui�Ͼ�ô��ɫͼƬ��129 48�����н���
        SetupCraftList();
    }

    //private void AssingCraftSlots()  //129���ȥ����  �±ߺ���Ҳ���޸ģ��뿴ԭ��ȥ��129 45����
    //{
    //    for (int i = 0; i < craftSlotParent.childCount; i++)
    //    {
    //        craftSlots.Add(craftSlotParent.GetChild(i).GetComponent<UI_CraftSlot>());
    //    }
    //}


    public void SetupCraftList()     //
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }
        
        //craftSlots  = new List<UI_CraftSlot>();

        for (int i = 0; i < craftEquipment.Count; i++)
        {
            GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);
            newSlot.GetComponent<UI_CraftSlot>().SetupCraftSlot(craftEquipment[i]);
        }

    }

    public void OnPointerDown(PointerEventData eventData)   //������Ʒ
    {
        SetupCraftList();   
    }

    public void SetupDefaultCraftWindow()   //��ʼ��craft���±��Ǹ�����
    {
        if (craftEquipment[0] != null )
        GetComponentInParent<UI>().craftWindow.SetupCraftWindow(craftEquipment[0]);
    }
}
