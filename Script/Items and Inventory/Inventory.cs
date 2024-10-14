using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;



//���  ʹ�õ���ģʽ
public class Inventory : MonoBehaviour, ISaveManager    //Inventory ���  ֱ�����ɱ�����洢��Ʒ�ĸ���  ����mc
{
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;


    public List<InventoryItem> inventory;                                    //��һ���б�  
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;   //�ֵ�洢


    [Header("Inventory  UI")]
    [SerializeField] private Transform iventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;


    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;    // ��������ֿ��    һ�����Ͻǣ�һ�����Ͻǰ��ֱ�洢 ����װ������ �� ��ͨ��������
    private UI_EquipmentSlot[] equipmentSlot;  //����װ��ui ���½�
    private UI_StatSlot[] statSlot; //�����ɫ����



    [Header("Items cooldown")]   //װ����cd 
    private float lastTimeUsedFlask;
    private float lastTimeUseArmor;

    public float flaskCooldown { get; private set; }   //ui_ingame ������ʹ�ø�Ϊpublic
    private float armorCooldown;     //��Ϊ����cdʹ��Time.time ������Ϸһ��ʼ�����޷�ʹ�õ�bug


    [Header("Data base")]
    //public string[] assetName;   ��֤ʱ����
    //private List<ItemData> itemDataBase;// public ��Ϊ˽�й���������֤ ����ȡ��
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();   //�����洢����װ����

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();  //�������ͨ��Ʒ��

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>(); //����ǿ�װ����Ʒ���б�   p108  ���޸�Ϊ�˽������װ������������

