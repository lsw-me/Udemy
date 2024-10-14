using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance; //������������������

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;


    private GameData gameData;    //����ִ��˳���߱������ݳ����� �����Ϊpublic   ���Ǹо����ã�
    private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;


    [ContextMenu("Delete save file")]
    public void DeleteSavedData() //����ɾ��������ļ������Ƿ��㲻�������Ϸ�������� (���Ҳ�õĵ���)
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
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);  //Application.persistentDataPath ��ͬϵͳ����һ����ͬ��Ĭ��·��
        saveManagers = FindAllSaveManagers(); //���� д�� 
        LoadGame();
    }
    public void  NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        // ��Ϸ������Դ�� data handler ����

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

        dataHandler.Save(gameData);      //data handler ����������
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }



    private List<ISaveManager> FindAllSaveManagers() //****//ȫ��Ѱ�Ҵ�ISave�Ľű��ĺ���
    {
        IEnumerable<ISaveManager> saveManager = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>(); //(true) �ǵ�Ļ��ʾ������trueֻ����ҵ�ǰ������б����漼�ܿ��ܻ����

        return new List<ISaveManager>(saveManager);
    }


    public bool  HasSaveData() //�����Ƿ񱣴�����Ȼ������continue��ť
    {
        if(dataHandler.Load() != null)
        {
            return true;
        }
        return false;
    }

}
