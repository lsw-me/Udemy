using System.Text;
using UnityEditor;
using UnityEngine;


// �ű��ļ�������ѧϰ����÷���ΪʲôҪʹ������ű��ļ�



public enum ItemType
{
    Material,
    Equipment
}


[CreateAssetMenu(fileName = "New Item Data",menuName ="Data/Item")] //��unity�д����˵�
public class ItemData : ScriptableObject
{

    public ItemType itemType; //��Ʒ����
    public string itemName; //����
    public Sprite icon; //ͼ��
    public string itemID; //id ����ʱ����


    [Range(0,100)]
    public float dropChance;// ���伸��



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
