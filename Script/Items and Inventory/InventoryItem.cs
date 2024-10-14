using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]  //因为没有序列化所以不能显示，在unity 尽管是public  后续要明白为什么会有这中情况
                //这里好像是因为  尽管 inventory中    public List<InventoryItem> inventory; 公开的unity的inspector中还是不显示这个列表，因为他并没有被序列化
public class InventoryItem  //直译库存项目   
{
    public ItemData data;
    public int stackSize;

    public InventoryItem(ItemData _newItemData)  //构造函数 
    {
        data = _newItemData;
        //TODO: add to stack
        AddStack();
    }

    public void AddStack()=> stackSize++;
    public void RemoveStack()=> stackSize--;
}

