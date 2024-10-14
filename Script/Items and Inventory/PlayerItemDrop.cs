using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{

    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToloseMaterials;


    public override void GenerateDrop()                             // �����������������Ʒ ҲҪ�����
    {

        Inventory inventory = Inventory.instance; //��ΪҪ�úü��Σ�����ֱ��д��
        //list of equipment

        //List<InventoryItem> currentStsh = inventory.GetStashList();
        //List<InventoryItem> currentEquipment = inventory.GetEquiomentList(); //��д����



        List<InventoryItem> itemToUnequipment = new List<InventoryItem>();  //�����µ�list ��� ֻ��currentEquipment ɾ��ʱ�Ӳ����ڵĵط�ɾ������
        List<InventoryItem> materialsToLose = new List<InventoryItem>();


        foreach (InventoryItem item in inventory.GetEquiomentList())
        {
            if(Random.Range(0,100) < chanceToLooseItems)
            {
                DropItem(item.data);
                
                itemToUnequipment.Add(item);      //����Ҫɾ������ӵ�һ�����б���
            }
        }

        for (int i = 0; i < itemToUnequipment.Count; i++) //ѭ��ɾ��
        {
            inventory.UnEquipItem(itemToUnequipment[i].data as ItemData_Equipment);
        }


        foreach (InventoryItem item in inventory.GetStashList())
        {
            if (Random.Range(0, 100) < chanceToloseMaterials)
            {
                DropItem(item.data);

                materialsToLose.Add(item);      //����Ҫɾ������ӵ�һ�����б���
            }
        }

        for (int i = 0; i < materialsToLose.Count; i++)
        {
            inventory.RemoveItem(materialsToLose[i].data);
        }
        //foreach item check if or not to loose
    }

}
