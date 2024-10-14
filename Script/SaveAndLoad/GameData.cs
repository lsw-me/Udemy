using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public int currency;

    public SerializableDictonary<string, bool> skillTree;  //保存技能树名字和解锁的bool值，以此达到保存技能的目的
    public SerializableDictonary<string, int> inventory;  //保存inventory的数据string  存储物品id  int 存储存了多少个这个物品
    public List<string> equipmentID;

    public SerializableDictonary<string, bool> checkPoints;//保持检查点 

    public float lostCurrencyX;
    public float lostCurrencyY;
    public int lostCurrencyAmount;  //这三个保存上次 死在那了，还有身上的魂


    public string closestCheckPointId;

    public GameData() //构造函数初始为0
    {
        currency = 0;
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;


        skillTree = new SerializableDictonary<string, bool>();
        inventory = new SerializableDictonary<string, int>();
        equipmentID = new List<string>();
        closestCheckPointId = string.Empty;
       checkPoints = new SerializableDictonary<string, bool>();
    }
}
