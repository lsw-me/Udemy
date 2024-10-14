using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�����������ǵ�����Ʒ �������
public class ItemDrop : MonoBehaviour
{


    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;  //���������б�һ��ʵ�ʵ��䣬һ�����ܵ���洢����Ʒ�ĵ����ʣ�ѭ�����ܵ������Ʒ������������ӵ�ʵ�ʵ��䣬�Դ���һ����������Ч��
    private List<ItemData> dropList = new List<ItemData>();
   
    [SerializeField] private GameObject dropPrefab;
    // [SerializeField] private ItemData item;  �ò�����
    

    public virtual void  GenerateDrop()
    {
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if (Random.Range(0, 100) <= possibleDrop[i].dropChance)
                dropList.Add(possibleDrop[i]); //��ӵ�ʵ�ʵ�����б�
        }

        for (int i = 0; i < possibleItemDrop; i++)
        {
            ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];
            if(possibleItemDrop <= dropList.Count)  //��ֹpossibleItemDrop �������dropList ����Խ������
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
