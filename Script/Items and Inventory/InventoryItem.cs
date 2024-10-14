using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]  //��Ϊû�����л����Բ�����ʾ����unity ������public  ����Ҫ����Ϊʲô�����������
                //�����������Ϊ  ���� inventory��    public List<InventoryItem> inventory; ������unity��inspector�л��ǲ���ʾ����б���Ϊ����û�б����л�
public class InventoryItem  //ֱ������Ŀ   
{
    public ItemData data;
    public int stackSize;

    public InventoryItem(ItemData _newItemData)  //���캯�� 
    {
        data = _newItemData;
        //TODO: add to stack
        AddStack();
    }

    public void AddStack()=> stackSize++;
    public void RemoveStack()=> stackSize--;
}

