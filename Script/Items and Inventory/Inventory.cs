using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;



//库存  使用单例模式
public class Inventory : MonoBehaviour, ISaveManager    //Inventory 库存  直接理解成背包里存储物品的格子  类似mc
{
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;


    public List<InventoryItem> inventory;                                    //存一个列表  
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;   //字典存储


    [Header("Inventory  UI")]
    [SerializeField] private Transform iventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;


    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;    // 两个储存仓库吧    一个左上角，一个右上角昂分别存储 武器装备类型 和 普通材料类型
    private UI_EquipmentSlot[] equipmentSlot;  //武器装备ui 左下角
    private UI_StatSlot[] statSlot; //管理角色界面



    [Header("Items cooldown")]   //装备的cd 
    private float lastTimeUsedFlask;
    private float lastTimeUseArmor;

    public float flaskCooldown { get; private set; }   //ui_ingame 中设置使用改为public
    private float armorCooldown;     //因为下面cd使用Time.time 所以游戏一开始会有无法使用的bug


    [Header("Data base")]
    //public string[] assetName;   验证时候用
    //private List<ItemData> itemDataBase;// public 改为私有公开用来验证 后续取消
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
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();   //声明存储武器装备的

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();  //这个是普通物品的

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>(); //这个是可装备物品的列表   p108  有修改为了解决可以装备多个剑的情况

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

        if (loadedItems.Count > 0)  //有保存 的数据就添加这些 然后返回
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
        //这里UI有问题，存储和load都有，但是不会更新在inventory中，解决了找了一个多小时 草，顺序问题 两个都在start中执行，会有先执行这个脚本的start在执行savemanager中的start，然后就出这个bug，
        //解决方法要不把savemanager 中start 代码移动到awake 要不就在设置里给savemanager脚本设置执行顺序-1

        for (int i = 0; i < startingItems.Count; i++)
        {
            //Debug.Log("enter startingItems ");

            if (startingItems[i] != null) //解决startingItems 列表中要是有空的，craft下边会有白色图片的问题
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

        newEquipment.AddModifiers();  //装上装备，添加数值（装备的数值）

        RemoveItem(_item);
        UpdataSlotUI();
    }

    public void UnEquipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {

            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers(); // 卸下更新数值
        }
    }

    private void UpdataSlotUI()
    {

        for (int i = 0; i < equipmentSlot.Length; i++)   //更新武器装备部分
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                    equipmentSlot[i].UpdateSlot(item.Value);
            }
        }



        for (int i = 0; i < inventoryItemSlot.Length; i++)  //两个循环更新ui进行清除工作
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

    public void UpdateStatsUI() //因为Dodge中有 AddModifier 需要更新ui
    {
        for (int i = 0; i < statSlot.Length; i++)   //更新角色ui中的各种信息
        {
            statSlot[i].UpdateStateValueUI();
        }
    }

    public void AddItem(ItemData _item)           //************************* 重新看这块   增加物品类型判断
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())  //解决slot满了的问题
            AddToInventory(_item);  //增加武器装备
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
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))   //有返回true 并且给 value赋值
        {
            value.AddStack();   //数量加1
        }
        else                            // 没有 new一个 添加到列表和字典 添加一个
        {
            InventoryItem newItem = new InventoryItem(_item);

            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);

        }
    }


    public bool CanAddItem()        //检查仓库slot的数量是否还足够添加物品
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
            if (value.stackSize <= 1) //没有或者只有一个就直接remove
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
                value.RemoveStack();  //大于1 就改数字
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
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)  //小于需要的材料
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
        // iteamd data Equipment 中创建这个cd数据 

        // 1 check if can use /cooldown  2.use flask 3.set cd
    }

    public bool CanUseArmor() //冻结敌人时间的盔甲
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);
        if (Time.time > lastTimeUseArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;       //解决开始不能使用道具的bug，最开始armorCooldown 为0 ，所以可以使用
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
            _data.inventory.Add(pair.Key.itemID, pair.Value.stackSize);  //这样（没法序列化字典，不清楚这么描述对不对）保存不行，可以看 unity手册 Application.persistentDataPath
        }

        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary) //同样保存stashDictionary
        {
            _data.inventory.Add(pair.Key.itemID, pair.Value.stackSize);
        }
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentID.Add(pair.Key.itemID);
        }

    }

    private List<ItemData> GetItemDataBase() //获得所有的equipmentData的IdName和data的函数
    {
        List<ItemData> itemDataBase = new List<ItemData>();

        string[] assetName = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (string SOName in assetName)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);//这是通过找到的文件名拿到对应的位置
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);//这是实打实的通过位置转换拿到相应的数据
            itemDataBase.Add(itemData);
        }
        return itemDataBase;
    }
}
