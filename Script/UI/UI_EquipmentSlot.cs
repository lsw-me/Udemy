using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_ItemSlot
{
    public EquipmentType slotType;


    private void OnValidate()
    {
         gameObject.name = "Equipment slot - "+ slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // inventory unequipment item 

        if(item ==null || item.data ==null )  //解决点空白报错的bug
            return;

        Inventory.instance.UnEquipItem(item.data as ItemData_Equipment);
        Inventory.instance.AddItem(item.data as ItemData_Equipment);  // 解决没有同类型装备无法脱下来装备的情况 （具体咋做的需要仔细了解一下）  从装备面板上脱下来

        ui.itemToolTip.HideToolTip();  //同样的解决装备物品后tooltip不消失的问题

        CleanUpSlot();

    }
}
