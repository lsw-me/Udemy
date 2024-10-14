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

        if(item ==null || item.data ==null )  //�����հױ����bug
            return;

        Inventory.instance.UnEquipItem(item.data as ItemData_Equipment);
        Inventory.instance.AddItem(item.data as ItemData_Equipment);  // ���û��ͬ����װ���޷�������װ������� ������զ������Ҫ��ϸ�˽�һ�£�  ��װ�������������

        ui.itemToolTip.HideToolTip();  //ͬ���Ľ��װ����Ʒ��tooltip����ʧ������

        CleanUpSlot();

    }
}
