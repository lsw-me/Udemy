using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : MonoBehaviour ,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler // UI_ItemSlot ��Ʒ��UI�ű� , eventSystem�ӿ�
{


    [SerializeField]protected Image itemImage;                     //image ��  UnityEngine.UI.image
    [SerializeField]protected TextMeshProUGUI itemText;


    protected UI ui;

    public InventoryItem item;  //��Ʒ

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void UpdateSlot(InventoryItem _newItem)      //������Ʒ�۵ķ���  ������Ʒ����δ��룿 ��OnTriggerEnter �л�ȡ Item�� ���ǵ� OnTriggerEnter ����������ֿ⣬��������ѭ�� UI_ItemSlot[] ����
    {
        item = _newItem;

        itemImage.color = Color.white;  //��ʼ��͸���ģ�unity inspector�а�����ͨ������0�ˣ����õ�һ��item�͸ı���ɫΪ��ɫ

        if (item != null)
        {
            itemImage.sprite = item.data.icon;  //����ͼ��

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();  //�Ѵ洢�ռ�תΪstringд�� itemText.text��
            }
            else
            {
                itemText.text = ""; //û�����ǿ� 
            }
        }
    }


    public void CleanUpSlot() //�����������Ʒ���ƣ� p108
    {
        item  = null;

        itemImage.sprite = null;
        itemImage.color  = Color.clear;


        itemText.text = "";

    }
    public virtual void OnPointerDown(PointerEventData eventData)   //ʵ�������
    {

        if (item ==null )             //����±�bug
            return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }

        if(item.data.itemType  ==ItemType.Equipment)                             //��װ�� �ĲŻᴥ��  ��������   bug :����հ׵�slot �ᱨ�� 
        {
            //Debug.Log("Equiped  new item + " + item.data.itemName);
            Inventory.instance.Equipment(item.data);
        }

        ui.itemToolTip.HideToolTip();               //�������װ����toolTip����ʧ������

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
