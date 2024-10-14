using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : MonoBehaviour ,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler // UI_ItemSlot 物品槽UI脚本 , eventSystem接口
{


    [SerializeField]protected Image itemImage;                     //image 类  UnityEngine.UI.image
    [SerializeField]protected TextMeshProUGUI itemText;


    protected UI ui;

    public InventoryItem item;  //物品

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void UpdateSlot(InventoryItem _newItem)      //更新物品槽的方法  传入物品，如何传入？ 在OnTriggerEnter 中获取 Item吗 ？是的 OnTriggerEnter 后会把它放入仓库，最后更新是循环 UI_ItemSlot[] 数组
    {
        item = _newItem;

        itemImage.color = Color.white;  //初始是透明的（unity inspector中阿尔法通道拉到0了），得到一个item就改变颜色为灰色

        if (item != null)
        {
            itemImage.sprite = item.data.icon;  //分配图标

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();  //把存储空间转为string写到 itemText.text上
            }
            else
            {
                itemText.text = ""; //没有则是空 
            }
        }
    }


    public void CleanUpSlot() //解决点击库存物品复制（ p108
    {
        item  = null;

        itemImage.sprite = null;
        itemImage.color  = Color.clear;


        itemText.text = "";

    }
    public virtual void OnPointerDown(PointerEventData eventData)   //实现鼠标点击
    {

        if (item ==null )             //解决下边bug
            return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }

        if(item.data.itemType  ==ItemType.Equipment)                             //能装备 的才会触发  测试例子   bug :点击空白的slot 会报错 
        {
            //Debug.Log("Equiped  new item + " + item.data.itemName);
            Inventory.instance.Equipment(item.data);
        }

        ui.itemToolTip.HideToolTip();               //解决穿上装备后toolTip不消失的问题

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null) 
            return;

        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
            return;
        ui.itemToolTip.HideToolTip();
    }
}
