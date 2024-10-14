using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;


    //private void OnValidate()   //���ű������ػ��߼�������ֵ���޸�ʱ���ô˺��� ,�����Ͳ���������start��������   p112 .17min ȥ����
    //{
    //    SetupVisuals();
    //}

    private void SetupVisuals()
    {
        if (itemData == null)
            return;


        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item object " + itemData.name;
    }


    public void SetUpItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;

        rb.velocity = _velocity;

        SetupVisuals();
    }


    // OnTriggerEnter2D(Collider2D other) �ƶ���ItemObjectTrigger��

    public void PickUpItem()
    {

        if(!Inventory.instance.CanAddItem() && itemData.itemType ==ItemType.Equipment)  //����ֿ����ˣ����Ǽ񵽶������ѻ����ٱ������Ʒbug
        {
            rb.velocity = new Vector2(0,7);  //���и��ٶȣ����ǲ��ᱻ������
            return;
        }

        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }



}
