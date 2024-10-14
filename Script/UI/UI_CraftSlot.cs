using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{


    protected override void Start()
    {
        base.Start();


    }
    //private void OnEnable()  //129 �ĵ��ⲿ��
    //{
    //    UpdateSlot(item);
    //}

    public void SetupCraftSlot(ItemData_Equipment _data)
    {

        if (_data == null)
            return;

        item.data = _data;


        itemImage.sprite = _data.icon;   //�����icon��Ϊitemicon  ���ǻ�ȫ�ģ�������û��
        itemText.text = _data.itemName;



        if(itemText.text.Length > 12)  //ͬ����ֹװ������̫����������ʾλ�ã� ���ó���12λ�ı�����
            itemText.fontSize = itemText.fontSize * .7f;
        else 
            itemText.fontSize = 24;
    }


    public override void OnPointerDown(PointerEventData eventData)     
    {
        //inventory  craft data

        //ItemData_Equipment craftData = item.data as ItemData_Equipment;
        //Inventory.instance.CanCraft(craftData, craftData.craftingMaterials);   //129 ��������

        ui.craftWindow.SetupCraftWindow(item.data as ItemData_Equipment);

    }
}
