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
        transform.parent.GetChild(0).GetComponent<UI_CraftList>().SetupCraftList(); // 初始化，这样开始ui上就么白色图片，129 48分钟有解释
        SetupCraftList();
    }

    //private void AssingCraftSlots()  //129最后去掉了  下边函数也有修改，想看原因去看129 45分钟
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

    public void OnPointerDown(PointerEventData eventData)   //制作物品
    {
        SetupCraftList();   
    }

    public void SetupDefaultCraftWindow()   //初始化craft中下变那个窗口
    {
        if (craftEquipment[0] != null )
        GetComponentInParent<UI>().craftWindow.SetupCraftWindow(craftEquipment[0]);
    }
}
