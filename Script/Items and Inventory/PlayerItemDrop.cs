using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{

    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToloseMaterials;


    public override void GenerateDrop()                             // 人物死亡随机掉落物品 也要爆金币
    {

        Inventory inventory = Inventory.instance; //因为要用好几次，所以直接写了
        //list of equipment

        //List<InventoryItem> currentStsh = inventory.GetStashList();
        //List<InventoryItem> currentEquipment = inventory.GetEquiomentList(); //换写法了



        List<InventoryItem> itemToUnequipment = new List<InventoryItem>();  //创建新的list 解决 只有currentEquipment 删除时从不存在的地方删除问题
        List<InventoryItem> materialsToLose = new List<InventoryItem>();


        foreach (InventoryItem item in inventory.GetEquiomentList())
        {
            if(Random.Range(0,100) < chanceToLooseItems)
            {
                DropItem(item.data);
                
                itemToUnequipment.Add(item);      //吧需要删除的添加到一个新列表中
            }
        }

        for (int i = 0; i < itemToUnequipment.Count; i++) //循环删除
        {
            inventory.UnEquipItem(itemToUnequipment[i].data as ItemData_Equipment);
        }


        foreach (InventoryItem item in inventory.GetStashList())
        {
            if (Random.Range(0, 100) < chanceToloseMaterials)
            {
                DropItem(item.data);

                materialsToLose.Add(item);      //吧需要删除的添加到一个新列表中
            }
        }

        for (int i = 0; i < materialsToLose.Count; i++)
        {
            inventory.RemoveItem(materialsToLose[i].data);
        }
        //foreach item check if or not to loose
    }

}