        inventoryItemSlot = iventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();

        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartingItems();

    }

    private void AddStartingItems()
    {
        foreach(ItemData_Equipment item  in loadedEquipment)
        {
            //Debug.Log("enter ItemData_Equipment item  in loadedEquipment ");
            Equipment(item);
        }

        //Debug.Log(loadedItems.Count);

        if (loadedItems.Count > 0)  //�б��� �����ݾ������Щ Ȼ�󷵻�
        {
            //Debug.Log("enter if ");
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.data);
                }
            }
            
            return;
        }
        //����UI�����⣬�洢��load���У����ǲ��������inventory�У����������һ����Сʱ �ݣ�˳������ ��������start��ִ�У�������ִ������ű���start��ִ��savemanager�е�start��Ȼ��ͳ����bug��
        //�������Ҫ����savemanager ��start �����ƶ���awake Ҫ�������������savemanager�ű�����ִ��˳��-1

        for (int i = 0; i < startingItems.Count; i++)
        {
            //Debug.Log("enter startingItems ");

            if (startingItems[i] != null) //���startingItems �б���Ҫ���пյģ�craft�±߻��а�ɫͼƬ������
                AddItem(startingItems[i]);
        }
    }

    public void Equipment(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oddEquipment = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                oddEquipment = item.Key;
        }
        if (oddEquipment != null)
        {
            UnEquipItem(oddEquipment);
            AddItem(oddEquipment);

        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);

        newEquipment.AddModifiers();  //װ��װ���������ֵ��װ������ֵ��

        RemoveItem(_item);
        UpdataSlotUI();
    }

    public void UnEquipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {

            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers(); // ж�¸�����ֵ
        }
    }

    private void UpdataSlotUI()
    {

        for (int i = 0; i < equipmentSlot.Length; i++)   //��������װ������
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                    equipmentSlot[i].UpdateSlot(item.Value);
            }
        }



        for (int i = 0; i < inventoryItemSlot.Length; i++)  //����ѭ������ui�����������
        {

            inventoryItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < stashItemSlot.Length; i++)
        {

            stashItemSlot[i].CleanUpSlot();
        }


        for (int i = 0; i < inventory.Count; i++)
        {

            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; i < stash.Count; i++)
        {

            stashItemSlot[i].UpdateSlot(stash[i]);
        }

        UpdateStatsUI();

    }

    public void UpdateStatsUI() //��ΪDodge���� AddModifier ��Ҫ����ui
    {
        for (int i = 0; i < statSlot.Length; i++)   //���½�ɫui�еĸ�����Ϣ
        {
            statSlot[i].UpdateStateValueUI();
        }
    }

    public void AddItem(ItemData _item)           //************************* ���¿����   ������Ʒ�����ж�
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())  //���slot���˵�����
            AddToInventory(_item);  //��������װ��
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);


        UpdataSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);

            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);

        }
    }

    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))   //�з���true ���Ҹ� value��ֵ
        {
            value.AddStack();   //������1
        }
        else                            // û�� newһ�� ��ӵ��б���ֵ� ���һ��
        {
            InventoryItem newItem = new InventoryItem(_item);

            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);

        }
    }


    public bool CanAddItem()        //���ֿ�slot�������Ƿ��㹻�����Ʒ
    {
        if (inventory.Count >= inventoryItemSlot.Length)
        {
            Debug.Log("No more space");
            return false;
        }
        return true;
    }
    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1) //û�л���ֻ��һ����ֱ��remove
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
                value.RemoveStack();  //����1 �͸�����
        }

        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
                stashValue.RemoveStack();
        }

        UpdataSlotUI();

    }


    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();


        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                //add this to  used material
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)  //С����Ҫ�Ĳ���
                {
                    Debug.Log("not enough materials");
                    return false;
                }
                else
                {
                    materialsToRemove.Add(stashValue);
                }
            }
            else
            {
                Debug.Log("not enough materials");
                return false;
            }
        }

        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data);
        }
        AddItem(_itemToCraft);
        Debug.Log("here is your item + " + _itemToCraft.name);
        return true;
    }


    public List<InventoryItem> GetEquiomentList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
                equipedItem = item.Key;
        }
        return equipedItem;
    }

    public void UseFlask()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);
        if (currentFlask == null)
            return;

        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if (canUseFlask)
        {
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.Effect(null);
            lastTimeUsedFlask = Time.time;
        }
        else
        {
            Debug.Log("Flask on cooldown");
        }
        // iteamd data Equipment �д������cd���� 

        // 1 check if can use /cooldown  2.use flask 3.set cd
    }

    public bool CanUseArmor() //�������ʱ��Ŀ���
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);
        if (Time.time > lastTimeUseArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;       //�����ʼ����ʹ�õ��ߵ�bug���ʼarmorCooldown Ϊ0 �����Կ���ʹ��
            lastTimeUseArmor = Time.time;
            return true;
        }
        Debug.Log("Amor on cooldown");
        return false;
    }

    public void LoadData(GameData _data)
    {
        //Debug.Log("Item load");
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            foreach (var item in GetItemDataBase())
            {
                if (item != null && item.itemID == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                    //Debug.Log("load success" + loadedItems.Count);
                  
                }
            }
        }

        foreach(string loadedItemId in _data.equipmentID)  //load equipment
        {
            foreach(var item in GetItemDataBase())
            {
                if(item != null && loadedItemId ==item.itemID)
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentID.Clear();

        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemID, pair.Value.stackSize);  //������û�����л��ֵ䣬�������ô�����Բ��ԣ����治�У����Կ� unity�ֲ� Application.persistentDataPath
        }

        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary) //ͬ������stashDictionary
        {
            _data.inventory.Add(pair.Key.itemID, pair.Value.stackSize);
        }
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentID.Add(pair.Key.itemID);
        }

    }

    private List<ItemData> GetItemDataBase() //������е�equipmentData��IdName��data�ĺ���
    {
        List<ItemData> itemDataBase = new List<ItemData>();

        string[] assetName = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (string SOName in assetName)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);//����ͨ���ҵ����ļ����õ���Ӧ��λ��
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);//����ʵ��ʵ��ͨ��λ��ת���õ���Ӧ������
            itemDataBase.Add(itemData);
        }
        return itemDataBase;
    }
}
