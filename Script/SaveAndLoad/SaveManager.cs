using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance; //单例用来管理保存数据

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;


    private GameData gameData;    //由于执行顺序后边保存数据出问题 这里改为public   但是感觉不好？
    private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;


    [ContextMenu("Delete save file")]
    public void DeleteSavedData() //方便删除保存的文件，就是方便不是这个游戏本体内容 (后边也用的到了)
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);

        dataHandler.Delete();

    }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;



    }

    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);  //Application.persistentDataPath 不同系统有着一个不同的默认路径
        saveManagers = FindAllSaveManagers(); //函数 写法 
        LoadGame();
    }
    public void  NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        // 游戏数据来源于 data handler 这里

        gameData = dataHandler.Load();
        if (this.gameData != null) 
            Debug.Log("data found");

        if (this.gameData == null)
        {
            Debug.Log("no game data found");
            NewGame();
        }
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach(ISaveManager saveManager in saveManagers )
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);      //data handler 来保存数据
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }



    private List<ISaveManager> FindAllSaveManagers() //****//全局寻找带ISave的脚本的函数
    {
        IEnumerable<ISaveManager> saveManager = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>(); //(true) 是弹幕提示，不加true只会查找当前活动对象列表，保存技能可能会出错

        return new List<ISaveManager>(saveManager);
    }


    public bool  HasSaveData() //根据是否保存数据然后隐藏continue按钮
    {
        if(dataHandler.Load() != null)
        {
            return true;
        }
        return false;
    }

}
