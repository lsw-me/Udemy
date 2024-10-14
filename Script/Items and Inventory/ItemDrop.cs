using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//这里想做的是掉落物品 爆金币了
public class ItemDrop : MonoBehaviour
{


    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;  //创建两个列表，一个实际掉落，一个可能掉落存储其物品的掉落率，循环可能掉落的物品，符合条件添加到实际掉落，以此做一个随机掉落的效果
    private List<ItemData> dropList = new List<ItemData>();
   
    [SerializeField] private GameObject dropPrefab;
    // [SerializeField] private ItemData item;  用不到了
    

    public virtual void  GenerateDrop()
    {
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if (Random.Range(0, 100) <= possibleDrop[i].dropChance)
                dropList.Add(possibleDrop[i]); //添加到实际掉落的列表
        }

        for (int i = 0; i < possibleItemDrop; i++)
        {
            ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];
            if(possibleItemDrop <= dropList.Count)  //防止possibleItemDrop 数量大过dropList 出现越界问题
            {
                dropList.Remove(randomItem);
            }
            DropItem(randomItem);
        }
    }


    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab,transform.position,Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));



        newDrop.GetComponent<ItemObject>().SetUpItem(_itemData, randomVelocity);
    }
}
