using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public int currency;

    public SerializableDictonary<string, bool> skillTree;  //���漼�������ֺͽ�����boolֵ���Դ˴ﵽ���漼�ܵ�Ŀ��
    public SerializableDictonary<string, int> inventory;  //����inventory������string  �洢��Ʒid  int �洢���˶��ٸ������Ʒ
    public List<string> equipmentID;

    public SerializableDictonary<string, bool> checkPoints;//���ּ��� 

    public float lostCurrencyX;
    public float lostCurrencyY;
    public int lostCurrencyAmount;  //�����������ϴ� �������ˣ��������ϵĻ�


    public string closestCheckPointId;

    public GameData() //���캯����ʼΪ0
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
