using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;


    //private void OnValidate()   //当脚本被加载或者检视面板的值被修改时调用此函数 ,这样就不用声明和start中启用了   p112 .17min 去掉了
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


    // OnTriggerEnter2D(Collider2D other) 移动到ItemObjectTrigger中

    public void PickUpItem()
    {

        if(!Inventory.instance.CanAddItem() && itemData.itemType ==ItemType.Equipment)  //解决仓库满了，但是捡到东西仍难会销毁被捡的物品bug
        {
            rb.velocity = new Vector2(0,7);  //会有个速度，但是不会被捡起来
            return;
        }

        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }



}
