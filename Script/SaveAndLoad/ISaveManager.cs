using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface ISaveManager 
{
    //保存数据的接口


    void LoadData(GameData _data);

    void SaveData(ref GameData _data);
        


}
