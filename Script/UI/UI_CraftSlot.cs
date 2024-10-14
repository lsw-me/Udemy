using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{


    protected override void Start()
    {
        base.Start();


    }
    //private void OnEnable()  //129 改掉这部分
    //{
    //    UpdateSlot(item);
    //}

    public void SetupCraftSlot(ItemData_Equipment _data)
    {

        if (_data == null)
            return;

        item.data = _data;


        itemImage.sprite = _data.icon;   //这里吧icon改为itemicon  但是会全改，所以我没改
        itemText.text = _data.itemName;



        if(itemText.text.Length > 12)  //同样防止装备名字太长，超出显示位置， 设置超过12位改变字体
            itemText.fontSize = itemText.fontSize * .7f;
        else 
            itemText.fontSize = 24;
    }


    public override void OnPointerDown(PointerEventData eventData)     
    {
        //inventory  craft data

        //ItemData_Equipment craftData = item.data as ItemData_Equipment;
        //Inventory.instance.CanCraft(craftData, craftData.craftingMaterials);   //129 更新这里

        ui.craftWindow.SetupCraftWindow(item.data as ItemData_Equipment);

    }
}
