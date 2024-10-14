using System.Text;
using UnityEditor;
using UnityEngine;


// 脚本文件，这里学习这个用法，为什么要使用这个脚本文件



public enum ItemType
{
    Material,
    Equipment
}


[CreateAssetMenu(fileName = "New Item Data",menuName ="Data/Item")] //在unity中创建菜单
public class ItemData : ScriptableObject
{

    public ItemType itemType; //物品类型
    public string itemName; //名字
    public Sprite icon; //图标
    public string itemID; //id 保存时候用


    [Range(0,100)]
    public float dropChance;// 掉落几率



    protected StringBuilder sb = new StringBuilder();

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);

        itemID = AssetDatabase.AssetPathToGUID(path);
#endif
    }


    public virtual string GetDescription()
    {
        return "";
    }
}
