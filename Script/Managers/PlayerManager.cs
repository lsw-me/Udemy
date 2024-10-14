using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour,ISaveManager
{
    public static PlayerManager instance; //单例模式  目的为了能够在任何脚本中访问PlayerManager

    public Player player;

    public int currency;//升级技能用的
    private void Awake()
    {
        if(instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }


    public bool HaveEnoughMoney(int _price) //判断
    {
        if(_price > currency)
        {
            Debug.Log("Not enough money");
            return false;
        }

        currency -= _price;
        return true;
    }


    public int GetCurrentCurrency() =>currency;

    public void LoadData(GameData _data)
    {
        this.currency = _data.currency;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = this.currency;
    }
}
